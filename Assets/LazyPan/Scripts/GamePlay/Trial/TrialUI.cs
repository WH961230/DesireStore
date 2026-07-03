using UnityEngine;
using UnityEngine.UI;

namespace LazyPan {
    /// <summary>
    /// 试用版UI组件示例
    /// 展示如何在游戏中集成试用版功能
    /// </summary>
    public class TrialUI : MonoBehaviour {
        [Header("UI引用")]
        public Button closeButton;                // 购买按钮
        public Button purchaseButton;             // 购买按钮
        public GameObject expiredPanel;           // 过期提示面板

        [Header("设置")]
        public bool autoCheckOnStart = true;      // 启动时自动检查

        private float updateInterval = 1f;        // UI更新间隔
        private float lastUpdateTime;

        private void Start() {
            // 注册事件
            if (TrialManager.Instance != null) {
                TrialManager.Instance.OnUsageTimeChanged += OnUsageTimeChanged;
                TrialManager.Instance.OnTrialExpired += OnTrialExpired;
            }

            // 购买按钮事件
            if (purchaseButton != null) {
                purchaseButton.onClick.AddListener(OnPurchaseClicked);
            }

            if (closeButton != null) {
                closeButton.onClick.AddListener(OnExitClicked);
            }

            // 启动时检查
            if (autoCheckOnStart) {
                CheckAndShowUI();
            }
        }

        private void OnDestroy() {
            if (TrialManager.Instance != null) {
                TrialManager.Instance.OnUsageTimeChanged -= OnUsageTimeChanged;
                TrialManager.Instance.OnTrialExpired -= OnTrialExpired;
            }
        }

        private void Update() {
            if (Time.time - lastUpdateTime >= updateInterval) {
                UpdateUI();
                lastUpdateTime = Time.time;
            }
        }

        // ============== UI更新 ==============

        private void UpdateUI() {
            if (TrialManager.Instance == null) {
                return;
            }

            if (TrialManager.Instance.IsTrialExpired) {
                ShowExpiredUI();
            }
        }

        private void ShowExpiredUI() {
            if (expiredPanel != null) {
                expiredPanel.SetActive(true);
            }

            if (TrialManager.Instance.CursorGo.activeSelf) {
                TrialManager.Instance.CursorGo.SetActive(false);
            }
        }

        private void CheckAndShowUI() {
            UpdateUI();
        }

        // ============== 事件回调 ==============

        private void OnUsageTimeChanged(int used, int limit) {
            UpdateUI();
        }

        private void OnTrialExpired() {
            ShowExpiredUI();
        }

        // ============== 按钮事件 ==============

        private void OnPurchaseClicked() {
            Application.OpenURL("https://store.steampowered.com/app/4062520/Meow_Paw_Cursor/");
        }

        private void OnExitClicked() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}
