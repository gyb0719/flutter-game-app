using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening; // DOTween 애니메이션 라이브러리

public class UIManager : MonoBehaviour
{
    [Header("메인 UI 패널들")]
    public CanvasGroup gameplayPanel;
    public CanvasGroup shopPanel;
    public CanvasGroup settingsPanel;
    public CanvasGroup achievementsPanel;
    
    [Header("상단 바")]
    public Text coinsText;
    public Text levelText;
    public Slider experienceBar;
    public Button settingsButton;
    public Button shopButton;
    
    [Header("게임플레이 UI")]
    public Button adRewardButton;
    public Text farmNameText;
    public Transform particleContainer;
    
    [Header("코지 UI 요소들")]
    public Image backgroundGradient;
    public ParticleSystem sparkleEffect;
    public Transform floatingCoinsContainer;
    
    [Header("색상 테마")]
    public Color primaryGreen = new Color(0.4f, 0.7f, 0.4f);
    public Color secondaryYellow = new Color(1f, 0.9f, 0.3f);
    public Color cozyBrown = new Color(0.6f, 0.4f, 0.2f);
    
    private Coroutine backgroundAnimationCoroutine;
    
    void Start()
    {
        InitializeUI();
        SetupAnimations();
        StartBackgroundAnimation();
    }
    
    void InitializeUI()
    {
        // 코지한 농장 이름들
        string[] farmNames = {"🌻 해바라기 농장", "🍎 사과나무 농장", "🌱 초록빛 농장", 
                             "🌸 벚꽃 농장", "🥕 당근밭", "🍓 딸기향 농장"};
        farmNameText.text = farmNames[Random.Range(0, farmNames.Length)];
        
        // 버튼 설정
        adRewardButton.onClick.AddListener(ShowAdRewardWithAnimation);
        settingsButton.onClick.AddListener(() => ShowPanel(settingsPanel));
        shopButton.onClick.AddListener(() => ShowPanel(shopPanel));
        
        // 초기 패널 상태
        ShowPanel(gameplayPanel);
    }
    
