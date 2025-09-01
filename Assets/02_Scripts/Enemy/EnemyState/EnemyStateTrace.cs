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
        
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }
}   