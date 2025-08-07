using System;
using UnityEngine;

/// <summary>
/// 입력을 받는 클래스
/// </summary>
public abstract class InputHandler : MonoBehaviour
{
    public abstract event Action<Vector3> OnMoveInput; // 이동 이벤트

}
