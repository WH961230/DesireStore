using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace LazyPan {
    public class ShopPanelCtrl : MonoBehaviour {
        [Header("商店网格容器")]
        public Transform gridParent;

        [Header("商店商品预设")]
        public ShopItemUI shopItemPrefab;

        [Header("关闭按钮")]
        public Button exitButton;

        [Header("动画设置")]
        public float slideDuration = 0.3f;

        private bool isShowing = false;
        private List<GameObject> shopItemObjects = new List<GameObject>();
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
        }

        public void Show() {
            if (isShowing) return;
            isShowing = true;
            gameObject.SetActive(true);
            RefreshShopList();
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

        public void RefreshShopList() {
            if (shopItemPrefab == null || gridParent == null) return;

            foreach (var obj in shopItemObjects) {
                Destroy(obj);
            }
            shopItemObjects.Clear();

            if (ShopCtrl.Instance == null) return;

            foreach (var item in ShopCtrl.Instance.GetShopItems()) {
                var itemObj = Instantiate(shopItemPrefab.gameObject, gridParent);
                var itemUI = itemObj.GetComponent<ShopItemUI>();
                if (itemUI != null) {
                    itemUI.Init(item);
                }
                shopItemObjects.Add(itemObj);
            }
        }
    }
}
