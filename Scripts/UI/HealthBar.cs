using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    public Image hpImg;
    public Image hpEffectImg; 
    public float maxHp = 0f;
    public float currentHp { get; set; }
    public float buffTime = 0.5f; 
    private Coroutine updateCoroutine;

    private void Start()
    {
        currentHp = maxHp;
        UpdateHealthBar();
    }
    public void SetHealth(float health)
    {
        currentHp = Mathf.Clamp(health,0f,maxHp);
        UpdateHealthBar();
        if (currentHp<0)
        {

        }
    }
    public void SetMaxHp(float maxHp)
    {
        this.maxHp = maxHp;
        currentHp = maxHp;
        UpdateHealthBar();
    }
    public void IncreaseHealth(float amont)
    {
        SetHealth(currentHp + amont);
    }
    public void DecreaseHealth(float amont)
    {
        SetHealth(currentHp - amont);
    }
    private void UpdateHealthBar()
    {
        hpImg.fillAmount = currentHp / maxHp; 
        if (updateCoroutine != null) 
            StopCoroutine(updateCoroutine); 
        updateCoroutine = StartCoroutine(UpdateHpEffect());
    }
    private IEnumerator UpdateHpEffect()
    {
        float elapsedTime = 0f;
        float effectLength = hpEffectImg.fillAmount - hpImg.fillAmount;

        while (elapsedTime < buffTime && effectLength != 0)
        {
            elapsedTime += Time.deltaTime;
            hpEffectImg.fillAmount = Mathf.Lerp(hpImg.fillAmount + effectLength, hpImg.fillAmount, elapsedTime / buffTime);
            yield return null;
            hpEffectImg.fillAmount = hpImg.fillAmount;
        }
    }
}
