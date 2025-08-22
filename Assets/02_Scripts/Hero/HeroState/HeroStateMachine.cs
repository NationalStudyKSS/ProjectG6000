using UnityEngine;

/// <summary>
/// ������ ���¸ӽ��� ����ϴ� Ŭ����
/// </summary>
public class HeroStateMachine
{
    Hero _hero;
    
    HeroStateIdle _idleState;
    HeroStateAttack1 _attackState;
    HeroStateMove _moveState;

    HeroState _currentState;

    // Property accessors for states
    public HeroStateIdle IdleState => _idleState;
    public HeroStateAttack1 AttackState => _attackState;
    public HeroStateMove MoveState => _moveState;
    public HeroState CurrentState => _currentState;

    public HeroStateMachine(Hero hero)
    {
        _hero = hero;

        // ���� ��ü ����
        _idleState = new HeroStateIdle(_hero, this);
        _attackState = new HeroStateAttack1(_hero, this);
        _moveState = new HeroStateMove(_hero, this);

        // ���� �ʱ�ȭ
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
