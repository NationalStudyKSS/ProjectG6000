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

    Dictionary<HeroStateType, HeroState> _states = new();

    public Mover Mover => _mover;
    public HeroAnimator Animator => _animator;
    public InteractableDetector InteractableDetector => _interactableDetector;
    public HeroStatusView StatusView => _statusView;

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
    }

    public void Move(Vector3 direction)
    {
        _mover.Move(direction);
    }

    public void Stop()
    {
        _mover.Move(Vector3.zero);
    }

    void OnMoved(Vector3 velocity)
    {
        _animator.OnMove(velocity);
    }

    public void Attack()
    {
        _animator.OnAttack();
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
