using UnityEngine;

namespace LazyPan {
    [CreateAssetMenu(menuName = "LazyPan/MySetting", fileName = "MySetting")]
    public class MySetting : Setting {
        public string Name;//昵称
        public int InteractPoint;//交互点
    }
}
