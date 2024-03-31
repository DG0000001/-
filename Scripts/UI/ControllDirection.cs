using Spine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControllDirection : ScrollRect
{
    protected float mRadius; //摇杆范围半径
    public GameObject rote { set; private get; }
    public float Radius { get; private set; }
    public Vector2 direction { set; get; }

    public GameObject Actor;
    public bool isOnDrag { get; private set; } = false;
    protected override void Start()
    {
        base.Start(); //原Start方法
        Actor = GameObject.Find("Actor");
        //计算摇杆范围半径
        mRadius = (transform as RectTransform).sizeDelta.x * 0.25f;
        Radius = mRadius * 0.75f;
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);

        isOnDrag = true;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        isOnDrag = false;
    }
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData); //原OnDrag方法
        //摇杆位置
        direction = this.content.anchoredPosition;

        //如果摇杆位置超出范围，则限制在范围内
        if (direction.magnitude > mRadius)
        {
            direction = direction.normalized * mRadius;
        }
        Debug.Log("contentPosition.normalized:" + direction.normalized);

        SetContentAnchoredPosition(direction);
    }


    public void Cancel(GameObject gameObject)
    {
        AudioManager.Instance.PlayOneShot(Resources.Load<AudioClip>("Music/g_ui_back"), 100f, 0f);
        Destroy(rote);
        GameObject BG = GameObject.Find("BG");
        BG.GetComponent<Image>().raycastTarget = true;
        BG.SetActive(false);
        Time.timeScale = 1f;
        Actor.SetActive(true);
        GetComponent<RectTransform>().position = new Vector3(0,-100,0);
        Debug.Log("Cancel");
    }
}
