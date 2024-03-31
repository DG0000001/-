using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : CharacterData
{
    //写出构造函数
    public EnemyData(int id, string name, string prefabPath, CharacterType characterType, float HP, float DEF, float ATK, float ATS, float MS):base(id,name,prefabPath,characterType,HP,DEF,ATK,ATS,MS) { }
}
