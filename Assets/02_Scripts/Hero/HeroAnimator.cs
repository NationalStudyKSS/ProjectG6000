using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HeroAnimator : MonoBehaviour
{
    Animator _animator;
    Vector3 _moveInput;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void OnMove(Vector3 input)
    {
        _moveInput = input;

        //_animator.SetFloat("MoveX", _moveInput.x);
        //_animator.SetFloat("MoveY", _moveInput.z);
        _animator.SetFloat("MoveSpeed", _moveInput.magnitude);
    }
}
