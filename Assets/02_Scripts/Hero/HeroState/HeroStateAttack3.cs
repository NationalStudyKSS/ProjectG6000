public class HeroStateAttack3 : HeroState
{
    public HeroStateAttack3(Hero hero, HeroStateMachine stateMachine) : base(hero, stateMachine)
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