using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AchievementManager : MonoBehaviour
{
    [Header("업적 UI")]
    public Transform achievementsParent;
    public GameObject achievementPrefab;
    public Button closeButton;
    
    private List<Achievement> achievements = new List<Achievement>();
    private List<AchievementUI> spawnedAchievements = new List<AchievementUI>();
    
    [System.Serializable]
    public class Achievement
    {
        public string id;
        public string name;
        public string description;
        public int targetValue;
        public int currentValue;
        public bool isCompleted;
        public AchievementType type;
        public int coinReward;
        public string emoji;
        
        public Achievement(string _id, string _name, string _description, int _target, AchievementType _type, int _reward, string _emoji)
        {
            id = _id;
            name = _name;
            description = _description;
            targetValue = _target;
            currentValue = PlayerPrefs.GetInt($"Achievement_{id}", 0);
            isCompleted = PlayerPrefs.GetInt($"Achievement_{id}_completed", 0) == 1;
            type = _type;
            coinReward = _reward;
            emoji = _emoji;
        }
    }
    
    public enum AchievementType
    {
        TotalMerges,
        CoinsEarned,
        LevelReached,
        ItemsCreated,
        AdsWatched,
        ConsecutiveLogins,
        TimeSpent
    }
    
    void Start()
    {
        InitializeAchievements();
        CreateAchievementUI();
        closeButton.onClick.AddListener(CloseAchievements);
    }
    
    void InitializeAchievements()
    {
        achievements = new List<Achievement>
        {
            // 머지 관련 업적
            new Achievement("first_merge", "🌱 첫 머지!", "첫 번째 아이템을 머지하세요", 1, AchievementType.TotalMerges, 100, "🌱"),
            new Achievement("merge_master_1", "🌿 머지 마스터", "아이템을 50번 머지하세요", 50, AchievementType.TotalMerges, 500, "🌿"),
            new Achievement("merge_master_2", "🌳 머지 전문가", "아이템을 200번 머지하세요", 200, AchievementType.TotalMerges, 1000, "🌳"),
            new Achievement("merge_master_3", "🌲 머지 달인", "아이템을 500번 머지하세요", 500, AchievementType.TotalMerges, 2500, "🌲"),
            
            // 코인 관련 업적
            new Achievement("first_coins", "🪙 첫 코인!", "첫 코인을 획득하세요", 1, AchievementType.CoinsEarned, 50, "🪙"),
            new Achievement("coin_collector_1", "💰 코인 수집가", "총 1,000 코인을 획득하세요", 1000, AchievementType.CoinsEarned, 200, "💰"),
            new Achievement("coin_collector_2", "💎 코인 부자", "총 10,000 코인을 획득하세요", 10000, AchievementType.CoinsEarned, 1000, "💎"),
            new Achievement("coin_collector_3", "👑 코인 왕", "총 50,000 코인을 획득하세요", 50000, AchievementType.CoinsEarned, 5000, "👑"),
            
            // 레벨 관련 업적  
            new Achievement("level_5", "⭐ 레벨 5 달성", "레벨 5에 도달하세요", 5, AchievementType.LevelReached, 300, "⭐"),
            new Achievement("level_10", "🌟 레벨 10 달성", "레벨 10에 도달하세요", 10, AchievementType.LevelReached, 800, "🌟"),
            new Achievement("level_25", "✨ 레벨 25 달성", "레벨 25에 도달하세요", 25, AchievementType.LevelReached, 2000, "✨"),
            
            // 아이템 생성 업적
            new Achievement("fruit_tree", "🍎 과일나무 키우기", "과일나무를 1개 만드세요", 1, AchievementType.ItemsCreated, 1000, "🍎"),
            new Achievement("fruit_master", "🌳 과수원 주인", "과일나무를 10개 만드세요", 10, AchievementType.ItemsCreated, 5000, "🌳"),
            
            // 광고 시청 업적
            new Achievement("ad_supporter", "📺 광고 시청자", "광고를 5번 시청하세요", 5, AchievementType.AdsWatched, 500, "📺"),
            new Achievement("ad_fan", "🎬 광고 팬", "광고를 25번 시청하세요", 25, AchievementType.AdsWatched, 2000, "🎬"),
            
            // 연속 접속 업적
            new Achievement("daily_1", "📅 첫 출석", "1일 연속 접속하세요", 1, AchievementType.ConsecutiveLogins, 100, "📅"),
            new Achievement("daily_7", "🗓️ 일주일 출석", "7일 연속 접속하세요", 7, AchievementType.ConsecutiveLogins, 1000, "🗓️"),
            new Achievement("daily_30", "📆 한달 출석", "30일 연속 접속하세요", 30, AchievementType.ConsecutiveLogins, 10000, "📆")
        };
    }
    
    void CreateAchievementUI()
    {
        foreach (Achievement achievement in achievements)
        {
            GameObject achObj = Instantiate(achievementPrefab, achievementsParent);
            AchievementUI achievementUI = achObj.GetComponent<AchievementUI>();
            
            if (achievementUI == null)
            {
                achievementUI = achObj.AddComponent<AchievementUI>();
            }
            
            achievementUI.SetupAchievement(achievement, this);
            spawnedAchievements.Add(achievementUI);
        }
    }
    
    public void UpdateAchievement(AchievementType type, int value = 1)
    {
        bool anyCompleted = false;
        
        foreach (Achievement achievement in achievements)
        {
            if (achievement.type == type && !achievement.isCompleted)
            {
                achievement.currentValue += value;
                PlayerPrefs.SetInt($"Achievement_{achievement.id}", achievement.currentValue);
                
                // 달성 여부 체크
                if (achievement.currentValue >= achievement.targetValue)
                {
                    CompleteAchievement(achievement);
                    anyCompleted = true;
                }
            }
        }
        
        if (anyCompleted)
        {
            // UI 업데이트
            RefreshAchievementUI();
        }
        
        PlayerPrefs.Save();
    }
    
    void CompleteAchievement(Achievement achievement)
    {
        if (achievement.isCompleted) return;
        
        achievement.isCompleted = true;
        PlayerPrefs.SetInt($"Achievement_{achievement.id}_completed", 1);
        
        // 보상 지급
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddCoins(achievement.coinReward);
        }
        
        // 축하 메시지
        ShowAchievementPopup(achievement);
        
        // 사운드 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayLevelUpSound(); // 업적 완료 소리
        }
        
        // 애널리틱스 로그
        if (FirebaseManager.Instance != null)
        {
            var parameters = new System.Collections.Generic.Dictionary<string, object>
            {
                {"achievement_id", achievement.id},
                {"achievement_name", achievement.name},
                {"reward", achievement.coinReward}
            };
            FirebaseManager.Instance.LogEvent("achievement_unlocked", parameters);
        }
        
        Debug.Log($"🏆 업적 달성: {achievement.name} (+{achievement.coinReward} 코인)");
    }
    
    void ShowAchievementPopup(Achievement achievement)
    {
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            string message = $"🏆 업적 달성!\n{achievement.emoji} {achievement.name}\n💰 +{achievement.coinReward} 코인!";
            uiManager.ShowTemporaryMessage(message, 3f);
        }
        
        // 특별한 축하 효과
        StartCoroutine(AchievementCelebration(achievement));
    }
    
    System.Collections.IEnumerator AchievementCelebration(Achievement achievement)
    {
        // 황금 파티클 효과
        for (int i = 0; i < 20; i++)
        {
            CreateGoldenParticle();
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    void CreateGoldenParticle()
    {
        GameObject particle = new GameObject("GoldenParticle");
        particle.transform.parent = transform;
        
        Text particleText = particle.AddComponent<Text>();
        particleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        particleText.text = "✨";
        particleText.fontSize = Random.Range(30, 50);
        particleText.color = Color.yellow;
        particleText.alignment = TextAnchor.MiddleCenter;
        
        // 랜덤 위치에서 시작
        Vector3 startPos = new Vector3(
            Random.Range(50f, Screen.width - 50f),
            Random.Range(50f, Screen.height - 50f),
            0
        );
        particle.transform.position = startPos;
        
        // 상승 애니메이션
        particle.transform.DOMoveY(startPos.y + Random.Range(100f, 300f), 2f);
        particleText.DOFade(0, 2f);
        
        Destroy(particle, 3f);
    }
    
    void RefreshAchievementUI()
    {
        for (int i = 0; i < spawnedAchievements.Count && i < achievements.Count; i++)
        {
            spawnedAchievements[i].UpdateUI(achievements[i]);
        }
    }
    
    void CloseAchievements()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
        
        gameObject.SetActive(false);
    }
    
    // 특정 업적 타입별 업데이트 메서드들
    public void OnItemMerged()
    {
        UpdateAchievement(AchievementType.TotalMerges, 1);
    }
    
    public void OnCoinsEarned(int amount)
    {
        UpdateAchievement(AchievementType.CoinsEarned, amount);
    }
    
    public void OnLevelUp(int level)
    {
        foreach (Achievement achievement in achievements)
        {
            if (achievement.type == AchievementType.LevelReached && level >= achievement.targetValue && !achievement.isCompleted)
            {
                achievement.currentValue = level;
                CompleteAchievement(achievement);
            }
        }
        RefreshAchievementUI();
    }
    
    public void OnItemCreated(int itemType)
    {
        if (itemType == 4) // 과일나무인 경우
        {
            UpdateAchievement(AchievementType.ItemsCreated, 1);
        }
    }
    
    public void OnAdWatched()
    {
        UpdateAchievement(AchievementType.AdsWatched, 1);
    }
    
    // 완료된 업적 개수 반환
    public int GetCompletedAchievementsCount()
    {
        int count = 0;
        foreach (Achievement achievement in achievements)
        {
            if (achievement.isCompleted) count++;
        }
        return count;
    }
    
    // 총 업적 개수 반환
    public int GetTotalAchievementsCount()
    {
        return achievements.Count;
    }
}

