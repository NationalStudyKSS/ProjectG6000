using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 객체를 Json 형식으로 변환하기 위한 필수 요소(JsonUtility)
// 1) 클래스가 System.Serializable로 설정되어 있어야 한다.
// 2) Json 직렬화/역직렬화를 할 변수(필드)들은 [SerializeField]이거나 public이어야 한다.

/// <summary>
/// 주인공 캐릭터 정보를 저장하는 데이터 클래스.
/// 이름, 스탯 등
/// </summary>
[Serializable]      // 인스펙터뷰 확인용/ 수정용 X
public class HeroData
{
    [SerializeField] string _heroName = "Hero";  // 주인공 이름
    [SerializeField] int _level = 1;             // 레벨은 1부터 시작
    [SerializeField] float _exp = 0.0f;
    [SerializeField] int _gold = 5;
    [SerializeField] Vector3 _position = Vector3.zero; // 주인공 위치

    // 레벨업에 필요한 기본 경험치
    const float _baseExpRequirement = 100f;
    // 레벨당 증가하는 경험치 배율
    const float _expGrowthRate = 1.5f;

    public string HeroName => _heroName;
    public int Level => _level;
    public float Exp => _exp;
    public int Gold => _gold;
    public Vector3 Position => _position;

    // 현재 레벨에서 다음 레벨까지 필요한 경험치 계산
    public float ExpRequiredForNextLevel => _baseExpRequirement * Mathf.Pow(_expGrowthRate, _level - 1);
    
    // 현재 레벨에서의 경험치 진행도 (0-1)
    public float ExpProgress => _exp / ExpRequiredForNextLevel;

    /// <summary>
    /// 주인공 캐릭터 이름을 변경하는 함수
    /// </summary>
    /// <param name="heroName">새 이름</param>
    public void SetHeroName(string heroName)
    {
        _heroName = heroName;
    }

    /// <summary>
    /// 경험치를 추가하고 레벨업 여부를 반환하는 함수
    /// </summary>
    /// <param name="expAmount">추가할 경험치</param>
    /// <returns>레벨업 발생 여부</returns>
    public bool AddExperience(float expAmount)
    {
        _exp += expAmount;
        bool leveledUp = false;

        // 레벨업 체크 (여러 레벨 한번에 오를 수 있도록)
        while (_exp >= ExpRequiredForNextLevel && _level < 99) // 최대 레벨 99
        {
            _exp -= ExpRequiredForNextLevel;
            _level++;
            leveledUp = true;
        }

        return leveledUp;
    }

    /// <summary>
    /// 골드를 추가하는 함수
    /// </summary>
    /// <param name="goldAmount">추가할 골드</param>
    public void AddGold(int goldAmount)
    {
        _gold = Mathf.Max(0, _gold + goldAmount);
    }

    /// <summary>
    /// 골드를 사용하는 함수
    /// </summary>
    /// <param name="goldAmount">사용할 골드</param>
    /// <returns>골드 사용 가능 여부</returns>
    public bool SpendGold(int goldAmount)
    {
        if (_gold >= goldAmount)
        {
            _gold -= goldAmount;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 위치를 업데이트하는 함수
    /// </summary>
    /// <param name="newPosition">새 위치</param>
    public void UpdatePosition(Vector3 newPosition)
    {
        _position = newPosition;
    }
}
