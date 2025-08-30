using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    [Header("게임 매니저들")]
    public GameManager gameManager;
    public UIManager uiManager;
    public MergeGame mergeGame;
    public AudioManager audioManager;
    public FirebaseManager firebaseManager;
    public AdMobManager adMobManager;
    public TutorialManager tutorialManager;
    
    [Header("게임 설정")]
    public bool enableTutorial = true;
    public bool enableSound = true;
    public bool enableAnalytics = true;
    
    void Awake()
    {
        // 게임 프레임률 설정
        Application.targetFrameRate = 60;
        
        // 화면 꺼짐 방지
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        
        // 백그라운드에서도 실행
        Application.runInBackground = true;
    }
    
    void Start()
    {
        StartCoroutine(InitializeGame());
    }
    
    IEnumerator InitializeGame()
    {
        Debug.Log("🌱 코지 머지 농장 게임 시작!");
        
        // 1. 오디오 매니저 초기화
        if (enableSound && audioManager == null)
        {
            audioManager = AudioManager.Instance;
        }
        
        yield return new WaitForSeconds(0.1f);
        
        // 2. Firebase 초기화
        if (enableAnalytics && firebaseManager == null)
        {
            firebaseManager = FirebaseManager.Instance;
        }
        
        yield return new WaitForSeconds(0.1f);
        
        // 3. AdMob 초기화
        if (adMobManager == null)
        {
            adMobManager = AdMobManager.Instance;
        }
        
        yield return new WaitForSeconds(0.1f);
        
        // 4. 게임 매니저 초기화
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }
        
        yield return new WaitForSeconds(0.1f);
        
        // 5. UI 매니저 연결
        if (uiManager != null && gameManager != null)
        {
            ConnectUIToGameManager();
        }
        
        yield return new WaitForSeconds(0.1f);
        
        // 6. 머지 게임 로직 초기화
        if (mergeGame != null)
        {
            SetupMergeGameEvents();
        }
        
        yield return new WaitForSeconds(0.1f);
        
        // 7. 튜토리얼 체크
        if (enableTutorial && tutorialManager != null)
        {
            bool tutorialCompleted = PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;
            if (!tutorialCompleted)
            {
                yield return new WaitForSeconds(1f);
                tutorialManager.StartTutorial();
            }
        }
        
        Debug.Log("✅ 게임 초기화 완료!");
        
        // Firebase 이벤트 로그
        if (firebaseManager != null)
        {
            firebaseManager.LogEvent("game_initialized");
        }
    }
    
    void ConnectUIToGameManager()
    {
        // UI 이벤트와 게임 매니저 연결
        if (gameManager != null && uiManager != null)
        {
            // 초기 UI 업데이트
            uiManager.UpdateCoinsUI(gameManager.coins);
            uiManager.UpdateLevelUI(gameManager.level, (float)gameManager.experience / gameManager.experienceToNext);
        }
    }
    
    void SetupMergeGameEvents()
    {
        // 머지 이벤트 리스너 설정
        // 실제 구현에서는 UnityEvent나 System.Action을 사용
    }
    
    public void OnItemMerged(int itemType, int coinsEarned)
    {
        // 소리 재생
        if (audioManager != null)
        {
            audioManager.PlayMergeSound();
            audioManager.PlayCoinSound();
        }
        
        // UI 업데이트
        if (uiManager != null && gameManager != null)
        {
            int previousCoins = gameManager.coins;
            gameManager.AddCoins(coinsEarned);
            uiManager.UpdateCoinsUI(gameManager.coins, previousCoins);
        }
        
        // 애널리틱스 로그
        if (firebaseManager != null)
        {
            firebaseManager.LogMergeEvent(itemType, coinsEarned);
        }
        
        Debug.Log($"🌱 아이템 머지! 타입: {itemType}, 코인: {coinsEarned}");
    }
    
    public void OnLevelUp(int newLevel)
    {
        // 레벨업 소리
        if (audioManager != null)
        {
            audioManager.PlayLevelUpSound();
        }
        
        // UI 업데이트
        if (uiManager != null)
        {
            uiManager.UpdateLevelUI(newLevel, 0f);
        }
        
        // 애널리틱스 로그
        if (firebaseManager != null)
        {
            firebaseManager.LogLevelUpEvent(newLevel);
        }
        
        Debug.Log($"🌟 레벨업! 새 레벨: {newLevel}");
    }
    
    public void OnAdWatched(int coinsEarned)
    {
        // 광고 보상 소리
        if (audioManager != null)
        {
            audioManager.PlayAdRewardSound();
        }
        
        // UI 업데이트
        if (uiManager != null && gameManager != null)
        {
            int previousCoins = gameManager.coins;
            uiManager.UpdateCoinsUI(gameManager.coins, previousCoins);
        }
        
        // 애널리틱스 로그
        if (firebaseManager != null)
        {
            firebaseManager.LogAdWatchedEvent(coinsEarned);
        }
        
        Debug.Log($"🎁 광고 보상! 코인: {coinsEarned}");
    }
    
    public void OnButtonClicked()
    {
        // 버튼 클릭 소리
        if (audioManager != null)
        {
            audioManager.PlayButtonClickSound();
        }
    }
    
    // 게임 일시정지/재개
    public void PauseGame()
    {
        Time.timeScale = 0f;
        
        if (audioManager != null)
        {
            audioManager.musicSource.Pause();
        }
    }
    
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        
        if (audioManager != null)
        {
            audioManager.musicSource.UnPause();
        }
    }
    
    // 게임 데이터 저장
    public void SaveGame()
    {
        if (gameManager != null)
        {
            gameManager.SaveGameData();
        }
        
        if (firebaseManager != null)
        {
            firebaseManager.SavePlayerData(gameManager.coins, gameManager.level, gameManager.experience);
        }
        
        Debug.log("💾 게임 데이터 저장됨");
    }
    
    // 앱 생명주기 이벤트
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PauseGame();
            SaveGame();
        }
        else
        {
            ResumeGame();
        }
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveGame();
        }
    }
    
    void OnApplicationQuit()
    {
        SaveGame();
        
        if (firebaseManager != null)
        {
            firebaseManager.LogEvent("game_quit");
        }
        
        Debug.Log("👋 게임 종료");
    }
    
    // 디버그용 치트 코드
    void Update()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        // 디버그 모드에서만 작동
        if (Input.GetKeyDown(KeyCode.C))
        {
            // C키로 코인 1000개 추가
            if (gameManager != null)
            {
                gameManager.AddCoins(1000);
                if (uiManager != null)
                {
                    uiManager.UpdateCoinsUI(gameManager.coins);
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            // L키로 레벨업
            if (gameManager != null)
            {
                gameManager.experience = gameManager.experienceToNext;
                gameManager.CheckLevelUp();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            // T키로 튜토리얼 리셋
            PlayerPrefs.DeleteKey("TutorialCompleted");
            if (tutorialManager != null)
            {
                tutorialManager.StartTutorial();
            }
        }
        #endif
    }
}