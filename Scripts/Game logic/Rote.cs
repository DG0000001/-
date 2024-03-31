using cfg.enemyData;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using SVector3 = System.Numerics.Vector3;
//上下左右方向枚举
public enum Direction
{
    Up,
    Down,
    Left,
    Right
}
public struct AttackRange
{
    public GameObject obj;
    public Material material;

    public AttackRange(GameObject obj,Material material) 
    {
        this.obj = obj;
        this.material = material;
    }

}

public abstract class RoteState
{
    protected Rote rote;
    public RoteState(Rote rote)
    {
        this.rote = rote;
    }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}
public class RoteDeadState : RoteState
{
    public RoteDeadState(Rote rote) : base(rote) { }

    public override void EnterState()
    {
        rote.animationManager.SetSkeletonDataAsset("Face");
        rote.animationManager.SetAnimation("Die", false, 1);
        rote.animationManager.AddAnimation("Idle", true, 1,0);
    }

    public override void ExitState()
    {
        GameObject.Destroy(rote.gameObject);
    }

    public override void UpdateState()
    {
        if (rote.animationManager.ReturnAnimationName().Equals("Idle"))
        {
            rote.currentState.ExitState();
        }
    }
}
public class RoteAttackState: RoteState
{
    public RoteAttackState(Rote rote) : base(rote) { }

    public override void EnterState()
    {
        rote.animationManager.SetAnimation("Attack", true, rote.roteData.ATS, rote);
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
        List<GameObject> list = new List<GameObject>(rote.BitEnemies);
        foreach (GameObject enemy in list)
        {
            if (!enemy.activeInHierarchy)
            {
                rote.BitEnemies.Remove(enemy);
            }
        }
        list = new List<GameObject>(rote.blockEnemies);
        foreach (GameObject enemy in list)
        {
            if (!enemy.activeInHierarchy)
            {
                rote.blockEnemies.Remove(enemy);
            }

        }
        if (!rote.IsEnemiesEnterAttackRange())
            rote.ChangeState(new RoteIDleState(rote));

        if (rote.GetComponent<HealthBar>().currentHp <= 0)
        {
            rote.ChangeState(new RoteDeadState(rote));
        }
    }
}
public class RoteIDleState : RoteState
{
    public RoteIDleState(Rote rote) : base(rote) { }

    public override void EnterState()
    {
        //rote.GetComponent<BoxCollider>().enabled = true;
        rote.animationManager.AddAnimation("Idle", true, 1, 0);

    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {
       if( rote.IsEnemiesEnterAttackRange())
       {
            rote.ChangeState(new  RoteAttackState(rote));
       }
    }
}
public class RotePrepareState : RoteState
{
    public RotePrepareState(Rote rote) : base(rote) { }

    public override void EnterState()
    {
        //rote.GetComponent<BoxCollider>().enabled = false;
        rote.healthBar.SetActive(false);
    }

    public override void ExitState()
    {
        rote.animationManager.SetAnimation("Start", false, 1);
        rote.healthBar.SetActive(true);
    }

    public override void UpdateState()
    {
        if (rote.DC != null)
        {
            if (rote.DC.GetComponent<ControllDirection>().isOnDrag)
            {
                rote.AttackRangeDisplayRelease();
                rote.ControlRoleAttackDirection();
            }

            if (rote.FinishCollisionDetection && !rote.DC.GetComponent<ControllDirection>().isOnDrag)
            {
                AudioManager.Instance.PlayOneShot(Resources.Load<AudioClip>("Music/g_ui_confirm"), 100f, 0f);
                GamePageManage.Instance.DeductionPurchasePoint(rote.roteData);
                rote.SetAttackRange(rote.AttackRangeDisplayRelease());
                rote.ChangeState(new RoteIDleState(rote));
                GameObject BG = GameObject.Find("BG");
                BG.GetComponent<Image>().raycastTarget = true;
                BG.SetActive(false);
                Time.timeScale = 1f;
                rote.Actor.SetActive(true); 
                rote.DC.transform.position = new Vector3(0,-120,0);
                rote.DC = null;
            }
        }

    }
}

public class Rote : Character
{    void FixedUpdate()
    {
        currentState.UpdateState();
    }
    public void ChangeState(RoteState newState)
    {
        if(currentState != newState)
        {
        // 切换状态
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
        }
    }

    public RoteData roteData;

    public GameObject Actor;

    RectTransform healthBarRectTransform;
    Vector3 posHealthBar;
    Vector3 SclHealthBar;

    public bool FinishCollisionDetection = false;
    [SerializeField]
    private GameObject AttackRange;
    public RoteState currentState;
    private Vector3 dir;
    
