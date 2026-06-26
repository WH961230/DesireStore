using UnityEditor;
using UnityEngine;

namespace LazyPan {
    [CustomEditor(typeof(ImageAnim))]
    public class ImageAnimEditor : Editor {
        private ImageAnim _script;
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            _script = (ImageAnim)target;
            if (GUILayout.Button("Play")) {
                _script.OnPlay("Test");
            }
        }
    }
}