using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 오디오 설정을 제어하는 UI 컴포넌트
/// </summary>
public class AudioSettingsView : MonoBehaviour
{
    [Header("----- 볼륨 슬라이더 -----")]
    [SerializeField] Slider _masterVolumeSlider;
    [SerializeField] Slider _musicVolumeSlider;
    [SerializeField] Slider _sfxVolumeSlider;

    [Header("----- 볼륨 텍스트 -----")]
    [SerializeField] TextMeshProUGUI _masterVolumeText;
    [SerializeField] TextMeshProUGUI _musicVolumeText;
    [SerializeField] TextMeshProUGUI _sfxVolumeText;

    [Header("----- 버튼 -----")]
    [SerializeField] Button _playTestSfxButton;
    [SerializeField] Button _toggleMusicButton;

    AudioManager _audioManager;

    void Start()
    {
        _audioManager = GameManager.Instance?.AudioManager;
        
        if (_audioManager == null)
        {
            Debug.LogWarning("AudioManager를 찾을 수 없습니다!");
            return;
        }

        // 슬라이더 초기값 설정
        InitializeSliders();
        
        // 슬라이더 이벤트 구독
        _masterVolumeSlider?.onValueChanged.AddListener(OnMasterVolumeChanged);
        _musicVolumeSlider?.onValueChanged.AddListener(OnMusicVolumeChanged);
        _sfxVolumeSlider?.onValueChanged.AddListener(OnSfxVolumeChanged);

        // 버튼 이벤트 구독
        _playTestSfxButton?.onClick.AddListener(PlayTestSfx);
        _toggleMusicButton?.onClick.AddListener(ToggleMusic);

        // 초기 UI 업데이트
        UpdateVolumeTexts();
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
        _masterVolumeSlider?.onValueChanged.RemoveListener(OnMasterVolumeChanged);
        _musicVolumeSlider?.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        _sfxVolumeSlider?.onValueChanged.RemoveListener(OnSfxVolumeChanged);
        
        _playTestSfxButton?.onClick.RemoveListener(PlayTestSfx);
        _toggleMusicButton?.onClick.RemoveListener(ToggleMusic);
    }

    /// <summary>
    /// 슬라이더 초기값을 설정하는 함수
    /// </summary>
    void InitializeSliders()
    {
        if (_masterVolumeSlider != null)
        {
            _masterVolumeSlider.value = _audioManager.MasterVolume;
        }
        
        if (_musicVolumeSlider != null)
        {
            _musicVolumeSlider.value = _audioManager.MusicVolume;
        }
        
        if (_sfxVolumeSlider != null)
        {
            _sfxVolumeSlider.value = _audioManager.SfxVolume;
        }
    }

    /// <summary>
    /// 볼륨 텍스트를 업데이트하는 함수
    /// </summary>
    void UpdateVolumeTexts()
    {
        if (_masterVolumeText != null)
        {
            _masterVolumeText.text = $"마스터: {(_audioManager.MasterVolume * 100):F0}%";
        }
        
        if (_musicVolumeText != null)
        {
            _musicVolumeText.text = $"음악: {(_audioManager.MusicVolume * 100):F0}%";
        }
        
        if (_sfxVolumeText != null)
        {
            _sfxVolumeText.text = $"효과음: {(_audioManager.SfxVolume * 100):F0}%";
        }
    }

    /// <summary>
    /// 마스터 볼륨 변경 이벤트 핸들러
    /// </summary>
    void OnMasterVolumeChanged(float value)
    {
        _audioManager.MasterVolume = value;
        UpdateVolumeTexts();
        
        // 볼륨 변경 피드백 사운드
        _audioManager.PlaySfx("VolumeChange");
    }

    /// <summary>
    /// 음악 볼륨 변경 이벤트 핸들러
    /// </summary>
    void OnMusicVolumeChanged(float value)
    {
        _audioManager.MusicVolume = value;
        UpdateVolumeTexts();
    }

    /// <summary>
    /// 효과음 볼륨 변경 이벤트 핸들러
    /// </summary>
    void OnSfxVolumeChanged(float value)
    {
        _audioManager.SfxVolume = value;
        UpdateVolumeTexts();
        
        // 볼륨 변경 피드백 사운드
        _audioManager.PlaySfx("VolumeChange");
    }

    /// <summary>
    /// 테스트 효과음 재생
    /// </summary>
    void PlayTestSfx()
    {
        _audioManager.PlaySfx("TestSound");
    }

    /// <summary>
    /// 음악 재생/정지 토글
    /// </summary>
    void ToggleMusic()
    {
        // 간단한 음악 토글 (실제로는 더 복잡한 로직 필요)
        _audioManager.PlayMusic("BackgroundMusic1");
    }

    /// <summary>
    /// 외부에서 UI를 업데이트할 때 사용하는 함수
    /// </summary>
    public void RefreshUI()
    {
        InitializeSliders();
        UpdateVolumeTexts();
    }
}