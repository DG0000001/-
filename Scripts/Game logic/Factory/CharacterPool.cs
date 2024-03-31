using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PoolConfig
{
    public GameObject prefab;
    public int size;
}
public class CharacterPool : MonoBehaviour
{
    public PoolConfig[] poolConfigs; // 对象池配置
    private List<GameObject> characterPool = new List<GameObject>(); // 对象池列表

    private void Awake()
    {
        GameObject characterPoolScene = Instantiate(new GameObject());
        characterPoolScene.name = "CharacterPool";
        poolConfigs = EnemyFactory.Instance.poolConfigs;
        foreach (PoolConfig item in poolConfigs)
        {
            // 在游戏开始时创建敌人对象并添加到对象池中
            for (int i = 0; i < item.size; i++)
            {
                GameObject character = Instantiate(item.prefab);
                character.transform.SetParent(characterPoolScene.transform);
                character.name = item.prefab.name;
                character.SetActive(false);
                characterPool.Add(character);
            }
        }
    }

    public GameObject GetCharacter(string name)
    {
        // 在对象池中查找并返回一个未激活的敌人对象
        foreach (GameObject character in characterPool)
        {
            if (!character.activeInHierarchy && character.name.Equals(name))
            {
                character.SetActive(true);
                return character;
            }
        }

        // 如果对象池中没有可用的敌人对象，则创建一个新的敌人对象并添加到对象池中
        GameObject newCharacter = Instantiate(Resources.Load(DataManage.Instance.GetEnemyData(name).prefabPath)) as GameObject;
        characterPool.Add(newCharacter);
        return newCharacter;
    }

    public void ReturnCharacter(GameObject character)
    {
        // 将敌人对象设置为未激活状态并重置其属性
        character.SetActive(false);
    }
}
