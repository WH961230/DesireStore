using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LazyPan {
    /// <summary>
    /// 负责“我”页面的 UI 与头像编辑逻辑的模块。
    /// 仅处理“我”区域内的展示和交互，不负责整体导航。
    /// </summary>
    public class SceneAProfileModule {
        private PlayerSaveV1 _save;
        private bool _bindingsInitialized;

        private Comp _meModule;

        /// <summary>
        /// 使用指定存档数据创建“我”页面 UI 模块。
        /// </summary>
        /// <param name="save">玩家存档数据引用。</param>
        public SceneAProfileModule(PlayerSaveV1 save) {
            _save = save;
        }

        /// <summary>
        /// 初始化“我”页面根结点并完成一次性事件绑定。
        /// </summary>
        /// <param name="meModule">“我”页面的根 Comp。</param>
        public void Init(Comp meModule) {
            _meModule = meModule;
            Refresh();
        }

        /// <summary>
        /// 每次切到“我”页面时调用，用于刷新展示内容。
        /// </summary>
        public void Refresh() {
            if (_meModule == null) {
                return;
            }

            _save ??= PlayerSaveStore.LoadOrCreate();

            if (!_bindingsInitialized) {
                _bindingsInitialized = true;
                BindMeUI(_meModule);
            }

            RenderMeUI(_meModule);
        }

        /// <summary>
        /// 绑定“我”页面里的交互（名字输入、头像点击等）。
        /// </summary>
        /// <param name="userBrief">“我”页面的根 Comp。</param>
        private void BindMeUI(Comp userBrief) {
            // 名字：优先用输入框（可编辑），若没有则只显示文字
            TMP_InputField nameInput = Cond.Instance.Get<TMP_InputField>(userBrief, "名字");

            if (nameInput != null) {
                nameInput.onEndEdit.RemoveAllListeners();
                nameInput.onEndEdit.AddListener(value => {
                    _save ??= PlayerSaveStore.LoadOrCreate();
                    string newName = (value ?? string.Empty).Trim();
                    if (string.IsNullOrWhiteSpace(newName)) {
                        newName = "玩家";
                    }

                    _save.PlayerName = newName;
                    PlayerSaveStore.Save(_save);
                    RenderMeUI(userBrief);
                });
            }

            // 头像：点击图片上传本地图片（会复制到存档目录）
            Comp avatarImageComp = Cond.Instance.Get<Comp>(userBrief, "头像");
            if (avatarImageComp != null) {
                BindAvatarClick(userBrief, avatarImageComp.GetComponent<Image>(), () => {
                    string selected = OpenImageFilePicker();
                    if (string.IsNullOrWhiteSpace(selected)) {
                        return;
                    }

                    string copiedFileName = CopyAvatarToSaveFolder(selected);
                    if (string.IsNullOrWhiteSpace(copiedFileName)) {
                        return;
                    }

                    _save ??= PlayerSaveStore.LoadOrCreate();
                    _save.CustomAvatarFileName = copiedFileName;
                    PlayerSaveStore.Save(_save);
                    RenderMeUI(userBrief);
                });
            }
        }

        /// <summary>
        /// 按当前存档内容渲染“我”页面。
        /// </summary>
        /// <param name="userBrief">“我”页面的根 Comp。</param>
        private void RenderMeUI(Comp userBrief) {
            _save ??= PlayerSaveStore.LoadOrCreate();

            // 显示名字（若有输入框就同步其内容；没有就用文字）
            TextMeshProUGUI nameText = Cond.Instance.Get<TextMeshProUGUI>(userBrief, "名字");
            if (nameText != null) {
                nameText.text = _save.PlayerName ?? string.Empty;
            }

            // 显示头像（与 BindMeUI 一致：通过 Comp「头像」取 Image，确保选完头像后“我”界面立即看到变化）
            Comp avatarComp = Cond.Instance.Get<Comp>(userBrief, "头像");
            Image avatarImage = avatarComp != null ? avatarComp.GetComponent<Image>() : null;
            if (avatarImage == null) {
                avatarImage = Cond.Instance.Get<Image>(userBrief, "头像");
            }
            if (avatarImage != null) {
                Sprite sp = AvatarCatalog.LoadAvatarSprite(_save);
                if (sp != null) {
                    avatarImage.sprite = sp;
                    avatarImage.enabled = true;
                    if (avatarImage.rectTransform != null) {
                        LayoutRebuilder.MarkLayoutForRebuild(avatarImage.rectTransform);
                    }
                }
            }
        }

        /// <summary>
        /// 绑定头像的点击事件，优先走 Comp 的 Pointer 事件，失败时退回 EventTrigger。
        /// </summary>
        /// <param name="userBrief">包含头像的父 Comp。</param>
        /// <param name="avatarImage">头像图片组件。</param>
        /// <param name="onLeftClick">左键点击时触发的回调。</param>
        private void BindAvatarClick(Comp userBrief, Image avatarImage, Action onLeftClick) {
            if (userBrief == null || avatarImage == null || onLeftClick == null) {
                return;
            }

            // 按项目约定：优先通过 Comp 的 OnPointerClickEvent 绑定点击（带 PointerEventData 参数）
            Comp avatarComp = Cond.Instance.Get<Comp>(userBrief, "头像");
            if (avatarComp != null) {
                avatarComp.OnPointerClickEvent.RemoveAllListeners();
                avatarComp.OnPointerClickEvent.AddListener(call => {
                    if (call != null && call.button == PointerEventData.InputButton.Left) {
                        onLeftClick();
                    }
                });
                return;
            }

            // 兜底：如果该节点没有挂 Comp（或事件为空），退回 EventTrigger，避免点击失效
            AddPointerClick(avatarImage.gameObject, onLeftClick);
        }

        /// <summary>
        /// 使用 EventTrigger 给指定 GameObject 添加 PointerClick 事件。
        /// </summary>
        /// <param name="go">要添加点击事件的对象。</param>
        /// <param name="onClick">点击时触发的回调。</param>
        private void AddPointerClick(GameObject go, Action onClick) {
            if (go == null || onClick == null) {
                return;
            }

            var trigger = go.GetComponent<EventTrigger>();
            if (trigger == null) {
                trigger = go.AddComponent<EventTrigger>();
            }

            if (trigger.triggers == null) {
                trigger.triggers = new System.Collections.Generic.List<EventTrigger.Entry>();
            }

            // 避免重复注册同类型事件：先移除已有 PointerClick
            trigger.triggers.RemoveAll(e => e != null && e.eventID == EventTriggerType.PointerClick);

            var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
            entry.callback.AddListener(_ => onClick());
            trigger.triggers.Add(entry);
        }

        /// <summary>
        /// 将用户选择的头像文件复制到存档目录，并返回新文件名。
        /// </summary>
        /// <param name="sourcePath">源图片文件绝对路径。</param>
        /// <returns>复制后的文件名，失败时返回 null。</returns>
        private string CopyAvatarToSaveFolder(string sourcePath) {
            try {
                if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath)) {
                    Debug.LogError("选择的头像文件不存在");
                    return null;
                }

                string dir = AvatarCatalog.GetCustomAvatarDirectory();
                if (!Directory.Exists(dir)) {
                    Directory.CreateDirectory(dir);
                }

                string ext = Path.GetExtension(sourcePath);
                if (string.IsNullOrWhiteSpace(ext)) {
                    ext = ".png";
                }

                string fileName = $"avatar_{Guid.NewGuid():N}{ext}";
                string destPath = Path.Combine(dir, fileName);

                File.Copy(sourcePath, destPath, true);
                return fileName;
            } catch (Exception e) {
                Debug.LogError($"复制头像失败：{e.Message}");
                return null;
            }
        }

        /// <summary>
        /// 打开本地文件选择器，让用户选择头像图片。
        /// 在编辑器下使用 UnityEditor 面板，在 Windows 平台尝试使用 System.Windows.Forms。
        /// </summary>
        /// <returns>用户选择的图片路径，取消或失败时返回 null。</returns>
        private string OpenImageFilePicker() {
#if UNITY_EDITOR
            try {
                return UnityEditor.EditorUtility.OpenFilePanel("选择头像图片", "", "png,jpg,jpeg");
            } catch (Exception e) {
                Debug.LogError($"打开文件选择器失败：{e.Message}");
                return null;
            }
#elif UNITY_STANDALONE_WIN
            // 尽量在不引入额外插件的前提下提供 Windows 选档：
            // 使用反射尝试 System.Windows.Forms.OpenFileDialog；若不可用则提示用户手动放文件。
            try {
                var asm = AppDomain.CurrentDomain.Load("System.Windows.Forms");
                var dialogType = asm.GetType("System.Windows.Forms.OpenFileDialog");
                if (dialogType == null) {
                    Debug.LogWarning("当前运行环境不支持打开文件选择器，请将头像图片复制到存档目录后再设置。");
                    return null;
                }

                dynamic dialog = Activator.CreateInstance(dialogType);
                dialog.Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";
                dialog.Multiselect = false;
                var result = dialog.ShowDialog();
                // DialogResult.OK == 1
                if ((int)result == 1) {
                    return (string)dialog.FileName;
                }

                return null;
            } catch (Exception) {
                Debug.LogWarning("当前运行环境不支持打开文件选择器，请将头像图片复制到存档目录后再设置。");
                return null;
            }
#else
            Debug.LogWarning("当前平台未实现本地头像选择器");
            return null;
#endif
        }
    }
}

