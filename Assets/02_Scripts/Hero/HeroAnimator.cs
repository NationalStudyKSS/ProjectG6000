using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HeroAnimator : MonoBehaviour
{
    Animator _animator;
    Vector3 _velocity;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void OnMove(Vector3 velocity)
    {
        _velocity = velocity;

        //_animator.SetFloat("MoveX", _moveInput.x);
        //_animator.SetFloat("MoveY", _moveInput.z);
        _animator.SetFloat("MoveSpeed", _velocity.magnitude);
    }

    public void OnAttack1()
    {
        _animator.SetTrigger("Attack1");
    }

    public void OnAttack2()
    {
        _animator.SetTrigger("Attack2");
    }

    public void OnAttack3()
    {
        _animator.SetTrigger("Attack3");
    }
}