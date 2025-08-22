using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 퀘스트 타입을 정의하는 열거형
/// </summary>
public enum QuestType
{
    Kill,           // 몬스터 처치
    Collect,        // 아이템 수집
    Talk,           // NPC 대화
    Reach,          // 특정 위치 도달
    Deliver,        // 아이템 전달
    Survive         // 생존
}

/// <summary>
/// 퀘스트 상태를 정의하는 열거형
/// </summary>
public enum QuestStatus
{
    NotStarted,     // 시작 안함
    InProgress,     // 진행 중
    Completed,      // 완료
    Failed,         // 실패
    TurnedIn        // 보상 수령 완료
}

/// <summary>
/// 퀘스트 보상 정보
/// </summary>
[Serializable]
public class QuestReward
{
    [SerializeField] int _experience;       // 경험치 보상
    [SerializeField] int _gold;             // 골드 보상
    [SerializeField] string[] _itemIds;     // 아이템 보상 ID 배열

    public int Experience => _experience;
    public int Gold => _gold;
    public string[] ItemIds => _itemIds;

    public QuestReward(int experience, int gold, params string[] itemIds)
    {
        _experience = experience;
        _gold = gold;
        _itemIds = itemIds;
    }
}

/// <summary>
/// 퀘스트 목표 정보
/// </summary>
[Serializable]
public class QuestObjective
{
    [SerializeField] string _description;      // 목표 설명
    [SerializeField] string _targetId;         // 대상 ID (몬스터ID, 아이템ID 등)
    [SerializeField] int _targetAmount;        // 목표 수량
    [SerializeField] int _currentAmount;       // 현재 진행량

    public string Description => _description;
    public string TargetId => _targetId;
    public int TargetAmount => _targetAmount;
    public int CurrentAmount => _currentAmount;
    public bool IsCompleted => _currentAmount >= _targetAmount;
    public float Progress => (float)_currentAmount / _targetAmount;

    public QuestObjective(string description, string targetId, int targetAmount)
    {
        _description = description;
        _targetId = targetId;
        _targetAmount = targetAmount;
        _currentAmount = 0;
    }

    /// <summary>
    /// 진행량을 증가시키는 함수
    /// </summary>
    /// <param name="amount">증가량</param>
    /// <returns>목표 완료 여부</returns>
    public bool AddProgress(int amount = 1)
    {
        _currentAmount = Mathf.Min(_currentAmount + amount, _targetAmount);
        return IsCompleted;
    }

    /// <summary>
    /// 진행량을 설정하는 함수
    /// </summary>
    /// <param name="amount">설정할 진행량</param>
    public void SetProgress(int amount)
    {
        _currentAmount = Mathf.Clamp(amount, 0, _targetAmount);
    }
}

/// <summary>
/// 퀘스트 데이터 클래스
/// </summary>
[Serializable]
public class Quest
{
    [SerializeField] string _id;                   // 퀘스트 고유 ID
    [SerializeField] string _title;                // 퀘스트 제목
    [SerializeField] string _description;          // 퀘스트 설명
    [SerializeField] QuestType _questType;         // 퀘스트 타입
    [SerializeField] QuestStatus _status;          // 퀘스트 상태
    [SerializeField] int _level;                   // 권장 레벨
    [SerializeField] List<QuestObjective> _objectives; // 퀘스트 목표들
    [SerializeField] QuestReward _reward;          // 퀘스트 보상
    [SerializeField] string[] _prerequisiteIds;    // 선행 퀘스트 ID들

    public string Id => _id;
    public string Title => _title;
    public string Description => _description;
    public QuestType QuestType => _questType;
    public QuestStatus Status => _status;
    public int Level => _level;
    public List<QuestObjective> Objectives => _objectives;
    public QuestReward Reward => _reward;
    public string[] PrerequisiteIds => _prerequisiteIds;
    
    // 모든 목표가 완료되었는지 확인
    public bool AreAllObjectivesCompleted => _objectives.TrueForAll(obj => obj.IsCompleted);

    // 퀘스트 상태 변경 이벤트
    public event Action<Quest, QuestStatus> OnStatusChanged;
    public event Action<Quest, QuestObjective> OnObjectiveUpdated;

    public Quest(string id, string title, string description, QuestType questType, 
                 int level, QuestReward reward, params string[] prerequisiteIds)
    {
        _id = id;
        _title = title;
        _description = description;
        _questType = questType;
        _status = QuestStatus.NotStarted;
        _level = level;
        _objectives = new List<QuestObjective>();
        _reward = reward;
        _prerequisiteIds = prerequisiteIds;
    }

    /// <summary>
    /// 퀘스트 목표를 추가하는 함수
    /// </summary>
    public void AddObjective(QuestObjective objective)
    {
        _objectives.Add(objective);
    }

    /// <summary>
    /// 퀘스트를 시작하는 함수
    /// </summary>
    public void StartQuest()
    {
        if (_status == QuestStatus.NotStarted)
        {
            _status = QuestStatus.InProgress;
            OnStatusChanged?.Invoke(this, _status);
            Debug.Log($"퀘스트 시작: {_title}");
        }
    }

    /// <summary>
    /// 퀘스트 목표 진행량을 업데이트하는 함수
    /// </summary>
    /// <param name="targetId">대상 ID</param>
    /// <param name="amount">진행량</param>
    public void UpdateObjective(string targetId, int amount = 1)
    {
        if (_status != QuestStatus.InProgress) return;

        foreach (var objective in _objectives)
        {
            if (objective.TargetId == targetId)
            {
                bool wasCompleted = objective.IsCompleted;
                objective.AddProgress(amount);
                
                OnObjectiveUpdated?.Invoke(this, objective);
                
                if (!wasCompleted && objective.IsCompleted)
                {
                    Debug.Log($"목표 완료: {objective.Description}");
                }
                
                // 모든 목표가 완료되면 퀘스트 완료
                if (AreAllObjectivesCompleted && _status == QuestStatus.InProgress)
                {
                    CompleteQuest();
                }
                break;
            }
        }
    }

    /// <summary>
    /// 퀘스트를 완료하는 함수
    /// </summary>
    void CompleteQuest()
    {
        _status = QuestStatus.Completed;
        OnStatusChanged?.Invoke(this, _status);
        Debug.Log($"퀘스트 완료: {_title}");
    }

    /// <summary>
    /// 퀘스트 보상을 수령하는 함수
    /// </summary>
    public void TurnInQuest()
    {
        if (_status == QuestStatus.Completed)
        {
            _status = QuestStatus.TurnedIn;
            OnStatusChanged?.Invoke(this, _status);
            
            // 보상 지급
            if (_reward != null)
            {
                GameManager.Instance.GiveExperience(_reward.Experience);
                GameManager.Instance.GiveGold(_reward.Gold);
                
                // TODO: 아이템 보상 지급 로직 추가 (인벤토리 시스템과 연동)
                foreach (string itemId in _reward.ItemIds)
                {
                    Debug.Log($"아이템 보상: {itemId}");
                    // GameManager.Instance.Inventory.AddItem(itemId);
                }
            }
            
            Debug.Log($"퀘스트 보상 수령: {_title}");
        }
    }

    /// <summary>
    /// 퀘스트를 실패시키는 함수
    /// </summary>
    public void FailQuest()
    {
        if (_status == QuestStatus.InProgress)
        {
            _status = QuestStatus.Failed;
            OnStatusChanged?.Invoke(this, _status);
            Debug.Log($"퀘스트 실패: {_title}");
        }
    }
}