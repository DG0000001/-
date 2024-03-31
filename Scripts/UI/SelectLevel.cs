using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
using UnityEngine.UI;
using System.Net.Sockets;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SelectLevel : MonoBehaviour
{
    public GameObject HomePage;
    private GameObject selectBox;
    public GameObject FormGroups;
    public GameObject container;
    private List<SceneData> levels;
    private GameObject selectedLevel;
    private GameObject oldedSelectedLevel;

    // Start is called before the first frame update
    void Start()
    {
        levels = new List<SceneData>();
        selectBox = Resources.Load<GameObject>("Prefabs/SelectBox");
        GenerateOptionalLevels();
    }
    public void GenerateOptionalLevels()//生成可选关卡
    {
        levels = DataManage.Instance.GetSceneData();
        foreach (SceneData level in levels)
        {
            GameObject singleSelectBox = Instantiate(selectBox);

            EventTrigger eventTrigger = singleSelectBox.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerUp;
            entry.callback.AddListener(SelectOneLevel);
            eventTrigger.triggers.Add(entry);

            singleSelectBox.transform.SetParent(container.transform, false);
            Sprite sprite = Resources.Load<Sprite>(level.image);
            singleSelectBox.GetComponent<Image>().sprite = sprite;
            singleSelectBox.GetComponent<Image>().color = new Color(255, 255, 255);
            singleSelectBox.name = level.name;
            Debug.Log("singleSelectBox.name：" + singleSelectBox.name);

            singleSelectBox.transform.Find("Image").gameObject.SetActive(false);
            Transform obj = singleSelectBox.transform.Find("Text");
            obj.gameObject.SetActive(true);
            obj.gameObject.GetComponent<Text>().text = level.name;
        }
    }
    public void TeamChose()//绑定到编辑队伍按钮
    {
        AudioManager.Instance.PlayOneShot(Resources.Load<AudioClip>("Music/g_ui_btn_n"), 100f, 0f);
        FormGroups.SetActive(true);
        gameObject.SetActive(false);
    }
    public void SelectOneLevel(BaseEventData data)//选择关卡(绑定到关卡按钮)
    {
        AudioManager.Instance.PlayOneShot(Resources.Load<AudioClip>("Music/g_ui_btn_n"), 100f, 0f);
        selectedLevel = data.selectedObject;
        if (oldedSelectedLevel != null)
        {
            oldedSelectedLevel.GetComponent<Outline>().enabled = false;
        }
        selectedLevel.GetComponent<Outline>().enabled = true;
        oldedSelectedLevel = selectedLevel;

    }
    //绑定到战斗按钮
    public void Fighting()
    {
        DataManage.Instance.selectedSceneName = selectedLevel.name;
        Debug.Log(DataManage.Instance.selectedSceneName);
        HomePage.SetActive(true);
        gameObject.SetActive(false);
        AudioManager.Instance.PlayOneShot(Resources.Load<AudioClip>("Music/g_ui_confirm"), 100f, 0f);
        //转换战斗场景
        AudioManager.Instance.Stop("m_sys_void_combine");
        AudioManager.Instance.PlayOneShot(Resources.Load<AudioClip>("Music/m_avg_tense_combine"), 100f, 0f);
        SceneManager.LoadScene(DataManage.Instance.selectedSceneName);
        DontDestroyOnLoad(DataManage.Instance);

    }
    //绑定到返回按钮
    public void Back()
    {
        AudioManager.Instance.PlayOneShot(Resources.Load<AudioClip>("Music/g_ui_back"), 100f, 0f);
        gameObject.SetActive(false);
        HomePage.SetActive(true);
    }


}
