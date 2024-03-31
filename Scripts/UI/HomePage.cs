using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomePage : MonoBehaviour
{
    public GameObject tips;
    public GameObject selectLevel;
    public GameObject FormGroups;

    //检测队伍是否满员
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
    //绑定到开始战斗按钮
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
    //绑定到编辑队伍按钮
    public void TeamChose()
    {
        AudioManager.Instance.PlayOneShot(Resources.Load<AudioClip>("Music/g_ui_btn_n"), 100f, 0f);
        FormGroups.SetActive(true);
        gameObject.SetActive(false);
    }
    //绑定到设置按钮
    public void GameSetting() 
    {
        AudioManager.Instance.PlayOneShot(Resources.Load<AudioClip>("Music/g_ui_btn_n"), 100f, 0f);
    }
    //绑定到Tips中Confirm对象
    public void TipsConfirm()
    {
        AudioManager.Instance.PlayOneShot(Resources.Load<AudioClip>("Music/g_ui_confirm"), 100f, 0f);
        tips.SetActive ( false );
    }
}
