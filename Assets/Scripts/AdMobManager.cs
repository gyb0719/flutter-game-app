using UnityEngine;
using UnityEngine.UI;
using System;

// AdMob SDK 설치 후 주석 해제
/*
using GoogleMobileAds.Api;
*/

public class AdMobManager : MonoBehaviour
{
    public static AdMobManager Instance;
    
    [Header("광고 ID 설정")]
    public string rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917"; // 테스트 ID
    public string bannerAdUnitId = "ca-app-pub-3940256099942544/6300978111"; // 테스트 ID
    public string interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712"; // 테스트 ID
    
    [Header("UI 요소")]
    public Button rewardedAdButton;
    public Text adButtonText;
    public Image adButtonIcon;
    
    // 광고 객체들 (AdMob SDK 설치 후 주석 해제)
    /*
    private RewardedAd rewardedAd;
    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    */
    
    private bool isRewardedAdReady = false;
    private Action onAdWatchedCallback;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        InitializeAds();
        SetupUI();
    }
    
    void InitializeAds()
    {
        // AdMob 초기화 (SDK 설치 후 주석 해제)
        /*
        MobileAds.Initialize(initStatus => {
            Debug.Log("AdMob 초기화 완료");
            LoadRewardedAd();
            LoadBannerAd();
            LoadInterstitialAd();
        });
        */
        
        // 개발 모드 - 임시로 광고 준비 완료 설정
        isRewardedAdReady = true;
        Debug.Log("AdMob 임시 초기화 (개발 모드)");
        UpdateAdButtonState();
    }
    
    void SetupUI()
    {
        if (rewardedAdButton != null)
        {
            rewardedAdButton.onClick.AddListener(ShowRewardedAd);
        }
        
        UpdateAdButtonState();
    }
    
    void LoadRewardedAd()
    {
        // 보상형 광고 로드 (SDK 설치 후 주석 해제)
        /*
        AdRequest request = new AdRequest.Builder().Build();
        
        RewardedAd.Load(rewardedAdUnitId, request, (RewardedAd ad, LoadAdError error) => {
            if (error != null || ad == null)
            {
                Debug.LogError($"보상형 광고 로드 실패: {error}");
                isRewardedAdReady = false;
                return;
            }
            
            Debug.Log("보상형 광고 로드 성공");
            rewardedAd = ad;
            isRewardedAdReady = true;
            RegisterEventHandlers(rewardedAd);
            UpdateAdButtonState();
        });
        */
    }
    
    void LoadBannerAd()
    {
        // 배너 광고 로드 (SDK 설치 후 주석 해제)
        /*
        bannerView = new BannerView(bannerAdUnitId, AdSize.Banner, AdPosition.Bottom);
        AdRequest request = new AdRequest.Builder().Build();
        bannerView.LoadAd(request);
        */
    }
    
    void LoadInterstitialAd()
    {
        // 전면 광고 로드 (SDK 설치 후 주석 해제)
        /*
        AdRequest request = new AdRequest.Builder().Build();
        
        InterstitialAd.Load(interstitialAdUnitId, request, (InterstitialAd ad, LoadAdError error) => {
            if (error != null || ad == null)
            {
                Debug.LogError($"전면 광고 로드 실패: {error}");
                return;
            }
            
            interstitialAd = ad;
            RegisterInterstitialEventHandlers(interstitialAd);
        });
        */
    }
    
    public void ShowRewardedAd(Action onAdWatched = null)
    {
        if (!isRewardedAdReady)
        {
            Debug.Log("보상형 광고가 아직 준비되지 않았습니다.");
            return;
        }
        
        onAdWatchedCallback = onAdWatched;
        
        // 광고 표시 (SDK 설치 후 주석 해제)
        /*
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) => {
                Debug.Log($"광고 시청 완료! 보상: {reward.Amount}");
                OnUserEarnedReward();
            });
        }
        */
        
        // 개발 모드 - 즉시 보상 지급
        Debug.Log("광고 시청 완료 (개발 모드)");
        OnUserEarnedReward();
    }
    
    void OnUserEarnedReward()
    {
        // 게임매니저에게 보상 지급 알림
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnAdWatched();
        }
        
        // 콜백 실행
        onAdWatchedCallback?.Invoke();
        
        // Firebase 이벤트 로그
        if (FirebaseManager.Instance != null)
        {
            FirebaseManager.Instance.LogAdWatchedEvent(100);
        }
        
        // 새로운 광고 로드
        isRewardedAdReady = false;
        UpdateAdButtonState();
        Invoke("LoadRewardedAd", 1f); // 1초 후 새 광고 로드
    }
    
    public void ShowInterstitialAd()
    {
        // 전면 광고 표시 (게임 오버나 레벨업 시)
        /*
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
        }
        */
        
        Debug.log("전면 광고 표시 (개발 모드)");
    }
    
    void UpdateAdButtonState()
    {
        if (rewardedAdButton == null) return;
        
        rewardedAdButton.interactable = isRewardedAdReady;
        
        if (adButtonText != null)
        {
            adButtonText.text = isRewardedAdReady ? "🎁 광고 보고\n코인 받기!" : "광고 로딩중...";
        }
        
        if (adButtonIcon != null)
        {
            adButtonIcon.color = isRewardedAdReady ? Color.white : Color.gray;
        }
    }
    
    // AdMob 이벤트 핸들러들 (SDK 설치 후 주석 해제)
    /*
    private void RegisterEventHandlers(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () => {
            Debug.Log("보상형 광고 닫힘");
            LoadRewardedAd();
        };
        
        ad.OnAdFullScreenContentFailed += (AdError error) => {
            Debug.LogError($"보상형 광고 표시 실패: {error}");
            LoadRewardedAd();
        };
    }
    
    private void RegisterInterstitialEventHandlers(InterstitialAd ad)
    {
        ad.OnAdFullScreenContentClosed += () => {
            Debug.Log("전면 광고 닫힘");
            LoadInterstitialAd();
        };
        
        ad.OnAdFullScreenContentFailed += (AdError error) => {
            Debug.LogError($"전면 광고 표시 실패: {error}");
            LoadInterstitialAd();
        };
    }
    */
    
    void OnDestroy()
    {
        // 광고 정리 (SDK 설치 후 주석 해제)
        /*
        bannerView?.Destroy();
        */
    }
}