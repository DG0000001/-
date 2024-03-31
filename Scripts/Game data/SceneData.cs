using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[Serializable]
public class GenerateEnemyConfigurationInOneScene
{
    public GenerateEnemyConfiguration[] generateEnemyConfigurations;
}
[Serializable]
public class GenerateEnemyConfiguration
{
    public float timing;
    public float duration;
    public int count;
    public string[] everyEnemiesName;
}
public class SceneData
{
    public int id { get; set; }
    public string name{ get; set; }
    public string image{ get; set; }
    public int AllowEnemiesPassCount{ get; set; }
    public int purchasePoint{ get; set; }
    public float PurchasePointAcceleration{ get; set; }
    public int AllowRotesPlacedCount{ get; set; }
    public GenerateEnemyConfigurationInOneScene generateEnemyConfigurationInOneScene { get; set; }
    public SceneData(int id, string name, string image, int allowEnemiesPass, int purchasePoint,float PurchasePointAcceleration,int AllowRotesPlacedCount, GenerateEnemyConfigurationInOneScene generateEnemyConfigurationInOneScene)
    {
        this.id = id;
        this.name = name;
        this.image = image;
        this.AllowEnemiesPassCount = allowEnemiesPass;
        this.purchasePoint = purchasePoint;
        this.PurchasePointAcceleration = PurchasePointAcceleration;
        this.AllowRotesPlacedCount = AllowRotesPlacedCount;
        this.generateEnemyConfigurationInOneScene = generateEnemyConfigurationInOneScene;
    }
    public SceneData(SceneData sceneData)
    {
        this.id = sceneData.id;
        this.name = sceneData.name;
        this.image = sceneData.image;
        this.AllowEnemiesPassCount = sceneData.AllowEnemiesPassCount;
        this.purchasePoint = sceneData.purchasePoint;
        this.PurchasePointAcceleration = sceneData.PurchasePointAcceleration;
        this.AllowRotesPlacedCount = sceneData.AllowRotesPlacedCount;
        this.generateEnemyConfigurationInOneScene = sceneData.generateEnemyConfigurationInOneScene;
    }
}
