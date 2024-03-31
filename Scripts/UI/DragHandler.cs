using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector3 screenPoint;
    private Vector3 offset;

    // 当开始拖动时调用
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 记录鼠标的位置
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    // 拖动过程中调用
    public void OnDrag(PointerEventData eventData)
    {
        // 更新鼠标的位置
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;


    }

    // 结束拖动时调用
    public void OnEndDrag(PointerEventData eventData)
    {
    }
}
