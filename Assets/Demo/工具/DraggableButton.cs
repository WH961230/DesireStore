using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class DraggableButton : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerDownHandler, IPointerUpHandler {
    private RectTransform rectTransform;
    private Canvas canvas;
    private RectTransform canvasRect;

    private Vector2 pressButtonPos; // 按下时按钮位置
    private Vector2 releaseButtonPos; // 抬起时按钮位置
    private Vector2 pointerOffset; // 鼠标与按钮的偏移（基于按钮局部坐标）

    private bool isDragging = false;
    private bool canDrag = false;
    private bool leftClickStartedOnButton = false;
    private bool middleClickStartedOnButton = false;

    private Button button;

    [Header("Optional Panel")] [SerializeField]
    private GameObject MainPanel;

    [Header("Click Settings")] [SerializeField]
    private float clickThreshold = 5f; // 判定点击的最大移动距离

    [SerializeField] private bool enableMiddleClick = true;
    [SerializeField] private bool enableLeftClick = true;

    [Header("Press Effect")] [SerializeField]
    private bool usePressEffect = true;

    [SerializeField] private AnimationCurve pressCurve = AnimationCurve.EaseInOut(0, 1, 1, 0.9f);
    [SerializeField] private float pressAnimDuration = 0.15f;
    private Vector3 originalScale;
    private Coroutine scaleCoroutine;

    void Awake() {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
        button = GetComponent<Button>();
        originalScale = rectTransform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData) {
        // 左键按下
        if (eventData.button == PointerEventData.InputButton.Left) {
            leftClickStartedOnButton = RectTransformUtility.RectangleContainsScreenPoint(
                rectTransform, eventData.position, eventData.pressEventCamera);

            if (leftClickStartedOnButton) {
                canDrag = true;
                isDragging = false;
                pressButtonPos = rectTransform.anchoredPosition;

                // ⚡ 修复点：用按钮自身坐标系记录鼠标在按钮内部的偏移
                Vector2 localPointerPos;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        rectTransform, // ✅ 注意这里用 rectTransform，而不是 canvasRect
                        eventData.position,
                        eventData.pressEventCamera,
                        out localPointerPos)) {
                    pointerOffset = localPointerPos; // 鼠标点在按钮内部的坐标
                }

                // 按下缩放动画
                if (usePressEffect) {
                    if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
                    scaleCoroutine = StartCoroutine(
                        AnimateScale(originalScale, originalScale * 0.9f, pressAnimDuration, pressCurve));
                }
            }
        }

        // 中键按下
        if (eventData.button == PointerEventData.InputButton.Middle) {
            middleClickStartedOnButton = RectTransformUtility.RectangleContainsScreenPoint(
                rectTransform, eventData.position, eventData.pressEventCamera);

            if (middleClickStartedOnButton) {
                pressButtonPos = rectTransform.anchoredPosition;
            }
        }
    }

    public void OnDrag(PointerEventData eventData) {
        if (!canDrag || canvas == null) return;
        if (eventData.button != PointerEventData.InputButton.Left) return;

        isDragging = true;

        // 鼠标在 Canvas 内的局部坐标
        Vector2 localPointerPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                eventData.position,
                eventData.pressEventCamera,
                out localPointerPos)) {
            // ⚡ 修复点：新位置 = 鼠标点 - 偏移（保证和点击位置对齐）
            Vector2 newPos = localPointerPos - pointerOffset;

            // 限制范围：不能拖出屏幕
            float halfWidth = rectTransform.rect.width * 0.5f;
            float halfHeight = rectTransform.rect.height * 0.5f;

            float minX = -canvasRect.rect.width / 2f + halfWidth;
            float maxX = canvasRect.rect.width / 2f - halfWidth;
            float minY = -canvasRect.rect.height / 2f + halfHeight;
            float maxY = canvasRect.rect.height / 2f - halfHeight;

            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
            newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

            rectTransform.anchoredPosition = newPos;
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
    }

    public void OnEndDrag(PointerEventData eventData) {
    }

    public void OnPointerUp(PointerEventData eventData) {
        releaseButtonPos = rectTransform.anchoredPosition;

        // 抬起缩放动画（左键）
        if (usePressEffect && eventData.button == PointerEventData.InputButton.Left && leftClickStartedOnButton) {
            if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
            scaleCoroutine = StartCoroutine(
                AnimateScale(rectTransform.localScale, originalScale, pressAnimDuration, pressCurve));
        }

        bool canTriggerLeft = eventData.button == PointerEventData.InputButton.Left && enableLeftClick;
        bool canTriggerMiddle = eventData.button == PointerEventData.InputButton.Middle && enableMiddleClick;

        // 左键点击：按下 + 抬起都在按钮内，且没发生拖动
        if (canTriggerLeft && leftClickStartedOnButton) {
            bool releasedOnButton = RectTransformUtility.RectangleContainsScreenPoint(
                rectTransform, eventData.position, eventData.pressEventCamera);

            if (Vector2.Distance(pressButtonPos, releaseButtonPos) < clickThreshold &&
                !isDragging && releasedOnButton) {
                if (MainPanel != null)
                    MainPanel.SetActive(!MainPanel.activeSelf);
            }

            isDragging = false;
            leftClickStartedOnButton = false;
        }

        // 中键点击
        if (canTriggerMiddle && middleClickStartedOnButton) {
            if (Vector2.Distance(pressButtonPos, releaseButtonPos) < clickThreshold) {
                if (MainPanel != null)
                    MainPanel.SetActive(!MainPanel.activeSelf);
            }

            middleClickStartedOnButton = false;
        }
    }

    private IEnumerator AnimateScale(Vector3 from, Vector3 to, float duration, AnimationCurve curve) {
        float t = 0f;
        while (t < duration) {
            float progress = t / duration;
            float evaluated = curve.Evaluate(progress);
            rectTransform.localScale = Vector3.LerpUnclamped(from, to, evaluated);
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        rectTransform.localScale = to;
    }
}