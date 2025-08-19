using UnityEngine;

/// <summary>
/// 영웅의 상태머신을 담당하는 클래스
/// </summary>
public class HeroStateMachine
{
    Hero _hero;
    HeroState _currentState;

    public HeroStateMachine(Hero hero)
    {
        _hero = hero;
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
