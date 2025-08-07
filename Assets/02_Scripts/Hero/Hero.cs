using UnityEngine;

public class Hero : MonoBehaviour
{
    [Header("¿”Ω√")]
    [SerializeField] InputHandler _inputHandler;
    [SerializeField] VCameraController _cameraController;

    [Header("Hero≤®")]
    [SerializeField] Mover _mover;
    [SerializeField] HeroAnimator _animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputHandler.OnMoveInput += _mover.Move;
        _inputHandler.OnMoveInput += _animator.OnMove;
        _inputHandler.OnCameraRotInput += _cameraController.Rotate;
    }
}
