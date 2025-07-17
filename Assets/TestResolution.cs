using UnityEngine;

public class TestResolution : MonoBehaviour {
    public int width = 640;
    public int height = 360;
    public bool fullscreen = false; // 设置为 false 强制窗口模式

    void Start() {
        // 设置分辨率 + 窗口模式
        Screen.SetResolution(width, height, fullscreen);
    }
}