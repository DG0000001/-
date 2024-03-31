using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using TMPro;
using Unity.Jobs.LowLevel.Unsafe;
using System.Linq;

public class SelectActor : MonoBehaviour
{
    public GameObject roteTeam;
    public GameObject FastGroup;
    public GameObject actorState;
    GameObject selectBox;
    public GameObject container;

    List<RoteData> roteData;
    List<string> olderRoteNames = new List<string>();
    public GameObject FormGroups;
    GameObject OldSelectObj;
    private GameObject enterBox;
    List<string> roteNames;

    void Start()
    {
        roteNames = DataManage.Instance.GetSelectedRoteNames();
        olderRoteNames = new List<string>(roteNames);
        selectBox = Resources.Load<GameObject>("Prefabs/SelectBox");
        GenerateAllKnownRote();
    }
    //���ñ��ҳ�����ʱ��ѡ�н�ɫ
    public void SetEnterBox(GameObject obj)
    {
        enterBox = obj;
    }
    private void GenerateAllKnownRote()//����������֪��ɫ
    {
        roteData = DataManage.Instance.GetRoteData();
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
        olderRoteNames = roteNames.ToList<string>();
        RecoverDontSelectObj();
        DontShowSelectedRote();
    }
    public void DontShowSelectedRote()
    {
        //��һ��ѡ��ҳ���ֹѡ�н�ɫ�б�
        for (int i = 0; i < container.transform.childCount; i++)
        {
            container.transform.GetChild(i).GetComponent<Selectable>().enabled = true;
            container.transform.GetChild(i).GetComponent<Image>().color = new Color(255, 255, 255);
        }
        //��selectactorҳ��ѡ�н�ɫ���ó���ѡ��
        foreach (string roteName in roteNames)
        {
            container.transform.Find(roteName).GetComponent<Image>().color = new Color(0, 0, 0);
            container.transform.Find(roteName).GetComponent<Selectable>().enabled = false;
        }
    }
    //�ָ���ȡ��ѡ�еĽ�ɫ
    public void RecoverDontSelectObj()
    {
        Debug.Log("name" + enterBox.name);
        if (roteNames.Remove(enterBox.name))
        {
            container.transform.Find(enterBox.name).GetComponent<Selectable>().enabled = true;
            container.transform.Find(enterBox.name).GetComponent<Image>().color = new Color(255, 255, 255);

        }
    }
    public void SelectRole(BaseEventData data)//ѡ��/ȡ��ѡ�н�ɫ��Ϊѡ�н�ɫ��ӵ���ѡ���б�
    {
        AudioManager.Instance.PlayOneShot(Resources.Load<AudioClip>("Music/g_ui_btn_n"), 100f, 0f);
        GameObject selectObj = data.selectedObject;
        SelectedRoleInformation(selectObj);

        if (OldSelectObj != null)
        {
            OldSelectObj.GetComponent<Outline>().enabled = false;
        }
        selectObj.GetComponent<Outline>().enabled = true;
        OldSelectObj = selectObj;
    }
    //��ݱ��onclick�󶨺���
    public void QuickFormation()
    {
        AudioManager.Instance.PlayOneShot(Resources.Load<AudioClip>("Music/g_ui_btn_n"), 100f, 0f);
        FastGroup.SetActive(true);
        gameObject.SetActive(false);
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
    public void Finish()
    {
        //��ѡ�н�ɫ��ӽ�����
        if(OldSelectObj!=null)
        {
            roteNames.Add(OldSelectObj.name);
            OldSelectObj.GetComponent<Outline>().enabled = false;
        }

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
            RoteData rote = DataManage.Instance.GetRoteData(roteName);
            roteTeam.transform.GetChild(count).name = roteName;
            roteTeam.transform.GetChild(count).GetComponent<Image>().sprite = Resources.Load<Sprite>(rote.imagePath);
            roteTeam.transform.GetChild(count).GetComponent<Image>().color = new Color(255, 255, 255);
            roteTeam.transform.GetChild(count).Find("Image").gameObject.SetActive(false);
            count++;
        }
        //��Ϊ���ɼ�
        AudioManager.Instance.PlayOneShot(Resources.Load<AudioClip>("Music/g_ui_confirm"), 100f, 0f);
        gameObject.SetActive(false);
        FormGroups.SetActive(true);
        OldSelectObj = null;
    }

}
