using System.Collections.Generic;
using UnityEngine;

namespace LazyPan {
    public class TaskCtrl : MonoBehaviour {
        public static TaskCtrl Instance;

        [Header("预设任务")]
        public List<TaskItem> taskItems = new List<TaskItem>();

        public System.Action<TaskItem> OnTaskCompleted;

        private void Awake() {
            Instance = this;
        }

        public void InitTasks() {
            taskItems = new List<TaskItem> {
                new TaskItem { id = 1, title = "早睡早起", expReward = 10 },
                new TaskItem { id = 2, title = "完成工作", expReward = 15 },
                new TaskItem { id = 3, title = "运动30分钟", expReward = 20 }
            };
        }

        public void CompleteTask(int taskId) {
            var task = taskItems.Find(t => t.id == taskId && !t.isCompleted);
            if (task != null) {
                task.isCompleted = true;
                DataManager.Instance.AddExp(task.expReward);
                OnTaskCompleted?.Invoke(task);
            }
        }

        public List<TaskItem> GetTasks() {
            return taskItems;
        }
    }

    [System.Serializable]
    public class TaskItem {
        public int id;
        public string title;
        public int expReward;
        public bool isCompleted;
    }
}
