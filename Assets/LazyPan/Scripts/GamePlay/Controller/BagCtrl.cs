using System.Collections.Generic;
using UnityEngine;

namespace LazyPan {
    public class BagCtrl : MonoBehaviour {
        public static BagCtrl Instance;

        [Header("背包物品")]
        public List<BagItem> bagItems = new List<BagItem>();

        public System.Action OnBagChanged;

        private void Awake() {
            Instance = this;
        }

        public void AddItem(BagItem newItem) {
            var existing = bagItems.Find(i => i.id == newItem.id);
            if (existing != null) {
                existing.count += newItem.count;
            } else {
                bagItems.Add(newItem);
            }
            OnBagChanged?.Invoke();
        }

        public void RemoveItem(int itemId, int count = 1) {
            var item = bagItems.Find(i => i.id == itemId);
            if (item != null) {
                item.count -= count;
                if (item.count <= 0) {
                    bagItems.Remove(item);
                }
                OnBagChanged?.Invoke();
            }
        }

        public List<BagItem> GetBagItems() {
            return bagItems;
        }

        public bool HasItem(int itemId) {
            return bagItems.Exists(i => i.id == itemId && i.count > 0);
        }
    }

    [System.Serializable]
    public class BagItem {
        public int id;
        public string name;
        public string description;
        public string icon;
        public int count;
    }
}
