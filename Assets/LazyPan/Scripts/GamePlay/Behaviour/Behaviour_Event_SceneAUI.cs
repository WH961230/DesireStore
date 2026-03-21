using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LazyPan {
    public class Behaviour_Event_SceneAUI : Behaviour {
        private Button _createBtn;
        private Button _cancelCreateBtn;
        private TextMeshProUGUI _createText;
        private TextMeshProUGUI _cancelCreateText;
        private string _createContext;
        private PlayerSaveV1 _save;
        private bool _profileBindingsInitialized;
        private bool _meProfileBindingsInitialized;

        public Behaviour_Event_SceneAUI(Entity entity, string behaviourSign) : base(entity, behaviourSign) {
            InitUI();
        }

        private void InitUI() {
            _save = PlayerSaveStore.LoadOrCreate();

            // 获取场景A的流程实例
            Flo.Instance.GetFlow(out Flow_SceneA flow);
            // 获取界面组件
            Comp interfaceComp = Cond.Instance.Get<Comp>(flow.GetUI(), "界面");
            // 获取详情组件
            Comp detailComp = Cond.Instance.Get<Comp>(interfaceComp, "详情");
            // 获取主页组件并激活
            Comp homePage = Cond.Instance.Get<Comp>(detailComp, "主页");
            homePage.gameObject.SetActive(true);

            // 获取模块入口组件
            Comp moduleEntrance = Cond.Instance.Get<Comp>(homePage, "模块入口");
            
            // 获取各个模块组件
            Comp meModule = Cond.Instance.Get<Comp>(detailComp, "我");
            Comp taskManagementModule = Cond.Instance.Get<Comp>(detailComp, "任务");
            Comp achievementModule = Cond.Instance.Get<Comp>(detailComp, "成就");
            Comp shopModule = null;
            try {
                shopModule = Cond.Instance.Get<Comp>(detailComp, "商店");
            } catch {
                // 商店节点未创建时忽略
            }

            // 获取各个按钮
            Button meBtn = Cond.Instance.Get<Button>(moduleEntrance, "我");
            Button taskManagementBtn = Cond.Instance.Get<Button>(moduleEntrance, "任务");
            Button achievementBtn = Cond.Instance.Get<Button>(moduleEntrance, "成就");
            Button shopBtn = null;
            try {
                shopBtn = Cond.Instance.Get<Button>(moduleEntrance, "商店");
            } catch {
                // 商店按钮未创建时忽略
            }

            // 获取状态栏组件
            Comp statusBar = Cond.Instance.Get<Comp>(interfaceComp, "状态栏");
            // 获取标题文本
            TextMeshProUGUI titleText = Cond.Instance.Get<TextMeshProUGUI>(statusBar, "标题");
            // 获取返回主页按钮
            Button backToHomeBtn = Cond.Instance.Get<Button>(statusBar, "返回主页");
            // 任务页相关按钮（可能在部分UI版本中不存在）
            _createBtn = Cond.Instance.Get<Button>(statusBar, "创建");
            _cancelCreateBtn = Cond.Instance.Get<Button>(statusBar, "取消创建");
            _createText = Cond.Instance.Get<TextMeshProUGUI>(statusBar, "创建");
            _cancelCreateText = Cond.Instance.Get<TextMeshProUGUI>(statusBar, "取消创建");

            _createBtn.gameObject.SetActive(false);
            _cancelCreateBtn.gameObject.SetActive(false);

            // 注册"我"按钮点击事件
            ButtonRegister.RemoveAllListener(meBtn);
            ButtonRegister.AddListener(meBtn, () => {
                homePage.gameObject.SetActive(false);
                taskManagementModule.gameObject.SetActive(false);
                achievementModule.gameObject.SetActive(false);
                if (shopModule != null) shopModule.gameObject.SetActive(false);
                meModule.gameObject.SetActive(true);

                // 更新标题为当前页面中文名称
                titleText.text = "我";
                backToHomeBtn.gameObject.SetActive(true);

                RefreshMe(meModule);

                _createBtn.gameObject.SetActive(false);
                _cancelCreateBtn.gameObject.SetActive(false);
                
                _createContext = string.Empty;
                UpdateCreateButtonsForContext();
            });

            // 注册"任务"按钮点击事件
            ButtonRegister.RemoveAllListener(taskManagementBtn);
            ButtonRegister.AddListener(taskManagementBtn, () => {
                homePage.gameObject.SetActive(false);
                meModule.gameObject.SetActive(false);
                achievementModule.gameObject.SetActive(false);
                if (shopModule != null) shopModule.gameObject.SetActive(false);
                taskManagementModule.gameObject.SetActive(true);

                // 更新标题为当前页面中文名称
                titleText.text = "任务";
                backToHomeBtn.gameObject.SetActive(true);
                RefreshAllTaskInfo();
                _createContext = "任务";
                
                _createBtn.gameObject.SetActive(true);
                _cancelCreateBtn.gameObject.SetActive(false);

                UpdateCreateButtonsForContext();
            });

            // 注册"成就"按钮点击事件
            if (achievementBtn != null && achievementModule != null) {
                ButtonRegister.RemoveAllListener(achievementBtn);
                ButtonRegister.AddListener(achievementBtn, () => {
                    homePage.gameObject.SetActive(false);
                    meModule.gameObject.SetActive(false);
                    taskManagementModule.gameObject.SetActive(false);
                    if (shopModule != null) shopModule.gameObject.SetActive(false);
                    achievementModule.gameObject.SetActive(true);
                    // 更新标题为当前页面中文名称
                    titleText.text = "成就";
                    backToHomeBtn.gameObject.SetActive(true);
                    RefreshAllAchievementInfo();
                    _createContext = "成就";

                    _createBtn.gameObject.SetActive(true);
                    _cancelCreateBtn.gameObject.SetActive(false);

                    UpdateCreateButtonsForContext();
                });
            }

            // 注册"商店"按钮点击事件
            if (shopBtn != null && shopModule != null) {
                ButtonRegister.RemoveAllListener(shopBtn);
                ButtonRegister.AddListener(shopBtn, () => {
                    homePage.gameObject.SetActive(false);
                    meModule.gameObject.SetActive(false);
                    taskManagementModule.gameObject.SetActive(false);
                    achievementModule.gameObject.SetActive(false);
                    shopModule.gameObject.SetActive(true);
                    titleText.text = "商店";
                    backToHomeBtn.gameObject.SetActive(true);
                    RefreshAllShopInfo();
                    _createContext = "商店";

                    _createBtn.gameObject.SetActive(true);
                    _cancelCreateBtn.gameObject.SetActive(false);

                    UpdateCreateButtonsForContext();
                });
            }

            if (_createBtn != null) {
                ButtonRegister.RemoveAllListener(_createBtn);
                ButtonRegister.AddListener(_createBtn, () => {
                    _createBtn.gameObject.SetActive(false);
                    _cancelCreateBtn.gameObject.SetActive(true);

                    UpdateCreateButtonsForContext();

                    if (_createContext == "成就") {
                        ShowCreateAchievementForm();
                    } else if (_createContext == "商店") {
                        ShowCreateShopItemForm();
                    } else {
                        ShowCreateTaskForm();
                    }
                });
            }

            // 状态栏：取消创建任务（回到任务列表态）
            if (_cancelCreateBtn != null) {
                ButtonRegister.RemoveAllListener(_cancelCreateBtn);
                ButtonRegister.AddListener(_cancelCreateBtn, () => {
                    SwitchToTaskBrowseButtons();
                    if (_createContext == "成就") {
                        RefreshAllAchievementInfo();
                    } else if (_createContext == "商店") {
                        RefreshAllShopInfo();
                    } else {
                        RefreshAllTaskInfo();
                    }
                });
            }

            ButtonRegister.RemoveAllListener(backToHomeBtn);
            ButtonRegister.AddListener(backToHomeBtn, () => {
                meModule.gameObject.SetActive(false);
                taskManagementModule.gameObject.SetActive(false);
                achievementModule.gameObject.SetActive(false);
                if (shopModule != null) shopModule.gameObject.SetActive(false);
                homePage.gameObject.SetActive(true);

                // 返回主页时标题显示“修行手册”
                titleText.text = "修行手册";
                backToHomeBtn.gameObject.SetActive(false);

                _createBtn.gameObject.SetActive(false);
                _cancelCreateBtn.gameObject.SetActive(false);

                RefreshUserInfo();
            });
            
            // 主页显示时隐藏返回主页按钮
            backToHomeBtn.gameObject.SetActive(false);
            
            // 初始进入时在主页，标题显示“修行手册”
            titleText.text = "修行手册";

            RefreshUserInfo();
        }

        #region 用户信息

        /// <summary>
        /// 刷新用户信息界面的方法
        /// 加载设置并更新界面上的用户名、交互点和日期信息
        /// </summary>
        private void RefreshUserInfo() {
            _save ??= PlayerSaveStore.LoadOrCreate();

            // 获取场景A的流程实例
            Flo.Instance.GetFlow(out Flow_SceneA flow);
            // 从流程中获取界面组件
            Comp interfaceComp = Cond.Instance.Get<Comp>(flow.GetUI(), "界面");
            Comp detailComp = Cond.Instance.Get<Comp>(interfaceComp, "详情");

            // 获取用户信息组件并激活
            Comp homePage = Cond.Instance.Get<Comp>(detailComp, "主页");
            homePage.gameObject.SetActive(true);

            // 主页用户简要交互点：TextMeshProUGUI: ui/界面/详情/主页/用户简要/交互点
            Comp userBrief = Cond.Instance.Get<Comp>(homePage, "用户简要");
            
            Image avatar = Cond.Instance.Get<Image>(userBrief, "头像");
            avatar.sprite = AvatarCatalog.LoadAvatarSprite(_save);
            
            TextMeshProUGUI name = Cond.Instance.Get<TextMeshProUGUI>(userBrief, "名字");
            name.text = _save.PlayerName ?? string.Empty;
            
            TextMeshProUGUI briefInteractPoint = Cond.Instance.Get<TextMeshProUGUI>(userBrief, "交互点");
            briefInteractPoint.text = $"交互点:{GetAllInteractPoint()}";
        }

        #endregion

        private void BindMeUI(Comp userBrief) {
            if (_profileBindingsInitialized) {
                return;
            }

            _profileBindingsInitialized = true;

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
        /// 刷新我
        /// </summary>
        /// <param name="meModule"></param>
        private void RefreshMe(Comp meModule) {
            if (meModule == null) {
                return;
            }

            _save ??= PlayerSaveStore.LoadOrCreate();

            if (!_meProfileBindingsInitialized) {
                _meProfileBindingsInitialized = true;
                BindMeUI(meModule);
            }

            RenderMeUI(meModule);
        }

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

        #region 成就信息

        /// <summary>
        /// 刷新所有成就信息的方法
        /// </summary>
        private void RefreshAllAchievementInfo() {
            _save ??= PlayerSaveStore.LoadOrCreate();

            // 获取场景流程组件
            Flo.Instance.GetFlow(out Flow_SceneA flow);

            // 获取界面组件
            Comp interfaceComp = Cond.Instance.Get<Comp>(flow.GetUI(), "界面");
            Comp detailComp = Cond.Instance.Get<Comp>(interfaceComp, "详情");

            // 获取成就界面组件并激活
            Comp achievementPage = Cond.Instance.Get<Comp>(detailComp, "成就");
            achievementPage.gameObject.SetActive(true);

            // 获取成就列表的父级Transform
            Transform achievementParent = Cond.Instance.Get<Transform>(achievementPage, "父物体");

            // 清空所有现有的成就子物体
            foreach (Transform tmp in achievementParent) {
                GameObject.Destroy(tmp.gameObject);
            }

            // 遍历所有成就数据并刷新显示
            foreach (var tmpAchievement in _save.Achievements) {
                RefreshAchievementInfo(tmpAchievement);
            }
        }

        /// <summary>
        /// 刷新成就信息的方法
        /// </summary>
        /// <param name="tmpAchievement">传入的成就数据对象</param>
        private void RefreshAchievementInfo(PlayerAchievementV1 tmpAchievement) {
            // 关闭所有已打开的成就信息
            CloseAllInfo();

            // 获取Flow场景实例中的UI界面组件
            Flo.Instance.GetFlow(out Flow_SceneA flow);
            Comp interfaceComp = Cond.Instance.Get<Comp>(flow.GetUI(), "界面");
            Comp detailComp = Cond.Instance.Get<Comp>(interfaceComp, "详情");

            // 获取并激活成就界面组件
            Comp achievementPage = Cond.Instance.Get<Comp>(detailComp, "成就");
            achievementPage.gameObject.SetActive(true);

            // 获取成就信息的父物体和模板组件
            Comp templateRoot = Cond.Instance.Get<Comp>(interfaceComp, "模板");
            Comp achievementTemplate = Cond.Instance.Get<Comp>(templateRoot, "成就模板");
            Transform achievementParent = Cond.Instance.Get<Transform>(achievementPage, "父物体");

            // 如果成就已完成，则直接返回
            if (tmpAchievement.IsFinished) {
                return;
            }

            // 实例化成就模板并激活
            Comp achievementInstance = GameObject.Instantiate(achievementTemplate, achievementParent);
            achievementInstance.gameObject.SetActive(true);

            // 设置成就名称、内容和奖励文本
            TextMeshProUGUI achievementTile = Cond.Instance.Get<TextMeshProUGUI>(achievementInstance, "成就名");
            achievementTile.text = tmpAchievement.Title;

            TextMeshProUGUI achievementContent = Cond.Instance.Get<TextMeshProUGUI>(achievementInstance, "成就内容");
            achievementContent.text = tmpAchievement.Content;

            TextMeshProUGUI achievementReward = Cond.Instance.Get<TextMeshProUGUI>(achievementInstance, "成就奖励交互点");
            achievementReward.text = string.Concat("成就奖励点:", tmpAchievement.RewardInteractPoint);

            // 注册成就任务按钮点击事件
            Button achievementTasksBtn = Cond.Instance.Get<Button>(achievementInstance, "成就所有任务");
            ButtonRegister.RemoveAllListener(achievementTasksBtn);
            ButtonRegister.AddListener(achievementTasksBtn, () => { RefreshTaskInfo(tmpAchievement); });

            // 注册完成按钮点击事件
            Button achievementFinishBtn = Cond.Instance.Get<Button>(achievementInstance, "完成");
            ButtonRegister.RemoveAllListener(achievementFinishBtn);
            ButtonRegister.AddListener(achievementFinishBtn, () => {
                // 标记成就已完成，刷新用户信息和所有成就信息
                tmpAchievement.IsFinished = true;
                PlayerSaveStore.Save(_save);

                RefreshUserInfo();
                RefreshAllAchievementInfo();
            });
        }

        #endregion

        #region 任务信息

        /// <summary>
        /// 刷新所有任务信息
        /// 该方法会关闭所有现有信息，然后重新加载成就设置，并刷新所有任务
        /// </summary>
        private void RefreshAllTaskInfo() {
            _save ??= PlayerSaveStore.LoadOrCreate();

            // 获取场景流程实例和界面组件
            Flo.Instance.GetFlow(out Flow_SceneA flow);
            Comp interfaceComp = Cond.Instance.Get<Comp>(flow.GetUI(), "界面");
            Comp detailComp = Cond.Instance.Get<Comp>(interfaceComp, "详情");

            // 打开任务界面
            Comp taskPage = Cond.Instance.Get<Comp>(detailComp, "任务");
            taskPage.gameObject.SetActive(true);

            // 获取任务模板与父物体
            Transform taskParent = Cond.Instance.Get<Transform>(taskPage, "父物体");

            // 每次打开前清空父物体
            ClearChildren(taskParent);

            // 遍历所有成就和任务数据，刷新每个任务的信息
            foreach (var tmpAchievement in _save.Achievements) {
                foreach (var tmpTask in tmpAchievement.Tasks) {
                    RefreshTaskInfo(tmpAchievement, tmpTask);
                }
            }
        }

        /// <summary>
        /// 刷新任务信息界面
        /// </summary>
        /// <param name="tmpAchievement">成就数据对象</param>
        private void RefreshTaskInfo(PlayerAchievementV1 tmpAchievement) {
            // 获取Flow场景实例
            Flo.Instance.GetFlow(out Flow_SceneA flow);
            // 从Flow中获取界面组件
            Comp interfaceComp = Cond.Instance.Get<Comp>(flow.GetUI(), "界面");
            Comp detailComp = Cond.Instance.Get<Comp>(interfaceComp, "详情");

            // 打开任务界面
            Comp taskPage = Cond.Instance.Get<Comp>(detailComp, "任务");
            taskPage.gameObject.SetActive(true);

            // 获取任务模板与父物体
            Comp templateRoot = Cond.Instance.Get<Comp>(interfaceComp, "模板");
            Comp taskTemplate = Cond.Instance.Get<Comp>(templateRoot, "任务模板");
            Transform taskParent = Cond.Instance.Get<Transform>(taskPage, "父物体");

            // 每次打开前清空父物体
            ClearChildren(taskParent);

            // 遍历并刷新所有任务信息
            foreach (var tmpTask in tmpAchievement.Tasks) {
                RefreshTaskInfo(tmpAchievement, tmpTask, taskTemplate, taskParent);
            }
        }

        /// <summary>
        /// 刷新任务信息的显示
        /// </summary>
        /// <param name="tmpAchievement">成就数据对象</param>
        /// <param name="tmpTask">任务数据对象</param>
        private void RefreshTaskInfo(PlayerAchievementV1 tmpAchievement, PlayerTaskV1 tmpTask) {
            // 获取Flow场景实例
            Flo.Instance.GetFlow(out Flow_SceneA flow);
            // 从Flow中获取UI界面组件
            Comp interfaceComp = Cond.Instance.Get<Comp>(flow.GetUI(), "界面");
            Comp detailComp = Cond.Instance.Get<Comp>(interfaceComp, "详情");

            // 打开任务界面
            Comp taskPage = Cond.Instance.Get<Comp>(detailComp, "任务");
            taskPage.gameObject.SetActive(true);

            // 获取任务模板与父物体
            Comp templateRoot = Cond.Instance.Get<Comp>(interfaceComp, "模板");
            Comp taskTemplate = Cond.Instance.Get<Comp>(templateRoot, "任务模板");
            Transform taskParent = Cond.Instance.Get<Transform>(taskPage, "父物体");

            RefreshTaskInfo(tmpAchievement, tmpTask, taskTemplate, taskParent);
        }

        private void RefreshTaskInfo(
            PlayerAchievementV1 tmpAchievement,
            PlayerTaskV1 tmpTask,
            Comp taskTemplate,
            Transform taskParent
        ) {
            // 如果任务已完成，则直接返回
            if (tmpTask.IsFinished) {
                return;
            }

            // 实例化任务模板并激活
            Comp taskInstance = GameObject.Instantiate(taskTemplate, taskParent);
            taskInstance.gameObject.SetActive(true);

            // 设置任务名称文本
            TextMeshProUGUI taskTile = Cond.Instance.Get<TextMeshProUGUI>(taskInstance, "任务名");
            taskTile.text = tmpTask.Title;

            // 设置任务内容文本
            TextMeshProUGUI taskContent = Cond.Instance.Get<TextMeshProUGUI>(taskInstance, "任务内容");
            taskContent.text = tmpTask.Content;

            // 设置任务所属成就文本（如果有配置）
            try {
                TextMeshProUGUI taskBelongAchievement =
                    Cond.Instance.Get<TextMeshProUGUI>(taskInstance, "任务所属成就");
                taskBelongAchievement.text = tmpAchievement?.Title ?? string.Empty;
            } catch (Exception) {
                // 可选显示：如果模板里没这个文案，则忽略
            }

            // 设置任务奖励文本
            TextMeshProUGUI taskReward = Cond.Instance.Get<TextMeshProUGUI>(taskInstance, "任务奖励交互点");
            taskReward.text = string.Concat("任务奖励点:", tmpTask.RewardInteractPoint);

            // 设置任务完成按钮并添加点击事件
            Button taskFinishBtn = Cond.Instance.Get<Button>(taskInstance, "任务完成");
            ButtonRegister.RemoveAllListener(taskFinishBtn);
            ButtonRegister.AddListener(taskFinishBtn, () => {
                tmpTask.IsFinished = true;
                PlayerSaveStore.Save(_save);

                RefreshUserInfo();
                RefreshAllTaskInfo();
            });
        }

        /// <summary>
        /// 获取所有交互点的总和
        /// </summary>
        /// <returns>返回总交互点数</returns>
        private int GetAllInteractPoint() {
            _save ??= PlayerSaveStore.LoadOrCreate();

            // 初始化任务交互点数为0
            int taskInteractPoint = 0;
            // 遍历所有成就数据
            foreach (var tmpAchievement in _save.Achievements) {
                // 如果成就已完成，则添加其奖励交互点
                if (tmpAchievement.IsFinished) {
                    taskInteractPoint += tmpAchievement.RewardInteractPoint;
                }

                // 遍历当前成就下的所有任务数据
                foreach (var tmpTask in tmpAchievement.Tasks) {
                    // 如果任务已完成，则添加其奖励交互点
                    if (tmpTask.IsFinished) {
                        taskInteractPoint += tmpTask.RewardInteractPoint;
                    }
                }
            }

            // 返回基础交互点与任务交互点的总和
            return _save.BaseInteractPoint + taskInteractPoint;
        }

        #endregion

        #region 商店信息

        private int GetShopTotalSpent() {
            _save ??= PlayerSaveStore.LoadOrCreate();
            int total = 0;
            if (_save.ShopProducts == null) {
                return 0;
            }
            foreach (var p in _save.ShopProducts) {
                if (p != null) {
                    total += p.Price * p.PurchasedQuantity;
                }
            }
            return total;
        }

        private int GetRemainingSpendable() {
            return GetAllInteractPoint() - GetShopTotalSpent();
        }

        private void RefreshAllShopInfo() {
            _save ??= PlayerSaveStore.LoadOrCreate();

            Flo.Instance.GetFlow(out Flow_SceneA flow);
            Comp interfaceComp = Cond.Instance.Get<Comp>(flow.GetUI(), "界面");
            Comp detailComp = Cond.Instance.Get<Comp>(interfaceComp, "详情");

            Comp shopPage = Cond.Instance.Get<Comp>(detailComp, "商店");
            shopPage.gameObject.SetActive(true);

            Transform shopParent = Cond.Instance.Get<Transform>(shopPage, "父物体");
            ClearChildren(shopParent);

            Comp templateRoot = Cond.Instance.Get<Comp>(interfaceComp, "模板");
            Comp productTemplate = null;
            try {
                productTemplate = Cond.Instance.Get<Comp>(templateRoot, "商品模板");
            } catch {
                return;
            }

            if (_save.ShopProducts == null) {
                return;
            }

            int remaining = GetRemainingSpendable();

            foreach (var product in _save.ShopProducts) {
                if (product == null) {
                    continue;
                }

                bool canBuy = remaining >= product.Price;
                if (product.Stock < 9999) {
                    canBuy = canBuy && product.PurchasedQuantity < product.Stock;
                }

                Comp instance = GameObject.Instantiate(productTemplate, shopParent);
                instance.gameObject.SetActive(true);

                TextMeshProUGUI productName = Cond.Instance.Get<TextMeshProUGUI>(instance, "商品名");
                if (productName != null) productName.text = product.Title ?? string.Empty;

                TextMeshProUGUI productPrice = Cond.Instance.Get<TextMeshProUGUI>(instance, "价格");
                if (productPrice != null) productPrice.text = $"价格:{product.Price}";

                string stockDisplay = product.Stock >= 9999 ? "无限" : $"库存:{product.Stock - product.PurchasedQuantity}";
                TextMeshProUGUI productStock = Cond.Instance.Get<TextMeshProUGUI>(instance, "库存");
                if (productStock != null) productStock.text = stockDisplay;

                TextMeshProUGUI productType = Cond.Instance.Get<TextMeshProUGUI>(instance, "类型");
                if (productType != null) productType.text = $"类型:{product.ProductType ?? "物品"}";

                Button buyBtn = Cond.Instance.Get<Button>(instance, "购买");
                if (buyBtn != null) {
                    buyBtn.interactable = canBuy;
                    if (canBuy) {
                        PlayerShopProductV1 captured = product;
                        ButtonRegister.RemoveAllListener(buyBtn);
                        ButtonRegister.AddListener(buyBtn, () => {
                            PurchaseProduct(captured);
                        });
                    }
                }
            }
        }

        private void PurchaseProduct(PlayerShopProductV1 product) {
            if (product == null) return;

            _save ??= PlayerSaveStore.LoadOrCreate();
            int remaining = GetRemainingSpendable();
            if (remaining < product.Price) {
                Debug.Log("余额不足，购买失败");
                return;
            }
            if (product.Stock < 9999 && product.PurchasedQuantity >= product.Stock) {
                Debug.Log("库存不足，购买失败");
                return;
            }

            product.PurchasedQuantity++;
            PlayerSaveStore.Save(_save);
            Debug.Log($"购买成功：{product.Title}");

            RefreshUserInfo();
            RefreshAllShopInfo();
        }

        private void ShowCreateShopItemForm() {
            Flo.Instance.GetFlow(out Flow_SceneA flow);
            Comp interfaceComp = Cond.Instance.Get<Comp>(flow.GetUI(), "界面");
            Comp detailComp = Cond.Instance.Get<Comp>(interfaceComp, "详情");

            Comp shopPage = Cond.Instance.Get<Comp>(detailComp, "商店");
            shopPage.gameObject.SetActive(true);

            Transform shopParent = Cond.Instance.Get<Transform>(shopPage, "父物体");
            ClearChildren(shopParent);

            Comp templateRoot = Cond.Instance.Get<Comp>(interfaceComp, "模板");
            Comp addProductTemplate = null;
            try {
                addProductTemplate = Cond.Instance.Get<Comp>(templateRoot, "添加商品模板");
            } catch {
                Debug.LogError("缺少 模板/添加商品模板");
                return;
            }

            Comp instance = GameObject.Instantiate(addProductTemplate, shopParent);
            instance.gameObject.SetActive(true);

            Button createDoneBtn = Cond.Instance.Get<Button>(instance, "创建完成");
            ButtonRegister.RemoveAllListener(createDoneBtn);
            ButtonRegister.AddListener(createDoneBtn, () => {
                string title = GetInputOrText(instance, "商品名").Trim();
                string priceText = GetInputOrText(instance, "价格").Trim();
                string stockText = GetInputOrText(instance, "库存").Trim();
                string productType = GetInputOrText(instance, "类型").Trim();
                if (string.IsNullOrWhiteSpace(productType)) productType = "物品";

                if (string.IsNullOrWhiteSpace(title)) {
                    Debug.LogError("商品名不能为空");
                    return;
                }
                if (!int.TryParse(priceText, out int price) || price < 0) {
                    Debug.LogError("价格必须是大于等于0的整数");
                    return;
                }
                if (!int.TryParse(stockText, out int stock) || stock < 1) {
                    Debug.LogError("库存必须是大于等于1的整数（1=一次性，9999=无限）");
                    return;
                }

                _save ??= PlayerSaveStore.LoadOrCreate();
                if (_save.ShopProducts == null) {
                    _save.ShopProducts = new System.Collections.Generic.List<PlayerShopProductV1>();
                }
                _save.ShopProducts.Add(new PlayerShopProductV1 {
                    Id = Guid.NewGuid().ToString("N"),
                    Title = title,
                    Price = price,
                    Stock = stock,
                    ProductType = productType,
                    PurchasedQuantity = 0
                });
                PlayerSaveStore.Save(_save);

                SwitchToTaskBrowseButtons();
                RefreshAllShopInfo();
            });
        }

        #endregion

        #region 通用

        /// <summary>
        /// 关闭所有信息显示面板
        /// </summary>
        private void CloseAllInfo() {
            // 获取场景A的流程实例
            Flo.Instance.GetFlow(out Flow_SceneA flow);
            Comp interfaceComp = Cond.Instance.Get<Comp>(flow.GetUI(), "界面");
            try {
                Comp info = Cond.Instance.Get<Comp>(interfaceComp, "任务信息");
                info.gameObject.SetActive(false);
            } catch (Exception) {
                // ignore: optional panel may not exist
            }

            try {
                Comp info = Cond.Instance.Get<Comp>(interfaceComp, "成就信息");
                info.gameObject.SetActive(false);
            } catch (Exception) {
                // ignore: optional panel may not exist
            }
        }

        private void ClearChildren(Transform parent) {
            foreach (Transform tmp in parent) {
                GameObject.Destroy(tmp.gameObject);
            }
        }

        private string GetInputOrText(Comp parent, string sign) {
            TMP_InputField input = Cond.Instance.Get<TMP_InputField>(parent, sign);
            if (input != null) {
                return input.text;
            }

            TextMeshProUGUI text = Cond.Instance.Get<TextMeshProUGUI>(parent, sign);
            if (text != null) {
                return text.text;
            }

            return string.Empty;
        }

        private void ShowCreateTaskForm() {
            Flo.Instance.GetFlow(out Flow_SceneA flow);
            Comp interfaceComp = Cond.Instance.Get<Comp>(flow.GetUI(), "界面");
            Comp detailComp = Cond.Instance.Get<Comp>(interfaceComp, "详情");

            // 打开任务界面
            Comp taskPage = Cond.Instance.Get<Comp>(detailComp, "任务");
            taskPage.gameObject.SetActive(true);

            // 目标父物体：ui/界面/详情/任务/父物体
            Transform taskParent = Cond.Instance.Get<Transform>(taskPage, "父物体");
            ClearChildren(taskParent);

            // 添加任务模板：ui/界面/模板/添加任务模板
            Comp templateRoot = Cond.Instance.Get<Comp>(interfaceComp, "模板");
            Comp addTaskTemplate = Cond.Instance.Get<Comp>(templateRoot, "添加任务模板");

            Comp instance = GameObject.Instantiate(addTaskTemplate, taskParent);
            instance.gameObject.SetActive(true);

            // 成就下拉选择：ui/界面/模板/添加任务模板/任务所属成就（TMP_Dropdown）
            TMP_Dropdown achievementDropdown = null;
            try {
                GameObject achievementDropdownGo = Cond.Instance.Get<GameObject>(instance, "任务所属成就");
                achievementDropdown = achievementDropdownGo.GetComponent<TMP_Dropdown>();
            } catch (Exception) {
                achievementDropdown = null;
            }

            if (achievementDropdown == null) {
                Debug.LogError("添加任务模板缺少“任务所属成就”(TMP_Dropdown)，无法创建任务");
                return;
            }

            // 填充下拉选项（成就名）
            _save ??= PlayerSaveStore.LoadOrCreate();
            var titles = new System.Collections.Generic.List<string>();
            var seen = new System.Collections.Generic.HashSet<string>();
            if (_save.Achievements != null) {
                foreach (var a in _save.Achievements) {
                    if (a == null) {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(a.Title)) {
                        continue;
                    }

                    if (seen.Add(a.Title)) {
                        titles.Add(a.Title);
                    }
                }
            }

            if (titles.Count == 0) {
                Debug.LogError("当前没有可选成就（存档内尚未建立任何成就），无法创建任务");
                return;
            }

            achievementDropdown.ClearOptions();
            achievementDropdown.AddOptions(titles);

            int defaultIdx = titles.IndexOf("打工效率部部长");
            achievementDropdown.value = defaultIdx >= 0 ? defaultIdx : 0;
            achievementDropdown.RefreshShownValue();

            Button createDoneBtn = Cond.Instance.Get<Button>(instance, "创建完成");
            ButtonRegister.RemoveAllListener(createDoneBtn);
            ButtonRegister.AddListener(createDoneBtn, () => {
                string taskTitle = GetInputOrText(instance, "任务名").Trim();
                string taskContent = GetInputOrText(instance, "任务内容").Trim();
                string rewardText = GetInputOrText(instance, "任务奖励交互点").Trim();

                string achievementTitle = string.Empty;
                if (achievementDropdown != null && achievementDropdown.options != null &&
                    achievementDropdown.options.Count > 0) {
                    int idx = achievementDropdown.value;
                    if (idx < 0 || idx >= achievementDropdown.options.Count) {
                        idx = 0;
                    }

                    achievementTitle = achievementDropdown.options[idx].text?.Trim();
                }

                if (string.IsNullOrWhiteSpace(achievementTitle)) {
                    Debug.LogError("必须选择一个成就");
                    return;
                }

                if (string.IsNullOrWhiteSpace(taskTitle)) {
                    Debug.LogError("任务名不能为空");
                    return;
                }

                if (!int.TryParse(rewardText, out int reward) || reward < 0) {
                    Debug.LogError("任务奖励交互点必须是大于等于0的整数");
                    return;
                }

                AddTaskToSave(achievementTitle, taskTitle, taskContent, reward);
                SwitchToTaskBrowseButtons();
                RefreshAllTaskInfo();
            });
        }

        private void ShowCreateAchievementForm() {
            Flo.Instance.GetFlow(out Flow_SceneA flow);
            Comp interfaceComp = Cond.Instance.Get<Comp>(flow.GetUI(), "界面");
            Comp detailComp = Cond.Instance.Get<Comp>(interfaceComp, "详情");

            Comp achievementPage = Cond.Instance.Get<Comp>(detailComp, "成就");
            achievementPage.gameObject.SetActive(true);

            Transform achievementParent = Cond.Instance.Get<Transform>(achievementPage, "父物体");
            ClearChildren(achievementParent);

            Comp templateRoot = Cond.Instance.Get<Comp>(interfaceComp, "模板");

            // 优先用添加成就模板，没有则参考添加任务模板（共用同一套表单项名：任务名/任务内容/任务奖励交互点）
            Comp addAchievementTemplate = Cond.Instance.Get<Comp>(templateRoot, "添加成就模板");

            Comp instance = GameObject.Instantiate(addAchievementTemplate, achievementParent);
            instance.gameObject.SetActive(true);

            Button createDoneBtn = Cond.Instance.Get<Button>(instance, "创建完成");

            ButtonRegister.RemoveAllListener(createDoneBtn);
            ButtonRegister.AddListener(createDoneBtn, () => {
                // 支持成就模板字段名或任务模板字段名
                string title = GetInputOrText(instance, "成就名").Trim();
                string content = GetInputOrText(instance, "成就内容").Trim();
                string rewardText = GetInputOrText(instance, "成就奖励交互点").Trim();
                title = title?.Trim() ?? string.Empty;
                content = content?.Trim() ?? string.Empty;
                rewardText = rewardText?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(title)) {
                    Debug.LogError("成就名不能为空");
                    return;
                }

                if (!int.TryParse(rewardText, out int reward) || reward < 0) {
                    Debug.LogError("成就奖励交互点必须是大于等于0的整数");
                    return;
                }

                _save ??= PlayerSaveStore.LoadOrCreate();
                if (_save.Achievements == null) {
                    _save.Achievements = new System.Collections.Generic.List<PlayerAchievementV1>();
                }
                _save.Achievements.Add(new PlayerAchievementV1 {
                    Id = Guid.NewGuid().ToString("N"),
                    Title = title,
                    Content = content ?? string.Empty,
                    IsFinished = false,
                    RewardInteractPoint = reward,
                    Tasks = new System.Collections.Generic.List<PlayerTaskV1>()
                });
                PlayerSaveStore.Save(_save);

                SwitchToTaskBrowseButtons();
                RefreshAllAchievementInfo();
            });
        }

        private void SwitchToTaskBrowseButtons() {
            _createBtn.gameObject.SetActive(true);
            _cancelCreateBtn.gameObject.SetActive(false);

            UpdateCreateButtonsForContext();
        }

        private void UpdateCreateButtonsForContext() {
            if (_createText == null || _cancelCreateText == null) {
                return;
            }

            switch (_createContext) {
                case "成就":
                    _createText.text = "创建成就";
                    _cancelCreateText.text = "取消创建成就";
                    break;
                case "商店":
                    _createText.text = "创建商品";
                    _cancelCreateText.text = "取消创建商品";
                    break;
                case "任务":
                default:
                    _createText.text = "创建任务";
                    _cancelCreateText.text = "取消创建任务";
                    break;
            }
        }

        private void AddTaskToSave(
            string achievementTitle,
            string taskTitle,
            string taskContent,
            int rewardInteractPoint
        ) {
            _save ??= PlayerSaveStore.LoadOrCreate();

            PlayerAchievementV1 targetAchievement = null;
            foreach (var a in _save.Achievements) {
                if (a != null && a.Title == achievementTitle) {
                    targetAchievement = a;
                    break;
                }
            }

            if (targetAchievement == null) {
                targetAchievement = new PlayerAchievementV1 {
                    Id = Guid.NewGuid().ToString("N"),
                    Title = achievementTitle,
                    Content = "任务预设归属（新添加的任务将先放在此成就名下）",
                    IsFinished = false,
                    RewardInteractPoint = 0,
                    Tasks = new System.Collections.Generic.List<PlayerTaskV1>()
                };
                _save.Achievements.Add(targetAchievement);
            }

            if (targetAchievement.Tasks == null) {
                targetAchievement.Tasks = new System.Collections.Generic.List<PlayerTaskV1>();
            }

            targetAchievement.Tasks.Add(new PlayerTaskV1 {
                Id = Guid.NewGuid().ToString("N"),
                Title = taskTitle,
                Content = taskContent,
                RewardInteractPoint = rewardInteractPoint,
                IsFinished = false
            });

            PlayerSaveStore.Save(_save);
        }

        #endregion

        public override void DelayedExecute() {
        }

        public override void Clear() {
            base.Clear();
        }
    }
}