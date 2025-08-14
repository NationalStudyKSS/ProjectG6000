using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���ΰ� ĳ���� UI ���
/// </summary>
public class HeroStatusView : MonoBehaviour
{
    [Header("----- ������Ʈ ���� -----")]
    [SerializeField] RectTransform _canvasRt;           // ĵ���� ��Ʈ Ʈ������
    [SerializeField] Image _hpBar;
    [SerializeField] Image _mpBar;
    [SerializeField] TextMeshProUGUI _heroNameText;
    [SerializeField] RectTransform _interactionGuideRt; // ��ȣ�ۿ� ���̵� ��Ʈ Ʈ������

    public void SetHpBar(float currentHp, float maxHp)
    {
        _hpBar.fillAmount = currentHp / maxHp;
    }

    public void SetMpBar(float currentMp, float maxMp)
    {
        _mpBar.fillAmount = currentMp / maxMp;
    }

    public void SetHeroNameText(string heroName)
    {
        _heroNameText.text = heroName;
    }

    /// <summary>
    /// ��ȣ�ۿ� ���̵� UI�� �����ϴ� �Լ�
    /// </summary>
    /// <param name="isActive">���̵� ǥ�� ����</param>
    /// <param name="worldPos">���̵� ǥ���� ���� ��ǥ</param>
    public void SetInetractionGuide(bool isActive, Vector3 worldPos)
    {
        // ��ȣ�ۿ� ���̵� �¿���
        _interactionGuideRt.gameObject.SetActive(isActive);
        if (isActive == false) return;

        // ���� ��ǥ -> ��ũ�� ��ǥ ��ȯ
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPos);

        // ��ũ�� ��ǥ -> ĵ���� ���� ��ǥ
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRt, screenPoint, null, out Vector2 localPoint);

        // ĵ���� ���� ��ǥ�� ��ȣ�ۿ� ���̵忡 ����
        _interactionGuideRt.anchoredPosition = localPoint;

    }
}
