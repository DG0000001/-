using UnityEngine;
using UnityEngine.UI;

public class ScrollRectLimit : MonoBehaviour
{
    //���ƹ����ķ�Χ
    public Vector2 leftPos;
    public Vector2 rightPos;
    //���ƹ������ٶ�
    public float speed = 1;
    private void Start()
    {
        leftPos = transform.position;   
    }

    private void LateUpdate()
    {
    }

    private void LimitScrollPosition()
    {

    }
}