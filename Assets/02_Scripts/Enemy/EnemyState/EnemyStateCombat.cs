using UnityEngine;

public class EnemyStateCombat : EnemyState
{
    public override EnemyStateType StateType => EnemyStateType.Combat;
    
    public EnemyStateCombat(Enemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }
    public override void Enter()
    {
        Debug.Log("전투 상태 진입");
    }

    public override void Update()
    {
        _enemy.CombatBehaviour();
    }

    public override void Exit()
    {
        Debug.Log("전투 상태 종료");
    }
}