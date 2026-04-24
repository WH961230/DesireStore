using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LazyPan {
    public class ShopItemUI : MonoBehaviour {
        public TextMeshProUGUI itemName;
        public TextMeshProUGUI itemDesc;
        public TextMeshProUGUI itemPrice;
        public Button buyButton;
        public TextMeshProUGUI buyText;

        private ShopItem currentItem;
        private int itemId;

        public void Init(ShopItem item) {
            currentItem = item;
            itemId = item.id;
            itemName.text = item.name;
            itemDesc.text = item.description;
            itemPrice.text = item.price.ToString();
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnBuyClicked);
            UpdateBuyButton();
        }

        private void OnBuyClicked() {
            if (ShopCtrl.Instance.BuyItem(itemId)) {
                Debug.Log($"购买成功: {currentItem.name}");
                UpdateBuyButton();
                if (StatusBarCtrl.Instance != null) {
                    StatusBarCtrl.Instance.UpdateGold();
                }
                if (BagCtrl.Instance != null) {
                    BagCtrl.Instance.OnBagChanged?.Invoke();
                }
            } else {
                Debug.Log("金币不足！");
            }
        }

        private void UpdateBuyButton() {
            var player = GameMainCtrl.Instance?.player;
            if (player != null && player.GetGold() >= currentItem.price) {
                buyButton.interactable = true;
                buyText.text = "购买";
            } else {
                buyButton.interactable = false;
                buyText.text = "金币不足";
            }
        }
    }
}
