using UnityEngine;

public class EnemyStateIdle : EnemyState
{
    public EnemyStateIdle(Enemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {

    }

    public override EnemyStateType StatType => EnemyStateType.Idle;

    public override void Enter()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        throw new System.NotImplementedException();
    }
}
