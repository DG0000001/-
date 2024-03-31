using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FastGroup : MonoBehaviour
{
    public GameObject FormGroups;
    public GameObject roteTeam;
    public GameObject container;
    public GameObject actorState;

    List<RoteData> roteData;
    List<string> olderRoteNames;
    List<string> roteNames;
    List<string> roteNamesTemporarily;
    GameObject selectBox;

    void Awake()
    {
        roteNames = DataManage.Instance.GetSelectedRoteNames();

        roteNamesTemporarily = new List<string>(roteNames);
        olderRoteNames = new List<string>(roteNames);

        roteData = DataManage.Instance.GetRoteData();
        selectBox = Resources.Load<GameObject>("Prefabs/SelectBox");
        GenerateAllKnownRote();
    }
    private void GenerateAllKnownRote()//����������֪��ɫ
    {
        foreach (RoteData rote in roteData)
        {
            GameObject singleSelectBox = Instantiate(selectBox);

            EventTrigger eventTrigger = singleSelectBox.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerUp;
            entry.callback.AddListener(SelectRole);
            eventTrigger.triggers.Add(entry);

            singleSelectBox.transform.SetParent(container.transform, false);
            Sprite sprite = Resources.Load<Sprite>(rote.imagePath);
            singleSelectBox.GetComponent<Image>().sprite = sprite;
            singleSelectBox.GetComponent<Image>().color = new Color(255, 255, 255);
            singleSelectBox.name = rote.name;
            singleSelectBox.transform.Find("Image").gameObject.SetActive(false);

        }
    }
    private void SelectedRoleInformation(GameObject selectBox)//��ʾѡ�н�ɫ�Ķ�Ӧ����
    {

        RoteData rote = DataManage.Instance.GetRoteData(selectBox.name.Replace("(Clone)", ""));
        actorState.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = rote.name;
        actorState.transform.Find("HP").GetComponent<TextMeshProUGUI>().text = rote.HP.ToString();
        actorState.transform.Find("ATK").GetComponent<TextMeshProUGUI>().text = rote.ATK.ToString();
        actorState.transform.Find("DEF").GetComponent<TextMeshProUGUI>().text = rote.DEF.ToString();
        actorState.transform.Find("ATS").GetComponent<TextMeshProUGUI>().text = rote.ATS.ToString();
        actorState.transform.Find("Occupation").GetComponent<TextMeshProUGUI>().text = rote.Occupation;
        actorState.transform.Find("RagePoint").GetComponent<TextMeshProUGUI>().text = rote.RagePoint.ToString();
        actorState.transform.Find("PurchasePoint").GetComponent<TextMeshProUGUI>().text = rote.PurchasePoint.ToString();
        actorState.transform.Find("AttackRange").GetComponent<TextMeshProUGUI>().text = rote.AttackRangeKey.ToString();
    }
    public void OnEnable()
    {
        roteNamesTemporarily = roteNames.ToList<string>();
        olderRoteNames = roteNames.ToList<string>();
        ShowAllSelectedRoleOutline();
    }
    public void ShowAllSelectedRoleOutline()
    {
        for (int i = 0; i < container.transform.childCount; i++)
        {
            container.transform.GetChild(i).GetComponent<Outline>().enabled = false;
        }
        foreach (string roteName in roteNames)
        {
            container.transform.Find(roteName).GetComponent<Outline>().enabled = true;
        }
    }
    public void SelectRole(BaseEventData data)//ѡ��/ȡ��ѡ�н�ɫ��Ϊѡ�н�ɫ��ӵ���ѡ���б�
    {
        AudioManager.Instance.PlayOneShot(Resources.Load<AudioClip>("Music/g_ui_btn_n"), 100f, 0f);
        GameObject selectObj = data.selectedObject;
        SelectedRoleInformation(selectObj);
        string selectObjName = selectObj.name.Replace("(Clone)", "");

        if (roteNamesTemporarily.Find(element => element == selectObjName) != null)
        {
            roteNamesTemporarily.Remove(selectObjName);
            Debug.Log("Remove:" + selectObjName);
            selectObj.GetComponent<Outline>().enabled = false;

        }
        else
        {
            roteNamesTemporarily.Add(selectObjName);
            Debug.Log("Add:" + selectObjName);
            selectObj.GetComponent<Outline>().enabled = true;
        }
    }

    //�󶨵����ذ�ť
    public void Back()
    {
        AudioManager.Instance.PlayOneShot(Resources.Load<AudioClip>("Music/g_ui_back"), 100f, 0f);
        roteNames.Clear();
        roteNames.AddRange(olderRoteNames);
        gameObject.SetActive(false);
        FormGroups.SetActive(true);
    }
    //�󶨵���ɰ�ť
    public void Finish()//�ڴ˽ڵ����������ѡ���ɫ������һ����ʾ��ѡ���ɫ
    {
        //��ѡ�н�ɫ��ӽ�����
        roteNames.Clear();
        roteNames.AddRange(roteNamesTemporarily);
        //��һ�α��ҳ��������
        for (int i = 0; i < roteTeam.transform.childCount; i++)
        {
            roteTeam.transform.GetChild(i).GetComponent<Image>().sprite = null;
            roteTeam.transform.GetChild(i).GetComponent<Image>().color = selectBox.GetComponent<Image>().color;
            roteTeam.transform.GetChild(i).Find("Image").gameObject.SetActive(true);
        }
        //��ʾ����ѡ�н�ɫ
        int count = 0;
        foreach (string roteName in roteNames)
        {
            RoteData roteData = DataManage.Instance.GetRoteData(roteName);
            roteTeam.transform.GetChild(count).GetComponent<Image>().sprite = Resources.Load<Sprite>(roteData.imagePath);
            roteTeam.transform.GetChild(count).GetComponent<Image>().color = new Color(255, 255, 255);
            roteTeam.transform.GetChild(count).Find("Image").gameObject.SetActive(false);
            count++;
        }
        //��Ϊ���ɼ�
        AudioManager.Instance.PlayOneShot(Resources.Load<AudioClip>("Music/g_ui_confirm"), 100f, 0f);
        gameObject.SetActive(false);
        FormGroups.SetActive(true);
    }
}
