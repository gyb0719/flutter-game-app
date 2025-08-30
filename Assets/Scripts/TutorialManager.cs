using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class TutorialManager : MonoBehaviour
{
    [Header("튜토리얼 UI")]
    public CanvasGroup tutorialOverlay;
    public Text tutorialText;
    public Button tutorialButton;
    public Image handPointer;
    public Transform spotlight;
    
    [Header("튜토리얼 단계")]
    public Transform[] highlightTargets;
    public string[] tutorialMessages;
    
    private int currentStep = 0;
    private bool tutorialActive = false;
    private bool tutorialCompleted = false;
    
    void Start()
    {
        // 첫 실행 확인
        tutorialCompleted = PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;
        
        if (!tutorialCompleted)
        {
            StartCoroutine(StartTutorialAfterDelay());
        }
        else
        {
            tutorialOverlay.gameObject.SetActive(false);
        }
        
        SetupTutorialMessages();
    }
    
    void SetupTutorialMessages()
    {
        tutorialMessages = new string[]
        {
            "🌟 코지 머지 농장에 오신 걸 환영해요! 🌟\n\n같은 식물끼리 드래그해서 합쳐보세요!",
            "🌱 씨앗 두 개를 드래그해서\n겹쳐보세요!\n\n✨ 새싹으로 자라날 거예요 ✨",
            "🪙 합칠 때마다 코인을 받아요!\n\n레벨이 높은 식물일수록\n더 많은 코인을 줍니다!",
            "🎁 광고를 보면 보너스 코인!\n\n매일 보상도 놓치지 마세요~",
            "🎉 완벽해요!\n\n이제 나만의 코지한 농장을\n만들어보세요! 🌻"
        };
    }
    
    IEnumerator StartTutorialAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        StartTutorial();
    }
    
    public void StartTutorial()
    {
        tutorialActive = true;
        currentStep = 0;
        
        tutorialOverlay.gameObject.SetActive(true);
        tutorialOverlay.alpha = 0;
        tutorialOverlay.DOFade(1, 0.5f);
        
        // 버튼 이벤트 설정
        tutorialButton.onClick.RemoveAllListeners();
        tutorialButton.onClick.AddListener(NextTutorialStep);
        
        ShowTutorialStep(currentStep);
    }
    
    void ShowTutorialStep(int step)
    {
        if (step >= tutorialMessages.Length)
        {
            CompleteTutorial();
            return;
        }
        
        // 메시지 업데이트
        tutorialText.text = tutorialMessages[step];
        
        // 텍스트 애니메이션
        tutorialText.transform.localScale = Vector3.zero;
        tutorialText.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
        
        // 단계별 특별한 효과
        switch (step)
        {
            case 0: // 환영 메시지
                CreateWelcomeEffect();
                tutorialButton.GetComponentInChildren<Text>().text = "시작하기! 🚀";
                break;
                
            case 1: // 첫 번째 머지 가르치기
                HighlightMergeableItems();
                ShowHandPointer();
                tutorialButton.GetComponentInChildren<Text>().text = "알았어요! 👍";
                break;
                
            case 2: // 코인 설명
                HighlightCoinsUI();
                CreateSparkleEffect();
                tutorialButton.GetComponentInChildren<Text>().text = "좋아요! ✨";
                break;
                
            case 3: // 광고 버튼 설명
                HighlightAdButton();
                tutorialButton.GetComponentInChildren<Text>().text = "이해했어요! 💡";
                break;
                
            case 4: // 완료
                CreateCelebrationEffect();
                tutorialButton.GetComponentInChildren<Text>().text = "농장 시작! 🌱";
                break;
        }
    }
    
    void CreateWelcomeEffect()
    {
        // 여러 개의 하트 이모지가 떨어지는 효과
        for (int i = 0; i < 8; i++)
        {
            StartCoroutine(CreateFallingEmoji("💚", i * 0.2f));
        }
    }
    
    IEnumerator CreateFallingEmoji(string emoji, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        GameObject emojiObj = new GameObject("FallingEmoji");
        emojiObj.transform.parent = tutorialOverlay.transform;
        
        Text emojiText = emojiObj.AddComponent<Text>();
        emojiText.font = tutorialText.font;
        emojiText.text = emoji;
        emojiText.fontSize = 40;
        emojiText.color = Color.white;
        emojiText.alignment = TextAnchor.MiddleCenter;
        
        // 화면 상단 랜덤 위치에서 시작
        Vector3 startPos = new Vector3(Random.Range(100f, Screen.width - 100f), Screen.height + 50f, 0);
        emojiObj.transform.position = startPos;
        
        // 떨어지는 애니메이션
        float fallDuration = Random.Range(2f, 4f);
        emojiObj.transform.DOMoveY(-100f, fallDuration).SetEase(Ease.InCubic);
        emojiObj.transform.DORotate(Vector3.forward * Random.Range(-180f, 180f), fallDuration);
        
        Destroy(emojiObj, fallDuration + 1f);
    }
    
    void HighlightMergeableItems()
    {
        // 머지 가능한 아이템들 강조
        MergeItem[] allItems = FindObjectsOfType<MergeItem>();
        
        foreach (MergeItem item in allItems)
        {
            if (item.itemType == 0) // 씨앗만 강조
            {
                StartCoroutine(PulseItem(item.transform));
            }
        }
    }
    
    IEnumerator PulseItem(Transform item)
    {
        while (tutorialActive && currentStep == 1)
        {
            item.DOScale(1.2f, 0.5f).SetEase(Ease.InOutSine);
            yield return new WaitForSeconds(0.5f);
            item.DOScale(1f, 0.5f).SetEase(Ease.InOutSine);
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    void ShowHandPointer()
    {
        handPointer.gameObject.SetActive(true);
        handPointer.color = Color.white;
        
        // 손가락이 드래그 모션을 시뮬레이션
        MergeItem[] seeds = FindObjectsOfType<MergeItem>();
        if (seeds.Length >= 2)
        {
            StartCoroutine(AnimateHandPointer(seeds[0].transform, seeds[1].transform));
        }
    }
    
    IEnumerator AnimateHandPointer(Transform from, Transform to)
    {
        while (tutorialActive && currentStep == 1)
        {
            // 첫 번째 씨앗 위치로 이동
            handPointer.transform.position = Camera.main.WorldToScreenPoint(from.position);
            handPointer.transform.localScale = Vector3.one;
            
            yield return new WaitForSeconds(1f);
            
            // 두 번째 씨앗으로 드래그 모션
            handPointer.transform.DOMove(Camera.main.WorldToScreenPoint(to.position), 1f)
                .SetEase(Ease.InOutCubic);
            handPointer.transform.DOScale(1.2f, 1f).SetEase(Ease.InOutSine);
            
            yield return new WaitForSeconds(1.5f);
            
            // 머지 완료 효과
            handPointer.DOFade(0, 0.3f).OnComplete(() => {
                handPointer.DOFade(1, 0.3f);
            });
            
            yield return new WaitForSeconds(1f);
        }
    }
    
    void HighlightCoinsUI()
    {
        // 코인 UI 하이라이트
        if (FindObjectOfType<UIManager>() != null)
        {
            Text coinsText = FindObjectOfType<UIManager>().coinsText;
            if (coinsText != null)
            {
                StartCoroutine(HighlightUI(coinsText.transform));
            }
        }
    }
    
    void HighlightAdButton()
    {
        // 광고 버튼 하이라이트
        if (FindObjectOfType<UIManager>() != null)
        {
            Button adButton = FindObjectOfType<UIManager>().adRewardButton;
            if (adButton != null)
            {
                StartCoroutine(HighlightUI(adButton.transform));
            }
        }
    }
    
    IEnumerator HighlightUI(Transform uiElement)
    {
        Vector3 originalScale = uiElement.localScale;
        
        for (int i = 0; i < 3; i++)
        {
            uiElement.DOScale(originalScale * 1.1f, 0.3f);
            yield return new WaitForSeconds(0.3f);
            uiElement.DOScale(originalScale, 0.3f);
            yield return new WaitForSeconds(0.3f);
        }
    }
    
    void CreateSparkleEffect()
    {
        // 반짝이는 별 효과
        for (int i = 0; i < 12; i++)
        {
            StartCoroutine(CreateSparkle(i * 0.1f));
        }
    }
    
    IEnumerator CreateSparkle(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        GameObject sparkle = new GameObject("Sparkle");
        sparkle.transform.parent = tutorialOverlay.transform;
        
        Text sparkleText = sparkle.AddComponent<Text>();
        sparkleText.font = tutorialText.font;
        sparkleText.text = "✨";
        sparkleText.fontSize = 30;
        sparkleText.color = Color.yellow;
        sparkleText.alignment = TextAnchor.MiddleCenter;
        
        // 랜덤 위치
        Vector3 pos = new Vector3(Random.Range(50f, Screen.width - 50f), 
                                 Random.Range(50f, Screen.height - 50f), 0);
        sparkle.transform.position = pos;
        
        // 반짝이는 애니메이션
        sparkle.transform.DOScale(1.5f, 0.5f).SetEase(Ease.OutBack);
        sparkleText.DOFade(0, 1f).SetDelay(0.5f);
        
        Destroy(sparkle, 2f);
    }
    
    void CreateCelebrationEffect()
    {
        // 축하 폭죽 효과
        string[] celebrationEmojis = {"🎉", "🎊", "🌟", "💫", "✨", "🎈"};
        
        for (int i = 0; i < 15; i++)
        {
            StartCoroutine(CreateCelebrationEmoji(celebrationEmojis[Random.Range(0, celebrationEmojis.Length)], i * 0.1f));
        }
    }
    
    IEnumerator CreateCelebrationEmoji(string emoji, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        GameObject celebObj = new GameObject("CelebrationEmoji");
        celebObj.transform.parent = tutorialOverlay.transform;
        
        Text celebText = celebObj.AddComponent<Text>();
        celebText.font = tutorialText.font;
        celebText.text = emoji;
        celebText.fontSize = Random.Range(30, 50);
        celebText.color = new Color(Random.value, Random.value, Random.value, 1f);
        celebText.alignment = TextAnchor.MiddleCenter;
        
        // 화면 중앙에서 폭발하듯 퍼져나감
        Vector3 center = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        celebObj.transform.position = center;
        
        Vector3 direction = Random.insideUnitCircle.normalized;
        Vector3 targetPos = center + direction * Random.Range(200f, 400f);
        
        celebObj.transform.DOMove(targetPos, 2f).SetEase(Ease.OutCubic);
        celebObj.transform.DORotate(Vector3.forward * Random.Range(-360f, 360f), 2f);
        celebText.DOFade(0, 2f).SetEase(Ease.OutCubic);
        
        Destroy(celebObj, 3f);
    }
    
    public void NextTutorialStep()
    {
        // 버튼 클릭 애니메이션
        tutorialButton.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 10, 0.5f);
        
        currentStep++;
        
        if (currentStep < tutorialMessages.Length)
        {
            ShowTutorialStep(currentStep);
        }
        else
        {
            CompleteTutorial();
        }
    }
    
    void CompleteTutorial()
    {
        tutorialActive = false;
        tutorialCompleted = true;
        
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();
        
        // 손가락 포인터 숨기기
        handPointer.gameObject.SetActive(false);
        
        // 튜토리얼 오버레이 페이드 아웃
        tutorialOverlay.DOFade(0, 0.5f).OnComplete(() => {
            tutorialOverlay.gameObject.SetActive(false);
        });
        
        // 게임 시작 환영 메시지
        if (FindObjectOfType<UIManager>() != null)
        {
            FindObjectOfType<UIManager>().ShowTemporaryMessage("🌻 행복한 농장 생활을 시작하세요! 🌻", 3f);
        }
    }
    
    public void SkipTutorial()
    {
        CompleteTutorial();
    }
}