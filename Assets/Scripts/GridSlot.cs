using UnityEngine;

public class GridSlot : MonoBehaviour
{
    public int x, y;
    public MergeGame mergeGame;
    private MergeItem currentItem;
    
    void Start()
    {
        // 그리드 슬롯 시각적 표시
        GameObject visual = new GameObject("SlotVisual");
        visual.transform.parent = transform;
        visual.transform.localPosition = Vector3.zero;
        
        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.sprite = CreateSquareSprite();
        sr.color = new Color(0.8f, 0.9f, 0.8f, 0.3f); // 연한 녹색
        
        // 콜라이더 추가
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one;
    }
    
    public bool IsEmpty()
    {
        return currentItem == null;
    }
    
    public void SetItem(MergeItem item)
    {
        currentItem = item;
        item.currentSlot = this;
        item.transform.position = transform.position;
        item.transform.parent = transform;
    }
    
    public void RemoveItem()
    {
        if (currentItem != null)
        {
            Destroy(currentItem.gameObject);
            currentItem = null;
        }
    }
    
    public MergeItem GetItem()
    {
        return currentItem;
    }
    
    Sprite CreateSquareSprite()
    {
        Texture2D texture = new Texture2D(64, 64);
        for (int x = 0; x < 64; x++)
        {
            for (int y = 0; y < 64; y++)
            {
                if (x < 2 || x > 61 || y < 2 || y > 61)
                    texture.SetPixel(x, y, Color.white);
                else
                    texture.SetPixel(x, y, Color.clear);
            }
        }
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
    }
}