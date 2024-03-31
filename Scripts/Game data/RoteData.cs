using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoteData : CharacterData
{
    public string imagePath { get; set; }
    public string Occupation { get; set; }
    public int RagePoint { get; set;}
    public int PurchasePoint { get; set;}

    public int AttackRangeKey { get; set;}
    public int BlockEnemyNumber { get; set;}
    public int CD { get; set;}

    public RoteData(int id, string name, string prefabPath, CharacterType characterType, float HP, float DEF, float ATK, float ATS,float MS,string imagePath, string occupation, int ragePoint, int purchasePoint, int attackRangeKey,int blockEnemyNumber,int cd) :base(id, name, prefabPath, characterType, HP, DEF, ATK, ATS,MS)
    {
        this.Occupation = occupation;
        this.RagePoint = ragePoint;
        this.PurchasePoint = purchasePoint;
        this.AttackRangeKey = attackRangeKey;
        this.imagePath = imagePath;
        this.BlockEnemyNumber = blockEnemyNumber;
        this.CD = cd;
    }
}
