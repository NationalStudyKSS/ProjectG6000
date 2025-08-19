public class HeroStateAttack : HeroState
{
    public HeroStateAttack(Hero hero, HeroStateMachine stateMachine) : base(hero)
    {
    }

    public override HeroStateType StateType => HeroStateType.Attack;

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