using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetAllActor : MonoBehaviour
{
    List<string > actorNames = new List<string>();
    ActorBar[] actorBars;
    public GameObject BG;
    public GameObject ActorState;
    private void Awake()
    {
        actorNames = DataManage.Instance.GetSelectedRoteNames();
        actorBars = FindObjectsOfType<ActorBar>();

        for (int i = 0; i < actorNames.Count; i++) 
        {
            string newString = DataManage.Instance.GetRoteData(actorNames[i]).imagePath;
            newString = newString.Replace(DataManage.Instance.GetRoteData(actorNames[i]).name, "Í·Ïñ_" + DataManage.Instance.GetRoteData(actorNames[i]).name);
            Debug.Log(DataManage.Instance.GetRoteData(actorNames[i]).prefabPath);
            actorBars[i].Rote = Resources.Load<GameObject>(DataManage.Instance.GetRoteData(actorNames[i]).prefabPath);
            actorBars[i].gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(newString);
        }
    }
    public void ClickAnthorPlace()
    {
        BG.GetComponent<Image>().raycastTarget = true;
        BG.SetActive(false);
        Time.timeScale = 1f;
        ActorState.SetActive(false);
    }
}
