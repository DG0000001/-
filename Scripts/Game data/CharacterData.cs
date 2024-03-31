using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData
{
    public int id { get; set;}
    public string name { get; set;}
    public string prefabPath { get; set;}
    public CharacterType characterType { get; set;}

    public float HP { get; set;}
    public float DEF { get; set;}
    public float ATK { get; set;}

    public float ATS { get; set;}
    public float MS { get; set;}

    public CharacterData(int id, string name, string prefabPath, CharacterType characterType, float HP, float DEF, float ATK, float ATS, float MS)
    {
        this.id = id;
        this.name = name;
        this.prefabPath = prefabPath;
        this.characterType = characterType;
        this.HP = HP;
        this.DEF = DEF;
        this.ATK = ATK;
        this.ATS = ATS;
        this.MS = MS;
    }
}

public enum CharacterType
{
    Enemy,
    Rote,
}