using UnityEngine;

namespace LazyPan {
    public class DataManager : MonoBehaviour {
        public static DataManager Instance;

        private int exp = 0;
        private int level = 1;
        private int hp = 100;
        private int maxExp = 100;

        public System.Action<int, int> OnExpChanged;
        public System.Action<int, int> OnLevelChanged;
        public System.Action<int, int> OnHpChanged;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }
        }

        public void AddExp(int amount) {
            exp += amount;
            OnExpChanged?.Invoke(exp, maxExp);

            while (exp >= maxExp) {
                exp -= maxExp;
                level++;
                maxExp = level * 100;
                OnLevelChanged?.Invoke(level, maxExp);
                OnExpChanged?.Invoke(exp, maxExp);
            }
        }

        public int GetExp() => exp;
        public int GetLevel() => level;
        public int GetHp() => hp;
        public int GetMaxExp() => maxExp;
    }
}
