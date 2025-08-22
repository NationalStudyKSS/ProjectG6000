using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 퀘스트 UI를 관리하는 클래스
/// </summary>
public class QuestUI : MonoBehaviour
{
    [Header("----- UI 컴포넌트 -----")]
    [SerializeField] GameObject _questPanel;                    // 퀘스트 패널
    [SerializeField] Transform _questListParent;                // 퀘스트 리스트 부모 오브젝트
    [SerializeField] GameObject _questItemPrefab;               // 퀘스트 아이템 프리팹
    [SerializeField] Button _toggleButton;                      // 퀘스트 창 토글 버튼

    [Header("----- 퀘스트 상세 정보 -----")]
    [SerializeField] GameObject _questDetailPanel;              // 퀘스트 상세 정보 패널
    [SerializeField] TextMeshProUGUI _questTitleText;           // 퀘스트 제목
    [SerializeField] TextMeshProUGUI _questDescriptionText;     // 퀘스트 설명
    [SerializeField] TextMeshProUGUI _questRewardText;          // 퀘스트 보상 정보
    [SerializeField] Transform _objectiveListParent;            // 목표 리스트 부모 오브젝트
    [SerializeField] GameObject _objectiveItemPrefab;           // 목표 아이템 프리팹
    [SerializeField] Button _startQuestButton;                  // 퀘스트 시작 버튼
    [SerializeField] Button _turnInQuestButton;                 // 퀘스트 완료 버튼

    QuestManager _questManager;
    Quest _selectedQuest;
    List<GameObject> _questUIItems = new List<GameObject>();
    List<GameObject> _objectiveUIItems = new List<GameObject>();

    void Start()
    {
        _questManager = GameManager.Instance?.GetComponent<QuestManager>();
        
        if (_questManager == null)
        {
            Debug.LogWarning("QuestManager를 찾을 수 없습니다!");
            return;
        }

        // 버튼 이벤트 등록
        _toggleButton?.onClick.AddListener(ToggleQuestPanel);
        _startQuestButton?.onClick.AddListener(StartSelectedQuest);
        _turnInQuestButton?.onClick.AddListener(TurnInSelectedQuest);

        // 퀘스트 이벤트 구독
        QuestManager.OnQuestStarted += OnQuestStarted;
        QuestManager.OnQuestCompleted += OnQuestCompleted;
        QuestManager.OnQuestObjectiveUpdated += OnQuestObjectiveUpdated;

        // 초기 UI 업데이트
        RefreshQuestList();
        
        // 초기에는 패널 숨김
        _questPanel?.SetActive(false);
        _questDetailPanel?.SetActive(false);
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
        QuestManager.OnQuestStarted -= OnQuestStarted;
        QuestManager.OnQuestCompleted -= OnQuestCompleted;
        QuestManager.OnQuestObjectiveUpdated -= OnQuestObjectiveUpdated;
        
        _toggleButton?.onClick.RemoveListener(ToggleQuestPanel);
        _startQuestButton?.onClick.RemoveListener(StartSelectedQuest);
        _turnInQuestButton?.onClick.RemoveListener(TurnInSelectedQuest);
    }

    /// <summary>
    /// 퀘스트 패널 토글 함수
    /// </summary>
    void ToggleQuestPanel()
    {
        bool isActive = !_questPanel.activeInHierarchy;
        _questPanel.SetActive(isActive);
        
        if (isActive)
        {
            RefreshQuestList();
        }
    }

    /// <summary>
    /// 퀘스트 목록을 새로고침하는 함수
    /// </summary>
    void RefreshQuestList()
    {
        // 기존 UI 아이템들 삭제
        foreach (GameObject item in _questUIItems)
        {
            Destroy(item);
        }
        _questUIItems.Clear();

        if (_questManager == null) return;

        // 활성 퀘스트 표시
        foreach (Quest quest in _questManager.ActiveQuests)
        {
            CreateQuestUIItem(quest, Color.yellow); // 진행 중 - 노란색
        }

        // 사용 가능한 퀘스트 표시
        foreach (Quest quest in _questManager.GetAvailableQuests())
        {
            CreateQuestUIItem(quest, Color.white); // 시작 가능 - 흰색
        }

        // 완료된 퀘스트 표시
        foreach (Quest quest in _questManager.CompletedQuests)
        {
            if (quest.Status == QuestStatus.Completed)
            {
                CreateQuestUIItem(quest, Color.green); // 완료 - 초록색
            }
        }
    }

