using UnityEditor;
using UnityEngine;

namespace LazyPan {
    public class LazyPanToolbox : EditorWindow {
        private Texture2D image;
        public void OnStart(LazyPanTool lazyPanTool) {
            image = AssetDatabase.LoadAssetAtPath<Texture2D>($"Packages/evoreek.lazypan/Editor/Image/FlowIcon.png");
        }

        public void OnCustomGUI(float areaX) {
            GUILayout.BeginArea(new Rect(areaX, 60, Screen.width, Screen.height));
            
            GUILayout.BeginHorizontal();
            GUIStyle style = LazyPanTool.GetGUISkin("LogoGUISkin").GetStyle("label");
            GUILayout.Label("TOOLBOX", style);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            style = LazyPanTool.GetGUISkin("AnnotationGUISkin").GetStyle("label");
            GUILayout.Label("@" + LazyPanTool.GetText("工具箱小标题"), style);
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
    }
}