using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 유니티 Input System의 PlayerInput을 활용해 입력을 받아 알리는 역할
/// </summary>
public class PlayerInputHandler : InputHandler
{
    public override event Action<Vector3> OnMoveInput;

    Vector3 _moveInput;

    /// <summary>
    /// Player Input으로부터 이동 입력을 받는 함수
    /// </summary>
    /// <param name="inputValue"></param>
    void OnMove(InputValue inputValue)
    {
        Vector2 moveInput = inputValue.Get<Vector2>();
        _moveInput = new Vector3(moveInput.x, 0, moveInput.y);
    }

    private void FixedUpdate()
    {
        //Debug.Log(_moveInput);
        OnMoveInput?.Invoke(_moveInput);
    }
}