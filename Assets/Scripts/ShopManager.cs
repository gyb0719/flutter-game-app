using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("상점 UI")]
    public Transform shopItemsParent;
    public GameObject shopItemPrefab;
    public Button closeButton;
    
    [Header("아이템 데이터")]
    public ShopItemData[] shopItems;
    
    private List<ShopItem> spawnedItems = new List<ShopItem>();
    
    [System.Serializable]
    public class ShopItemData
    {
        public string name;
        public string description;
        public int coinPrice;
        public int gemPrice;
        public Sprite icon;
        public ShopItemType itemType;
        public int value;
    }
    
    public enum ShopItemType
    {
        CoinPack,
        GemPack,
        EnergyPack,
        AutoMerge,
        DoubleCoins,
        NoAds
    }
    
    void Start()
    {
        SetupShopItems();
        closeButton.onClick.AddListener(CloseShop);
    }
    
    void SetupShopItems()
    {
        shopItems = new ShopItemData[]
        {
            new ShopItemData 
            { 
                name = "🪙 코인 팩 (소)", 
                description = "1,000 코인을 획득하세요!",
                coinPrice = 0,
                gemPrice = 10,
                itemType = ShopItemType.CoinPack,
                value = 1000
            },
            new ShopItemData 
            { 
                name = "🪙 코인 팩 (대)", 
                description = "10,000 코인을 획득하세요!",
                coinPrice = 0,
                gemPrice = 80,
                itemType = ShopItemType.CoinPack,
                value = 10000
            },
            new ShopItemData 
            { 
                name = "⚡ 에너지 충전", 
                description = "에너지를 완전히 충전합니다!",
                coinPrice = 500,
                gemPrice = 0,
                itemType = ShopItemType.EnergyPack,
                value = 100
            },
            new ShopItemData 
            { 
                name = "🤖 자동 머지", 
                description = "30분간 자동으로 머지해줍니다!",
                coinPrice = 2000,
                gemPrice = 0,
                itemType = ShopItemType.AutoMerge,
                value = 30
            },
            new ShopItemData 
            { 
                name = "💰 더블 코인", 
                description = "1시간동안 코인을 2배로 획득!",
                coinPrice = 1500,
                gemPrice = 0,
                itemType = ShopItemType.DoubleCoins,
                value = 60
            },
            new ShopItemData 
            { 
                name = "🚫 광고 제거", 
                description = "모든 광고를 영구히 제거합니다!",
                coinPrice = 0,
                gemPrice = 299,
                itemType = ShopItemType.NoAds,
                value = 1
            }
        };
        
        CreateShopItems();
    }
    
    void CreateShopItems()
    {
        foreach (ShopItemData data in shopItems)
        {
            GameObject itemObj = Instantiate(shopItemPrefab, shopItemsParent);
            ShopItem shopItem = itemObj.GetComponent<ShopItem>();
            
            if (shopItem == null)
            {
                shopItem = itemObj.AddComponent<ShopItem>();
            }
            
            shopItem.SetupItem(data, this);
            spawnedItems.Add(shopItem);
        }
    }
    
    public void PurchaseItem(ShopItemData item)
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager == null) return;
        
        bool canPurchase = false;
        
        // 구매 가능 여부 체크
        if (item.coinPrice > 0 && gameManager.coins >= item.coinPrice)
        {
            canPurchase = true;
        }
        else if (item.gemPrice > 0) // 젬은 실제로는 IAP로 구매
        {
            canPurchase = true; // 임시로 항상 구매 가능
        }
        
        if (!canPurchase)
        {
            ShowMessage("코인이 부족합니다! 💸");
            return;
        }
        
        // 코인 차감
        if (item.coinPrice > 0)
        {
            gameManager.SpendCoins(item.coinPrice);
        }
        
        // 아이템 효과 적용
        ApplyItemEffect(item);
        
        // 구매 완료 메시지
        ShowMessage($"{item.name} 구매 완료! ✨");
        
        // 사운드 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCoinSound();
        }
        
        // 애널리틱스 로그
        if (FirebaseManager.Instance != null)
        {
            var parameters = new System.Collections.Generic.Dictionary<string, object>
            {
                {"item_name", item.name},
                {"item_type", item.itemType.ToString()},
                {"coin_price", item.coinPrice},
                {"gem_price", item.gemPrice}
            };
            FirebaseManager.Instance.LogEvent("shop_purchase", parameters);
        }
    }
    
    void ApplyItemEffect(ShopItemData item)
    {
        GameManager gameManager = GameManager.Instance;
        
        switch (item.itemType)
        {
            case ShopItemType.CoinPack:
                gameManager.AddCoins(item.value);
                break;
                
            case ShopItemType.EnergyPack:
                // 에너지 시스템이 있다면 충전
                PlayerPrefs.SetInt("Energy", 100);
                break;
                
            case ShopItemType.AutoMerge:
                // 자동 머지 활성화
                PlayerPrefs.SetFloat("AutoMergeEndTime", Time.time + (item.value * 60f));
                StartAutoMerge(item.value * 60f);
                break;
                
            case ShopItemType.DoubleCoins:
                // 더블 코인 활성화
                PlayerPrefs.SetFloat("DoubleCoinsEndTime", Time.time + (item.value * 60f));
                break;
                
            case ShopItemType.NoAds:
                // 광고 제거
                PlayerPrefs.SetInt("NoAds", 1);
                break;
        }
        
        PlayerPrefs.Save();
    }
    
    void StartAutoMerge(float duration)
    {
        StartCoroutine(AutoMergeCoroutine(duration));
    }
    
    System.Collections.IEnumerator AutoMergeCoroutine(float duration)
    {
        float endTime = Time.time + duration;
        MergeGame mergeGame = FindObjectOfType<MergeGame>();
        
        while (Time.time < endTime)
        {
            if (mergeGame != null)
            {
                // 자동으로 머지 가능한 아이템들 찾아서 머지
                MergeItem[] allItems = FindObjectsOfType<MergeItem>();
                
                for (int i = 0; i < allItems.Length - 1; i++)
                {
                    for (int j = i + 1; j < allItems.Length; j++)
                    {
                        if (allItems[i].itemType == allItems[j].itemType && 
                            allItems[i].itemType < 4) // 최고 레벨이 아닐 때만
                        {
                            mergeGame.TryMerge(allItems[i], allItems[j]);
                            yield return new WaitForSeconds(1f); // 1초마다 머지
                            break;
                        }
                    }
                }
            }
            
            yield return new WaitForSeconds(2f); // 2초마다 체크
        }
        
        ShowMessage("🤖 자동 머지 종료!");
    }
    
    void ShowMessage(string message)
    {
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowTemporaryMessage(message, 2f);
        }
        else
        {
            Debug.Log(message);
        }
    }
    
    void CloseShop()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
        
        gameObject.SetActive(false);
    }
    
    // 더블 코인 효과가 활성화되어 있는지 체크
    public static bool IsDoubleCoinsActive()
    {
        float endTime = PlayerPrefs.GetFloat("DoubleCoinsEndTime", 0f);
        return Time.time < endTime;
    }
    
    // 광고 제거 구매 여부 체크
    public static bool IsNoAdsActive()
    {
        return PlayerPrefs.GetInt("NoAds", 0) == 1;
    }
}

