using System.Collections.Generic;
using UnityEngine;

namespace LazyPan {
    public class ShopCtrl : MonoBehaviour {
        public static ShopCtrl Instance;

        [Header("预设商品")]
        public List<ShopItem> shopItems = new List<ShopItem>();

        private void Awake() {
            Instance = this;
        }

        private void Start() {
            InitShopItems();
        }

        public void InitShopItems() {
            shopItems = new List<ShopItem> {
                new ShopItem { id = 1, name = "体力药水", description = "恢复50点体力", price = 10, icon = "potion" },
                new ShopItem { id = 2, name = "经验卷轴", description = "获得100点经验", price = 25, icon = "scroll" },
                new ShopItem { id = 3, name = "幸运符", description = "提升掉落率", price = 50, icon = "charm" },
                new ShopItem { id = 4, name = "护盾", description = "抵挡一次伤害", price = 30, icon = "shield" },
                new ShopItem { id = 5, name = "传送卷轴", description = "随机传送", price = 15, icon = "portal" },
                new ShopItem { id = 6, name = "超级装备箱", description = "随机获得一件装备", price = 100, icon = "chest" }
            };
        }

        public List<ShopItem> GetShopItems() {
            return shopItems;
        }

        public bool BuyItem(int itemId) {
            var item = shopItems.Find(i => i.id == itemId);
            if (item == null) return false;

            var player = GameMainCtrl.Instance?.player;
            if (player == null) return false;

            if (player.SpendGold(item.price)) {
                BagCtrl.Instance.AddItem(new BagItem {
                    id = item.id,
                    name = item.name,
                    description = item.description,
                    icon = item.icon,
                    count = 1
                });
                return true;
            }
            return false;
        }
    }

    [System.Serializable]
    public class ShopItem {
        public int id;
        public string name;
        public string description;
        public int price;
        public string icon;
    }
}
