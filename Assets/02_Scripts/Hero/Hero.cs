using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [Header("Hero꺼")]
    [SerializeField] Mover _mover;
    [SerializeField] HeroAnimator _animator;
    [SerializeField] InteractableDetector _interactableDetector;
    [SerializeField] HeroStatusView _statusView;

    HeroStateMachine _stateMachine;
    [SerializeField] HeroStateType _stateType;

    bool _attackInput;          // 공격 가능한지 여부에 대한 토큰(이게 있어야 공격 머신에 동전을 넣을수 있음)

    public Mover Mover => _mover;
    public HeroAnimator Animator => _animator;
    public InteractableDetector InteractableDetector => _interactableDetector;
    public HeroStatusView StatusView => _statusView;
    public bool AttackInput => _attackInput;

    public void Initialize()
    {
        _mover.OnMoved += OnMoved;
        _interactableDetector.OnDetected += OnInteractableDetected;
        _interactableDetector.OnMissed += OnInteractableMissed;

        _stateMachine = new HeroStateMachine(this);
    }

    public void Update()
    {
        _stateMachine.UpdateState();
        _stateType = _stateMachine.CurrentState.StateType;
    }

    /// <summary>
    /// 이동 입력을 받았을 때 Mover의 Move함수를 호출하여
    /// 영웅을 움직이는 함수
    /// </summary>
    /// <param name="direction"></param>
    public void Move(Vector3 direction)
    {
        _mover.Move(direction);
    }

    /// <summary>
    /// 영웅의 움직임을 멈추는 함수
    /// </summary>
    public void Stop()
    {
        _mover.Move(Vector3.zero);
    }

    /// <summary>
    /// 영웅이 움직일때 호출되는 함수
    /// 이동했을 때와 관련된 함수들을 호출한다.
    /// </summary>
    /// <param name="velocity"></param>
    void OnMoved(Vector3 velocity)
    {
        _animator.OnMove(velocity);
    }

    /// <summary>
    /// 공격 입력을 받았을 때 플래그를 true로 전환하는 함수
    /// </summary>
    public void OnAttackInput()
    {
        _attackInput = true;
    }

    /// <summary>
    /// 공격 입력 플래그를 false로 전환하는 함수
    /// 상태 머신에서 조건을 판단하여 이 함수를 호출해 플래그를 조절
    /// </summary>
    public void ClearAttackInput()
    {
        _attackInput = false;
    }

    /// <summary>
    /// IInteractable을 감지했을 때 자동으로 호출되는 함수
    /// </summary>
    /// <param name="interactable"></param>
    void OnInteractableDetected(IInteractable interactable)
    {
        _statusView.SetInetractionGuide(true, interactable.GuidePoint);
    }

    /// <summary>
    /// IInteractable 감지를 실패했을 때 자동으로 호출되는 함수
    /// </summary>
    void OnInteractableMissed()
    {
        _statusView.SetInetractionGuide(false, Vector3.zero);
    }

    /// <summary>
    /// 상호작용을 수행하는 함수
    /// </summary>
    public void ExecuteInteraction()
    {
        _interactableDetector.ExecuteInteraction();
        Debug.Log("상호작용 실시");
    }
}
