using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;

public class 透明桌面 : MonoBehaviour {
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
    
    [DllImport("user32.dll")]
    private static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, uint X, uint Y, uint cx, uint cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    private struct Margins {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hwnd, ref Margins margins);

    const int GWL_EXSTYLE = -20;
    private const uint WS_EX_LAYERED = 0x00080000;
    private const uint WS_EX_TRANSPARENT = 0x00000020;

    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOACTIVATE = 0x0010;
    private const uint SWP_SHOWWINDOW = 0x0040;

    private IntPtr hwnd;
    
    private EventSystem cachedEventSystem;
    private PointerEventData cachedPointerEventData;
    private List<RaycastResult> cachedRaycastResults;
    private Vector3 lastMousePosition;
    private bool isClickThrough;
    private float topmostCheckInterval = 2f;
    private float lastTopmostCheckTime;

    private void Awake() {
        cachedRaycastResults = new List<RaycastResult>(8);
    }

    private void Start() {
#if !UNITY_EDITOR
        hwnd = GetActiveWindow();
        Margins margins = new Margins{ cxLeftWidth = -1};
        DwmExtendFrameIntoClientArea(hwnd, ref margins);
        SetWindowLong(hwnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
        SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
        lastTopmostCheckTime = Time.realtimeSinceStartup;
#endif
        Application.runInBackground = true;
        cachedEventSystem = EventSystem.current;
        lastMousePosition = Input.mousePosition;
    }

    private void Update() {
        CheckWindowTopmost();
        CheckMousePosition();
    }

    private void CheckWindowTopmost() {
#if !UNITY_EDITOR
        float currentTime = Time.realtimeSinceStartup;
        if (currentTime - lastTopmostCheckTime >= topmostCheckInterval) {
            lastTopmostCheckTime = currentTime;
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
        }
#endif
    }

    private void CheckMousePosition() {
        Vector3 currentMousePos = Input.mousePosition;
        if (currentMousePos != lastMousePosition) {
            lastMousePosition = currentMousePos;
            UpdateClickThrough();
        }
    }
    
    private void UpdateClickThrough() {
        bool shouldBeClickThrough = !IsMouseOver2DUI();
        if (shouldBeClickThrough != isClickThrough) {
            isClickThrough = shouldBeClickThrough;
            SetClickThrough(isClickThrough);
        }
    }
    
    bool IsMouseOver2DUI() {
        EventSystem eventSystem = cachedEventSystem;
        if (eventSystem == null) {
            eventSystem = EventSystem.current;
            cachedEventSystem = eventSystem;
        }
        if (eventSystem == null) return false;

        if (cachedPointerEventData == null) {
            cachedPointerEventData = new PointerEventData(eventSystem);
        }
        cachedPointerEventData.position = Input.mousePosition;
        
        cachedRaycastResults.Clear();
        eventSystem.RaycastAll(cachedPointerEventData, cachedRaycastResults);
        return cachedRaycastResults.Count > 0;
    }

    void SetClickThrough(bool clickThrough) {
#if !UNITY_EDITOR
        if (clickThrough) {
            SetWindowLong(hwnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
        } else {
            SetWindowLong(hwnd, GWL_EXSTYLE, WS_EX_LAYERED);
        }
#endif
    }
}