using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("설정 UI")]
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Toggle notificationsToggle;
    public Toggle vibrationToggle;
    public Button resetProgressButton;
    public Button restorePurchasesButton;
    public Button rateUsButton;
    public Button closeButton;
    
    [Header("언어 설정")]
    public Dropdown languageDropdown;
    
    [Header("정보 표시")]
    public Text versionText;
    public Text creditsText;
    
    private bool isInitialized = false;
    
    void Start()
    {
        InitializeSettings();
        SetupUI();
    }
    
    void InitializeSettings()
    {
        // 버전 정보 표시
        if (versionText != null)
        {
            versionText.text = $"버전 {Application.version}";
        }
        
        // 크레딧 정보
        if (creditsText != null)
        {
            creditsText.text = "🎮 코지 머지 농장\n\n" +
                              "💻 개발: AI Code Assistant\n" +
                              "🎨 디자인: 프로시저럴 생성\n" +
                              "🎵 음악: AI 작곡\n\n" +
                              "❤️ 플레이해 주셔서 감사합니다!";
        }
        
        // 슬라이더 초기값 설정
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.6f);
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }
        
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
        
        // 토글 초기값 설정
        if (notificationsToggle != null)
        {
            notificationsToggle.isOn = PlayerPrefs.GetInt("NotificationsEnabled", 1) == 1;
            notificationsToggle.onValueChanged.AddListener(OnNotificationsToggled);
        }
        
        if (vibrationToggle != null)
        {
            vibrationToggle.isOn = PlayerPrefs.GetInt("VibrationEnabled", 1) == 1;
            vibrationToggle.onValueChanged.AddListener(OnVibrationToggled);
        }
        
        isInitialized = true;
    }
    
    void SetupUI()
    {
        // 버튼 이벤트 연결
        if (resetProgressButton != null)
        {
            resetProgressButton.onClick.AddListener(OnResetProgressClicked);
        }
        
        if (restorePurchasesButton != null)
        {
            restorePurchasesButton.onClick.AddListener(OnRestorePurchasesClicked);
        }
        
        if (rateUsButton != null)
        {
            rateUsButton.onClick.AddListener(OnRateUsClicked);
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseSettings);
        }
        
        // 언어 드롭다운 설정
        if (languageDropdown != null)
        {
            languageDropdown.ClearOptions();
            languageDropdown.AddOptions(new System.Collections.Generic.List<string>
            {
                "한국어 🇰🇷",
                "English 🇺🇸", 
                "日本語 🇯🇵",
                "中文 🇨🇳"
            });
            
            languageDropdown.value = PlayerPrefs.GetInt("Language", 0);
            languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        }
    }
    
    void OnMusicVolumeChanged(float value)
    {
        if (!isInitialized) return;
        
        PlayerPrefs.SetFloat("MusicVolume", value);
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
        }
    }
    
    void OnSFXVolumeChanged(float value)
    {
        if (!isInitialized) return;
        
        PlayerPrefs.SetFloat("SFXVolume", value);
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);
            // 변경 즉시 효과음으로 확인
            AudioManager.Instance.PlayButtonClickSound();
        }
    }
    
    void OnNotificationsToggled(bool enabled)
    {
        if (!isInitialized) return;
        
        PlayerPrefs.SetInt("NotificationsEnabled", enabled ? 1 : 0);
        
        // 실제 앱에서는 푸시 알림 권한 설정
        #if UNITY_ANDROID || UNITY_IOS
        if (enabled)
        {
            // 푸시 알림 권한 요청
            Debug.Log("푸시 알림 활성화");
        }
        else
        {
            // 푸시 알림 비활성화
            Debug.Log("푸시 알림 비활성화");
        }
        #endif
        
        PlayerPrefs.Save();
    }
    
    void OnVibrationToggled(bool enabled)
    {
        if (!isInitialized) return;
        
        PlayerPrefs.SetInt("VibrationEnabled", enabled ? 1 : 0);
        
        // 진동 테스트
        if (enabled)
        {
            #if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
            #endif
        }
        
        PlayerPrefs.Save();
    }
    
    void OnLanguageChanged(int index)
    {
        if (!isInitialized) return;
        
        PlayerPrefs.SetInt("Language", index);
        PlayerPrefs.Save();
        
        // 언어 변경 적용 (실제로는 게임 재시작 필요)
        string[] languages = {"Korean", "English", "Japanese", "Chinese"};
        Debug.Log($"언어 변경: {languages[index]}");
        
        ShowMessage("언어 설정이 저장되었습니다!\n게임을 재시작하면 적용됩니다. 🌍");
    }
    
    void OnResetProgressClicked()
    {
        // 확인 다이얼로그 표시
        ShowConfirmDialog(
            "⚠️ 진행 상황 초기화",
            "정말로 모든 진행 상황을 초기화하시겠습니까?\n이 작업은 되돌릴 수 없습니다!",
            () => {
                // 진행 상황 초기화
                ResetAllProgress();
                ShowMessage("모든 진행 상황이 초기화되었습니다! 🔄");
            }
        );
    }
    
    void ResetAllProgress()
    {
        // 게임 데이터 초기화
        PlayerPrefs.DeleteKey("Coins");
        PlayerPrefs.DeleteKey("Level");
        PlayerPrefs.DeleteKey("Experience");
        PlayerPrefs.DeleteKey("ExperienceToNext");
        PlayerPrefs.DeleteKey("TutorialCompleted");
        
        // 업적 초기화
        string[] achievementKeys = PlayerPrefs.GetString("AchievementKeys", "").Split(',');
        foreach (string key in achievementKeys)
        {
            if (!string.IsNullOrEmpty(key))
            {
                PlayerPrefs.DeleteKey(key);
            }
        }
        
        // 상점 구매 초기화 (광고 제거 등)
        PlayerPrefs.DeleteKey("NoAds");
        PlayerPrefs.DeleteKey("AutoMergeEndTime");
        PlayerPrefs.DeleteKey("DoubleCoinsEndTime");
        
        PlayerPrefs.Save();
        
        // 게임 매니저 리셋
        if (GameManager.Instance != null)
        {
            GameManager.Instance.coins = 0;
            GameManager.Instance.level = 1;
            GameManager.Instance.experience = 0;
            GameManager.Instance.experienceToNext = 100;
            GameManager.Instance.UpdateUI();
        }
        
        // 애널리틱스 로그
        if (FirebaseManager.Instance != null)
        {
            FirebaseManager.Instance.LogEvent("progress_reset");
        }
    }
    
    void OnRestorePurchasesClicked()
    {
        ShowMessage("구매 복원 기능은\n개발 중입니다! 🛠️");
        
        // 실제 앱에서는 IAP 구매 복원 로직
        /*
        // Google Play / App Store 구매 복원
        RestorePurchases();
        */
    }
    
    void OnRateUsClicked()
    {
        // 스토어 평점 페이지로 이동
        string storeUrl = "";
        
        #if UNITY_ANDROID
        storeUrl = "market://details?id=" + Application.identifier;
        #elif UNITY_IOS
        storeUrl = "itms-apps://itunes.apple.com/app/id" + "YOUR_APP_ID";
        #endif
        
        if (!string.IsNullOrEmpty(storeUrl))
        {
            Application.OpenURL(storeUrl);
        }
        else
        {
            ShowMessage("평점을 남겨주시면\n더 좋은 게임을 만들 수 있어요! ⭐");
        }
        
        // 애널리틱스 로그
        if (FirebaseManager.Instance != null)
        {
            FirebaseManager.Instance.LogEvent("rate_us_clicked");
        }
    }
    
    void ShowMessage(string message)
    {
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowTemporaryMessage(message, 3f);
        }
        else
        {
            Debug.Log(message);
        }
    }
    
    void ShowConfirmDialog(string title, string message, System.Action onConfirm)
    {
        // 간단한 확인 다이얼로그 (실제로는 UI 팝업 구현)
        bool confirm = true; // 임시로 항상 확인
        
        if (confirm)
        {
            onConfirm?.Invoke();
        }
        
        Debug.Log($"{title}: {message}");
    }
    
    void CloseSettings()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
        
        // 설정 저장
        PlayerPrefs.Save();
        
        gameObject.SetActive(false);
    }
    
    // 진동 효과 재생 (설정이 켜져 있을 때)
    public static void PlayVibration()
    {
        if (PlayerPrefs.GetInt("VibrationEnabled", 1) == 1)
        {
            #if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
            #endif
        }
    }
    
    // 현재 언어 설정 반환
    public static string GetCurrentLanguage()
    {
        string[] languages = {"ko", "en", "ja", "zh"};
        int index = PlayerPrefs.GetInt("Language", 0);
        return languages[Mathf.Clamp(index, 0, languages.Length - 1)];
    }
    
    // 알림이 활성화되어 있는지 확인
    public static bool AreNotificationsEnabled()
    {
        return PlayerPrefs.GetInt("NotificationsEnabled", 1) == 1;
    }
    
    // 디바이스 정보 로그 (디버깅용)
    void LogDeviceInfo()
    {
        Debug.Log($"디바이스: {SystemInfo.deviceModel}");
        Debug.Log($"OS: {SystemInfo.operatingSystem}");
        Debug.Log($"메모리: {SystemInfo.systemMemorySize}MB");
        Debug.Log($"그래픽: {SystemInfo.graphicsDeviceName}");
        Debug.Log($"해상도: {Screen.width}x{Screen.height}");
        
        // Firebase로 전송 (실제 앱에서)
        if (FirebaseManager.Instance != null)
        {
            var deviceInfo = new System.Collections.Generic.Dictionary<string, object>
            {
                {"device_model", SystemInfo.deviceModel},
                {"os_version", SystemInfo.operatingSystem},
                {"memory_size", SystemInfo.systemMemorySize},
                {"graphics_device", SystemInfo.graphicsDeviceName},
                {"screen_resolution", $"{Screen.width}x{Screen.height}"}
            };
            
            FirebaseManager.Instance.LogEvent("device_info", deviceInfo);
        }
    }
    
    void OnEnable()
    {
        // 설정 창이 열릴 때마다 디바이스 정보 로그
        LogDeviceInfo();
    }
}