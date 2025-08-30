using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MergeGame : MonoBehaviour
{
    [Header("게임 설정")]
    public int gridWidth = 5;
    public int gridHeight = 5;
    public GameObject itemPrefab;
    public Transform gridParent;
    
    [Header("농장 아이템들")]
    public Sprite[] farmItems; // 0: 씨앗, 1: 새싹, 2: 작은나무, 3: 큰나무, 4: 과일
    
    private GridSlot[,] grid;
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = Camera.main;
        CreateGrid();
        SpawnRandomItem();
    }
    
    void CreateGrid()
    {
        grid = new GridSlot[gridWidth, gridHeight];
        
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GameObject slot = new GameObject($"Slot_{x}_{y}");
                slot.transform.parent = gridParent;
                slot.transform.localPosition = new Vector3(x * 1.2f, y * 1.2f, 0);
                
                GridSlot gridSlot = slot.AddComponent<GridSlot>();
                gridSlot.x = x;
                gridSlot.y = y;
                gridSlot.mergeGame = this;
                
                grid[x, y] = gridSlot;
            }
        }
    }
    
    void SpawnRandomItem()
    {
        List<GridSlot> emptySlots = new List<GridSlot>();
        
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y].IsEmpty())
                {
                    emptySlots.Add(grid[x, y]);
                }
            }
        }
        
        if (emptySlots.Count > 0)
        {
            GridSlot randomSlot = emptySlots[Random.Range(0, emptySlots.Count)];
            CreateItem(randomSlot, 0); // 씨앗부터 시작
        }
    }
    
    public void CreateItem(GridSlot slot, int itemType)
    {
        if (itemType >= farmItems.Length) return;
        
        GameObject newItem = Instantiate(itemPrefab, slot.transform);
        newItem.transform.localPosition = Vector3.zero;
        
        MergeItem mergeItem = newItem.GetComponent<MergeItem>();
        mergeItem.itemType = itemType;
        mergeItem.GetComponent<SpriteRenderer>().sprite = farmItems[itemType];
        
        slot.SetItem(mergeItem);
    }
    
    public bool TryMerge(MergeItem item1, MergeItem item2)
    {
        if (item1.itemType == item2.itemType && item1.itemType < farmItems.Length - 1)
        {
            // 아이템1 위치에 업그레이드된 아이템 생성
            GridSlot slot1 = item1.currentSlot;
            GridSlot slot2 = item2.currentSlot;
            
            // 기존 아이템들 제거
            slot1.RemoveItem();
            slot2.RemoveItem();
            
            // 새로운 아이템 생성
            CreateItem(slot1, item1.itemType + 1);
            
            // 코인 획득
            GameManager.Instance.AddCoins(GetCoinReward(item1.itemType + 1));
            
            // 새 아이템 스폰
            StartCoroutine(SpawnNewItemDelayed());
            
            return true;
        }
        return false;
    }
    
    int GetCoinReward(int itemType)
    {
        return Mathf.Pow(2, itemType) * 10; // 레벨이 높을수록 기하급수적으로 증가
    }
    
    IEnumerator SpawnNewItemDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        SpawnRandomItem();
    }
    
    void Update()
    {
        // 자동 아이템 생성 (5초마다)
        if (Time.time % 5f < Time.deltaTime)
        {
            SpawnRandomItem();
        }
    }
}