    /// <summary>
    /// 퀘스트 UI 아이템을 생성하는 함수
    /// </summary>
    void CreateQuestUIItem(Quest quest, Color color)
    {
        if (_questItemPrefab == null || _questListParent == null) return;

        GameObject questItem = Instantiate(_questItemPrefab, _questListParent);
        _questUIItems.Add(questItem);

        // 퀘스트 정보 표시
        TextMeshProUGUI titleText = questItem.GetComponentInChildren<TextMeshProUGUI>();
        if (titleText != null)
        {
            titleText.text = $"{quest.Title} (Lv.{quest.Level})";
            titleText.color = color;
        }

        // 클릭 이벤트 추가
        Button button = questItem.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => SelectQuest(quest));
        }
    }

    /// <summary>
    /// 퀘스트를 선택하는 함수
    /// </summary>
    void SelectQuest(Quest quest)
    {
        _selectedQuest = quest;
        ShowQuestDetail(quest);
    }

    /// <summary>
    /// 퀘스트 상세 정보를 표시하는 함수
    /// </summary>
    void ShowQuestDetail(Quest quest)
    {
        if (_questDetailPanel == null) return;

        _questDetailPanel.SetActive(true);

        // 퀘스트 정보 표시
        if (_questTitleText != null)
        {
            _questTitleText.text = quest.Title;
        }

        if (_questDescriptionText != null)
        {
            _questDescriptionText.text = quest.Description;
        }

        if (_questRewardText != null)
        {
            string rewardText = $"보상: ";
            if (quest.Reward != null)
            {
                rewardText += $"경험치 {quest.Reward.Experience}, 골드 {quest.Reward.Gold}";
                if (quest.Reward.ItemIds.Length > 0)
                {
                    rewardText += $", 아이템: {string.Join(", ", quest.Reward.ItemIds)}";
                }
            }
            _questRewardText.text = rewardText;
        }

        // 목표 목록 표시
        RefreshObjectiveList(quest);

        // 버튼 상태 업데이트
        UpdateButtonStates(quest);
    }

    /// <summary>
    /// 목표 목록을 새로고침하는 함수
    /// </summary>
    void RefreshObjectiveList(Quest quest)
    {
        // 기존 목표 UI 아이템들 삭제
        foreach (GameObject item in _objectiveUIItems)
        {
            Destroy(item);
        }
        _objectiveUIItems.Clear();

        if (_objectiveItemPrefab == null || _objectiveListParent == null) return;

        // 목표들 표시
        foreach (QuestObjective objective in quest.Objectives)
        {
            GameObject objectiveItem = Instantiate(_objectiveItemPrefab, _objectiveListParent);
            _objectiveUIItems.Add(objectiveItem);

            TextMeshProUGUI objectiveText = objectiveItem.GetComponentInChildren<TextMeshProUGUI>();
            if (objectiveText != null)
            {
                string statusIcon = objective.IsCompleted ? "✓" : "○";
                objectiveText.text = $"{statusIcon} {objective.Description} ({objective.CurrentAmount}/{objective.TargetAmount})";
                objectiveText.color = objective.IsCompleted ? Color.green : Color.white;
            }
        }
    }

    /// <summary>
    /// 버튼 상태를 업데이트하는 함수
    /// </summary>
    void UpdateButtonStates(Quest quest)
    {
        if (_startQuestButton != null)
        {
            _startQuestButton.gameObject.SetActive(quest.Status == QuestStatus.NotStarted);
        }

        if (_turnInQuestButton != null)
        {
            _turnInQuestButton.gameObject.SetActive(quest.Status == QuestStatus.Completed);
        }
    }

    /// <summary>
    /// 선택된 퀘스트를 시작하는 함수
    /// </summary>
    void StartSelectedQuest()
    {
        if (_selectedQuest != null && _questManager != null)
        {
            if (_questManager.StartQuest(_selectedQuest.Id))
            {
                RefreshQuestList();
                ShowQuestDetail(_selectedQuest);
            }
        }
    }

    /// <summary>
    /// 선택된 퀘스트를 완료하는 함수
    /// </summary>
    void TurnInSelectedQuest()
    {
        if (_selectedQuest != null && _questManager != null)
        {
            if (_questManager.TurnInQuest(_selectedQuest.Id))
            {
                RefreshQuestList();
                _questDetailPanel.SetActive(false);
                _selectedQuest = null;
            }
        }
    }

    // 퀘스트 이벤트 핸들러들
    void OnQuestStarted(Quest quest)
    {
        RefreshQuestList();
        Debug.Log($"퀘스트 시작 UI 업데이트: {quest.Title}");
    }

    void OnQuestCompleted(Quest quest)
    {
        RefreshQuestList();
        if (_selectedQuest == quest)
        {
            ShowQuestDetail(quest);
        }
        Debug.Log($"퀘스트 완료 UI 업데이트: {quest.Title}");
    }

    void OnQuestObjectiveUpdated(Quest quest, QuestObjective objective)
    {
        if (_selectedQuest == quest)
        {
            RefreshObjectiveList(quest);
        }
    }
}