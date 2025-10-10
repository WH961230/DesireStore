using System.Collections.Generic;
using LazyPan;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour {
    public static TaskManager Instance;

    public const string TASKDATAFILENAME = "任务信息";
    public TaskData _taskData;

    [Header("UI References")] public Transform taskListContent; // ScrollView Content
    public GameObject taskItemPrefab;

    public Button _taskAdd;
    public Comp _taskCreator;

    private void Awake() {
        if (Instance == null) Instance = this;
        
        //初始化用户信息
        _taskData = SaveLoad.Instance.Load<TaskData>(TASKDATAFILENAME);
        if (_taskData == default) {
            _taskData = new TaskData();
            _taskData.tasks = new List<Task>();
            SaveLoad.Instance.Save(TASKDATAFILENAME, _taskData);
        }
        
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
            Button add = Cond.Instance.Get<Button>(_taskCreator, "创建");
            add.onClick.RemoveAllListeners();
            add.onClick.AddListener(() => {
                CreateTask(title.text, describe.text, int.Parse(reward.text));
                _taskCreator.gameObject.SetActive(false);
            });
        });
    }

    // 创建任务
    public void CreateTask(string title, string description, int reward) {
        Task newTask = new Task {
            id = ++_taskData.nextTaskIndex,
            title = title,
            description = description,
            reward = reward,
            isCompleted = false
        };
        _taskData.tasks.Add(newTask);
        SaveLoad.Instance.Save(TASKDATAFILENAME, _taskData);
        DisplayUI();
    }

    // 完成任务
    public void CompleteTask(int id) {
        Task t = _taskData.tasks.Find(x => x.id == id);
        if (t != null && !t.isCompleted) {
            t.isCompleted = true;
            UserManager.Instance.AddCoins(t.reward);
            SaveLoad.Instance.Save(TASKDATAFILENAME, _taskData);
        }
    }

    public void ChangeTaskRemark(int id, string remark) {
        Task t = _taskData.tasks.Find(x => x.id == id);
        if (t != null) {
            t.remark = remark;
            SaveLoad.Instance.Save(TASKDATAFILENAME, _taskData);
        }
    }

    // 删除任务
    public void DeleteTask(int id) {
        _taskData.tasks.RemoveAll(x => x.id == id);
        SaveLoad.Instance.Save(TASKDATAFILENAME, _taskData);
    }

    public void DisplayUI() {
        foreach (Transform child in taskListContent) {
            Destroy(child.gameObject);
        }

        int count = 0;
        foreach (var task in _taskData.tasks) {
            if (task.isCompleted) {
                continue;
            }

            GameObject obj = Instantiate(taskItemPrefab, taskListContent);
            
            obj.transform.Find("TitleText").GetComponent<TextMeshProUGUI>().text = string.Concat("任务:", task.title);
            obj.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>().text = string.Concat("描述:", task.description);
            //备注
            TMP_InputField remark = obj.transform.Find("Remark").GetComponent<TMP_InputField>();
            remark.text = task.remark;
            remark.onEndEdit.RemoveAllListeners();
            remark.onEndEdit.AddListener((value) => {
                ChangeTaskRemark(task.id, value);
            });
            obj.transform.Find("RewardText").GetComponent<TextMeshProUGUI>().text = string.Concat("奖励:", task.reward.ToString());
            
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

            count++;
        }

        Vector2 sizeDelta = taskListContent.GetComponent<RectTransform>().sizeDelta;
        sizeDelta = new Vector2(sizeDelta.x, 300 * count);
        taskListContent.GetComponent<RectTransform>().sizeDelta = sizeDelta;
    }
}