using System.Collections;
using UnityEngine;

// Firebase 설치 후 주석 해제
/*
using Firebase;
using Firebase.Analytics;
using Firebase.Database;
using Firebase.Extensions;
*/

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;
    
    // Firebase 초기화 상태
    private bool firebaseInitialized = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFirebase();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeFirebase()
    {
        // Firebase 설치 전에는 이 부분이 주석처리됨
        /*
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                firebaseInitialized = true;
                Debug.Log("Firebase 초기화 성공!");
                
                // 애널리틱스 시작
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                
                // 첫 실행 이벤트 로그
                LogEvent("game_start", null);
            }
            else
            {
                Debug.LogError($"Firebase 초기화 실패: {dependencyStatus}");
            }
        });
        */
        
        // 임시로 초기화 성공으로 설정 (개발용)
        firebaseInitialized = true;
        Debug.Log("Firebase 임시 초기화 (개발 모드)");
    }
    
    public void LogEvent(string eventName, System.Collections.Generic.Dictionary<string, object> parameters = null)
    {
        if (!firebaseInitialized) return;
        
        Debug.Log($"Firebase 이벤트: {eventName}");
        
        // Firebase Analytics 이벤트 로그
        /*
        if (parameters != null)
        {
            FirebaseAnalytics.LogEvent(eventName, parameters.Select(kvp => 
                new Parameter(kvp.Key, kvp.Value.ToString())).ToArray());
        }
        else
        {
            FirebaseAnalytics.LogEvent(eventName);
        }
        */
    }
    
    public void SavePlayerData(int coins, int level, int experience)
    {
        if (!firebaseInitialized) return;
        
        // Firebase Realtime Database에 플레이어 데이터 저장
        /*
        string userId = SystemInfo.deviceUniqueIdentifier;
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        
        var playerData = new Dictionary<string, object>
        {
            {"coins", coins},
            {"level", level},
            {"experience", experience},
            {"lastSave", ServerValue.Timestamp}
        };
        
        reference.Child("players").Child(userId).SetValueAsync(playerData).ContinueWithOnMainThread(task => {
            if (task.IsCompletedSuccessfully)
            {
                Debug.Log("플레이어 데이터 저장 성공!");
            }
            else
            {
                Debug.LogError($"데이터 저장 실패: {task.Exception}");
            }
        });
        */
        
        Debug.Log($"플레이어 데이터 저장: 코인={coins}, 레벨={level}, 경험치={experience}");
    }
    
    public void LoadPlayerData(System.Action<int, int, int> onDataLoaded)
    {
        if (!firebaseInitialized)
        {
            // 로컬 데이터 로드
            int coins = PlayerPrefs.GetInt("Coins", 0);
            int level = PlayerPrefs.GetInt("Level", 1);
            int experience = PlayerPrefs.GetInt("Experience", 0);
            onDataLoaded?.Invoke(coins, level, experience);
            return;
        }
        
        // Firebase에서 플레이어 데이터 로드
        /*
        string userId = SystemInfo.deviceUniqueIdentifier;
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        
        reference.Child("players").Child(userId).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    int coins = int.Parse(snapshot.Child("coins").Value.ToString());
                    int level = int.Parse(snapshot.Child("level").Value.ToString());
                    int experience = int.Parse(snapshot.Child("experience").Value.ToString());
                    
                    onDataLoaded?.Invoke(coins, level, experience);
                    Debug.Log("클라우드 데이터 로드 성공!");
                }
                else
                {
                    // 클라우드에 데이터 없음, 로컬 데이터 사용
                    int coins = PlayerPrefs.GetInt("Coins", 0);
                    int level = PlayerPrefs.GetInt("Level", 1);
                    int experience = PlayerPrefs.GetInt("Experience", 0);
                    onDataLoaded?.Invoke(coins, level, experience);
                }
            }
            else
            {
                Debug.LogError($"데이터 로드 실패: {task.Exception}");
                // 에러 시 로컬 데이터 사용
                int coins = PlayerPrefs.GetInt("Coins", 0);
                int level = PlayerPrefs.GetInt("Level", 1);
                int experience = PlayerPrefs.GetInt("Experience", 0);
                onDataLoaded?.Invoke(coins, level, experience);
            }
        });
        */
    }
    
    // 게임 이벤트 로깅
    public void LogMergeEvent(int itemType, int coinsEarned)
    {
        var parameters = new System.Collections.Generic.Dictionary<string, object>
        {
            {"item_type", itemType},
            {"coins_earned", coinsEarned}
        };
        LogEvent("item_merged", parameters);
    }
    
    public void LogLevelUpEvent(int newLevel)
    {
        var parameters = new System.Collections.Generic.Dictionary<string, object>
        {
            {"new_level", newLevel}
        };
        LogEvent("level_up", parameters);
    }
    
    public void LogAdWatchedEvent(int coinsEarned)
    {
        var parameters = new System.Collections.Generic.Dictionary<string, object>
        {
            {"coins_earned", coinsEarned}
        };
        LogEvent("ad_watched", parameters);
    }
}