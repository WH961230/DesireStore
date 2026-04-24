using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace LazyPan {
    public class TaskPanelCtrl : MonoBehaviour {
        [Header("任务列表容器")]
        public Transform taskListContainer;

        [Header("任务项预设")]
        public GameObject taskItemPrefab;

        [Header("关闭按钮")]
        public Button exitButton;

        [Header("动画设置")]
        public float slideDuration = 0.3f;
        public float hideOffset = 800f;

        private bool isShowing = false;
        private List<GameObject> taskItemObjects = new List<GameObject>();
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;

        private void Start() {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null) {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            // 初始状态设为隐藏
            isShowing = false;
            canvasGroup.alpha = 0;
            gameObject.SetActive(false);
            if (exitButton != null) {
                exitButton.onClick.AddListener(Hide);
            }
            if (GameMainCtrl.Instance != null) {
                GameMainCtrl.Instance.RegisterPanel(this);
            }
        }

        public void Show() {
            if (isShowing) return;
            isShowing = true;
            gameObject.SetActive(true);
            RefreshTaskList();
            transform.DOKill();
            canvasGroup.DOKill();
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, slideDuration).SetEase(Ease.OutQuad);
        }

        public void Hide() {
            if (!isShowing) return;
            isShowing = false;
            transform.DOKill();
            canvasGroup.DOKill();
            canvasGroup.DOFade(0, slideDuration).SetEase(Ease.InQuad).OnComplete(() => gameObject.SetActive(false));
        }

        public void Toggle() {
            if (isShowing) Hide();
            else Show();
        }

        public bool IsShowing() {
            return isShowing;
        }

        public void RefreshTaskList() {
            if (taskItemPrefab == null || taskListContainer == null) return;

            foreach (var obj in taskItemObjects) {
                Destroy(obj);
            }
            taskItemObjects.Clear();

            if (TaskCtrl.Instance == null) return;

            foreach (var task in TaskCtrl.Instance.GetTasks()) {
                var itemObj = Instantiate(taskItemPrefab, taskListContainer);
                var itemUI = itemObj.GetComponent<TaskItemUI>();
                if (itemUI != null) {
                    itemUI.Init(task);
                }
                taskItemObjects.Add(itemObj);
            }
        }
    }
}
