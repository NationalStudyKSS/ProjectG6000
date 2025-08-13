using UnityEngine;

public class Hero : MonoBehaviour
{
    [Header("Hero²¨")]
    [SerializeField] Mover _mover;
    [SerializeField] HeroAnimator _animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize()
    {
        _mover.OnMoved += OnMoved;
    }

    public void Move(Vector3 direction)
    {
        _mover.Move(direction);
    }

    void OnMoved(Vector3 velocity)
    {
        _animator.OnMove(velocity);
    }

    public void Attack()
    {
        _animator.OnAttack();
    }
}
