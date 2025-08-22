using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 게임 전체 오디오를 관리하는 매니저 클래스
/// 배경음악, 효과음, 볼륨 제어 등을 담당
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("----- 오디오 설정 -----")]
    [SerializeField] AudioMixerGroup _masterMixerGroup;     // 마스터 오디오 믹서
    [SerializeField] AudioMixerGroup _musicMixerGroup;      // 음악 오디오 믹서
    [SerializeField] AudioMixerGroup _sfxMixerGroup;        // 효과음 오디오 믹서

    [Header("----- 배경음악 -----")]
    [SerializeField] AudioSource _musicAudioSource;         // 배경음악 오디오 소스
    [SerializeField] AudioClip[] _backgroundMusics;         // 배경음악 클립 배열

    [Header("----- 효과음 설정 -----")]
    [SerializeField] int _sfxPoolSize = 10;                 // 효과음 오디오 소스 풀 크기
    [SerializeField] AudioClip[] _commonSfx;                // 공통 효과음 클립 배열

    [Header("----- 볼륨 설정 -----")]
    [Range(0f, 1f)]
    [SerializeField] float _masterVolume = 1f;              // 마스터 볼륨
    [Range(0f, 1f)]
    [SerializeField] float _musicVolume = 0.7f;             // 음악 볼륨
    [Range(0f, 1f)]
    [SerializeField] float _sfxVolume = 0.8f;               // 효과음 볼륨

    // 효과음 오디오 소스 풀
    Queue<AudioSource> _sfxAudioSourcePool = new Queue<AudioSource>();
    List<AudioSource> _activeSfxSources = new List<AudioSource>();

    // 오디오 클립 캐시 (이름으로 빠른 검색)
    Dictionary<string, AudioClip> _audioClipCache = new Dictionary<string, AudioClip>();

    // 현재 재생 중인 배경음악 인덱스
    int _currentMusicIndex = 0;

    public float MasterVolume 
    { 
        get => _masterVolume; 
        set 
        { 
            _masterVolume = Mathf.Clamp01(value);
            UpdateMasterVolume();
        } 
    }
    
    public float MusicVolume 
    { 
        get => _musicVolume; 
        set 
        { 
            _musicVolume = Mathf.Clamp01(value);
            UpdateMusicVolume();
        } 
    }
    
    public float SfxVolume 
    { 
        get => _sfxVolume; 
        set 
        { 
            _sfxVolume = Mathf.Clamp01(value);
            UpdateSfxVolume();
        } 
    }

    // 오디오 이벤트
    public static event Action<string> OnMusicChanged;
    public static event Action<string> OnSfxPlayed;

    public void Initialize()
    {
        // 배경음악 오디오 소스 설정
        if (_musicAudioSource == null)
        {
            _musicAudioSource = gameObject.AddComponent<AudioSource>();
        }
        _musicAudioSource.outputAudioMixerGroup = _musicMixerGroup;
        _musicAudioSource.loop = true;
        _musicAudioSource.playOnAwake = false;

        // 효과음 오디오 소스 풀 생성
        CreateSfxAudioSourcePool();

        // 오디오 클립 캐시 구성
        BuildAudioClipCache();

        // 볼륨 설정 적용
        UpdateAllVolumes();

        Debug.Log("AudioManager 초기화 완료");
    }

    /// <summary>
    /// 효과음 오디오 소스 풀을 생성하는 함수
    /// </summary>
    void CreateSfxAudioSourcePool()
    {
        for (int i = 0; i < _sfxPoolSize; i++)
        {
            GameObject sfxObj = new GameObject($"SfxAudioSource_{i}");
            sfxObj.transform.SetParent(transform);
            
            AudioSource audioSource = sfxObj.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = _sfxMixerGroup;
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            
            _sfxAudioSourcePool.Enqueue(audioSource);
        }
    }

    /// <summary>
    /// 오디오 클립 캐시를 구성하는 함수
    /// </summary>
    void BuildAudioClipCache()
    {
        // 배경음악 캐시
        for (int i = 0; i < _backgroundMusics.Length; i++)
        {
            if (_backgroundMusics[i] != null)
            {
                _audioClipCache[_backgroundMusics[i].name] = _backgroundMusics[i];
            }
        }

        // 효과음 캐시
        for (int i = 0; i < _commonSfx.Length; i++)
        {
            if (_commonSfx[i] != null)
            {
                _audioClipCache[_commonSfx[i].name] = _commonSfx[i];
            }
        }
    }

    /// <summary>
    /// 배경음악을 재생하는 함수
    /// </summary>
    /// <param name="musicName">재생할 음악 이름</param>
    /// <param name="fadeIn">페이드인 여부</param>
    public void PlayMusic(string musicName, bool fadeIn = true)
    {
        if (_audioClipCache.TryGetValue(musicName, out AudioClip clip))
        {
            if (fadeIn && _musicAudioSource.isPlaying)
            {
                StartCoroutine(CrossFadeMusic(clip));
            }
            else
            {
                _musicAudioSource.clip = clip;
                _musicAudioSource.Play();
            }
            
            OnMusicChanged?.Invoke(musicName);
            Debug.Log($"배경음악 재생: {musicName}");
        }
        else
        {
            Debug.LogWarning($"배경음악을 찾을 수 없습니다: {musicName}");
        }
    }

    /// <summary>
    /// 배경음악을 인덱스로 재생하는 함수
    /// </summary>
    /// <param name="index">배경음악 인덱스</param>
    public void PlayMusicByIndex(int index)
    {
        if (index >= 0 && index < _backgroundMusics.Length && _backgroundMusics[index] != null)
        {
            _currentMusicIndex = index;
            PlayMusic(_backgroundMusics[index].name);
        }
    }

    /// <summary>
    /// 다음 배경음악을 재생하는 함수
    /// </summary>
    public void PlayNextMusic()
    {
        _currentMusicIndex = (_currentMusicIndex + 1) % _backgroundMusics.Length;
        PlayMusicByIndex(_currentMusicIndex);
    }

    /// <summary>
    /// 배경음악을 정지하는 함수
    /// </summary>
    /// <param name="fadeOut">페이드아웃 여부</param>
    public void StopMusic(bool fadeOut = true)
    {
        if (fadeOut)
        {
            StartCoroutine(FadeOutMusic());
        }
        else
        {
            _musicAudioSource.Stop();
        }
    }

    /// <summary>
    /// 효과음을 재생하는 함수
    /// </summary>
    /// <param name="sfxName">재생할 효과음 이름</param>
    /// <param name="volume">볼륨 (0-1, 기본값은 SfxVolume 사용)</param>
    /// <param name="pitch">피치 (기본값 1)</param>
    public void PlaySfx(string sfxName, float volume = -1f, float pitch = 1f)
    {
        if (_audioClipCache.TryGetValue(sfxName, out AudioClip clip))
        {
            AudioSource audioSource = GetSfxAudioSource();
            if (audioSource != null)
            {
                audioSource.clip = clip;
                audioSource.volume = volume < 0 ? _sfxVolume : volume;
                audioSource.pitch = pitch;
                audioSource.Play();
                
                StartCoroutine(ReturnSfxAudioSourceToPool(audioSource, clip.length / pitch));
                OnSfxPlayed?.Invoke(sfxName);
            }
        }
        else
        {
            Debug.LogWarning($"효과음을 찾을 수 없습니다: {sfxName}");
        }
    }

    /// <summary>
    /// 3D 공간상에서 효과음을 재생하는 함수
    /// </summary>
    /// <param name="sfxName">재생할 효과음 이름</param>
    /// <param name="position">재생 위치</param>
    /// <param name="volume">볼륨</param>
    public void PlaySfx3D(string sfxName, Vector3 position, float volume = -1f)
    {
        if (_audioClipCache.TryGetValue(sfxName, out AudioClip clip))
        {
            AudioSource audioSource = GetSfxAudioSource();
            if (audioSource != null)
            {
                audioSource.transform.position = position;
                audioSource.clip = clip;
                audioSource.volume = volume < 0 ? _sfxVolume : volume;
                audioSource.spatialBlend = 1f; // 3D 사운드
                audioSource.Play();
                
                StartCoroutine(ReturnSfxAudioSourceToPool(audioSource, clip.length));
            }
        }
    }

    /// <summary>
    /// 사용 가능한 효과음 오디오 소스를 가져오는 함수
    /// </summary>
    AudioSource GetSfxAudioSource()
    {
        if (_sfxAudioSourcePool.Count > 0)
        {
            AudioSource audioSource = _sfxAudioSourcePool.Dequeue();
            _activeSfxSources.Add(audioSource);
            return audioSource;
        }
        
        Debug.LogWarning("효과음 오디오 소스 풀이 부족합니다!");
        return null;
    }

    /// <summary>
    /// 효과음 오디오 소스를 풀에 반환하는 코루틴
    /// </summary>
    IEnumerator ReturnSfxAudioSourceToPool(AudioSource audioSource, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        _activeSfxSources.Remove(audioSource);
        audioSource.spatialBlend = 0f; // 2D 사운드로 리셋
        _sfxAudioSourcePool.Enqueue(audioSource);
    }

    /// <summary>
    /// 배경음악 크로스 페이드 코루틴
    /// </summary>
    IEnumerator CrossFadeMusic(AudioClip newClip)
    {
        float fadeTime = 1f;
        float originalVolume = _musicAudioSource.volume;
        
        // 페이드아웃
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            _musicAudioSource.volume = Mathf.Lerp(originalVolume, 0, t / fadeTime);
            yield return null;
        }
        
        // 새 음악으로 교체
        _musicAudioSource.clip = newClip;
        _musicAudioSource.Play();
        
        // 페이드인
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            _musicAudioSource.volume = Mathf.Lerp(0, originalVolume, t / fadeTime);
            yield return null;
        }
        
        _musicAudioSource.volume = originalVolume;
    }

    /// <summary>
    /// 배경음악 페이드아웃 코루틴
    /// </summary>
    IEnumerator FadeOutMusic()
    {
        float fadeTime = 1f;
        float originalVolume = _musicAudioSource.volume;
        
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            _musicAudioSource.volume = Mathf.Lerp(originalVolume, 0, t / fadeTime);
            yield return null;
        }
        
        _musicAudioSource.Stop();
        _musicAudioSource.volume = originalVolume;
    }

    // 볼륨 업데이트 함수들
    void UpdateMasterVolume()
    {
        if (_masterMixerGroup != null)
        {
            _masterMixerGroup.audioMixer.SetFloat("MasterVolume", Mathf.Log10(_masterVolume) * 20);
        }
    }

    void UpdateMusicVolume()
    {
        if (_musicMixerGroup != null)
        {
            _musicMixerGroup.audioMixer.SetFloat("MusicVolume", Mathf.Log10(_musicVolume) * 20);
        }
        if (_musicAudioSource != null)
        {
            _musicAudioSource.volume = _musicVolume;
        }
    }

    void UpdateSfxVolume()
    {
        if (_sfxMixerGroup != null)
        {
            _sfxMixerGroup.audioMixer.SetFloat("SfxVolume", Mathf.Log10(_sfxVolume) * 20);
        }
    }

    void UpdateAllVolumes()
    {
        UpdateMasterVolume();
        UpdateMusicVolume();
        UpdateSfxVolume();
    }
}