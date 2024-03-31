using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Spine.Unity;
using System;
using Spine;
using cfg.enemyData;
using static UnityEngine.EventSystems.EventTrigger;

public abstract class EnemyState
{
    protected Enemy enemy;
    public EnemyState(Enemy enemy)
    {
        this.enemy = enemy;
    }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}
public class EnemyWalkingState : EnemyState
{
    public EnemyWalkingState(Enemy enemy) : base(enemy) { }

    public override void EnterState()
    {
        enemy.animationManager.SetAnimation("Move", true, 1);
    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {
        if (enemy.GetComponent<HealthBar>().currentHp <= 0)
        {
            enemy.ChangeState(new EnemyDeadState(enemy));
        }

        enemy.Move();
        //enemy.Rotate();
        if (enemy.CurrentPointPositionReached())
        {
            enemy.UpdateCurrentPointIndex();
        }
    }
}
public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(Enemy enemy) : base(enemy) { }

    public override void EnterState()
    {
        enemy.animationManager.SetAnimation("Attack", true, enemy.enemyData.ATS, enemy);
    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {
        if (enemy.GetComponent<HealthBar>().currentHp <= 0)
        {
            enemy.ChangeState(new EnemyDeadState(enemy));
        }
    }
}
public class EnemyDeadState : EnemyState
{
    public EnemyDeadState(Enemy enemy) : base(enemy) { }

    public override void EnterState()
    {
        enemy.animationManager.SetAnimation("Die", false, 1);
        enemy.animationManager.SetAnimation("Idle", true, 1);
    }

    public override void ExitState()
    {
        EnemyFactory.Instance.DestoryCharacter(enemy.gameObject);
    }

    public override void UpdateState()
    {
        if(enemy.animationManager.ReturnAnimationName().Equals("Idle"))
        {
            enemy.currentState.ExitState();
        }
    }
}

public class Enemy : Character
{    void FixedUpdate()
    {
        currentState.UpdateState();
    }
    public void ChangeState(EnemyState newState)
    {
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }
    #region 敌人初始数据
    public EnemyData enemyData { get; set; }
    #endregion
    private SpriteRenderer _spriteRenderer;
    #region spine骨骼动画
    public AnimationManager animationManager;
    #endregion
    [SerializeField]
    private GameObject bitRoteObj;

    public WayPoint Waypoint { get; set; }
    private Vector3 CurrentPointPosition;
    private Vector3 _lastPointPosition;
    private int _currentWaypointIndex = 1;

    public EnemyState currentState;
    public void Awake()
    {
        enemyData = DataManage.Instance.GetEnemyData(gameObject.name);
        gameObject.GetComponent<HealthBar>().SetMaxHp(enemyData.HP);
    }
    void Start()
    {
        transform.position = Waypoint.Points[0];
        CurrentPointPosition = Waypoint.Points[_currentWaypointIndex];
        _lastPointPosition = transform.position;
        animationManager = gameObject.GetComponent<AnimationManager>();

        currentState = new EnemyWalkingState(this);
        currentState.EnterState();
    }
    public void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, CurrentPointPosition, enemyData.MS * Time.deltaTime);
    }
    public void Rotate()
    {
        if (CurrentPointPosition.x > _lastPointPosition.x)

            _spriteRenderer.flipX = false;

        else
            _spriteRenderer.flipX = true;
    }
    public bool CurrentPointPositionReached()
    {
        float distanceToNextPointPosition = (transform.position - CurrentPointPosition).magnitude;
        if (distanceToNextPointPosition < 0.1f)
        {
            CurrentPointPosition = Waypoint.Points[_currentWaypointIndex];
            _lastPointPosition = transform.position;
            return true;
        }
        return false;
    }
    public void UpdateCurrentPointIndex()
    {
        int lastWaypointIndex = Waypoint.Points.Length - 1;
        if (_currentWaypointIndex < lastWaypointIndex)
        {
            _currentWaypointIndex++;
        }
    }
    public void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Rote")
        {
            bitRoteObj = collider.gameObject;
        }
    }
    public override void HandleAnimationStateEvent(TrackEntry trackEntry, Spine.Event e)// 在攻击动画完成时被调用的函数
    {
        bitRoteObj.GetComponent<HealthBar>().DecreaseHealth(DataManage.Instance.DamageCalculationInterface(DataManage.Instance.GetRoteData(bitRoteObj.name), enemyData, 1));
    }

}

