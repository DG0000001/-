using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class test : MonoBehaviour
{
    public Transform object1Transform; // 第一个物体的Transform组件
   public  Transform object2Transform; // 第二个物体的Transform组件

    Vector3 worldPosition; // 第一个物体的世界坐标

    Vector3 modelPosition;
    Vector3 modelScale;
    private void Start()
    {
        worldPosition = object1Transform.position;
        modelPosition = object2Transform.InverseTransformPoint(worldPosition);
        Debug.Log("modelPosition:" + modelPosition);
    }
    private void Update()
    {
    }
}
