using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class DoTweenUIAnimationJumpAndSpill : MonoBehaviour {
    [Header("洒出设置")]
    public GameObject coinPrefab;
    public Transform parent;
    public RectTransform target;
    
    [Header("第一段弹出参数")]
    public int 硬币数量 = 20;
    public float 扩散范围 = 200f;
    public float 持续时间 = 1f;
    public float 弹出力量 = 300f;
    public List<AudioClip> 掉落音效;

    [Header("第二段弹出参数")]
    public float 二段持续时间 = 1f;
    public float 二段结尾缩放 = 1.1f;
    public List<AudioClip> 收集音效;
    
    [Header("目标位置Dotween")]
    public DOTweenAnimation ScaleDotween;

    private List<GameObject> coins = new List<GameObject>();

    public void TestSpillCoins() {
        SpillCoins(Vector2.zero);
    }
    
    public void SpillCoins(Vector2 startPosition) {
        ClearCoins();
        int completedCount = 0;
        
        AudioClip randomSound = 掉落音效[Random.Range(0, 掉落音效.Count)];
        AudioSource.PlayClipAtPoint(randomSound, Camera.main.transform.position, 0.7f);
        
        for (int i = 0; i < 硬币数量; i++) {
            GameObject coin = Instantiate(coinPrefab, parent);
            RectTransform coinRect = coin.GetComponent<RectTransform>();
            coinRect.anchoredPosition = Vector2.zero; // 先归零
            coinRect.localPosition = Vector2.zero;

            // 随机目标位置
            Vector2 targetPos = startPosition + Random.insideUnitCircle * 扩散范围;
            Sequence spillSequence = DOTween.Sequence();
            
            spillSequence.AppendInterval(Random.Range(0f, 0.3f));
            spillSequence.Append(coinRect.DOJumpAnchorPos(targetPos, 弹出力量, 1, 持续时间).SetEase(Ease.OutQuad));
            spillSequence.Join(coinRect.DORotate(new Vector3(0, 0, 360), 持续时间, RotateMode.FastBeyond360).SetEase(Ease.Linear));
        
            spillSequence.OnComplete(() => {
                completedCount++;
                // 当所有硬币动画都完成时调用回调
                if (completedCount == 硬币数量) {
                    CollectCoins(target.localPosition);
                }
            });

            coins.Add(coin);
        }
    }
    
    public void SpillCoinsAtWorldPosition(Vector3 worldPosition) {
        ClearCoins();
    
        // 世界坐标转UI坐标
        Canvas canvas = GetComponentInParent<Canvas>();
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(
            canvas.worldCamera, 
            worldPosition
        );
    
        SpillCoins(screenPoint);
    }

    public void CollectCoins(Vector2 targetPosition) {
        for (int i = 0; i < coins.Count; i++) {
            GameObject coin = coins[i];
            if (coin != null) {
                RectTransform coinRect = coin.GetComponent<RectTransform>();
                AudioClip randomSound = 收集音效[Random.Range(0, 收集音效.Count)];

                GameObject coinToDestroy = coin;
                Sequence collectSequence = DOTween.Sequence();
                
                collectSequence.AppendInterval(i * 0.1f);
                collectSequence.Append(coinRect.DOAnchorPos(targetPosition, 二段持续时间).SetEase(Ease.InBack));
                collectSequence.Join(coinRect.DOScale(二段结尾缩放, 二段持续时间));
                collectSequence.OnComplete(() => {
                    AudioSource.PlayClipAtPoint(randomSound, Camera.main.transform.position, 0.7f);
                    ScaleDotween.DORestart();
                    Destroy(coinToDestroy);
                });
            }
        }

        coins.Clear();
    }

    private void ClearCoins() {
        foreach (GameObject coin in coins) {
            if (coin != null) Destroy(coin);
        }

        coins.Clear();
    }
}