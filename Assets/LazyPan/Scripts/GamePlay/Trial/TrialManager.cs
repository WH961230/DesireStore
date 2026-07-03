using System;
using UnityEngine;
using UnityEngine.Events;

namespace LazyPan {
    /// <summary>
    /// 试用版管理系统
    /// - 每天免费2小时（累计）
    /// - 每天零点自动恢复额度
    /// - 购买后永久无限制
    /// - 可与 Steam DLC 集成
    /// </summary>
    public class TrialManager : MonoBehaviour {
        public static TrialManager Instance { get; private set; }

        // ============== 配置 ==============
        [Header("试用版配置")]
        [SerializeField] private int freeDailyMinutes = 120; // 每天免费分钟数（默认2小时=120分钟）
        [SerializeField] private bool useRealtimeTimer = true; // 是否使用实时计时（精确到秒）

        // ============== 存储键名 ==============
        private const string KEY_TRIAL_USED_SECONDS = "Trial_UsedSeconds";
        private const string KEY_TRIAL_LAST_DATE = "Trial_LastDate";

        // ============== 状态 ==============
        private int usedSecondsToday;     // 今日已使用秒数（更精确）
        private DateTime lastActiveDate;  // 最后活跃日期
        private long activeTimestamp;     // 开始计时的时间戳

        // ============== 事件 ==============
        public event Action<int, int> OnUsageTimeChanged;  // 已用秒数, 限制秒数
        public event Action OnTrialExpired;                 // 试用到期
        public event Action<int> OnRemainingSecondsChanged; // 剩余秒数变化（每秒）
        public GameObject CursorGo;

        // ============== 属性 ==============
        public bool IsTrialExpired => !Version.Instance.IsFullVersion && usedSecondsToday >= freeDailyMinutes * 60;
        public int UsedMinutesToday => usedSecondsToday / 60;
        public int UsedSecondsToday => usedSecondsToday;
        public int FreeDailyMinutes => freeDailyMinutes;
        public int RemainingMinutesToday => Mathf.Max(0, freeDailyMinutes - UsedMinutesToday);
        public int RemainingSecondsToday => Mathf.Max(0, freeDailyMinutes * 60 - usedSecondsToday);