    #region spine骨骼动画
    public AnimationManager animationManager;
    #endregion
    //攻击范围、变色
    public Material attackRangeMaterial;
    private float GirdOffset = 52.90475f;//52.16896f;
    //得出攻击范围对应地砖
    private List<Vector3> attackRange;
    private Stack<AttackRange> attackRangeStack = new Stack<AttackRange>();
    //需要阻挡的敌人
    public List< GameObject> blockEnemies = new List<GameObject>();
    //需要攻击的敌人
    public List<GameObject> BitEnemies = new List<GameObject>();
    //攻击方向摇杆
    public GameObject DC;
    //血条
    public GameObject healthBar;
    //对应的ActorBar
    public ActorBar actorBar;

    void Start()
    {
        roteData = DataManage.Instance.GetRoteData(gameObject.name);

        DC = FindAnyObjectByType<ControllDirection>().gameObject;
        DC.transform.position = new Vector3(transform.position.x, 120f, transform.position.z)  ;
        DC.SetActive(true);
        DC.GetComponent<ControllDirection>().rote = gameObject;
        Actor = DC.GetComponent<ControllDirection>().Actor;
        animationManager = gameObject.GetComponent<AnimationManager>();
        gameObject.GetComponent<HealthBar>().SetMaxHp(roteData.HP);
        attackRange = new List<Vector3>(DataManage.Instance.GetAttackRange(roteData.AttackRangeKey, transform.position));
        dir = transform.localScale;

        healthBar = GameObject.Find("HealthBar");
        healthBarRectTransform = healthBar.GetComponent<RectTransform>();
        posHealthBar = new Vector3(healthBarRectTransform.position.x, healthBarRectTransform.position.y, healthBarRectTransform.position.z);
        SclHealthBar = new Vector3(healthBarRectTransform.localScale.x, healthBarRectTransform.localScale.y, healthBarRectTransform.localScale.z);


        currentState = new RotePrepareState(this);
        currentState.EnterState();
    }

    // Update is called once per frame

    #region 自定义函数
    public void AttackRangeDisplay(Direction direction)//显示攻击范围
    {
        foreach (Vector3 vector in attackRange)
        {
            Vector3 targetVector = new Vector3();
            switch (direction)
            {
                case Direction.Left:
                    targetVector = vector;
                    break;
                case Direction.Right:
                    //绕Y轴旋转180度
                    targetVector = (Quaternion.Euler(0, 180, 0) * (vector - transform.position)) + transform.position;
                    break;
                case Direction.Up:
                    //绕Y轴旋转90度
                    targetVector = (Quaternion.Euler(0, 90, 0) * (vector - transform.position)) + transform.position;
                    break;
                case Direction.Down:
                    //绕Y轴旋转270度
                    targetVector = (Quaternion.Euler(0, 270, 0) * (vector - transform.position)) + transform.position;
                    break;
            }
            if (Physics.CheckSphere(new Vector3(targetVector.x, targetVector.y - GirdOffset, targetVector.z), 0.6f))
            {

                Collider[] colliders = Physics.OverlapSphere(new Vector3(targetVector.x, targetVector.y - GirdOffset, targetVector.z), 0.6f);
                foreach (Collider collider in colliders)
                {
                    if (collider.CompareTag("StandHighPlatform") || collider.CompareTag("StandGroundPlatform") || collider.CompareTag("DontStandGroundPlatform"))
                    {
                        attackRangeStack.Push(new AttackRange(collider.gameObject, collider.GetComponent<MeshRenderer>().material));
                        collider.GetComponent<MeshRenderer>().material = attackRangeMaterial;
                        Debug.Log("collider:" + collider.gameObject.name);
                    }
                }
            }
            if (Physics.CheckSphere(targetVector, 0.6f))
            {
                Collider[] colliders = Physics.OverlapSphere(targetVector, 0.6f);
                foreach (Collider collider in colliders)
                {
                    if (collider.CompareTag("StandHighPlatform") || collider.CompareTag("StandGroundPlatform") || collider.CompareTag("DontStandGroundPlatform"))
                    {
                        attackRangeStack.Push(new AttackRange(collider.gameObject, collider.GetComponent<MeshRenderer>().material));
                        collider.GetComponent<MeshRenderer>().material = attackRangeMaterial;
                        Debug.Log("collider:" + collider.gameObject.name);
                    }
                }
            }
        }
        
    }
    public List<AttackRange> AttackRangeDisplayRelease()
    {
        List<AttackRange> attackRangeList = new List<AttackRange>();
        while (attackRangeStack.Count > 0)
        {
            AttackRange attackRange = attackRangeStack.Pop();
            attackRange.obj.GetComponent<MeshRenderer>().material = attackRange.material;
            attackRangeList.Add(attackRange);
        }
        return new List<AttackRange>(attackRangeList);
    }
    //清楚显示攻击范围
    public void ControlRoleAttackDirection()//控制角色攻击方向
    {

        Vector2 direct = DC.GetComponent<ControllDirection>().direction;
        float radius = DC.GetComponent<ControllDirection>().Radius;
        healthBarRectTransform.localScale = new Vector3(-SclHealthBar.x, SclHealthBar.y, SclHealthBar.z);
        healthBarRectTransform.position = new Vector3(-posHealthBar.x, posHealthBar.y, posHealthBar.z);
        if (direct.normalized.x > 0.9f && Math.Abs(direct.x) > radius)
        {
            AttackRangeDisplay(Direction.Left);
            animationManager.SetSkeletonDataAsset("Face");
            healthBarRectTransform.localScale = new Vector3(-SclHealthBar.x, SclHealthBar.y, SclHealthBar.z);
            healthBarRectTransform.position = new Vector3(-posHealthBar.x, posHealthBar.y, posHealthBar.z);
            transform.localScale = new Vector3(dir.x, dir.y, dir.z);
            FinishCollisionDetection = true;
            actorBar.SetCoolDownEnable(true);

        }
        else if (direct.normalized.x < -0.9f && Math.Abs(direct.x) > radius)
        {
            AttackRangeDisplay(Direction.Right);
            animationManager.SetSkeletonDataAsset("Face");
            healthBarRectTransform.localScale = new Vector3(-SclHealthBar.x, SclHealthBar.y, SclHealthBar.z);
            healthBarRectTransform.position = new Vector3(-posHealthBar.x, posHealthBar.y, posHealthBar.z);
            transform.localScale = new Vector3(-dir.x, dir.y, dir.z);
            FinishCollisionDetection = true;
            actorBar.SetCoolDownEnable(true);

        }
        else if (direct.normalized.y < -0.9f && Math.Abs(direct.y) > radius)
        {
            AttackRangeDisplay(Direction.Up);
            animationManager.SetSkeletonDataAsset("Back");
            healthBarRectTransform.localScale = new Vector3(SclHealthBar.x, SclHealthBar.y, SclHealthBar.z);
            healthBarRectTransform.position = new Vector3(posHealthBar.x, posHealthBar.y, posHealthBar.z);
            FinishCollisionDetection = true;
            actorBar.SetCoolDownEnable(true);

        }
        else if (direct.normalized.y > 0.9f && Math.Abs(direct.y) > radius)
        {
            AttackRangeDisplay(Direction.Down);
            animationManager.SetSkeletonDataAsset("Face");
            healthBarRectTransform.localScale = new Vector3(-SclHealthBar.x, SclHealthBar.y, SclHealthBar.z);
            healthBarRectTransform.position = new Vector3(-posHealthBar.x, posHealthBar.y, posHealthBar.z);
            FinishCollisionDetection = true;
            actorBar.SetCoolDownEnable(true);
        }
        else
        {
            FinishCollisionDetection = false;

        }
    }
    public void UnplaceRote()//销毁函数
    {
        //销毁自己
        Destroy(gameObject);
    }

