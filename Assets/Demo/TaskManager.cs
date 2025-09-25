using System.Collections.Generic;
using LazyPan;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour {
    public static TaskManager Instance;

    public List<Task> tasks = new List<Task>();
    private int nextId = 1;

    [Header("UI References")] public Transform taskListContent; // ScrollView Content
    public GameObject taskItemPrefab;

    public Button _taskAdd;
    public Comp _taskCreator;

    private void Awake() {
        if (Instance == null) Instance = this;
        
        _taskAdd.onClick.AddListener(() => {
            _taskCreator.gameObject.SetActive(true);
            TMP_InputField title = Cond.Instance.Get<TMP_InputField>(_taskCreator, "标题"); 
            title.text = "";
            TMP_InputField describe = Cond.Instance.Get<TMP_InputField>(_taskCreator, "描述");
            describe.text = "";
            TMP_InputField type = Cond.Instance.Get<TMP_InputField>(_taskCreator, "类型");
            type.text = "";
            TMP_InputField reward = Cond.Instance.Get<TMP_InputField>(_taskCreator, "奖励");
            reward.text = "";
            _taskAdd.onClick.AddListener(() => {
                CreateTask(title.text, describe.text, int.Parse(reward.text));
                _taskCreator.gameObject.SetActive(false);
            });
        });
    }

    // 创建任务
    public void CreateTask(string title, string description, int reward) {
        Task newTask = new Task {
            id = nextId++,
            title = title,
            description = description,
            reward = reward,
            isCompleted = false
        };
        tasks.Add(newTask);
    }

    // 完成任务
    public void CompleteTask(int id) {
        Task t = tasks.Find(x => x.id == id);
        if (t != null && !t.isCompleted) {
            t.isCompleted = true;
            UserManager.Instance.AddCoins(t.reward);
            Debug.Log("Task Completed: " + t.title);
        }
    }

    // 删除任务
    public void DeleteTask(int id) {
        tasks.RemoveAll(x => x.id == id);
    }

    public void DisplayUI() {
        foreach (Transform child in taskListContent) {
            Destroy(child.gameObject);
        }

        foreach (var task in tasks) {
            if (task.isCompleted) {
                continue;
            }

            GameObject obj = Instantiate(taskItemPrefab, taskListContent);
            
            obj.transform.Find("TitleText").GetComponent<TextMeshProUGUI>().text = task.title;
            obj.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>().text = task.description;
            obj.transform.Find("RewardText").GetComponent<TextMeshProUGUI>().text = task.reward.ToString();
            
            Button completeBtn = obj.transform.Find("CompleteButton").GetComponent<Button>();
            completeBtn.onClick.AddListener(() => {
                CompleteTask(task.id);
                DisplayUI(); // 完成后刷新界面
            });
            
            obj.transform.Find("DeleteButton").GetComponent<Button>()
                .onClick.AddListener(() => {
                    DeleteTask(task.id);
                    DisplayUI();
                });
        }
    }
}