using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomePage : MonoBehaviour
{
    public GameObject tips;
    public GameObject selectLevel;
    public GameObject FormGroups;

    //�������Ƿ���Ա
    public bool CheckTeamIsFull()
    {
        int TeamsCount = DataManage.Instance.GetSelectedRoteNames().Count;
        if(TeamsCount == 12 )
        {
            return true;
        }
        else if( TeamsCount < 12 )
        {
            return false;
        }
        return true;
    }
    //�󶨵���ʼս����ť
    public void GameStart() 
    {
        AudioManager.Instance.PlayOneShot(Resources.Load<AudioClip>("Music/g_ui_btn_n"), 100f, 0f);
        if( CheckTeamIsFull() ) 
        {
            gameObject.SetActive(false);
            selectLevel.SetActive(true);
        }
        else
        {
            tips.SetActive(true);
        }
    } 
    //�󶨵��༭���鰴ť
    public void TeamChose()
    {
        AudioManager.Instance.PlayOneShot(Resources.Load<AudioClip>("Music/g_ui_btn_n"), 100f, 0f);
        FormGroups.SetActive(true);
        gameObject.SetActive(false);
    }
    //�󶨵����ð�ť
    public void GameSetting() 
    {
        AudioManager.Instance.PlayOneShot(Resources.Load<AudioClip>("Music/g_ui_btn_n"), 100f, 0f);
    }
    //�󶨵�Tips��Confirm����
    public void TipsConfirm()
    {
        AudioManager.Instance.PlayOneShot(Resources.Load<AudioClip>("Music/g_ui_confirm"), 100f, 0f);
        tips.SetActive ( false );
    }
}
