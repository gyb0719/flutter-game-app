using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("오디오 소스")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource ambientSource;
    
    [Header("BGM")]
    public AudioClip[] backgroundMusics;
    public float musicVolume = 0.6f;
    
    [Header("효과음")]
    public AudioClip mergeSound;
    public AudioClip coinSound;
    public AudioClip levelUpSound;
    public AudioClip buttonClickSound;
    public AudioClip adRewardSound;
    public AudioClip[] happySounds; // 랜덤 기쁜 소리들
    
    [Header("환경음")]
    public AudioClip[] birdSounds;
    public AudioClip windSound;
    public AudioClip[] natureSounds;
    
    private Coroutine ambientSoundCoroutine;
    private int currentMusicIndex = 0;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeAudio()
    {
        // 오디오 소스 설정
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        if (ambientSource == null)
        {
            ambientSource = gameObject.AddComponent<AudioSource>();
        }
        
        // BGM 설정
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        
        // 환경음 설정
        ambientSource.loop = false;
        ambientSource.volume = 0.3f;
        
        // 효과음 설정
        sfxSource.loop = false;
        sfxSource.volume = 0.8f;
        
        // 프로시저럴 사운드 생성
        GenerateProceduralSounds();
        
        // BGM 시작
        PlayBackgroundMusic();
        
        // 환경음 시작
        StartAmbientSounds();
    }
    
    void GenerateProceduralSounds()
    {
        // 프로시저럴하게 사운드 생성 (간단한 버전)
        mergeSound = GenerateNote(440f, 0.3f); // A4 음
        coinSound = GenerateNote(523f, 0.2f); // C5 음
        levelUpSound = GenerateChord(new float[]{440f, 554f, 659f}, 0.5f); // A 메이저 코드
        buttonClickSound = GenerateNote(880f, 0.1f); // A5 음
        adRewardSound = GenerateNote(698f, 0.4f); // F5 음
        
        // 행복한 소리들 생성
        happySounds = new AudioClip[5];
        for (int i = 0; i < happySounds.Length; i++)
        {
            float frequency = 440f + (i * 100f); // 다양한 주파수
            happySounds[i] = GenerateNote(frequency, 0.2f);
        }
    }
    
    AudioClip GenerateNote(float frequency, float duration)
    {
        int sampleRate = 44100;
        int sampleCount = Mathf.RoundToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];
        
        for (int i = 0; i < sampleCount; i++)
        {
            float time = (float)i / sampleRate;
            float amplitude = Mathf.Exp(-time * 3f); // 감쇠
            samples[i] = amplitude * Mathf.Sin(2f * Mathf.PI * frequency * time);
            
            // 부드러운 엔벨로프
            if (i < sampleCount * 0.1f) // Attack
            {
                samples[i] *= i / (sampleCount * 0.1f);
            }
            else if (i > sampleCount * 0.8f) // Release
            {
                samples[i] *= (sampleCount - i) / (sampleCount * 0.2f);
            }
        }
        
        AudioClip clip = AudioClip.Create("GeneratedNote", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
    
    AudioClip GenerateChord(float[] frequencies, float duration)
    {
        int sampleRate = 44100;
        int sampleCount = Mathf.RoundToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];
        
        for (int i = 0; i < sampleCount; i++)
        {
            float time = (float)i / sampleRate;
            float amplitude = Mathf.Exp(-time * 2f);
            
            // 여러 주파수 합성
            for (int f = 0; f < frequencies.Length; f++)
            {
                samples[i] += amplitude * Mathf.Sin(2f * Mathf.PI * frequencies[f] * time) / frequencies.Length;
            }
            
            // 엔벨로프
            if (i < sampleCount * 0.1f)
            {
                samples[i] *= i / (sampleCount * 0.1f);
            }
            else if (i > sampleCount * 0.7f)
            {
                samples[i] *= (sampleCount - i) / (sampleCount * 0.3f);
            }
        }
        
        AudioClip clip = AudioClip.Create("GeneratedChord", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
    
    void PlayBackgroundMusic()
    {
        if (backgroundMusics != null && backgroundMusics.Length > 0)
        {
            musicSource.clip = backgroundMusics[currentMusicIndex];
            musicSource.Play();
        }
        else
        {
            // 프로시저럴 BGM 생성 (펜타토닉 스케일 기반)
            StartCoroutine(GenerateProceduralMusic());
        }
    }
    
    IEnumerator GenerateProceduralMusic()
    {
        float[] pentatonicScale = {261.63f, 293.66f, 329.63f, 392f, 440f}; // C 펜타토닉
        
        while (true)
        {
            // 4/4 박자의 간단한 멜로디 생성
            for (int measure = 0; measure < 8; measure++) // 8마디
            {
                for (int beat = 0; beat < 4; beat++) // 4박자
                {
                    if (Random.value > 0.3f) // 70% 확률로 음 연주
                    {
                        float frequency = pentatonicScale[Random.Range(0, pentatonicScale.Length)];
                        AudioClip note = GenerateNote(frequency, 0.5f);
                        musicSource.clip = note;
                        musicSource.Play();
                    }
                    
                    yield return new WaitForSeconds(0.6f); // 120 BPM
                }
            }
            
            yield return new WaitForSeconds(1f); // 마디 간 휴식
        }
    }
    
    void StartAmbientSounds()
    {
        ambientSoundCoroutine = StartCoroutine(PlayAmbientSounds());
    }
    
    IEnumerator PlayAmbientSounds()
    {
        while (true)
        {
            // 랜덤하게 자연 소리 재생
            yield return new WaitForSeconds(Random.Range(3f, 8f));
            
            if (Random.value > 0.5f)
            {
                // 새소리 재생 (프로시저럴 생성)
                AudioClip birdSound = GenerateBirdSound();
                ambientSource.clip = birdSound;
                ambientSource.Play();
            }
            else
            {
                // 바람소리나 다른 자연 소리
                AudioClip windClip = GenerateWindSound();
                ambientSource.clip = windClip;
                ambientSource.Play();
            }
        }
    }
    
    AudioClip GenerateBirdSound()
    {
        // 새의 지저귐 소리 시뮬레이션
        int sampleRate = 44100;
        float duration = Random.Range(0.8f, 2f);
        int sampleCount = Mathf.RoundToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];
        
        float baseFreq = Random.Range(800f, 1500f);
        
        for (int i = 0; i < sampleCount; i++)
        {
            float time = (float)i / sampleRate;
            float progress = time / duration;
            
            // 주파수 변조 (새소리 특유의 주파수 변화)
            float frequencyModulation = Mathf.Sin(time * Random.Range(20f, 40f)) * Random.Range(50f, 200f);
            float frequency = baseFreq + frequencyModulation;
            
            // 진폭 변조 (볼륨 변화)
            float amplitude = Mathf.Sin(progress * Mathf.PI) * Random.Range(0.1f, 0.3f);
            
            samples[i] = amplitude * Mathf.Sin(2f * Mathf.PI * frequency * time);
            
            // 노이즈 추가 (더 자연스러운 소리)
            samples[i] += Random.Range(-0.02f, 0.02f) * amplitude;
        }
        
        AudioClip clip = AudioClip.Create("BirdSound", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
    
    AudioClip GenerateWindSound()
    {
        // 바람 소리 (화이트 노이즈 기반)
        int sampleRate = 44100;
        float duration = Random.Range(2f, 4f);
        int sampleCount = Mathf.RoundToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];
        
        for (int i = 0; i < sampleCount; i++)
        {
            float time = (float)i / sampleRate;
            float progress = time / duration;
            
            // 저주파 노이즈 (바람의 기본 소리)
            float noise = Random.Range(-1f, 1f);
            
            // 로우패스 필터 효과 (고주파 제거)
            if (i > 0)
            {
                samples[i] = 0.7f * samples[i-1] + 0.3f * noise;
            }
            else
            {
                samples[i] = noise;
            }
            
            // 볼륨 엔벨로프
            float amplitude = Mathf.Sin(progress * Mathf.PI) * 0.1f;
            samples[i] *= amplitude;
        }
        
        AudioClip clip = AudioClip.Create("WindSound", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
    
    // 공개 메서드들 - 게임 이벤트에서 호출
    public void PlayMergeSound()
    {
        if (mergeSound != null)
        {
            sfxSource.PlayOneShot(mergeSound);
        }
    }
    
    public void PlayCoinSound()
    {
        if (coinSound != null)
        {
            // 피치 변조로 다양함 추가
            sfxSource.pitch = Random.Range(0.9f, 1.1f);
            sfxSource.PlayOneShot(coinSound);
            sfxSource.pitch = 1f;
        }
    }
    
    public void PlayLevelUpSound()
    {
        if (levelUpSound != null)
        {
            sfxSource.PlayOneShot(levelUpSound);
        }
    }
    
    public void PlayButtonClickSound()
    {
        if (buttonClickSound != null)
        {
            sfxSource.PlayOneShot(buttonClickSound, 0.5f);
        }
    }
    
    public void PlayAdRewardSound()
    {
        if (adRewardSound != null)
        {
            sfxSource.PlayOneShot(adRewardSound);
        }
        
        // 추가로 행복한 소리들 연속 재생
        StartCoroutine(PlayHappySounds());
    }
    
    IEnumerator PlayHappySounds()
    {
        for (int i = 0; i < 3; i++)
        {
            if (happySounds != null && happySounds.Length > 0)
            {
                AudioClip happySound = happySounds[Random.Range(0, happySounds.Length)];
                sfxSource.PlayOneShot(happySound, 0.6f);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
    }
    
    public void SetSFXVolume(float volume)
    {
        float sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }
    
    public void SetAmbientVolume(float volume)
    {
        float ambientVolume = Mathf.Clamp01(volume);
        ambientSource.volume = ambientVolume;
        PlayerPrefs.SetFloat("AmbientVolume", ambientVolume);
    }
    
    void LoadAudioSettings()
    {
        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 0.6f));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 0.8f));
        SetAmbientVolume(PlayerPrefs.GetFloat("AmbientVolume", 0.3f));
    }
    
    void Start()
    {
        LoadAudioSettings();
    }
    
    void OnDestroy()
    {
        if (ambientSoundCoroutine != null)
        {
            StopCoroutine(ambientSoundCoroutine);
        }
    }
}