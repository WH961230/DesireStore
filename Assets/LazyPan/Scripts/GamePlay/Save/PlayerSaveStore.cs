using System;
using System.Collections.Generic;
using UnityEngine;

namespace LazyPan {
    [Serializable]
    public class PlayerSaveV1 {
        public int SchemaVersion = 1;

        public string PlayerName = "玩家";
        // 预置头像的 key（预设会从 Resources/Avatars/<AvatarId> 加载 Sprite）
        public string AvatarId = "avatar_01";
        // 若玩家选择上传本地头像，会把图片复制到 persistentDataPath/Avatars/ 下，
        // 并在此记录文件名（只存文件名，避免绝对路径跨机器失效）
        public string CustomAvatarFileName = "";

        // 目前 UI 只用到交互点显示；先存成玩家基础交互点（可日后扩充玩家信息）
        public int BaseInteractPoint = 0;

        public List<PlayerAchievementV1> Achievements = new List<PlayerAchievementV1>();
    }

    [Serializable]
    public class PlayerAchievementV1 {
        public string Id;
        public string Title;
        public string Content;
        public bool IsFinished;
        public int RewardInteractPoint;
        public List<PlayerTaskV1> Tasks = new List<PlayerTaskV1>();
    }

    [Serializable]
    public class PlayerTaskV1 {
        public string Id;
        public string Title;
        public string Content;
        public int RewardInteractPoint;
        public bool IsFinished;
    }

    public static class PlayerSaveStore {
        public const string SaveFileName = "player_save.json";
        public const int CurrentSchemaVersion = 1;

        public static PlayerSaveV1 LoadOrCreate() {
            PlayerSaveV1 data = null;
            try {
                data = SaveLoad.Instance.Load<PlayerSaveV1>(SaveFileName);
            } catch (Exception e) {
                Debug.LogError($"读取存档失败，将建立新存档。{e.Message}");
                data = null;
            }

            if (data == null) {
                data = new PlayerSaveV1();
            }

            Normalize(data);

            // 目前没有 migrate 逻辑（schemaVersion 先预留），版本不符时先以「不崩」为第一优先
            if (data.SchemaVersion <= 0) {
                data.SchemaVersion = CurrentSchemaVersion;
            }

            if (data.SchemaVersion != CurrentSchemaVersion) {
                Debug.LogWarning($"存档版本 {data.SchemaVersion} 与当前 {CurrentSchemaVersion} 不一致，尚未实现迁移，将以现有数据继续执行。");
            }

            // 默认第一次打开项目成就和任务为空，不自动添加预设成就
            Save(data);
            return data;
        }

        public static void Save(PlayerSaveV1 data) {
            if (data == null) {
                return;
            }

            Normalize(data);

            try {
                SaveLoad.Instance.Save(SaveFileName, data);
            } catch (Exception e) {
                Debug.LogError($"写入存档失败：{e.Message}");
            }
        }

        private static void Normalize(PlayerSaveV1 data) {
            if (string.IsNullOrWhiteSpace(data.PlayerName)) {
                data.PlayerName = "玩家";
            }

            if (string.IsNullOrWhiteSpace(data.AvatarId)) {
                data.AvatarId = "avatar_01";
            }

            if (data.CustomAvatarFileName == null) {
                data.CustomAvatarFileName = "";
            }

            if (data.Achievements == null) {
                data.Achievements = new List<PlayerAchievementV1>();
            }

            foreach (var a in data.Achievements) {
                if (a == null) {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(a.Id)) {
                    a.Id = Guid.NewGuid().ToString("N");
                }

                if (a.Tasks == null) {
                    a.Tasks = new List<PlayerTaskV1>();
                }

                foreach (var t in a.Tasks) {
                    if (t == null) {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(t.Id)) {
                        t.Id = Guid.NewGuid().ToString("N");
                    }
                }
            }
        }

    }
}