public class AchievementUI : MonoBehaviour
{
    [Header("UI 요소들")]
    public Text nameText;
    public Text descriptionText;
    public Text progressText;
    public Slider progressSlider;
    public Image completedIcon;
    public Button claimButton;
    
    private AchievementManager.Achievement achievementData;
    private AchievementManager achievementManager;
    
    public void SetupAchievement(AchievementManager.Achievement achievement, AchievementManager manager)
    {
        achievementData = achievement;
        achievementManager = manager;
        
        // UI 요소가 없으면 자동 생성
        if (nameText == null)
        {
            CreateAchievementUI();
        }
        
        UpdateUI(achievement);
        
        if (claimButton != null)
        {
            claimButton.onClick.AddListener(() => ClaimReward());
        }
    }
    
    public void UpdateUI(AchievementManager.Achievement achievement)
    {
        achievementData = achievement;
        
        nameText.text = $"{achievement.emoji} {achievement.name}";
        descriptionText.text = achievement.description;
        
        // 진행도 업데이트
        float progress = Mathf.Clamp01((float)achievement.currentValue / achievement.targetValue);
        progressSlider.value = progress;
        progressText.text = $"{achievement.currentValue} / {achievement.targetValue}";
        
        // 완료 상태 표시
        if (achievement.isCompleted)
        {
            completedIcon.gameObject.SetActive(true);
            completedIcon.color = Color.yellow;
            claimButton.gameObject.SetActive(false);
            
            // 배경색 변경 (완료된 업적)
            Image background = GetComponent<Image>();
            if (background != null)
            {
                background.color = new Color(1f, 1f, 0.7f, 0.3f); // 연한 노란색
            }
        }
        else if (achievement.currentValue >= achievement.targetValue)
        {
            // 완료되었지만 보상 미수령
            claimButton.gameObject.SetActive(true);
            claimButton.GetComponentInChildren<Text>().text = $"보상 받기! (+{achievement.coinReward})";
        }
        else
        {
            completedIcon.gameObject.SetActive(false);
            claimButton.gameObject.SetActive(false);
        }
    }
    
