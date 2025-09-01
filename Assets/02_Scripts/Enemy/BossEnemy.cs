using UnityEngine;

public class BossEnemy : Enemy
{
    public override void CombatBehaviour()
    {
        throw new System.NotImplementedException();
    }

    public override void DeadBehaviour()
    {
        throw new System.NotImplementedException();
    }

    public override void IdleBehaviour()
    {
        DetectTarget();
    }

    public void UseSkill1()
    {

    }
}