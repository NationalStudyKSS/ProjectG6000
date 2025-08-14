using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �̴ϸ� ī�޶��� ��ġ�� �����ϴ� Ŭ����
/// </summary>
public class MiniMapCameraController : MonoBehaviour
{
    [SerializeField] Camera _miniMapCamera; // �̴ϸ� ī�޶� ������Ʈ
    
    public void FollowTarget(Transform target)
    {
        transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
    }
}
