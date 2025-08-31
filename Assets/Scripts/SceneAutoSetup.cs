using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneAutoSetup : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("CozyMergeFarm/Auto Setup Scene")]
    public static void AutoSetupScene()
    {
        Debug.Log("🌱 코지 머지 농장 자동 씬 설정 시작!");

        // 기존 오브젝트들 정리
        GameObject[] existingObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in existingObjects)
        {
            if (obj.name.Contains("GameController") || obj.name.Contains("Canvas"))
            {
                DestroyImmediate(obj);
            }
        }

        CreateGameController();
        CreateUICanvas();
        CreateGridSystem();

        Debug.Log("✅ 자동 씬 설정 완료! Play 버튼을 누르세요!");
    }

    static void CreateGameController()
    {
        // GameController 생성
        GameObject gameController = new GameObject("GameController");
        
        // 모든 매니저 컴포넌트 추가
        GameController gc = gameController.AddComponent<GameController>();
        GameManager gm = gameController.AddComponent<GameManager>();
        UIManager um = gameController.AddComponent<UIManager>();
        MergeGame mg = gameController.AddComponent<MergeGame>();
        AudioManager am = gameController.AddComponent<AudioManager>();
        FirebaseManager fm = gameController.AddComponent<FirebaseManager>();
        AdMobManager admob = gameController.AddComponent<AdMobManager>();

        // AudioSource들 추가
        AudioSource musicSource = gameController.AddComponent<AudioSource>();
        AudioSource sfxSource = gameController.AddComponent<AudioSource>();
        AudioSource ambientSource = gameController.AddComponent<AudioSource>();

        // AudioManager 설정
        am.musicSource = musicSource;
        am.sfxSource = sfxSource;
        am.ambientSource = ambientSource;

        // 음악 소스 설정
        musicSource.loop = true;
        musicSource.volume = 0.6f;
        musicSource.playOnAwake = false;

        // 효과음 소스 설정
        sfxSource.loop = false;
        sfxSource.volume = 0.8f;
        sfxSource.playOnAwake = false;

        // 환경음 소스 설정
        ambientSource.loop = false;
        ambientSource.volume = 0.3f;
        ambientSource.playOnAwake = false;

        // GameController 연결
        gc.gameManager = gm;
        gc.uiManager = um;
        gc.mergeGame = mg;
        gc.audioManager = am;
        gc.firebaseManager = fm;
        gc.adMobManager = admob;

        Debug.Log("✅ GameController 생성 완료!");
    }

    static void CreateUICanvas()
    {
        // Canvas 생성
        GameObject canvasObj = new GameObject("GameCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        GraphicRaycaster raycaster = canvasObj.AddComponent<GraphicRaycaster>();

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        // 코인 텍스트
        GameObject coinsTextObj = CreateUIText("CoinsText", canvasObj.transform, "🪙 0");
        RectTransform coinsRect = coinsTextObj.GetComponent<RectTransform>();
        coinsRect.anchorMin = new Vector2(0, 1);
        coinsRect.anchorMax = new Vector2(0.5f, 1);
        coinsRect.anchoredPosition = new Vector2(50, -50);
        coinsRect.sizeDelta = new Vector2(400, 80);
        Text coinsText = coinsTextObj.GetComponent<Text>();
        coinsText.fontSize = 36;
        coinsText.fontStyle = FontStyle.Bold;
        coinsText.color = new Color(1f, 0.9f, 0.3f);

        // 레벨 텍스트  
        GameObject levelTextObj = CreateUIText("LevelText", canvasObj.transform, "🌟 레벨 1");
        RectTransform levelRect = levelTextObj.GetComponent<RectTransform>();
        levelRect.anchorMin = new Vector2(0.5f, 1);
        levelRect.anchorMax = new Vector2(1, 1);
        levelRect.anchoredPosition = new Vector2(-50, -50);
        levelRect.sizeDelta = new Vector2(400, 80);
        Text levelText = levelTextObj.GetComponent<Text>();
        levelText.fontSize = 36;
        levelText.fontStyle = FontStyle.Bold;
        levelText.color = new Color(0.4f, 0.7f, 1f);
        levelText.alignment = TextAnchor.MiddleRight;

        // 경험치 바
        GameObject expBarObj = CreateProgressBar("ExperienceBar", canvasObj.transform);
        RectTransform expRect = expBarObj.GetComponent<RectTransform>();
        expRect.anchorMin = new Vector2(0.1f, 1);
        expRect.anchorMax = new Vector2(0.9f, 1);
        expRect.anchoredPosition = new Vector2(0, -120);
        expRect.sizeDelta = new Vector2(0, 20);

        // 농장 이름 텍스트
        GameObject farmNameObj = CreateUIText("FarmNameText", canvasObj.transform, "🌻 해바라기 농장");
        RectTransform farmRect = farmNameObj.GetComponent<RectTransform>();
        farmRect.anchorMin = new Vector2(0, 1);
        farmRect.anchorMax = new Vector2(1, 1);
        farmRect.anchoredPosition = new Vector2(0, -160);
        farmRect.sizeDelta = new Vector2(0, 60);
        Text farmText = farmNameObj.GetComponent<Text>();
        farmText.fontSize = 28;
        farmText.fontStyle = FontStyle.Bold;
        farmText.color = new Color(0.3f, 0.6f, 0.3f);
        farmText.alignment = TextAnchor.MiddleCenter;

        // 광고 버튼
        GameObject adButtonObj = CreateUIButton("AdRewardButton", canvasObj.transform, "🎁 광고 보고\n코인 받기!");
        RectTransform adRect = adButtonObj.GetComponent<RectTransform>();
        adRect.anchorMin = new Vector2(0.7f, 0);
        adRect.anchorMax = new Vector2(1, 0);
        adRect.anchoredPosition = new Vector2(-20, 100);
        adRect.sizeDelta = new Vector2(200, 100);
        
        // 게임플레이 패널 (빈 패널)
        GameObject gameplayPanel = new GameObject("GameplayPanel");
        gameplayPanel.transform.SetParent(canvasObj.transform, false);
        CanvasGroup cg = gameplayPanel.AddComponent<CanvasGroup>();
        RectTransform gpRect = gameplayPanel.AddComponent<RectTransform>();
        gpRect.anchorMin = Vector2.zero;
        gpRect.anchorMax = Vector2.one;
        gpRect.offsetMin = Vector2.zero;
        gpRect.offsetMax = Vector2.zero;

        // 파티클 컨테이너
        GameObject particleContainer = new GameObject("ParticleContainer");
        particleContainer.transform.SetParent(canvasObj.transform, false);
        RectTransform pcRect = particleContainer.AddComponent<RectTransform>();
        pcRect.anchorMin = Vector2.zero;
        pcRect.anchorMax = Vector2.one;
        pcRect.offsetMin = Vector2.zero;
        pcRect.offsetMax = Vector2.zero;

        // 떠다니는 코인 컨테이너
        GameObject floatingContainer = new GameObject("FloatingCoinsContainer");
        floatingContainer.transform.SetParent(canvasObj.transform, false);
        RectTransform fcRect = floatingContainer.AddComponent<RectTransform>();
        fcRect.anchorMin = Vector2.zero;
        fcRect.anchorMax = Vector2.one;
        fcRect.offsetMin = Vector2.zero;
        fcRect.offsetMax = Vector2.zero;

        // GameManager와 UIManager에 UI 요소들 연결
        GameManager gameManager = FindObjectOfType<GameManager>();
        UIManager uiManager = FindObjectOfType<UIManager>();
        AdMobManager adMobManager = FindObjectOfType<AdMobManager>();

        if (gameManager != null)
        {
            gameManager.coinsText = coinsText;
            gameManager.levelText = levelText;
            gameManager.watchAdButton = adButtonObj.GetComponent<Button>();
        }

        if (uiManager != null)
        {
            uiManager.gameplayPanel = cg;
            uiManager.coinsText = coinsText;
            uiManager.levelText = levelText;
            uiManager.experienceBar = expBarObj.GetComponent<Slider>();
            uiManager.adRewardButton = adButtonObj.GetComponent<Button>();
            uiManager.farmNameText = farmText;
            uiManager.particleContainer = particleContainer.transform;
            uiManager.floatingCoinsContainer = floatingContainer.transform;
        }

        if (adMobManager != null)
        {
            adMobManager.rewardedAdButton = adButtonObj.GetComponent<Button>();
            adMobManager.adButtonText = adButtonObj.GetComponentInChildren<Text>();
        }

        Debug.Log("✅ UI Canvas 생성 완료!");
    }

    static void CreateGridSystem()
    {
        // 그리드 부모 오브젝트
        GameObject gridParent = new GameObject("GridParent");
        gridParent.transform.position = new Vector3(-2f, -2f, 0f);

        // MergeGame에 연결
        MergeGame mergeGame = FindObjectOfType<MergeGame>();
        if (mergeGame != null)
        {
            mergeGame.gridWidth = 5;
            mergeGame.gridHeight = 5;
            mergeGame.gridParent = gridParent.transform;
        }

        Debug.Log("✅ 그리드 시스템 생성 완료!");
    }

    static GameObject CreateUIText(string name, Transform parent, string text)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        
        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontSize = 24;
        textComponent.color = Color.white;
        textComponent.alignment = TextAnchor.MiddleLeft;

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 50);

        return textObj;
    }

    static GameObject CreateUIButton(string name, Transform parent, string text)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);

        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.8f, 0.2f, 0.8f);

        Button button = buttonObj.AddComponent<Button>();
        
        // 버튼 텍스트
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontSize = 16;
        textComponent.color = Color.white;
        textComponent.alignment = TextAnchor.MiddleCenter;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(150, 50);

        return buttonObj;
    }

    static GameObject CreateProgressBar(string name, Transform parent)
    {
        GameObject sliderObj = new GameObject(name);
        sliderObj.transform.SetParent(parent, false);

        // 배경
        Image bgImage = sliderObj.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);

        // 슬라이더 컴포넌트
        Slider slider = sliderObj.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0f;

        // Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = Vector2.zero;
        fillAreaRect.offsetMax = Vector2.zero;

        // Fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(1f, 0.9f, 0.3f, 1f);
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        slider.fillRect = fillRect;

        RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
        sliderRect.sizeDelta = new Vector2(200, 20);

        return sliderObj;
    }

    [MenuItem("CozyMergeFarm/Create Merge Item Prefab")]
    public static void CreateMergeItemPrefab()
    {
        // 머지 아이템 프리팹 생성
        GameObject prefab = new GameObject("MergeItem");
        
        // SpriteRenderer 추가
        SpriteRenderer sr = prefab.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Items";
        
        // 기본 스프라이트 생성 (흰색 원)
        Texture2D texture = new Texture2D(64, 64);
        for (int x = 0; x < 64; x++)
        {
            for (int y = 0; y < 64; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(32, 32));
                if (distance < 30)
                {
                    texture.SetPixel(x, y, new Color(0.3f, 0.8f, 0.3f, 1f));
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }
        texture.Apply();
        
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
        sr.sprite = sprite;
        
        // MergeItem 스크립트 추가
        prefab.AddComponent<MergeItem>();
        
        // BoxCollider2D 추가
        BoxCollider2D collider = prefab.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one;
        
        // 프리팹으로 저장
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        
        GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(prefab, "Assets/Prefabs/MergeItem.prefab");
        DestroyImmediate(prefab);
        
        // MergeGame에 프리팹 연결
        MergeGame mergeGame = FindObjectOfType<MergeGame>();
        if (mergeGame != null)
        {
            mergeGame.itemPrefab = savedPrefab;
        }
        
        Debug.Log("✅ 머지 아이템 프리팹 생성 완료!");
    }

    [MenuItem("CozyMergeFarm/Full Auto Setup")]
    public static void FullAutoSetup()
    {
        AutoSetupScene();
        CreateMergeItemPrefab();
        
        // 씬 저장
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        
        Debug.Log("🎉 코지 머지 농장 완전 자동 설정 완료!\n▶️ Play 버튼을 눌러 게임을 실행하세요!");
    }
#endif
}