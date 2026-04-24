using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace LazyPan {
    public class PlayerCtrl : MonoBehaviour, IPointerClickHandler {
        [Header("呼吸设置")]
        public float breathHeight = 0.3f;
        public float breathDuration = 1f;

        [Header("金币")]
        public int gold = 0;

        private Vector3 originalPos;
        private Tween breathTween;

        public System.Action OnPlayerClicked;

        public int GetGold() => gold;
        public void AddGold(int amount) => gold += amount;
        public bool SpendGold(int amount) {
            if (gold >= amount) {
                gold -= amount;
                return true;
            }
            return false;
        }

        private void Start() {
            originalPos = transform.localPosition;
            StartBreath();
        }

        private void StartBreath() {
            breathTween = transform.DOLocalMoveY(originalPos.y + breathHeight, breathDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void OnPointerClick(PointerEventData eventData) {
            Debug.Log("小人被点击了！");
            OnPlayerClicked?.Invoke();
        }
    }
}
