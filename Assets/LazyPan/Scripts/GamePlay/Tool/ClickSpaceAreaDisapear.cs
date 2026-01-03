using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickSpaceAreaDisapear : MonoBehaviour {
    public const int VK_LBUTTON = 0x01;
    
    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);
    
    void Update() {
        if (gameObject.activeSelf && (GetAsyncKeyState(VK_LBUTTON) & 0x8000) != 0) {
            EventSystem eventSystem = EventSystem.current;
            PointerEventData pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = Input.mousePosition;
            var results = new List<RaycastResult>();
            eventSystem.RaycastAll(pointerEventData, results);
            bool isMouseButtonDownPanel = results.Count > 0;
            if (!isMouseButtonDownPanel) {
                gameObject.SetActive(false);
            }
        }
    }
}