using System.Collections.Generic;

[System.Serializable]
public class Task {
    public int id;
    public string title;
    public string type;
    public string description;
    public bool isCompleted;
    public int reward;
}

[System.Serializable]
public class TaskData {
    public int nextTaskIndex;
    public List<Task> tasks;
}