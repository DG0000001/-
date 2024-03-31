using cfg.enemyData;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class EnemyFactory : CharacterFactory
{
    private static EnemyFactory instance;
    public static EnemyFactory Instance
    {
        get
        {
            // 如果实例不存在，则查找场景中是否存在该类型的对象
            if (instance == null)
            {
                instance = FindObjectOfType<EnemyFactory>();
            }
            return instance;
        }
    }
    private CharacterPool characterPool;
    public PoolConfig[] poolConfigs;
    private void Awake()
    {
        characterPool = gameObject.AddComponent<CharacterPool>();
    }
    public override Character CreateCharacter(string name)
    {
        GameObject enemy = characterPool.GetCharacter(name);
        enemy.AddComponent<Enemy>();
        enemy.GetComponent<Enemy>().enemyData = DataManage.Instance.GetEnemyData(name);
        return enemy.GetComponent<Enemy>();
    }
    public override GameObject DestoryCharacter(GameObject gameObject)
    {
        characterPool.ReturnCharacter(gameObject);
        gameObject.GetComponent<AnimationManager>().SetAnimation("Idle", true, 1);
        Destroy(gameObject.GetComponent<Enemy>());

        return gameObject;
    }
}
