using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

// RigidbodyMover ������Ʈ�� ������ �ش� ���ӿ�����Ʈ�� Rigidbody�� �ݵ�� �־�� �Ѵ�.
[RequireComponent(typeof(Rigidbody))]
public class RigidbodyMover : Mover
{
    public override event Action<Vector3> OnMoved;

    Rigidbody _rigid;       // ������ٵ� ����
    Vector3 _velocity;      // �̵� �ӵ� ����
    Quaternion _targetRotation; // ���� ��ǥ ȸ����

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _targetRotation = _rigid.rotation;
    }

    private void FixedUpdate()
    {
        // ������ٵ��� y ���� �ӷ°� �״�� ����
        _velocity.y = _rigid.linearVelocity.y;

        // ������ٵ� ���ϴ� �ӵ� ���� ����
        _rigid.linearVelocity = _velocity;

        // ������ٵ� ��ǥ ȸ������ ���� �ε巯�� ȸ�� ����
        //_rigid.rotation = Quaternion.RotateTowards(
        //    _rigid.rotation,
        //    _targetRotation,
        //    _rotSpeed * Time.fixedDeltaTime);

        // �̵� �̺�Ʈ ����
        _velocity.y = 0;
        OnMoved?.Invoke(_velocity);
    }

    public override void Move(Vector3 direction)
    {
        // y ���� �̵��� ����
        direction.y = 0;

        // �̵� ������ ũ�Ⱑ ���� 0�̸�
        //if (direction.magnitude < Util.Epsilon)
        if (direction.magnitude < 0.1f)
        {
            // ���� �ӵ��� 0���� ����
            _velocity = Vector3.zero;
        }
        else
        {
            // ���� �ӵ��� ����
            _velocity = direction.normalized * _moveSpeed;

            //_targetRotation = Quaternion.LookRotation(direction);
        }
    }
}