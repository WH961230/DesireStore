using System.Collections.Generic;

[System.Serializable]
public class Task {
    public int id;
    public string title;//标题
    public string type;//类型
    public string description;//描述
    public string remark;//备注
    public bool isCompleted;//是否完成
    public int reward;//奖励
}

[System.Serializable]
public class TaskData {
    public int nextTaskIndex;
    public List<Task> tasks;
}