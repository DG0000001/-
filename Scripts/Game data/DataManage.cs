using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using cfg;
using cfg.item;
using SimpleJSON;
using System.Data;
using cfg.sceneData;
using static UnityEditor.Progress;

public class DataManage : MonoBehaviour
{
    private Dictionary<int, List<Vector3>> AttackRangeDic = new Dictionary<int, List<Vector3>>();
    private List<EnemyData> enemyDataList = new List<EnemyData>();//所有敌人数据
    private List<RoteData> roteDataList = new List<RoteData>();//所有角色数据
    private List<SceneData> sceneDataList = new List<SceneData>();//所有关卡数据
    private List<string> selectedRoteNames = new List<string>();//UI中选中的角色名字
    public string selectedSceneName { get; set; }//UI中选中的关卡名字
    private static DataManage instance;

    public void Awake()
    {
        //selectedSceneName = "经典2";
        sceneDataList = new List<SceneData>();
        roteDataList = new List<RoteData>();
        selectedRoteNames = new List<string>();
        LoadConfigData();
        SetAttackRange();
    }
    public static DataManage Instance
    {
        get {
            // 如果实例不存在，则查找场景中是否存在该类型的对象
            if (instance == null)
            {
                instance = FindObjectOfType<DataManage>();

                // 如果场景中不存在该类型的对象，则创建一个新的实例
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<DataManage>();
                    singletonObject.name = typeof(DataManage).ToString() + " (SingletonDataManage)";

                    // 保证在场景切换时不被销毁
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return instance;
        }
    }

    private JSONNode Loader(string fileName)
    {
        return JSON.Parse(File.ReadAllText(Application.dataPath + "../../GenerateDatas/json/" + fileName + ".json"));
    }
    private List<GenerateEnemyConfigurationInOneScene> LoadSceneJsonData()
    {
        string filePath = Application.dataPath + "../../GenerateDatas/myJson/" + "GenerateEnemyConfiguration-scenedata" + ".json";
        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);                 //读取文件
            GenerateEnemyConfigurationList generateEnemyConfigurationList = JsonUtility.FromJson<GenerateEnemyConfigurationList>(dataAsJson);
            return generateEnemyConfigurationList.generateEnemyConfigurationByOneSceneDatas;
        }
        return null;
    }
    private void LoadConfigData()
    {
        Tables table = new Tables(Loader);
        List<cfg.enemyData.Enemy> enemies = table.TBenemyData.DataList;//角色数据初始化
        List<cfg.item.Item> items = table.TbItem.DataList;//角色数据初始化
        List<Scene> scenes = table.TBsceneData.DataList;//关卡数据初始化
        List<GenerateEnemyConfigurationInOneScene> generateEnemyConfigurationAllScene = LoadSceneJsonData();
        foreach (cfg.enemyData.Enemy enemy in enemies)
        {
            enemyDataList.Add(new EnemyData(enemy.Id, enemy.Name, enemy.PrefabPath, CharacterType.Rote, enemy.HP, enemy.DEF, enemy.ATK, enemy.ATS, enemy.MS));
        }
        foreach (cfg.item.Item item in items)
        {
            roteDataList.Add(new RoteData(item.Id, item.Name, item.PrefabPath, CharacterType.Enemy, item.HP, item.DEF, item.ATK, item.ATS,item.MS, item.ImagePath,item.Occupation, item.RagePoint, item.PurchasePoint, item.AttackRangeKey,item.BlockEnemyNumber,item.CD));
        }
        foreach (Scene scene in scenes)
        {
            sceneDataList.Add(new SceneData(scene.Id, scene.Name, scene.Image, scene.AllowEnemiesPassCount, scene.PurchasePoint,scene.PurchasePointAcceleration,scene.AllowRotesPlacedCount,null));
        }
        for (int i = 0; i < generateEnemyConfigurationAllScene.Count; i++)
        {
            sceneDataList[i].generateEnemyConfigurationInOneScene = generateEnemyConfigurationAllScene[i];
        }
        
    }

    public List<Vector3> GetAttackRange(int key,Vector3 pos)
    {
        List<Vector3> attackRange = new List<Vector3>();
        for (int i = 0; i < AttackRangeDic[key].Count; i++)
        {
            attackRange.Add(AttackRangeDic[key][i] + pos);
        }
        return attackRange;
    }
    public void SetAttackRange()
    {
        //默认左正(x)，下正(z)
        //边长长度
        int length = 100;
        List<Vector3> list = new List<Vector3>();
        //key:1
        list.Add(Vector3.zero);
        AttackRangeDic.Add(1, new List<Vector3>( list));
        list.Clear();
        //key:2
        list.Add(Vector3.zero);
        list.Add(new Vector3(length, 0, 0));
        AttackRangeDic.Add(2, new List<Vector3>(list));
        list.Clear();
        //key:3
        list.Add(Vector3.zero);
        list.Add(new Vector3(length, 0, 0));
        list.Add(new Vector3(length*2, 0, 0));
        AttackRangeDic.Add(3, new List<Vector3>(list));
        list.Clear();
        //key:4
        //key:5
        //key:6
        //key:7
        list.Add(Vector3.zero);
        list.Add(new Vector3(0, 0, length));
        list.Add(new Vector3(0, 0, -length));
        list.Add(new Vector3(length, 0, length));
        list.Add(new Vector3(length, 0, -length));
        list.Add(new Vector3(length, 0, 0));
        list.Add(new Vector3(length * 2, 0, 0));
        AttackRangeDic.Add(7, new List<Vector3>(list));
        list.Clear();
        //key:8
        //key:9
        //key:10
        list.Add(Vector3.zero);
        list.Add(new Vector3(0, 0, length));
        list.Add(new Vector3(0, 0, -length));
        list.Add(new Vector3(length, 0, length));
        list.Add(new Vector3(length, 0, -length));
        list.Add(new Vector3(length, 0, 0));
        list.Add(new Vector3(length*2, 0, length));
        list.Add(new Vector3(length*2, 0, -length));
        list.Add(new Vector3(length * 2, 0, 0));
        list.Add(new Vector3(length * 3, 0, length));
        list.Add(new Vector3(length * 3, 0, -length));
        list.Add(new Vector3(length * 3, 0, 0));
        AttackRangeDic.Add(10, new List<Vector3>(list));
        list.Clear();
        //key:12
        list.Add(Vector3.zero);
        list.Add(new Vector3(0, 0, length));
        list.Add(new Vector3(0, 0, -length));
        list.Add(new Vector3(length, 0, length));
        list.Add(new Vector3(length, 0, -length));
        list.Add(new Vector3(length, 0, 0));
        list.Add(new Vector3(length * 2, 0, length));
        list.Add(new Vector3(length * 2, 0, -length));
        list.Add(new Vector3(length * 2, 0, 0));
        list.Add(new Vector3(length * 3, 0, 0));
        AttackRangeDic.Add(12, new List<Vector3>(list));
        list.Clear();
    }
    public List<string> GetSelectedRoteNames()
    {
        return selectedRoteNames;
    }
    public List<EnemyData> GetEnemyData()
    {
        return enemyDataList;
    }
    public List<RoteData> GetRoteData()
    {
        return roteDataList;
    }
    public List<SceneData> GetSceneData()
    {
        return sceneDataList;
    }
    public EnemyData GetEnemyData(string name)
    {
        foreach (EnemyData enemyData in enemyDataList)
        {
            if (enemyData.name == name)
            {
                return enemyData;
            }
        }
        return null;
    }
    public RoteData GetRoteData(string name)
    {
        foreach (RoteData roteData in roteDataList)
        {
            if(roteData.name == name)
            {
                return roteData;
            }
        }
        return null;
    }
    public SceneData GetSceneData(string name)
    {
        foreach (SceneData sceneData in sceneDataList)
        {
            if (sceneData.name == name)
            {
                return sceneData;
            }
        }
        return null;
    }
    public float DamageCalculationInterface(RoteData roteData,EnemyData enemyData,int damageItem)
    {
        switch (damageItem)
        {
            case 1:
                return Mathf.Max(enemyData.ATK - roteData.DEF, enemyData.ATK * 0.1f);
            case 2:
                return Mathf.Max(roteData.ATK - enemyData.DEF,roteData.ATK * 0.1f);
        }
        return 0;
    }

}
