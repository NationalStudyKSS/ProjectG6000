using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 적 캐릭터를 담당하는 클래스
/// 동작(행동) - 정지, 배회, 추적, 공격, 회전
/// 
/// 상태
/// 방치(Idle): 주기적으로 '배회' 동작을 하는 상태
/// 추적(Trace): 주기적으로 '추적' 동작을 하는 상태
/// 전투(Combat): 주기적으로 '공격' 동작을 하는 상태
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("----- 타겟 -----")]
    [SerializeField] NavMeshAgent _navAgent;

    [Header("----- 컴포넌트 참조 -----")]
    [SerializeField] CombatCharacterModel _model;
    [SerializeField] Transform _target;
    [SerializeField] Animator _animator;
    [SerializeField] CharacterAnimatorHandler _animatorHandler;
    [SerializeField] DamageableDetector _damageableDetector;
    [SerializeField] EnemyHud _enemyHud;
    [SerializeField] Collider _collider; 

    [Header("----- AI -----")]
    [SerializeField] float _thinkSpan;              // AI 판단 간격(초)
    [SerializeField] float _roamDistance;           // 배회 거리
    [SerializeField] float _roamSpan;               // 배회 간격(초)
    [SerializeField] float _traceDistance;          // 추적 거리
    [SerializeField] float _attackDistance;         // 공격 거리
    [SerializeField] float _attackSpan;             // 공격 간격(초)
    [SerializeField] float _rotAngleThreshhold;     // 회전 각도 임계값
    [SerializeField] float _deadDuration;               // 사망 애니메이션 재생 시간

    /// <summary>
    /// 적 제거 이벤트 변수
    /// </summary>
    public event Action<Enemy> OnRemoved;
    
    /// <summary>
    /// 적 캐릭터 상태 객체들
    /// </summary>
    EnemyState[] _states = new EnemyState[(int)EnemyStateType.Count];

    /// <summary>
    /// 현재 상태
    /// </summary>
    EnemyState _currentState;

    /// <summary>
    /// 현재 이동 속력
    /// </summary>
    float _currentSpeed;

    /// <summary>
    /// 현재 회전 중인지 여부
    /// </summary>
    bool _isRotating = false;

    Coroutine _calculateStateRoutine;

    public float ThinkSpan => _thinkSpan;
    public float RoamSpan => _roamSpan;
    public float AttackSpan => _attackSpan;
    public float DeadDuration => _deadDuration;

    private void Awake()
    {
        // 사망 이벤트 구독
        _model.Ondead += OnDead;

        // 공격 판정 이벤트 구독
        //_animatorHandler.OnAttacked += _damageableDetector.DetectDamageable;
        _animatorHandler.OnAttacked += OnAttacked;

        // IDamageable 감지 이벤트 구독
        //_damageableDetector.OnDetected += _model.Hit;
        _damageableDetector.OnDetected += Hit;

        // 체력 변경 이벤트 구독
        _model.OnHpChanged += _enemyHud.SetHpBar;
    }

    /// <summary>
    /// Enemy 컴포넌트를 초기화하는 함수
    /// </summary>
    /// <param name="target">추적할 타겟 트랜스폼</param>
    public void Initialize(Transform target)
    {
        _target = target;

        _collider.enabled = true; // 콜라이더 활성화
        _navAgent.enabled = true; // NavMeshAgent 활성화

        _model.Initialize();

        _navAgent.speed = _model.MoveSpeed;
        _navAgent.angularSpeed = _model.RotSpeed;

        _states[(int)EnemyStateType.Idle] = new IdleState(this);
        _states[(int)EnemyStateType.Trace] = new TraceState(this);
        _states[(int)EnemyStateType.Combat] = new CombatState(this);
        _states[(int)EnemyStateType.Dead] = new DeadState(this);

        _currentState = _states[(int)EnemyStateType.Idle];
        _currentState.Enter();

        // 상태 계산 코루틴 실행
        _calculateStateRoutine = StartCoroutine(CalculateStateRoutine());
    }

    private void Update()
    {
        _currentState.Update();

        if (_isRotating == false)
        {
            _currentSpeed = _navAgent.velocity.magnitude;
        }
        else
        {
            _currentSpeed = _model.MoveSpeed;
        }
        _animator.SetFloat(AnimatorParameters.MoveSpeed, _currentSpeed);
    }

    IEnumerator CalculateStateRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_thinkSpan);
            CalculateState();
        }
    }

    /// <summary>
    /// 현재 상태를 판단하는(계산하는) 함수
    /// </summary>
    public void CalculateState()
    {
        float distance = Vector3.Distance(transform.position, _target.position);
        if (distance > _traceDistance)
        {
            ChangeState(EnemyStateType.Idle);
        }
        else if (distance > _attackDistance)
        {
            ChangeState(EnemyStateType.Trace);
        }
        else
        {
            ChangeState(EnemyStateType.Combat);
        }
    }

    /// <summary>
    /// 상태를 변경하는 함수
    /// </summary>
    /// <param name="stateType"></param>
    void ChangeState(EnemyStateType stateType)
    {
        // 기존 상태가 새로 바꾸려는 상태와 동일하면 리턴
        if (_currentState.StatType == stateType) return;

        // 기존 상태가 사망 상태인 경우 리턴
        if (_currentState.StatType == EnemyStateType.Dead) return;

        // 없는 상태로 바꾸려는 경우 리턴
        int stateIndex = (int)stateType;
        if(stateIndex < 0 || stateIndex >= _states.Length) return;

        // 기존 상태 종료
        _currentState.Exit();
        _isRotating = false;

        // 새 상태 적용
        _currentState = _states[stateIndex];

        // 새 상태 실행
        _currentState.Enter();

        Debug.Log(stateType.ToString());
    }

    /// <summary>
    /// 배회 동작을 실행하는 함수
    /// </summary>
    public void Roam()
    {
        // 랜덤한 x, y 방향 벡터 생성
        Vector2 offset = UnityEngine.Random.insideUnitCircle * _roamDistance;

        // 현재 위치 기준으로 랜덤 방향의 목표 지점 설정
        Vector3 targetPos = transform.position;
        targetPos.x += offset.x;
        targetPos.z += offset.y;

        // 설정된 목표 지점을 NavMeshAgent의 목표지로 적용
        _navAgent.SetDestination(targetPos);
    }

    /// <summary>
    /// 타겟을 목적지로 설정하는 함수
    /// (NavMeshAgent를 사용해서 자동으로 이동)
    /// </summary>
    public void FollowTarget()
    {
        // NavMeshAgent에 목적지를 설정하는 함수
        _navAgent.SetDestination(_target.position);
    }

    /// <summary>
    /// 이동을 멈추는 함수
    /// </summary>
    public void Stop()
    {
        // NavMeshAgent 이동을 멈추는 코드
        _navAgent.isStopped = true;

        // NavMeshAgent 경로 초기화
        _navAgent.ResetPath();
    }

    /// <summary>
    /// 공격을 실행하는 함수
    /// </summary>
    public void Attack()
    {
        _animator.SetTrigger(AnimatorParameters.OnAttack);
    }

    /// <summary>
    /// 공격 판정 시 자동으로 실행되는 함수
    /// </summary>
    void OnAttacked()
    {
        Debug.Log("공격 판정 시도....");
        _damageableDetector.DetectDamageable();
    }

    /// <summary>
    /// 감지된 IDamageable에 데미지를 입히는 함수.
    /// IDamageable이 감지되었을 때 자동으로 실행되는 함수
    /// </summary>
    /// <param name="damageable"></param>
    void Hit(IDamageable damageable)
    {
        _model.Hit(damageable);
    }

    /// <summary>
    /// 타겟을 향해 회전하는 함수
    /// </summary>
    public void RotateTowardTarget()
    {
        // 바라봐야 하는 방향
        Vector3 direction = _target.position - transform.position;
        direction.y = 0;
        if (direction == Vector3.zero) return;

        // 목표 회전값 설정
        Quaternion targetRotation =
            Quaternion.LookRotation(direction, Vector3.up);

        // 현재 회전값과 목표 회전값 사이 각도 계산
        float angle = Quaternion.Angle(
            transform.rotation, targetRotation);

        if (angle < _rotAngleThreshhold)
        {
            _isRotating = false;
            return;
        }

        // 회전 적용
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            _model.RotSpeed * Time.deltaTime);
        _isRotating = true;
    }

    /// <summary>
    /// 적이 사망했을 때 실행되는 함수
    /// </summary>
    void OnDead()
    {
        // AI 상태 계산 코루틴 중지
        StopCoroutine(_calculateStateRoutine);

        // 사망 애니메이션 재생
        _animator.SetTrigger(AnimatorParameters.OnDeath);

        // 콜라이더 컴포넌트 Off
        _collider.enabled = false;

        // 사망 상태로 변경
        ChangeState(EnemyStateType.Dead);

        // NavMeshAgnet 컴포넌트 Off
        _navAgent.enabled = false;
    }

    /// <summary>
    /// 적을 제거하는 함수
    /// </summary>
    public void Remove()
    {
        OnRemoved?.Invoke(this);    // 제거 이벤트 발행
        OnRemoved = null;           // 제거 이벤트 전체 구독 해지

        gameObject.DestroyORReturnToPool(); // 게임 오브젝트 제거
    }

    // 풀로 되돌아가지 못하고 게임에서 제거될 수 있기 때문에
    // 외부와 통하는 이벤트 변수는 전부 구독 해제
    private void OnDestroy()
    {
        OnRemoved = null;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _traceDistance);
    }
}

//private void Update()
//{
//    float distance = Vector3.Distance(transform.position, _target.position);
//    if (distance < _traceDistance)
//    {
//        // 배회 루틴이 켜져 있으면
//        if (_roamRoutine != null)
//        {
//            StopCoroutine(_roamRoutine);
//        }
//        // 추적 루틴이 꺼져 있으면
//        if (_traceRoutine == null)
//        {
//            _traceRoutine = StartCoroutine(TraceRoutine());
//        }
//    }
//    else
//    {
//        // 추적 루틴이 켜져 있으면
//        if (_traceRoutine != null)
//        {
//            StopCoroutine(_traceRoutine);
//        }
//        // 배회 루틴이 꺼져 있으면
//        if (_roamRoutine == null)
//        {
//            _roamRoutine = StartCoroutine(RoamRoutine());
//        }
//    }
//}