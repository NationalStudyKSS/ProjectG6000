using System.Collections;
using UnityEngine;

public class BossEnemy : Enemy
{
    [Header("----- Component References -----")]
    [SerializeField] Transform _missilePortPos; // 미사일 발사 위치
    [SerializeField] GameObject _missilePrefab; // 미사일 프리팹

    [Header("----- Boss Stats(Temp) -----")]
    [SerializeField] float _skill1CoolTime = 10f; // 스킬1 쿨타임

    [Header("----- ReadOnly -----")]
    [SerializeField] float _currentSkill1Timer; // 현재 스킬1 타이머
    [SerializeField] bool _isSkill1Ready; // 스킬1 사용 가능 여부

    private void Update()
    {
        if (_hasFirstMet)
        {
            _currentSkill1Timer += Time.deltaTime;
            if(_currentSkill1Timer >= _skill1CoolTime)
            {
                _currentSkill1Timer = 0f;
                _isSkill1Ready = true;
            }
        }
    }

    // Animation Event용 함수
    public void OnAttackAnimationEnd()
    {
        _isAttacking = false;
        Debug.Log("공격 애니메이션 종료");
    }

    public void UseSkill1()
    {
        BossMissile missile = Instantiate(_missilePrefab, _missilePortPos.position, Quaternion.identity).GetComponent<BossMissile>();
        missile.Initialize(_target);
    }

    public void SetSkill1Ready(bool isReady)
    {
        _isSkill1Ready = isReady;
    }
}