public class HeroStateAttack1 : HeroState
{
    float _timer;

    public HeroStateAttack1(Hero hero, HeroStateMachine stateMachine) : base(hero)
    {

    }

    public override HeroStateType StateType => HeroStateType.Attack;

    public override void Enter()
    {
        _hero.Animator.OnAttack1();
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