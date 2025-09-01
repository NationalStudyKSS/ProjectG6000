using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 적 캐릭터를 담당하는 클래스
/// 동작(행동) - 정지, 추적, 공격, 스킬, 사망
/// 
/// 상태
/// 대기(Idle): 대기 상태에 해야 할 동작을 하는 상태
/// 추적(Trace): 주기적으로 '추적' 동작을 하는 상태(FollwTarget)
/// 전투(Combat): 전투 상태에 진입했을 때 공격(Attack) 또는 스킬(UseSkill1) 동작을 하는 상태
/// 사망(Dead): '사망' 동작을 하는 상태(Die)
/// </summary>
public abstract class Enemy : MonoBehaviour
{
    [Header("----- Component References -----")]
    [SerializeField] protected EnemyModel _model; // 적 모델 컴포넌트    
    [SerializeField] protected Mover _mover; // 이동 컴포넌트
    [SerializeField] protected DamageableDetector _damageableDetector; // 데미지 감지 컴포넌트
    [SerializeField] protected EnemyAnimator _animator; // 애니메이터 컴포넌트

    [SerializeField] protected NavMeshAgent _navMeshAgent; // 적 AI 컴포넌트
    [SerializeField] protected Rigidbody _rigidbody; // Rigidbody 컴포넌트

    [Header("----- Enemy Stats(Temp) -----")]
    [SerializeField] protected float _detectRange = 10f; // 감지 범위

    [Header("----- ReadOnly -----")]
    [SerializeField] protected Transform _target; // 추적 대상
    [SerializeField] protected bool _hasDetected; // 감지에 성공했는지 여부
    [SerializeField] protected EnemyStateType _stateType; // 현재 상태 타입

    protected EnemyStateMachine _stateMachine; // 상태 머신

    public EnemyAnimator Animator => _animator;
    public Transform Target => _target;
    public EnemyModel Model => _model;
    public bool HasDetected => _hasDetected;    

    public void FixedUpdate()
    {
        _stateMachine.UpdateState();

        _stateType = _stateMachine.CurrentState.StateType;
    }

    /// <summary>
    /// 초기화 함수
    /// </summary>
    /// <param name="target"></param>
    public void Initialize(Transform target)
    {
        SetTarget(target);
    
        _stateMachine = new EnemyStateMachine(this);

        _mover.SetMoveSpeed(_model.MoveSpeed);
        _mover.SetRotSpeed(_model.RotSpeed);

    }

    /// <summary>
    /// 적이 대기 상태일 때 수행하는 동작
    /// </summary>
    public abstract void IdleBehaviour();

    /// <summary>
    /// 적이 전투 상태일 때 수행하는 동작
    /// </summary>
    public abstract void CombatBehaviour();

    /// <summary>
    /// 적이 사망 상태일 때 수행하는 동작
    /// </summary>
    public abstract void DeadBehaviour();

    /// <summary>
    /// 목표 대상의 Transform을 설정하는 함수
    /// </summary>
    /// <param name="target">목표대상의 Transform</param>
    public void SetTarget(Transform target)
    {
        Debug.Log("타겟 설정");
        _target = target;
    }

    public void DetectTarget()
    {
        Vector3 distance = _target.position - transform.position;

        if (!_hasDetected && distance.magnitude <= _detectRange)
        {
            _hasDetected = true;
            Debug.Log("타겟 감지");
            _stateMachine.ChangeState(EnemyStateType.Combat);
        }
    }
}