using UnityEngine;

public class EnemyStateTrace : EnemyState
{
    public override EnemyStateType StateType => EnemyStateType.Trace;
    public EnemyStateTrace(Enemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {

    }

    public override void Enter()
    {
        
    }

    public override void Update()
    {
        FollowTarget();

        if(_enemy as BossEnemy)?.

        if (Vector3.Distance(transform.position, _target.position) <= _attackRange)
        {
            Debug.Log("공격 범위 내 진입");
            _stateMachine.ChangeState(EnemyStateType.Combat);
        }
    }

    public override void Exit()
    {
        
    }
}   