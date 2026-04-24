using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LazyPan {
    public class StatusBarCtrl : MonoBehaviour {
        public static StatusBarCtrl Instance { get; private set; }

        public TextMeshProUGUI levelText;
        public Image expFillImage;
        public TextMeshProUGUI goldText;

        private void Awake() {
            Instance = this;
        }

        private void Start() {
            if (DataManager.Instance != null) {
                DataManager.Instance.OnLevelChanged += UpdateLevel;
                DataManager.Instance.OnExpChanged += UpdateExp;
                UpdateLevel(DataManager.Instance.GetLevel(), DataManager.Instance.GetMaxExp());
                UpdateExp(DataManager.Instance.GetExp(), DataManager.Instance.GetMaxExp());
            }
            UpdateGold();
        }

        private void OnDestroy() {
            if (DataManager.Instance != null) {
                DataManager.Instance.OnLevelChanged -= UpdateLevel;
                DataManager.Instance.OnExpChanged -= UpdateExp;
            }
        }

        private void UpdateLevel(int level, int maxExp) {
            levelText.text = $"Lv.{level}";
        }

        private void UpdateExp(int exp, int maxExp) {
            float fillAmount = (float)exp / maxExp;
            expFillImage.fillAmount = fillAmount;
        }

        public void UpdateGold() {
            if (GameMainCtrl.Instance != null && GameMainCtrl.Instance.player != null) {
                goldText.text = GameMainCtrl.Instance.player.GetGold().ToString();
            }
        }
    }
}
