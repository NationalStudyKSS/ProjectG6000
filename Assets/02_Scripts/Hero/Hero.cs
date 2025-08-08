using UnityEngine;

public class Hero : MonoBehaviour
{
    [Header("Hero²¨")]
    [SerializeField] Mover _mover;
    [SerializeField] HeroAnimator _animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize()
    {

    }

    public void Move(Vector3 direction)
    {
        _mover.Move(direction);
        _animator.OnMove(direction);
    }
}
