using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LazyPan {
    public class TaskItemUI : MonoBehaviour {
        public Toggle taskToggle;
        public TextMeshProUGUI taskTitle;
        public TextMeshProUGUI taskExp;
        public TextMeshProUGUI taskStatus;

        private TaskItem currentTask;

        public void Init(TaskItem task) {
            currentTask = task;
            taskTitle.text = task.title;
            taskExp.text = $"+{task.expReward}经验";

            if (task.isCompleted) {
                taskToggle.isOn = true;
                taskToggle.interactable = false;
                taskStatus.text = "[已完成]";
            } else {
                taskToggle.isOn = false;
                taskToggle.interactable = true;
                taskStatus.text = "[未完成]";
            }

            taskToggle.onValueChanged.AddListener(OnToggleChanged);
        }

        private void OnToggleChanged(bool isOn) {
            if (isOn && currentTask != null && !currentTask.isCompleted) {
                TaskCtrl.Instance.CompleteTask(currentTask.id);
                taskToggle.interactable = false;
                taskStatus.text = "[已完成]";
            }
        }
    }
}
