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
    public PoolConfig[] poolConfigs; // ���������
    private List<GameObject> characterPool = new List<GameObject>(); // ������б�

    private void Awake()
    {
        GameObject characterPoolScene = Instantiate(new GameObject());
        characterPoolScene.name = "CharacterPool";
        poolConfigs = EnemyFactory.Instance.poolConfigs;
        foreach (PoolConfig item in poolConfigs)
        {
            // ����Ϸ��ʼʱ�������˶�����ӵ��������
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
        // �ڶ�����в��Ҳ�����һ��δ����ĵ��˶���
        foreach (GameObject character in characterPool)
        {
            if (!character.activeInHierarchy && character.name.Equals(name))
            {
                character.SetActive(true);
                return character;
            }
        }

        // ����������û�п��õĵ��˶����򴴽�һ���µĵ��˶�����ӵ��������
        GameObject newCharacter = Instantiate(Resources.Load(DataManage.Instance.GetEnemyData(name).prefabPath)) as GameObject;
        characterPool.Add(newCharacter);
        return newCharacter;
    }

    public void ReturnCharacter(GameObject character)
    {
        // �����˶�������Ϊδ����״̬������������
        character.SetActive(false);
    }
}
