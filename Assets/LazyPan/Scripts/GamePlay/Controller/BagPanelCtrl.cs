using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace LazyPan {
    public class BagPanelCtrl : MonoBehaviour {
        [Header("背包网格容器")]
        public Transform gridParent;

        [Header("背包物品预设")]
        public BagItemUI bagItemPrefab;

        [Header("关闭按钮")]
        public Button exitButton;

        [Header("动画设置")]
        public float slideDuration = 0.3f;

        private bool isShowing = false;
        private List<GameObject> itemObjects = new List<GameObject>();
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;

        private void Start() {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null) {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            isShowing = false;
            canvasGroup.alpha = 0;
            gameObject.SetActive(false);
            if (exitButton != null) {
                exitButton.onClick.AddListener(Hide);
            }
            if (GameMainCtrl.Instance != null) {
                GameMainCtrl.Instance.RegisterPanel(this);
            }
            if (BagCtrl.Instance != null) {
                BagCtrl.Instance.OnBagChanged += RefreshBagList;
            }
        }

        private void OnDestroy() {
            if (BagCtrl.Instance != null) {
                BagCtrl.Instance.OnBagChanged -= RefreshBagList;
            }
        }

        public void Show() {
            if (isShowing) return;
            isShowing = true;
            gameObject.SetActive(true);
            RefreshBagList();
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

        public void RefreshBagList() {
            if (bagItemPrefab == null || gridParent == null) return;

            foreach (var obj in itemObjects) {
                Destroy(obj);
            }
            itemObjects.Clear();

            if (BagCtrl.Instance == null) return;

            foreach (var item in BagCtrl.Instance.GetBagItems()) {
                var itemObj = Instantiate(bagItemPrefab.gameObject, gridParent);
                var itemUI = itemObj.GetComponent<BagItemUI>();
                if (itemUI != null) {
                    itemUI.Init(item);
                }
                itemObjects.Add(itemObj);
            }
        }
    }
}
