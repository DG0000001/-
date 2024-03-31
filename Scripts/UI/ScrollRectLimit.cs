using UnityEngine;
using UnityEngine.UI;

public class ScrollRectLimit : MonoBehaviour
{
    //限制滚动的范围
    public Vector2 leftPos;
    public Vector2 rightPos;
    //限制滚动的速度
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