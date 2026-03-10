using UnityEngine;

namespace LazyPan {
    [CreateAssetMenu(menuName = "LazyPan/MySetting", fileName = "MySetting")]
    public class MySetting : Setting {
        public string Name;//昵称
        public Texture2D Icon;//头像
        public int Level;//等级
        public int InteractPoint;//交互点
    }
}
