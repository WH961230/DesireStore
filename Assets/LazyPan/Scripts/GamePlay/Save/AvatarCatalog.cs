using System;
using System.IO;
using UnityEngine;

namespace LazyPan {
    public static class AvatarCatalog {
        // 预设：把预置头像放在 Assets/Resources/Avatars/<avatarId>.png
        public static Sprite LoadAvatarSprite(string avatarId) {
            if (string.IsNullOrWhiteSpace(avatarId)) {
                avatarId = "avatar_01";
            }

            // Resources.Load 参数不含副档名
            return Resources.Load<Sprite>($"Avatars/{avatarId}");
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

