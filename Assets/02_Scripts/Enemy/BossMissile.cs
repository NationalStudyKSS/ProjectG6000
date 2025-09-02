using UnityEngine;

public class BossMissile : MonoBehaviour
{
    [Header("----- Component References -----")]
    [SerializeField] RigidbodyMover _mover; // 미사일 이동 컴포넌트

    [Header("----- Missile Settings -----")]
    [SerializeField] float _moveSpeed = 10f; // 미사일 이동 속도
    [SerializeField] float _rotSpeed = 5f; // 미사일 회전 속도
    [SerializeField] float _lifeTime = 5f; // 미사일 생존 시간
    [SerializeField] float _trackingTime = 2f; // 미사일의 최대 추적 시간

    [Header("ReadOnly")]
    [SerializeField] Transform _target; // 미사일 추적 대상
    [SerializeField] float _currentTrackingTimer; // 현재 추적 시간
    [SerializeField] Vector3 _moveDir; // 미사일 이동 방향

    public void Initialize(Transform target)
    {
        _target = target;
        _mover.SetMoveSpeed(_moveSpeed);
        _mover.SetRotSpeed(_rotSpeed);

        Destroy(gameObject, _lifeTime); // 일정 시간 후 미사일 파괴
    }

    private void FixedUpdate()
    {
        if (_target == null)
        {
            Debug.Log("목표를 포착하지 못했다...");
            return;
        }

        // 미사일 이동 및 회전
        if(_currentTrackingTimer < _trackingTime)
        {
            // 추적 시
            _moveDir = (_target.position - transform.position).normalized;
            _currentTrackingTimer += Time.fixedDeltaTime;
        }

        else
        {
            // 추적 종료 시 그냥 앞으로 가기
            _moveDir = transform.forward; 
        }

        _mover.Move(_moveDir);
    }
}