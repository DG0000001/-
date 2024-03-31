using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllowEnemiesPassDetection : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
            GamePageManage.Instance.AllowEnemiesPassCount.GetComponent<Text>().text = (int.Parse(GamePageManage.Instance.AllowEnemiesPassCount.GetComponent<Text>().text) - 1).ToString();
    }
}
