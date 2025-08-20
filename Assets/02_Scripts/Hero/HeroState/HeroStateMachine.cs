using UnityEngine;

/// <summary>
/// 영웅의 상태머신을 담당하는 클래스
/// </summary>
public class HeroStateMachine
{
    Hero _hero;
    
    HeroStateIdle _idleState;
    HeroStateAttack1 _attackState;
    HeroStateMove _moveState;

    HeroState _currentState;

    public HeroStateMachine(Hero hero)
    {
        _hero = hero;

        // 상태 객체 생성
        _idleState = new HeroStateIdle(_hero, this);
        _attackState = new HeroStateAttack1(_hero, this);
        _moveState = new HeroStateMove(_hero, this);

        // 상태 초기화
        _currentState = _idleState;
    }

    public void ChangeState(HeroState newState)
    {
        _currentState.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    public void UpdateState()
    {
        _currentState.Update();
    }
}
