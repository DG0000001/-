using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class test : MonoBehaviour
{
    public Transform object1Transform; // ��һ�������Transform���
   public  Transform object2Transform; // �ڶ��������Transform���

    Vector3 worldPosition; // ��һ���������������

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
