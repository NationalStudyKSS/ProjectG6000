using System;
using UnityEngine;

public class EnemyStateFirstMeet : EnemyState
{
    public event Action OnFirstMeetStarted;

    public EnemyStateFirstMeet(Enemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {

    }

    public override EnemyStateType StateType => EnemyStateType.FirstMeet;

    public override void Enter()
    {
        _enemy.Animator.PlayWelcome();
        OnFirstMeetStarted?.Invoke();
    }

    public override void Exit()
    {
        
    }

    public override void Update()
    {
        // 타겟 바라보기
        Vector3 lookPos = _enemy.Target.position - _enemy.transform.position;
        lookPos.y = 0;
        _enemy.transform.rotation = Quaternion.LookRotation(lookPos);
    }

    // Animation Event용 함수
    public void OnFirstMeetAnimationEnd()
    {
        // 앞으로는 FirstMeet 안 들어가게
        _enemy.SetFirstMet();

        // 전투 상태 진입
        _stateMachine.ChangeState(EnemyStateType.Combat);
    }
}