using UnityEditor;
using UnityEngine;

namespace LazyPan {
    [CustomEditor(typeof(ImageAnim))]
    public class ImageAnimEditor : Editor {
        private ImageAnim _script;
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            _script = (ImageAnim)target;
            if (GUILayout.Button("Test")) {
                _script.OnPlay("Test");
            }

            if (GUILayout.Button("TestBack")) {
                _script.OnPlay("TestBack");
            }
        }
    }
}