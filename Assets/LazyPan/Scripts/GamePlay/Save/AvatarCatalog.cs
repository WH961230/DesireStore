using System;
using System.IO;
using UnityEngine;

namespace LazyPan {
    public static class AvatarCatalog {
        private const string DefaultAvatarProjectPath = "LazyPan/Bundles/Images/最终.png";

        // 预设：把预置头像放在 Assets/Resources/Avatars/<avatarId>.png
        public static Sprite LoadAvatarSprite(string avatarId) {
            if (string.IsNullOrWhiteSpace(avatarId)) {
                avatarId = "avatar_01";
            }

            // Resources.Load 参数不含副档名
            Sprite sprite = Resources.Load<Sprite>($"Avatars/{avatarId}");
            if (sprite != null) {
                return sprite;
            }

            // 读取项目内默认头像：Assets/LazyPan/Bundles/Images/最终.png
            sprite = LoadDefaultAvatarSprite();
            if (sprite != null) {
                return sprite;
            }

            // 兜底：若资源不存在，生成默认占位头像
            return CreateFallbackAvatarSprite();
        }

        private static Sprite LoadDefaultAvatarSprite() {
#if UNITY_EDITOR
            // Editor 中可直接通过 AssetDatabase 读取
            try {
                string assetPath = $"Assets/{DefaultAvatarProjectPath}";
                var sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                if (sprite != null) {
                    return sprite;
                }
            } catch (Exception) {
                // ignore and fallback to file load
            }
#endif
            // 运行时尝试用文件读取（仅在该文件存在时可用）
            try {
                string path = Path.Combine(Application.dataPath, DefaultAvatarProjectPath);
                if (File.Exists(path)) {
                    byte[] bytes = File.ReadAllBytes(path);
                    var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    if (tex.LoadImage(bytes)) {
                        tex.name = "default_avatar_from_file";
                        return Sprite.Create(
                            tex,
                            new Rect(0, 0, tex.width, tex.height),
                            new Vector2(0.5f, 0.5f),
                            100f
                        );
                    }
                }
            } catch (Exception) {
                // ignore and fallback to placeholder
            }

            return null;
        }

        private static Sprite CreateFallbackAvatarSprite() {
            const int size = 64;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.name = "fallback_avatar";

            // 背景色
            var bg = new Color32(220, 220, 220, 255);
            var fg = new Color32(120, 120, 120, 255);

            for (int y = 0; y < size; y++) {
                for (int x = 0; x < size; x++) {
                    tex.SetPixel(x, y, bg);
                }
            }

            // 简单画一个圆形头像轮廓
            int cx = size / 2;
            int cy = size / 2 + 6;
            int r = size / 3;
            int r2 = r * r;
            for (int y = 0; y < size; y++) {
                for (int x = 0; x < size; x++) {
                    int dx = x - cx;
                    int dy = y - cy;
                    if (dx * dx + dy * dy <= r2) {
                        tex.SetPixel(x, y, fg);
                    }
                }
            }

            tex.Apply();
            return Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f),
                100f
            );
        }

        public static string GetCustomAvatarDirectory() {
            return Path.Combine(Application.persistentDataPath, "Avatars");
        }

        public static Sprite LoadAvatarSprite(PlayerSaveV1 save) {
            if (save == null) {
                return null;
            }

            // 若有自定义头像，优先显示
            if (!string.IsNullOrWhiteSpace(save.CustomAvatarFileName)) {
                try {
                    string dir = GetCustomAvatarDirectory();
                    string path = Path.Combine(dir, save.CustomAvatarFileName);
                    if (File.Exists(path)) {
                        byte[] bytes = File.ReadAllBytes(path);
                        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                        if (tex.LoadImage(bytes)) {
                            tex.name = $"custom_avatar_{save.CustomAvatarFileName}";
                            return Sprite.Create(
                                tex,
                                new Rect(0, 0, tex.width, tex.height),
                                new Vector2(0.5f, 0.5f),
                                100f
                            );
                        }
                    }
                } catch (Exception) {
                    // ignore and fallback to preset avatar
                }
            }

            return LoadAvatarSprite(save.AvatarId);
        }
    }
}

