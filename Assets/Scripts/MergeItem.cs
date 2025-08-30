using UnityEngine;

public class MergeItem : MonoBehaviour
{
    public int itemType;
    public GridSlot currentSlot;
    
    private bool isDragging = false;
    private Vector3 startPosition;
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = Camera.main;
    }
    
    void OnMouseDown()
    {
        isDragging = true;
        startPosition = transform.position;
    }
    
    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            transform.position = mousePos;
        }
    }
    
    void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;
            
            // 가장 가까운 그리드 슬롯 찾기
            GridSlot nearestSlot = FindNearestSlot();
            
            if (nearestSlot != null && Vector3.Distance(transform.position, nearestSlot.transform.position) < 1f)
            {
                // 해당 슬롯에 아이템이 있는 경우 머지 시도
                if (!nearestSlot.IsEmpty())
                {
                    MergeItem otherItem = nearestSlot.GetItem();
                    if (FindObjectOfType<MergeGame>().TryMerge(this, otherItem))
                    {
                        return; // 머지 성공시 여기서 종료
                    }
                }
                else
                {
                    // 빈 슬롯으로 이동
                    currentSlot.RemoveItem();
                    nearestSlot.SetItem(this);
                    transform.position = nearestSlot.transform.position;
                    return;
                }
            }
            
            // 유효하지 않은 이동의 경우 원래 위치로 복귀
            transform.position = startPosition;
        }
    }
    
    GridSlot FindNearestSlot()
    {
        GridSlot nearestSlot = null;
        float minDistance = float.MaxValue;
        
        GridSlot[] allSlots = FindObjectsOfType<GridSlot>();
        foreach (GridSlot slot in allSlots)
        {
            float distance = Vector3.Distance(transform.position, slot.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestSlot = slot;
            }
        }
        
        return nearestSlot;
    }
}