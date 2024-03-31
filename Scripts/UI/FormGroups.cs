using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormGroups : MonoBehaviour
{
    public GameObject HomePage;
    public GameObject SelectActor;
    public GameObject FastGroup;
    //�󶨵����ذ�ť
    public void Back()
    {
        AudioManager.Instance.PlayOneShot(Resources.Load<AudioClip>("Music/g_ui_back"), 100f, 0f);
        gameObject.SetActive(false);
        HomePage.SetActive(true);
    }
    //�󶨵���ɰ�ť
    public void Finish()
    {
        AudioManager.Instance.PlayOneShot(Resources.Load<AudioClip>("Music/g_ui_confirm"), 100f, 0f);
        gameObject.SetActive(false);
        HomePage.SetActive(true);
        foreach (var item in DataManage.Instance.GetSelectedRoteNames())
        {
            Debug.Log("GetSelectedRoteNames:" + item);
        }
    }
    //��ݱ��onclick�󶨺���
    public void QuickFormation()
    {
        AudioManager.Instance.PlayOneShot(Resources.Load<AudioClip>("Music/g_ui_btn_n"), 100f, 0f);
        FastGroup.SetActive(true);
        gameObject.SetActive(false);
    }
    //�󶨵�ÿ��ѡ�н�ɫ��ť
    public void SelectRote(GameObject obj)
    {
        AudioManager.Instance.PlayOneShot(Resources.Load<AudioClip>("Music/g_ui_btn_n"), 100f, 0f);
        gameObject.SetActive(false);
        SelectActor.GetComponent<SelectActor>().SetEnterBox(obj);
        SelectActor.SetActive(true);
    }
}
