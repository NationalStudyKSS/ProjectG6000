using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HeroAnimator : MonoBehaviour
{
    Animator animator;
    Vector3 moveInput;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnMove(Vector3 input)
    {
        moveInput = input;

        animator.SetFloat("MoveX", moveInput.x);
        animator.SetFloat("MoveY", moveInput.z);
    }
}