    void SetupAnimations()
    {
        // 상단 바 슬라이드 인 애니메이션
        Vector3 originalPos = coinsText.transform.position;
        coinsText.transform.position += Vector3.up * 100;
        coinsText.transform.DOMoveY(originalPos.y, 0.8f).SetEase(Ease.OutBounce);
        
        levelText.transform.position += Vector3.up * 100;
        levelText.transform.DOMoveY(originalPos.y, 1f).SetEase(Ease.OutBounce).SetDelay(0.2f);
        
        // 광고 버튼 펄스 애니메이션
        adRewardButton.transform.DOScale(1.05f, 0.8f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
    
    void StartBackgroundAnimation()
    {
        backgroundAnimationCoroutine = StartCoroutine(AnimateBackground());
    }
    
    IEnumerator AnimateBackground()
    {
        float time = 0;
        while (true)
        {
            time += Time.deltaTime * 0.3f;
            
            // 부드러운 그라데이션 색상 변화
            Color topColor = Color.Lerp(primaryGreen, new Color(0.3f, 0.6f, 0.8f), 
                (Mathf.Sin(time) + 1) * 0.5f);
            Color bottomColor = Color.Lerp(secondaryYellow, new Color(0.9f, 0.7f, 0.4f), 
                (Mathf.Cos(time * 0.8f) + 1) * 0.5f);
            
            // 배경 그라데이션 업데이트 (가상의 구현)
            // backgroundGradient.color = Color.Lerp(topColor, bottomColor, 0.5f);
            
            yield return null;
        }
    }
    
    public void UpdateCoinsUI(int coins, int previousCoins = 0)
    {
        // 코인 증가 애니메이션
        if (coins > previousCoins)
        {
            StartCoroutine(AnimateCoinsIncrease(previousCoins, coins));
            CreateFloatingCoinsEffect(coins - previousCoins);
            
            // 화면 shake 효과
            Camera.main.transform.DOShakePosition(0.3f, 0.1f, 10, 90, false, true);
        }
        
        coinsText.text = $"🪙 {coins:N0}";
        
        // 코인 텍스트 펄스 효과
        coinsText.transform.DOScale(1.2f, 0.2f)
            .OnComplete(() => coinsText.transform.DOScale(1f, 0.2f));
    }
    
    IEnumerator AnimateCoinsIncrease(int from, int to)
    {
        float duration = 0.5f;
        float elapsed = 0;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            int currentCoins = Mathf.RoundToInt(Mathf.Lerp(from, to, elapsed / duration));
            coinsText.text = $"🪙 {currentCoins:N0}";
            yield return null;
        }
    }
    
    void CreateFloatingCoinsEffect(int coinsEarned)
    {
        for (int i = 0; i < Mathf.Min(coinsEarned / 10, 5); i++)
        {
            GameObject floatingCoin = new GameObject("FloatingCoin");
            floatingCoin.transform.parent = floatingCoinsContainer;
            
            Text coinText = floatingCoin.AddComponent<Text>();
            coinText.font = coinsText.font;
            coinText.text = $"+{coinsEarned}🪙";
            coinText.color = secondaryYellow;
            coinText.fontSize = 24;
            coinText.alignment = TextAnchor.MiddleCenter;
            
            // 랜덤 시작 위치
            Vector3 startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            startPos.z = 0;
            floatingCoin.transform.position = startPos + Vector3.right * Random.Range(-1f, 1f);
            
            // 위로 올라가면서 페이드아웃
            Sequence floatSequence = DOTween.Sequence();
            floatSequence.Append(floatingCoin.transform.DOMoveY(startPos.y + 2f, 1.5f).SetEase(Ease.OutCubic));
            floatSequence.Join(coinText.DOFade(0, 1.5f).SetEase(Ease.OutCubic));
            floatSequence.OnComplete(() => Destroy(floatingCoin));
            
            // 약간의 딜레이로 순차 실행
            floatSequence.SetDelay(i * 0.1f);
        }
    }
    
    public void UpdateLevelUI(int level, float experienceProgress)
    {
        levelText.text = $"🌟 레벨 {level}";
        experienceBar.value = experienceProgress;
        
        // 레벨업 시 특별한 효과
        if (experienceProgress == 0) // 방금 레벨업했을 때
        {
            StartCoroutine(LevelUpCelebration());
        }
    }
    
    IEnumerator LevelUpCelebration()
    {
        // 레벨업 텍스트 애니메이션
        levelText.transform.DOScale(1.5f, 0.3f).SetEase(Ease.OutBack);
        levelText.color = Color.yellow;
        
        // 파티클 효과
        if (sparkleEffect != null)
        {
            sparkleEffect.Play();
        }
        
        // 축하 메시지
        ShowTemporaryMessage("🎉 레벨업! 🎉", 2f);
        
        yield return new WaitForSeconds(0.5f);
        
        // 원래 크기로 복귀
        levelText.transform.DOScale(1f, 0.3f);
        levelText.DOColor(Color.white, 0.3f);
    }
    
    void ShowAdRewardWithAnimation()
    {
        // 광고 시청 전 버튼 애니메이션
        adRewardButton.transform.DOPunchScale(Vector3.one * 0.1f, 0.5f, 10, 0.5f);
        
        // AdMob 매니저를 통해 광고 표시
        if (AdMobManager.Instance != null)
        {
            AdMobManager.Instance.ShowRewardedAd(() => {
                // 광고 시청 완료 후 축하 효과
                StartCoroutine(AdRewardCelebration());
            });
        }
    }
    
    IEnumerator AdRewardCelebration()
    {
        // 무지개 효과
        for (int i = 0; i < 10; i++)
        {
            CreateRainbowParticle();
            yield return new WaitForSeconds(0.1f);
        }
        
        ShowTemporaryMessage("🎁 광고 보상 획득! 🎁", 2f);
    }
    
    void CreateRainbowParticle()
    {
        GameObject particle = new GameObject("RainbowParticle");
        particle.transform.parent = particleContainer;
        
        Image particleImg = particle.AddComponent<Image>();
        particleImg.color = new Color(Random.value, Random.value, Random.value, 1f);
        
        // 랜덤 위치에서 시작
        Vector3 startPos = new Vector3(Random.Range(-5f, 5f), Random.Range(-3f, 3f), 0);
        particle.transform.position = startPos;
        
        // 애니메이션
        particle.transform.DOMove(startPos + Vector3.up * Random.Range(2f, 4f), 2f).SetEase(Ease.OutCubic);
        particle.transform.DORotate(Vector3.forward * Random.Range(180f, 360f), 2f).SetEase(Ease.OutCubic);
        particleImg.DOFade(0, 2f).OnComplete(() => Destroy(particle));
    }
    
    public void ShowPanel(CanvasGroup targetPanel)
    {
        // 모든 패널 숨기기
        CanvasGroup[] allPanels = {gameplayPanel, shopPanel, settingsPanel, achievementsPanel};
        
        foreach (var panel in allPanels)
        {
            if (panel != null && panel != targetPanel)
            {
                panel.DOFade(0, 0.3f).OnComplete(() => {
                    panel.interactable = false;
                    panel.blocksRaycasts = false;
                });
            }
        }
        
        // 타겟 패널 보이기
        if (targetPanel != null)
        {
            targetPanel.interactable = true;
            targetPanel.blocksRaycasts = true;
            targetPanel.DOFade(1, 0.3f);
        }
    }
    
    public void ShowTemporaryMessage(string message, float duration)
    {
        GameObject msgObj = new GameObject("TempMessage");
        msgObj.transform.parent = transform;
        
        Text msgText = msgObj.AddComponent<Text>();
        msgText.font = coinsText.font;
        msgText.text = message;
        msgText.fontSize = 36;
        msgText.color = Color.white;
        msgText.alignment = TextAnchor.MiddleCenter;
        
        msgObj.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.7f, 0);
        
        // 애니메이션
        msgObj.transform.DOScale(1.2f, 0.2f).SetEase(Ease.OutBack);
        msgText.DOFade(0, duration).SetDelay(0.5f);
        
        Destroy(msgObj, duration + 1f);
    }
    
    void OnDestroy()
    {
        if (backgroundAnimationCoroutine != null)
        {
            StopCoroutine(backgroundAnimationCoroutine);
        }
    }
}