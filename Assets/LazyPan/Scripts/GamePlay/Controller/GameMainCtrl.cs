using UnityEngine;

namespace LazyPan {
    public class GameMainCtrl : MonoBehaviour {
        public static GameMainCtrl Instance { get; private set; }

        [Header("小人")]
        public PlayerCtrl player;

        [Header("任务面板")]
        public TaskPanelCtrl taskPanel;

        [Header("背包面板")]
        public BagPanelCtrl bagPanel;

        [Header("商店面板")]
        public ShopPanelCtrl shopPanel;

        [Header("状态栏")]
        public StatusBarCtrl statusBar;

        private int currentPanelIndex = -1;

        private void Awake() {
            Instance = this;
        }

        private void Start() {
            if (DataManager.Instance == null) {
                var dmObj = new GameObject("DataManager");
                dmObj.AddComponent<DataManager>();
            }

            if (TaskCtrl.Instance == null) {
                var taskCtrlObj = new GameObject("TaskCtrl");
                taskCtrlObj.AddComponent<TaskCtrl>();
            }

            if (ShopCtrl.Instance == null) {
                var shopCtrlObj = new GameObject("ShopCtrl");
                shopCtrlObj.AddComponent<ShopCtrl>();
            }

            if (BagCtrl.Instance == null) {
                var bagCtrlObj = new GameObject("BagCtrl");
                bagCtrlObj.AddComponent<BagCtrl>();
            }

            if (player == null) {
                player = FindObjectOfType<PlayerCtrl>();
            }

            TaskCtrl.Instance.InitTasks();

            if (player != null) {
                player.OnPlayerClicked += OnPlayerClicked;
            }
        }

        public void RegisterPanel(TaskPanelCtrl panel) { taskPanel = panel; }
        public void RegisterPanel(BagPanelCtrl panel) { bagPanel = panel; }
        public void RegisterPanel(ShopPanelCtrl panel) { shopPanel = panel; }

        private void OnPlayerClicked() {
            if (IsAnyPanelShowing()) {
                HideAllPanels();
                return;
            }

            currentPanelIndex++;
            if (currentPanelIndex > 2) currentPanelIndex = 0;

            switch (currentPanelIndex) {
                case 0:
                    taskPanel?.Show();
                    break;
                case 1:
                    shopPanel?.Show();
                    break;
                case 2:
                    bagPanel?.Show();
                    break;
            }
        }

        private bool IsAnyPanelShowing() {
            return (taskPanel != null && taskPanel.IsShowing()) ||
                   (bagPanel != null && bagPanel.IsShowing()) ||
                   (shopPanel != null && shopPanel.IsShowing());
        }

        private void HideAllPanels() {
            taskPanel?.Hide();
            bagPanel?.Hide();
            shopPanel?.Hide();
        }

        private void OnDestroy() {
            if (player != null) {
                player.OnPlayerClicked -= OnPlayerClicked;
            }
        }
    }
}