    void ClaimReward()
    {
        if (achievementData.currentValue >= achievementData.targetValue && !achievementData.isCompleted)
        {
            // 보상 수령
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddCoins(achievementData.coinReward);
            }
            
            achievementData.isCompleted = true;
            PlayerPrefs.SetInt($"Achievement_{achievementData.id}_completed", 1);
            PlayerPrefs.Save();
            
            UpdateUI(achievementData);
            
            // 사운드 재생
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCoinSound();
            }
        }
    }
    
    void CreateAchievementUI()
    {
        // 간단한 업적 UI 생성
        GameObject background = new GameObject("Background");
        background.transform.parent = transform;
        Image bg = background.AddComponent<Image>();
        bg.color = new Color(0.9f, 0.9f, 0.9f, 0.5f);
        
        // 이름 텍스트
        GameObject nameObj = new GameObject("NameText");
        nameObj.transform.parent = transform;
        nameText = nameObj.AddComponent<Text>();
        nameText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        nameText.fontSize = 18;
        nameText.color = Color.black;
        
        // 설명 텍스트
        GameObject descObj = new GameObject("DescriptionText");
        descObj.transform.parent = transform;
        descriptionText = descObj.AddComponent<Text>();
        descriptionText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        descriptionText.fontSize = 14;
        descriptionText.color = Color.gray;
        
        // 진행도 텍스트
        GameObject progressObj = new GameObject("ProgressText");
        progressObj.transform.parent = transform;
        progressText = progressObj.AddComponent<Text>();
        progressText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        progressText.fontSize = 12;
        progressText.color = Color.blue;
        
        // 진행도 슬라이더
        GameObject sliderObj = new GameObject("ProgressSlider");
        sliderObj.transform.parent = transform;
        progressSlider = sliderObj.AddComponent<Slider>();
        
        // 완료 아이콘
        GameObject iconObj = new GameObject("CompletedIcon");
        iconObj.transform.parent = transform;
        completedIcon = iconObj.AddComponent<Image>();
        completedIcon.color = Color.yellow;
        completedIcon.gameObject.SetActive(false);
        
        // 보상 수령 버튼
        GameObject buttonObj = new GameObject("ClaimButton");
        buttonObj.transform.parent = transform;
        claimButton = buttonObj.AddComponent<Button>();
        claimButton.gameObject.SetActive(false);
    }
}