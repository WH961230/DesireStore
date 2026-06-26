using System;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class CatPawCursor : MonoBehaviour {
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    private static readonly IntPtr HWND_TOP = new IntPtr(0);
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOSIZE = 0x0001;

    [Header("猫爪图片（留空则自动查找）")]
    public Image catPawImage;

    [Header("头部与鼠标的偏移量")]
    public Vector2 headOffset = new Vector2(0, -20f);

    [Header("旋转跟随速度")]
    public float rotationSpeed = 10f;

    [Header("任务栏高度阈值")]
    public float taskbarThreshold = 40f;

    [Header("点击特效预制体")]
    public GameObject pawPrintPrefab;
    
    [Header("2d动画系统")]
    public ImageAnim pawImageAnim;

    [Header("爪印存活时间")]
    public float pawPrintLifetime = 2f;

    [Header("速度缩放系数")]
    public float velocityScaleCoefficient = 0.015f;
    public float velocitySmoothing = 0.1f;

    private RectTransform rectTransform;
    private Canvas rootCanvas;
    private static IntPtr windowHandle;
    private bool isVisible = true;

    private float targetAngle;
    private float currentAngle;
    private float positionSpeed = 15f;
    private Vector2 lastPosition;
    private float currentVelocity;
    private float currentScale = 1f;

    void Start() {
        if (catPawImage == null) {
            catPawImage = GetComponentInChildren<Image>();
        }

        if (catPawImage == null) {
            Debug.LogError("[CatPawCursor] 未找到猫爪图片！请确保场景中有挂载Image组件的子物体。");
            return;
        }

        rectTransform = catPawImage.GetComponent<RectTransform>();
        rootCanvas = GetComponentInParent<Canvas>();

        #if !UNITY_EDITOR
        windowHandle = GetActiveWindow();
        SetWindowPos(windowHandle, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        #endif

        Cursor.visible = false;
        lastPosition = Vector2.zero;

        透明桌面.OnDesktopClick += OnDesktopClickHandler;
    }

    private void OnDesktopClickHandler() {
        if (pawImageAnim != null) {
            pawImageAnim.OnPlay("Test");
        }
    }

    void Update() {
        Vector2 mouseScreenPos = Input.mousePosition;

        // 将鼠标屏幕坐标转为Canvas本地坐标
        Vector2 mouseCanvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.GetComponent<RectTransform>(),
            mouseScreenPos,
            null,
            out mouseCanvasPos
        );

        // 计算移动距离并平滑累积速度
        float moveDistance = Vector2.Distance(mouseCanvasPos, lastPosition);
        currentVelocity = Mathf.MoveTowards(currentVelocity, moveDistance, velocitySmoothing);
        lastPosition = mouseCanvasPos;

        // 头部目标位置 = 鼠标位置 + 偏移
        Vector2 headTargetPos = mouseCanvasPos + headOffset;

        // 计算从根部到头部的向量（根据当前旋转角度）
        // 猫爪图片的Pivot在底部(0.5, 0)，所以根部在pivot，头部在顶端
        // 图片高度就是从根部到头部的距离
        float imageHeight = rectTransform.rect.height * rectTransform.lossyScale.y;

        // 计算头部到根部的向量（与旋转方向相反）
        float angleRad = currentAngle * Mathf.Deg2Rad;
        Vector2 rootToHead = new Vector2(
            Mathf.Sin(angleRad) * imageHeight,
            Mathf.Cos(angleRad) * imageHeight
        );

        // 根部位置 = 头部位置 - 从根部到头部的向量
        Vector2 rootPos = headTargetPos - rootToHead;

        // 平滑跟随头部位置
        Vector2 currentRootPos = rectTransform.anchoredPosition;
        Vector2 newRootPos = Vector2.Lerp(currentRootPos, rootPos, Time.deltaTime * positionSpeed);
        rectTransform.anchoredPosition = newRootPos;

        // 计算朝向鼠标的角度
        Vector2 direction = mouseCanvasPos - newRootPos;
        if (direction.x != 0 || direction.y != 0) {
            targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        }

        // 平滑旋转
        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * rotationSpeed);
        rectTransform.localRotation = Quaternion.Euler(0, 0, -currentAngle);

        // 更新根部到头部的向量（用于下次计算）
        angleRad = currentAngle * Mathf.Deg2Rad;
        rootToHead = new Vector2(
            Mathf.Sin(angleRad) * imageHeight,
            Mathf.Cos(angleRad) * imageHeight
        );

        // 任务栏检测
        CheckTaskbarOcclusion(mouseScreenPos);

        // 点击特效
        if (Input.GetMouseButtonDown(0) && isVisible) {
            SpawnPawPrint(headTargetPos);
        }

        // 速度缩放效果：速度 * 系数，使用平滑过渡
        float targetScale = 1f + currentVelocity * velocityScaleCoefficient;
        currentScale = Mathf.MoveTowards(currentScale, targetScale, Time.deltaTime * 10f);
        rectTransform.localScale = new Vector3(currentScale, currentScale, 1f);
    }

    void CheckTaskbarOcclusion(Vector2 screenPos) {
        if (screenPos.y < taskbarThreshold) {
            if (isVisible) {
                SetCatPawVisible(false);
            }
        } else {
            if (!isVisible) {
                SetCatPawVisible(true);
            }
        }
    }

    void SetCatPawVisible(bool visible) {
        isVisible = visible;
        Color c = catPawImage.color;
        c.a = visible ? 1f : 0f;
        catPawImage.color = c;
    }

    void SpawnPawPrint(Vector2 position) {
        if (pawPrintPrefab == null) return;

        var print = Instantiate(pawPrintPrefab, transform.parent);
        print.GetComponent<RectTransform>().anchoredPosition = position;
        Destroy(print, pawPrintLifetime);
    }

    void OnDestroy() {
        透明桌面.OnDesktopClick -= OnDesktopClickHandler;
        Cursor.visible = true;
    }
}