        private void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadData();
                CheckAndResetDaily();
            } else {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// 加载数据
        /// </summary>
        private void LoadData() {
            //今日使用的秒
            usedSecondsToday = PlayerPrefs.GetInt(KEY_TRIAL_USED_SECONDS, 0);

            string lastDateStr = PlayerPrefs.GetString(KEY_TRIAL_LAST_DATE, "");
            if (string.IsNullOrEmpty(lastDateStr)) {
                lastActiveDate = DateTime.Today;
            } else {
                DateTime.TryParse(lastDateStr, out lastActiveDate);
            }

            Debug.Log($"[TrialManager] 加载数据 - {(Version.Instance.IsFullVersion ? "正式版" : "试用版")}, 今日已用:{UsedMinutesToday}分钟{usedSecondsToday % 60}秒");
        }
        
        /// <summary>
        /// 检查并重置每日数据
        /// </summary>
        private void CheckAndResetDaily() {
            DateTime today = DateTime.Today;

            if (lastActiveDate.Date != today.Date) {
                usedSecondsToday = 0;
                lastActiveDate = today;
                SaveData();
                Debug.Log("[TrialManager] 新的一天，使用时间已重置");
            }
        }

        private void Start() {
            // 通知当前状态
            OnUsageTimeChanged?.Invoke(usedSecondsToday, freeDailyMinutes * 60);
        }

        private void OnDestroy() {
            SaveData();
        }

        // ============== 实时计时 ==============
        // 累加器，避免每帧调用 Time.time 产生精度问题
        private float secondsAccumulator = 0f;

        private void Update() {
            // 已购买则不计时
            if (Version.Instance.IsFullVersion) {
                return;
            }

            // 已用满则停止
            if (usedSecondsToday >= freeDailyMinutes * 60) {
                return;
            }

            if (useRealtimeTimer) {
                // 用 unscaledDeltaTime 不受 Time.timeScale 影响（暂停/慢动作不影响试用计时）
                secondsAccumulator += Time.unscaledDeltaTime;

                // 每累计 1 秒增加一次
                while (secondsAccumulator >= 1f) {
                    secondsAccumulator -= 1f;

                    if (usedSecondsToday >= freeDailyMinutes * 60) {
                        secondsAccumulator = 0f; // 清零避免溢出
                        break;
                    }

                    usedSecondsToday++;
                    OnRemainingSecondsChanged?.Invoke(RemainingSecondsToday);

                    // 每分钟触发一次时间变化事件 + 存档
                    if (usedSecondsToday % 60 == 0) {
                        OnUsageTimeChanged?.Invoke(usedSecondsToday, freeDailyMinutes * 60);
                        SaveData();
                    }

                    // 检查是否刚过期
                    if (usedSecondsToday >= freeDailyMinutes * 60) {
                        OnTrialExpired?.Invoke();
                        SaveData();
                        break;
                    }
                }
            }
        }

        // ============== 核心方法 ==============
        
        /// <summary>
        /// 获取剩余时间格式化字符串（如 "1小时30分钟"）
        /// </summary>
        public string GetRemainingTimeString() {
            if (Version.Instance.IsFullVersion) {
                return "永久无限制";
            }

            int remaining = RemainingSecondsToday;
            int hours = remaining / 3600;
            int minutes = (remaining % 3600) / 60;
            int seconds = remaining % 60;

            if (hours > 0) {
                return $"{hours}小时{minutes}分钟";
            } else if (minutes > 0) {
                return $"{minutes}分钟{seconds}秒";
            } else {
                return $"{seconds}秒";
            }
        }

        /// <summary>
        /// 获取使用进度格式化字符串
        /// </summary>
        public string GetUsageProgressString() {
            if (Version.Instance.IsFullVersion) {
                return "已购买";
            }

            int remaining = RemainingSecondsToday;
            int hours = remaining / 3600;
            int minutes = (remaining % 3600) / 60;

            if (hours > 0) {
                return $"剩余 {hours}小时{minutes}分钟";
            } else if (minutes > 0) {
                return $"剩余 {minutes}分钟";
            } else {
                return "今日额度已用完";
            }
        }

        /// <summary>
        /// 获取使用进度百分比（0-1）
        /// </summary>
        public float GetUsageProgress() {
            if (Version.Instance.IsFullVersion) {
                return 1f;
            }
            return (float)usedSecondsToday / (freeDailyMinutes * 60);
        }

        // ============== 数据管理 ==============

        /// <summary>
        /// 保存数据
        /// </summary>
        private void SaveData() {
            PlayerPrefs.SetInt(KEY_TRIAL_USED_SECONDS, usedSecondsToday);
            PlayerPrefs.SetString(KEY_TRIAL_LAST_DATE, lastActiveDate.ToString("yyyy-MM-dd"));
            PlayerPrefs.Save();
        }

        // ============== 调试 ==============

        /// <summary>
        /// 重置试用数据（测试用）
        /// </summary>
        [ContextMenu("重置试用数据")]
        public void ResetTrialData() {
            usedSecondsToday = 0;
            lastActiveDate = DateTime.Today;
            PlayerPrefs.DeleteKey(KEY_TRIAL_USED_SECONDS);
            PlayerPrefs.DeleteKey(KEY_TRIAL_LAST_DATE);
            PlayerPrefs.Save();
            OnUsageTimeChanged?.Invoke(0, freeDailyMinutes * 60);
            Debug.Log("[TrialManager] 试用数据已重置");
        }

        /// <summary>
        /// 增加60秒（测试用，模拟过了一分钟）
        /// </summary>
        [ContextMenu("模拟过1分钟")]
        public void DebugAddMinute() {
            usedSecondsToday = Mathf.Min(usedSecondsToday + 60, freeDailyMinutes * 60);
            SaveData();
            OnUsageTimeChanged?.Invoke(usedSecondsToday, freeDailyMinutes * 60);
            Debug.Log($"[TrialManager] 已用时间：{UsedMinutesToday}分钟{usedSecondsToday % 60}秒");
        }
    }
}
