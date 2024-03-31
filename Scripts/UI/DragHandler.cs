using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector3 screenPoint;
    private Vector3 offset;

    // ����ʼ�϶�ʱ����
    public void OnBeginDrag(PointerEventData eventData)
    {
        // ��¼����λ��
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    // �϶������е���
    public void OnDrag(PointerEventData eventData)
    {
        // ��������λ��
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;


    }

    // �����϶�ʱ����
    public void OnEndDrag(PointerEventData eventData)
    {
    }
}
