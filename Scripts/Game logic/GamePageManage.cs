using cfg.sceneData;
using Spine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class PlayingState
{
    protected GamePageManage gamePageManage;
    public PlayingState(GamePageManage gamePageManage)
    {
        this.gamePageManage = gamePageManage;
    }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}

public class PlayingProceedState : PlayingState
{
    public PlayingProceedState(GamePageManage gamePageManage) : base(gamePageManage) { }

    float currentValue = 0f;  // 当前值
    float targetValue = 1f;  // 目标值
    float duration = 1f;      // 持续时间
    float elapsedTime = 0f;   // 已经过去的时间

    public override void EnterState()
    {
        gamePageManage.ShowEnemyGeneration_total(0);
    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {
        gamePageManage.UpdateGameTime();
        if(gamePageManage.gameIntTime > gamePageManage.previousGameIntTime)
        {
            gamePageManage.GenerateEnemy();
            gamePageManage.previousGameIntTime = gamePageManage.gameIntTime;
        }

        if (elapsedTime < duration)
        {
            // 增加已经过去的时间
            elapsedTime += Time.deltaTime;
            // 计算插值
            float t = elapsedTime / duration;
            // 使用Lerp函数更新当前值
            currentValue = Mathf.Lerp(0f, targetValue, t);
            gamePageManage.PurchasePointSlider.GetComponent<Slider>().value = currentValue;
        }
        else
        {
            gamePageManage.PurchasePoint.GetComponent<Text>().text = (int.Parse(gamePageManage.PurchasePoint.GetComponent<Text>().text) + 1).ToString();
            elapsedTime = 0f;
            currentValue = 0f;
        }
        
        if(int.Parse( gamePageManage.AllowEnemiesPassCount.GetComponent<Text>().text) <= 0 )
        {
            gamePageManage.ChangeState(new PlayingLostState(gamePageManage));
        }
        if (GameObject.FindObjectsOfType<Enemy>().Length<=0&&gamePageManage.currentEnemiesCount == gamePageManage.allGenerationEnemiesCount)
        {
            gamePageManage.ChangeState(new PlayingEndState(gamePageManage));
        }
    }
}
public class PlayingEndState : PlayingState
{
    public PlayingEndState(GamePageManage gamePageManage) : base(gamePageManage) { }

    public override void EnterState()
    {
        gamePageManage.BattleSettlement.transform.Find("Win").gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {

    }
}
public class PlayingLostState : PlayingState
{
    public PlayingLostState(GamePageManage gamePageManage) : base(gamePageManage) { }

    public override void EnterState()
    {
        gamePageManage.BattleSettlement.transform.Find("Lost").gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {

    }
}

public class GamePageManage : MonoBehaviour
{           
    private static GamePageManage instance;
    public static GamePageManage Instance
    {
        get
        {
            // 如果实例不存在，则查找场景中是否存在该类型的对象
            if (instance == null)
            {
                instance = FindObjectOfType<GamePageManage>();

                // 如果场景中不存在该类型的对象，则创建一个新的实例
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<GamePageManage>();
                    singletonObject.name = typeof(GamePageManage).ToString() + " (SingletonGamePageManage)";
                }
            }

            return instance;
        }
    }

    private PlayingState currentState;

    public int previousGameIntTime { get; set; }
    public int gameIntTime { get; set; }
    public float gameFloatTime { get;set;}

    public GameObject State;
    public GameObject PurchasePointSlider { get; set; }
    public GameObject PurchasePoint { get; set; }

    public GameObject EnemyGeneration_total { get; set; }

    public GameObject AllowEnemiesPassCount { get; set; }
    public int currentEnemiesCount = 0;
    public int allGenerationEnemiesCount = 0;

    public GameObject BattleSettlement;
    public SceneData selectedSceneData;

