using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [Header("Hero��")]
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
    public HeroStateMachine StateMachine => _stateMachine;

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
        // Only allow attack if not already attacking
        if (_stateMachine.CurrentState.StateType != HeroStateType.Attack)
        {
            _stateMachine.ChangeState(_stateMachine.AttackState);
        }
    }

    /// <summary>
    /// IInteractable�� �������� �� �ڵ����� ȣ��Ǵ� �Լ�
    /// </summary>
    /// <param name="interactable"></param>
    void OnInteractableDetected(IInteractable interactable)
    {
        _statusView.SetInetractionGuide(true, interactable.GuidePoint);
    }

    /// <summary>
    /// IInteractable ������ �������� �� �ڵ����� ȣ��Ǵ� �Լ�
    /// </summary>
    void OnInteractableMissed()
    {
        _statusView.SetInetractionGuide(false, Vector3.zero);
    }

    /// <summary>
    /// ��ȣ�ۿ��� �����ϴ� �Լ�
    /// </summary>
    public void ExecuteInteraction()
    {
        _interactableDetector.ExecuteInteraction();
        Debug.Log("��ȣ�ۿ� �ǽ�");
    }
}