    public void SetAttackRange(List<AttackRange> attackRanges)//设置攻击范围
    {
        foreach (AttackRange item in attackRanges)
        {
            BoxCollider boxCollider = AttackRange.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            boxCollider.size = new Vector3(1.5f, 0.02f, 2.2f);
            boxCollider.center = AttackRange.transform.InverseTransformPoint(item.obj.transform.position);
        }

    }

    public override void HandleAnimationStateEvent(TrackEntry trackEntry, Spine.Event e)//调用敌人扣血函数,攻击特效
    {
        GameObject enemy = NearestEnemy();
        //Debug.Log("rote:" + gameObject.name + "nearsetEnemy:" + enemy.name);
        if (enemy != null)
        {
            //Debug.Log("rote:"+ gameObject.name + "nearsetEnemy:"+ enemy);
            enemy.GetComponent<HealthBar>().DecreaseHealth(DataManage.Instance.DamageCalculationInterface(roteData, DataManage.Instance.GetEnemyData(enemy.name), 2));
        }
        


    }
    public bool IsEnemiesEnterAttackRange() //检测是否有敌人进入攻击范围，转攻击状态
    {
        if(BitEnemies.Count > 0)
        {
            return true;
        }
            return false;
    }
    public GameObject NearestEnemy() //检测距离角色最近的敌人，返回敌人
    {
        if (BitEnemies.Count > 0)
        {
            GameObject nearsetEnemy = BitEnemies[0];
            foreach (GameObject enemy in BitEnemies)
            {
                if (Vector3.Distance(nearsetEnemy.transform.position, transform.position) > Vector3.Distance(enemy.transform.position, transform.position))
                {
                    nearsetEnemy = enemy;
                }
            }
            return nearsetEnemy;
        }
        return null;
    }
    #endregion

    public void OnTriggerEnter(Collider collider)//添加进阻挡敌人
    {
        foreach (var item in blockEnemies)
        {
            if (!item.gameObject.activeInHierarchy)
                blockEnemies.Remove(item);
        }
        if (blockEnemies.Count < roteData.BlockEnemyNumber)
        {
            blockEnemies.Add(NearestEnemy());
        }
        NearestEnemy().GetComponent<Enemy>().ChangeState(new EnemyAttackState(NearestEnemy().GetComponent<Enemy>()));
    }
}