public class ShopItem : MonoBehaviour
{
    [Header("UI 요소들")]
    public Text nameText;
    public Text descriptionText;
    public Text priceText;
    public Image iconImage;
    public Button purchaseButton;
    
    private ShopManager.ShopItemData itemData;
    private ShopManager shopManager;
    
    public void SetupItem(ShopManager.ShopItemData data, ShopManager manager)
    {
        itemData = data;
        shopManager = manager;
        
        // UI 요소가 없으면 자동 생성
        if (nameText == null)
        {
            CreateItemUI();
        }
        
        // 데이터 설정
        nameText.text = data.name;
        descriptionText.text = data.description;
        
        if (data.coinPrice > 0)
        {
            priceText.text = $"🪙 {data.coinPrice:N0}";
        }
        else if (data.gemPrice > 0)
        {
            priceText.text = $"💎 {data.gemPrice}";
        }
        
        // 아이콘 설정 (없으면 기본 이모지)
        if (iconImage != null && data.icon != null)
        {
            iconImage.sprite = data.icon;
        }
        
        // 구매 버튼 이벤트
        purchaseButton.onClick.AddListener(() => shopManager.PurchaseItem(itemData));
    }
    
    void CreateItemUI()
    {
        // 간단한 UI 레이아웃 생성
        GameObject background = new GameObject("Background");
        background.transform.parent = transform;
        Image bg = background.AddComponent<Image>();
        bg.color = new Color(0.8f, 0.9f, 0.8f, 0.3f);
        
        // 이름 텍스트
        GameObject nameObj = new GameObject("NameText");
        nameObj.transform.parent = transform;
        nameText = nameObj.AddComponent<Text>();
        nameText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        nameText.fontSize = 20;
        nameText.color = Color.black;
        
        // 설명 텍스트
        GameObject descObj = new GameObject("DescriptionText");
        descObj.transform.parent = transform;
        descriptionText = descObj.AddComponent<Text>();
        descriptionText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        descriptionText.fontSize = 14;
        descriptionText.color = Color.gray;
        
        // 가격 텍스트
        GameObject priceObj = new GameObject("PriceText");
        priceObj.transform.parent = transform;
        priceText = priceObj.AddComponent<Text>();
        priceText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        priceText.fontSize = 18;
        priceText.color = Color.blue;
        
        // 구매 버튼
        GameObject buttonObj = new GameObject("PurchaseButton");
        buttonObj.transform.parent = transform;
        purchaseButton = buttonObj.AddComponent<Button>();
        Image buttonImg = buttonObj.AddComponent<Image>();
        buttonImg.color = new Color(0.2f, 0.8f, 0.2f, 0.8f);
        
        Text buttonText = new GameObject("ButtonText").AddComponent<Text>();
        buttonText.transform.parent = buttonObj.transform;
        buttonText.text = "구매";
        buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        buttonText.fontSize = 16;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;
    }
}