using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 레벨과 경험치를 표시하는 UI 컴포넌트
/// </summary>
public class LevelExpView : MonoBehaviour
{
    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] TextMeshProUGUI _levelText;        // 레벨 텍스트
    [SerializeField] TextMeshProUGUI _expText;          // 경험치 텍스트
    [SerializeField] Image _expBar;                     // 경험치 바
    [SerializeField] GameObject _levelUpEffect;         // 레벨업 이펙트 (옵션)

    [Header("----- 애니메이션 설정 -----")]
    [SerializeField] float _expBarAnimSpeed = 2f;       // 경험치 바 애니메이션 속도
    [SerializeField] float _levelUpEffectDuration = 2f; // 레벨업 이펙트 표시 시간

    float _targetExpFillAmount = 0f;                    // 목표 경험치 바 채움량
    Coroutine _expBarAnimCoroutine;                     // 경험치 바 애니메이션 코루틴
    
    private void Start()
    {
        // GameManager의 레벨/경험치 이벤트 구독
        GameManager.OnLevelUp += OnLevelUp;
        GameManager.OnExperienceGained += OnExperienceGained;
        
        // 초기 UI 업데이트
        UpdateLevelExpDisplay();
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        GameManager.OnLevelUp -= OnLevelUp;
        GameManager.OnExperienceGained -= OnExperienceGained;
    }

    /// <summary>
    /// 레벨과 경험치 표시를 업데이트하는 함수
    /// </summary>
    public void UpdateLevelExpDisplay()
    {
        if (GameManager.Instance == null || GameManager.Instance.HeroData == null)
            return;

        HeroData heroData = GameManager.Instance.HeroData;
        
        // 레벨 텍스트 업데이트
        if (_levelText != null)
        {
            _levelText.text = $"Level {heroData.Level}";
        }

        // 경험치 텍스트 업데이트
        if (_expText != null)
        {
            _expText.text = $"{heroData.Exp:F0} / {heroData.ExpRequiredForNextLevel:F0}";
        }

        // 경험치 바 업데이트 (부드러운 애니메이션)
        if (_expBar != null)
        {
            _targetExpFillAmount = heroData.ExpProgress;
            
            if (_expBarAnimCoroutine != null)
            {
                StopCoroutine(_expBarAnimCoroutine);
            }
            _expBarAnimCoroutine = StartCoroutine(AnimateExpBar());
        }
    }

    /// <summary>
    /// 경험치 바를 부드럽게 애니메이션하는 코루틴
    /// </summary>
    IEnumerator AnimateExpBar()
    {
        float currentFillAmount = _expBar.fillAmount;
        
        while (Mathf.Abs(currentFillAmount - _targetExpFillAmount) > 0.01f)
        {
            currentFillAmount = Mathf.MoveTowards(currentFillAmount, _targetExpFillAmount, _expBarAnimSpeed * Time.deltaTime);
            _expBar.fillAmount = currentFillAmount;
            yield return null;
        }
        
        _expBar.fillAmount = _targetExpFillAmount;
    }

    /// <summary>
    /// 레벨업 이벤트 처리 함수
    /// </summary>
    /// <param name="newLevel">새로운 레벨</param>
    void OnLevelUp(int newLevel)
    {
        Debug.Log($"UI: 레벨업! 새 레벨: {newLevel}");
        
        // 레벨업 이펙트 표시
        if (_levelUpEffect != null)
        {
            StartCoroutine(ShowLevelUpEffect());
        }
        
        // UI 업데이트
        UpdateLevelExpDisplay();
    }

    /// <summary>
    /// 경험치 획득 이벤트 처리 함수
    /// </summary>
    /// <param name="expGained">획득한 경험치</param>
    void OnExperienceGained(float expGained)
    {
        // UI 업데이트
        UpdateLevelExpDisplay();
    }

    /// <summary>
    /// 레벨업 이펙트를 표시하는 코루틴
    /// </summary>
    IEnumerator ShowLevelUpEffect()
    {
        if (_levelUpEffect == null) yield break;
        
        _levelUpEffect.SetActive(true);
        yield return new WaitForSeconds(_levelUpEffectDuration);
        _levelUpEffect.SetActive(false);
    }
}