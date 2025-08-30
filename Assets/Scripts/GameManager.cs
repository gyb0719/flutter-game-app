using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("UI 요소들")]
    public Text coinsText;
    public Text levelText;
    public Button watchAdButton;
    
    [Header("게임 데이터")]
    public int coins = 0;
    public int level = 1;
    public int experience = 0;
    public int experienceToNext = 100;
    
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
        UpdateUI();
        LoadGameData();
        
        // 광고 버튼 설정
        if (watchAdButton != null)
        {
            watchAdButton.onClick.AddListener(ShowRewardedAd);
        }
    }
    
    public void AddCoins(int amount)
    {
        coins += amount;
        experience += amount / 10; // 코인 10개당 경험치 1
        
        CheckLevelUp();
        UpdateUI();
        SaveGameData();
    }
    
    public void SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            UpdateUI();
            SaveGameData();
        }
    }
    
    void CheckLevelUp()
    {
        while (experience >= experienceToNext)
        {
            experience -= experienceToNext;
            level++;
            experienceToNext = Mathf.RoundToInt(experienceToNext * 1.2f); // 20% 증가
            
            // 레벨업 보상
            coins += level * 50;
            
            Debug.Log($"레벨업! 현재 레벨: {level}");
        }
    }
    
    void UpdateUI()
    {
        if (coinsText != null)
            coinsText.text = $"코인: {coins:N0}";
            
        if (levelText != null)
            levelText.text = $"레벨: {level}";
    }
    
    public void ShowRewardedAd()
    {
        // AdMob 보상형 광고 표시
        // 광고 시청 완료 시 OnAdWatched() 호출
        Debug.Log("광고 시청 중...");
        OnAdWatched(); // 임시로 바로 보상 지급
    }
    
    public void OnAdWatched()
    {
        int reward = Mathf.RoundToInt(coins * 0.1f) + 100; // 현재 코인의 10% + 기본 100
        AddCoins(reward);
        
        Debug.Log($"광고 보상: {reward} 코인!");
    }
    
    void SaveGameData()
    {
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.SetInt("Experience", experience);
        PlayerPrefs.SetInt("ExperienceToNext", experienceToNext);
        PlayerPrefs.Save();
    }
    
    void LoadGameData()
    {
        coins = PlayerPrefs.GetInt("Coins", 0);
        level = PlayerPrefs.GetInt("Level", 1);
        experience = PlayerPrefs.GetInt("Experience", 0);
        experienceToNext = PlayerPrefs.GetInt("ExperienceToNext", 100);
        
        UpdateUI();
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            SaveGameData();
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            SaveGameData();
    }
}