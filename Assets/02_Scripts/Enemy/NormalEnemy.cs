using UnityEngine;

public class NormalEnemy : Enemy
{
    public override void IdleBehaviour()
    {
        DetectTarget();
    }

    /// <summary>
    /// 목표 대상 방향을 향해 이동하며 추적하는 함수
    /// </summary>
    public void FollowTarget()
    {
        _navMeshAgent.SetDestination(_target.position);
        Vector3 direction = _navMeshAgent.desiredVelocity.normalized;
        _mover.Move(direction);
    }

    /// <summary>
    /// 적의 움직임을 멈추고 대기 상태로 전환하는 함수
    /// </summary>
    public void Stop()
    {
        _mover.Move(Vector3.zero);
        _stateMachine.ChangeState(EnemyStateType.Idle);
    }

    public override void CombatBehaviour()
    {
        throw new System.NotImplementedException();
    }

    public override void DeadBehaviour()
    {
        throw new System.NotImplementedException();
    }
}   