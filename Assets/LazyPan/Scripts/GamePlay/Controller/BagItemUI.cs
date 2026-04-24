using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LazyPan {
    public class BagItemUI : MonoBehaviour {
        public TextMeshProUGUI itemName;
        public TextMeshProUGUI itemDesc;
        public TextMeshProUGUI itemCount;

        public void Init(BagItem item) {
            itemName.text = item.name;
            itemDesc.text = item.description;
            itemCount.text = item.count > 1 ? $"x{item.count}" : "";
        }
    }
}
