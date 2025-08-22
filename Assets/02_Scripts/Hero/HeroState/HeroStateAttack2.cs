using UnityEngine;

public class HeroStateAttack2 : HeroState
{
    float _attackTimer = 0f;
    const float _attackDuration = 0.9f; // Slightly longer for second attack

    public HeroStateAttack2(Hero hero, HeroStateMachine stateMachine) : base(hero, stateMachine)
    {
    }

    public override HeroStateType StateType => HeroStateType.Attack;

    public override void Enter()
    {
        _attackTimer = 0f;
        _hero.Stop(); // Stop movement during attack
        _hero.Animator.OnAttack(); // Trigger attack animation
        
        // Play attack sound effect
        GameManager.Instance.AudioManager?.PlaySfx("Attack2");
    }

    public override void Exit()
    {
        _attackTimer = 0f;
    }

    public override void Update()
    {
        _attackTimer += UnityEngine.Time.deltaTime;
        
        // Return to idle state after attack animation duration
        if (_attackTimer >= _attackDuration)
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
        }
    }
}
