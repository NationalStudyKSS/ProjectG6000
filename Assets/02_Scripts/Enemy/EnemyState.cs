using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 상태 패턴(State Pattern)
// 어떤 객체가 상태에 따라 다르게 행동할 때
// 각 상태를 객체화하여 필요에 따라 다르게 행동하도록 위임하는 디자인 패턴
// -> 행동들은 본래 클래스에 함수로 정의
// -> 상태들은 별도 클래스로 분리해서 본래 객체의 행동 함수들을 실행

/// <summary>
/// 적 캐릭터의 상태를 나타내는 열거형
/// </summary>
public enum EnemyStateType
{
    Idle,       // 방치 상태
    Trace,      // 추적 상태
    Combat,     // 전투 상태
    Dead,      // 사망 상태
    Count,      // 상태 종류 수 카운트 용
}

/// <summary>
/// 적 캐릭터 상태 클래스들의 공통 부모 추상 클래스
/// </summary>
public abstract class EnemyState
{
    protected Enemy _enemy;

    /// <summary>
    /// 상태 종류를 반환
    /// </summary>
    public abstract EnemyStateType StatType { get; }

    /// <summary>
    /// Enemy 상태 객체 생성자
    /// </summary>
    /// <param name="enemy">상태를 적용할 Enemy 객체(컴포넌트)</param>
    public EnemyState(Enemy enemy)
    {
        _enemy = enemy;
    }

    /// <summary>
    /// 상태 진입 시 호출되는 함수
    /// </summary>
    public abstract void Enter();

    /// <summary>
    /// 상태 유지 시 매 프레임 호출되는 함수
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// 상태 종료 시 호출되는 함수
    /// </summary>
    public abstract void Exit();
}

/// <summary>
/// 적 캐릭터가 방치되어 있는 상태
/// 주기적으로 '배회(Roam)' 동작을 실행
/// </summary>
public class IdleState : EnemyState
{
    float _timer;
    public override EnemyStateType StatType => EnemyStateType.Idle;

    public IdleState(Enemy enemy) : base(enemy)
    {
        
    }

    public override void Enter()
    {
        _enemy.Stop();
        _timer = 0;
    }

    public override void Exit()
    {
        _enemy.Stop();
    }

    public override void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > _enemy.RoamSpan)
        {
            _timer = 0;
            _enemy.Roam();
        }
    }
}

public class TraceState : EnemyState
{
    float _timer;
    public override EnemyStateType StatType => EnemyStateType.Trace;
    public TraceState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        _timer = 0;
    }

    public override void Exit()
    {
        _enemy.Stop();
    }

    public override void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > _enemy.ThinkSpan)
        {
            _timer = 0;
            _enemy.FollowTarget();
        }
    }
}

public class CombatState : EnemyState
{
    float _timer;
    public override EnemyStateType StatType => EnemyStateType.Combat;
    public CombatState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        _timer = 0;
        _enemy.Attack();
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        _enemy.RotateTowardTarget();
        _timer += Time.deltaTime;
        if (_timer > _enemy.AttackSpan)
        {
            _timer = 0;
            _enemy.Attack();
        }
    }
}

public class DeadState : EnemyState
{
    float _timer = 0;
    public override EnemyStateType StatType => EnemyStateType.Dead;
    public DeadState(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        _enemy.Stop();
    }

    public override void Exit()
    {
        
    }

    public override void Update()
    {
        _timer += Time.deltaTime;
        if(_timer > _enemy.DeadDuration)
        {
            _timer = 0;
            _enemy.Remove();
        }
    }
}