    void Awake()
    {
        selectedSceneData = new SceneData(DataManage.Instance.GetSceneData("Level02"));//new SceneData(DataManage.Instance.GetSceneData(DataManage.Instance.selectedSceneName));

        PurchasePointSlider = State.transform.Find("RightPoints/PurchasePointSlider").gameObject;
        PurchasePoint = State.transform.Find("RightPoints/PurchasePoint").gameObject;
        EnemyGeneration_total = State.transform.Find("UpState/EnemyGeneration_total").gameObject;
        AllowEnemiesPassCount = State.transform.Find("UpState/AllowEnemiesPassCount").gameObject;

        AllowEnemiesPassCount.GetComponent<Text>().text = selectedSceneData.AllowEnemiesPassCount.ToString();
        PurchasePoint.GetComponent<Text>().text = selectedSceneData.purchasePoint.ToString();

        foreach (var item in selectedSceneData.generateEnemyConfigurationInOneScene.generateEnemyConfigurations)
        {
            allGenerationEnemiesCount = item.count + allGenerationEnemiesCount;
        }
        currentState = new PlayingProceedState(this);
        currentState.EnterState();
    }
    void Update()
    {
        currentState.UpdateState();
    }
    public void ChangeState(PlayingState newState)
    {
        // 切换状态
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }
    public void UpdateGameTime()
    {
        gameIntTime = (int)gameFloatTime;
        gameFloatTime += Time.deltaTime;
        //Debug.Log(gameTime);
    }

    #region 生成敌人、敌人路径管理部分
    public void GenerateEnemy()
    {
        GenerateEnemyConfiguration[] generateEnemyConfiguration = selectedSceneData.generateEnemyConfigurationInOneScene.generateEnemyConfigurations;
        for (int i = 0; i < generateEnemyConfiguration.Length;i++ )
        {
            if (generateEnemyConfiguration[i].everyEnemiesName.Length > 0)
            {
                if (generateEnemyConfiguration[i].timing <= gameIntTime && generateEnemyConfiguration[i].timing + generateEnemyConfiguration[i].duration >= gameIntTime)
                {
                    float refreshInterval = generateEnemyConfiguration[i].duration / generateEnemyConfiguration[i].count;//每个敌人生成的间隔时间
                    //Debug.Log(gameIntTime % refreshInterval);
                    if (gameIntTime % refreshInterval == 0)
                    {                      
                        //按照每个敌人生成的间隔时间生成敌人
                        GameObject enemy = EnemyFactory.Instance.CreateCharacter(generateEnemyConfiguration[i].everyEnemiesName[generateEnemyConfiguration[i].everyEnemiesName.Length - 1]).gameObject;
                        ShowEnemyGeneration_total(1);
                        foreach (GameObject item in GameObject.FindGameObjectsWithTag("Route"))
                        {
                            if(item.name.Equals("Route0"+(i+1)))
                            {
                                enemy.GetComponent<Enemy>().Waypoint = item.GetComponent<WayPoint>();
                            }
                        }
                        generateEnemyConfiguration[i].everyEnemiesName = generateEnemyConfiguration[i].everyEnemiesName.Take(generateEnemyConfiguration[i].everyEnemiesName.Length - 1).ToArray();
                    }
                }
            }
        }
        
    }


    #endregion
    public void ShowEnemyGeneration_total(int number)//当一个敌人生成时，显示敌人生成数量
    {
        currentEnemiesCount += number;
        EnemyGeneration_total.GetComponent<Text>().text = currentEnemiesCount.ToString() + "\\" + allGenerationEnemiesCount.ToString();
    }
    public void DeductionPurchasePoint(RoteData roteData)
    {

        PurchasePoint.GetComponent<Text>().text = (int.Parse(PurchasePoint.GetComponent<Text>().text) - roteData.PurchasePoint).ToString();
    }
    public bool DetectionPurchasePoint(RoteData roteData)
    {
        if (int.Parse(PurchasePoint.GetComponent<Text>().text) - roteData.PurchasePoint < 0)
        {
            return false;
        }
            return true;
    }
    public void BackToHomePage()
    {
        AudioManager.Instance.PlayOneShot(Resources.Load<AudioClip>("Music/g_ui_confirm"), 100f, 0f);
        AudioManager.Instance.Stop("m_avg_tense_combine");
        SceneManager.LoadScene("HomePage");
    }
}
