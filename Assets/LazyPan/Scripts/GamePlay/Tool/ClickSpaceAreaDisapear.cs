using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickSpaceAreaDisapear : MonoBehaviour {
    public const int VK_LBUTTON = 0x01;

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    private bool isMouseDown = false; // 记录鼠标是否按下
    private bool mouseDownOutsidePanel = false; // 记录按下时是否在panel外部

    void Update() {
        if (!gameObject.activeSelf) return;
        bool isLeftButtonDown = (GetAsyncKeyState(VK_LBUTTON) & 0x8000) != 0;

        // 获取当前鼠标位置的UI检测结果
        EventSystem eventSystem = EventSystem.current;
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;
        var results = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerEventData, results);
        bool isMouseOverUI = results.Count > 0; // true: 在panel上, false: 在panel外

        // 鼠标按下事件
        if (!isMouseDown && isLeftButtonDown) {
            isMouseDown = true;
            mouseDownOutsidePanel = !isMouseOverUI; // 记录按下时是否在panel外部
        }

        // 鼠标松开事件
        if (isMouseDown && !isLeftButtonDown) {
            // 只有在按下时和松开时都在panel外部，才关闭
            if (mouseDownOutsidePanel && !isMouseOverUI) {
                gameObject.SetActive(false);
            }

            isMouseDown = false; // 重置状态
            mouseDownOutsidePanel = false;
        }

        // 如果鼠标松开或者失去焦点，重置状态
        if (!isLeftButtonDown) {
            isMouseDown = false;
            mouseDownOutsidePanel = false;
        }
    }
}