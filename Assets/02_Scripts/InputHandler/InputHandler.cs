using System;
using UnityEngine;

/// <summary>
/// 입력을 받는 클래스
/// </summary>
public abstract class InputHandler : MonoBehaviour
{
    public event Action<Vector2> OnMove; // 이동 이벤트

}
