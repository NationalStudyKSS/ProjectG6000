using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 퀘스트 시스템을 관리하는 매니저 클래스
/// </summary>
public class QuestManager : MonoBehaviour
{
    [Header("----- 퀘스트 설정 -----")]
    [SerializeField] List<Quest> _availableQuests = new List<Quest>();    // 사용 가능한 모든 퀘스트
    [SerializeField] List<Quest> _activeQuests = new List<Quest>();       // 현재 진행 중인 퀘스트
    [SerializeField] List<Quest> _completedQuests = new List<Quest>();    // 완료된 퀘스트
    [SerializeField] int _maxActiveQuests = 10;                           // 최대 동시 진행 퀘스트 수

    // 퀘스트 관련 이벤트
    public static event Action<Quest> OnQuestStarted;
    public static event Action<Quest> OnQuestCompleted;
    public static event Action<Quest> OnQuestFailed;
    public static event Action<Quest, QuestObjective> OnQuestObjectiveUpdated;

    public List<Quest> ActiveQuests => _activeQuests;
    public List<Quest> CompletedQuests => _completedQuests;
    public int MaxActiveQuests => _maxActiveQuests;

    public void Initialize()
    {
        // 초기 테스트 퀘스트들 생성
        CreateTestQuests();
        
        Debug.Log("QuestManager 초기화 완료");
    }

    /// <summary>
    /// 테스트용 퀘스트들을 생성하는 함수
    /// </summary>
    void CreateTestQuests()
    {
        // 첫 번째 퀘스트: 적 처치
        Quest killQuest = new Quest(
            "kill_enemies_001", 
            "적 처치하기", 
            "주변의 적들을 처치하여 지역을 안전하게 만드세요.", 
            QuestType.Kill, 
            1,
            new QuestReward(100, 50, "Sword")
        );
        killQuest.AddObjective(new QuestObjective("적 처치", "Enemy", 3));
        
        // 두 번째 퀘스트: 레벨업
        Quest levelQuest = new Quest(
            "level_up_001",
            "성장하기",
            "경험치를 얻어 레벨을 올리세요.",
            QuestType.Collect,
            1,
            new QuestReward(50, 100)
        );
        levelQuest.AddObjective(new QuestObjective("레벨 2 달성", "LevelUp", 1));

        // 세 번째 퀘스트: 골드 수집
        Quest goldQuest = new Quest(
            "collect_gold_001",
            "부를 축적하기",
            "골드를 모아 부를 축적하세요.",
            QuestType.Collect,
            2,
            new QuestReward(75, 25, "Apple")
        );
        goldQuest.AddObjective(new QuestObjective("골드 수집", "Gold", 200));

        _availableQuests.Add(killQuest);
        _availableQuests.Add(levelQuest);
        _availableQuests.Add(goldQuest);

        // 퀘스트 이벤트 구독
        foreach (var quest in _availableQuests)
        {
            quest.OnStatusChanged += OnQuestStatusChanged;
            quest.OnObjectiveUpdated += OnQuestObjectiveProgressUpdated;
        }
    }

    /// <summary>
    /// 퀘스트를 시작하는 함수
    /// </summary>
    /// <param name="questId">시작할 퀘스트 ID</param>
    /// <returns>퀘스트 시작 성공 여부</returns>
    public bool StartQuest(string questId)
    {
        // 이미 활성화된 퀘스트가 최대 개수에 도달했는지 확인
        if (_activeQuests.Count >= _maxActiveQuests)
        {
            Debug.LogWarning("최대 퀘스트 개수에 도달했습니다!");
            return false;
        }

        // 사용 가능한 퀘스트에서 찾기
        Quest quest = _availableQuests.FirstOrDefault(q => q.Id == questId);
        if (quest == null)
        {
            Debug.LogWarning($"퀘스트를 찾을 수 없습니다: {questId}");
            return false;
        }

        // 퀘스트가 이미 진행 중이거나 완료된 경우
        if (quest.Status != QuestStatus.NotStarted)
        {
            Debug.LogWarning($"퀘스트가 이미 시작되었거나 완료되었습니다: {questId}");
            return false;
        }

        // 선행 퀘스트 확인
        if (!ArePrerequisitesMet(quest))
        {
            Debug.LogWarning($"선행 퀘스트가 완료되지 않았습니다: {questId}");
            return false;
        }

        // 퀘스트 시작
        quest.StartQuest();
        _activeQuests.Add(quest);
        OnQuestStarted?.Invoke(quest);

        return true;
    }

