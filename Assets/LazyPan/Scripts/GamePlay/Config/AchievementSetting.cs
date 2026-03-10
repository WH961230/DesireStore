using System;
using System.Collections.Generic;
using UnityEngine;

namespace LazyPan {
    [CreateAssetMenu(menuName = "LazyPan/AchievementSetting", fileName = "AchievementSetting")]
    public class AchievementSetting : Setting {
        public List<AchievementData> AchievementDatas = new List<AchievementData>();
    }

    [Serializable]
    public class AchievementData {
        public string AchievementTitle;
        public string AchievementContent;
        public bool IsAchievementFinished;
        public int RewardInteractPoint;
        public List<TaskData> TaskDatas = new List<TaskData>();
    }

    [Serializable]
    public class TaskData {
        public string TaskContent;
        public string TaskTitle;
        public int RewardInteractPoint;
        public bool IsTaskFinished;
    }
}