    /// <summary>
    /// 선행 퀘스트 조건이 충족되었는지 확인하는 함수
    /// </summary>
    bool ArePrerequisitesMet(Quest quest)
    {
        foreach (string prerequisiteId in quest.PrerequisiteIds)
        {
            Quest prerequisite = _completedQuests.FirstOrDefault(q => q.Id == prerequisiteId);
            if (prerequisite == null || prerequisite.Status != QuestStatus.TurnedIn)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 퀘스트 목표를 업데이트하는 함수
    /// </summary>
    /// <param name="targetId">대상 ID</param>
    /// <param name="amount">진행량</param>
    public void UpdateQuestObjective(string targetId, int amount = 1)
    {
        foreach (Quest quest in _activeQuests)
        {
            quest.UpdateObjective(targetId, amount);
        }
    }

    /// <summary>
    /// 퀘스트 보상을 수령하는 함수
    /// </summary>
    /// <param name="questId">보상을 수령할 퀘스트 ID</param>
    /// <returns>보상 수령 성공 여부</returns>
    public bool TurnInQuest(string questId)
    {
        Quest quest = _activeQuests.FirstOrDefault(q => q.Id == questId);
        if (quest == null)
        {
            Debug.LogWarning($"활성 퀘스트에서 찾을 수 없습니다: {questId}");
            return false;
        }

        if (quest.Status != QuestStatus.Completed)
        {
            Debug.LogWarning($"퀘스트가 완료되지 않았습니다: {questId}");
            return false;
        }

        quest.TurnInQuest();
        return true;
    }

    /// <summary>
    /// 특정 ID의 퀘스트를 가져오는 함수
    /// </summary>
    /// <param name="questId">퀘스트 ID</param>
    /// <returns>퀘스트 객체 (없으면 null)</returns>
    public Quest GetQuest(string questId)
    {
        return _availableQuests.FirstOrDefault(q => q.Id == questId) ??
               _activeQuests.FirstOrDefault(q => q.Id == questId) ??
               _completedQuests.FirstOrDefault(q => q.Id == questId);
    }

    /// <summary>
    /// 시작 가능한 퀘스트 목록을 가져오는 함수
    /// </summary>
    /// <returns>시작 가능한 퀘스트 목록</returns>
    public List<Quest> GetAvailableQuests()
    {
        return _availableQuests
            .Where(q => q.Status == QuestStatus.NotStarted && ArePrerequisitesMet(q))
            .ToList();
    }

    /// <summary>
    /// 퀘스트 상태 변경 이벤트 핸들러
    /// </summary>
    void OnQuestStatusChanged(Quest quest, QuestStatus status)
    {
        switch (status)
        {
            case QuestStatus.Completed:
                OnQuestCompleted?.Invoke(quest);
                GameManager.Instance.AudioManager?.PlaySfx("QuestComplete");
                break;
                
            case QuestStatus.Failed:
                OnQuestFailed?.Invoke(quest);
                _activeQuests.Remove(quest);
                break;
                
            case QuestStatus.TurnedIn:
                _activeQuests.Remove(quest);
                _completedQuests.Add(quest);
                GameManager.Instance.AudioManager?.PlaySfx("QuestTurnIn");
                break;
        }
    }

    /// <summary>
    /// 퀘스트 목표 진행 상황 업데이트 이벤트 핸들러
    /// </summary>
    void OnQuestObjectiveProgressUpdated(Quest quest, QuestObjective objective)
    {
        OnQuestObjectiveUpdated?.Invoke(quest, objective);
        GameManager.Instance.AudioManager?.PlaySfx("QuestProgress");
    }

    /// <summary>
    /// 게임 이벤트와 퀘스트를 연동하는 함수들
    /// </summary>
    public void OnEnemyKilled(string enemyType)
    {
        UpdateQuestObjective("Enemy", 1);
        UpdateQuestObjective(enemyType, 1); // 특정 적 타입
    }

    public void OnItemCollected(string itemId, int amount = 1)
    {
        UpdateQuestObjective(itemId, amount);
    }

    public void OnLevelUp(int newLevel)
    {
        UpdateQuestObjective("LevelUp", 1);
        UpdateQuestObjective($"Level{newLevel}", 1);
    }

    public void OnGoldChanged(int totalGold)
    {
        // 골드 수집 퀘스트는 현재 총 골드로 진행도 체크
        foreach (Quest quest in _activeQuests)
        {
            foreach (var objective in quest.Objectives)
            {
                if (objective.TargetId == "Gold")
                {
                    objective.SetProgress(totalGold);
                    OnQuestObjectiveProgressUpdated(quest, objective);
                }
            }
        }
    }

    public void OnLocationReached(string locationId)
    {
        UpdateQuestObjective(locationId, 1);
    }

    public void OnNpcTalk(string npcId)
    {
        UpdateQuestObjective(npcId, 1);
    }

    /// <summary>
    /// 모든 퀘스트 정보를 로그로 출력하는 디버그 함수
    /// </summary>
    [ContextMenu("Debug Quest Info")]
    public void DebugQuestInfo()
    {
        Debug.Log("=== Quest Manager Debug Info ===");
        Debug.Log($"Available Quests: {_availableQuests.Count}");
        Debug.Log($"Active Quests: {_activeQuests.Count}");
        Debug.Log($"Completed Quests: {_completedQuests.Count}");
        
        foreach (var quest in _activeQuests)
        {
            Debug.Log($"Active: {quest.Title} - {quest.Status}");
            foreach (var objective in quest.Objectives)
            {
                Debug.Log($"  Objective: {objective.Description} ({objective.CurrentAmount}/{objective.TargetAmount})");
            }
        }
    }
}