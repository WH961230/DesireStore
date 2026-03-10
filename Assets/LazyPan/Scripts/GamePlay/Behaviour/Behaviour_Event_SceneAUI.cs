using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LazyPan {
    public class Behaviour_Event_SceneAUI : Behaviour {
        private Button _createTaskBtn;
        private Button _cancelCreateTaskBtn;

        public Behaviour_Event_SceneAUI(Entity entity, string behaviourSign) : base(entity, behaviourSign) {
            InitUI();
        }

        private void InitUI() {
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

            // 获取各个按钮
            Button meBtn = Cond.Instance.Get<Button>(moduleEntrance, "我");
            Button taskManagementBtn = Cond.Instance.Get<Button>(moduleEntrance, "任务");

            // 获取状态栏组件
            Comp statusBar = Cond.Instance.Get<Comp>(interfaceComp, "状态栏");
            // 获取标题文本
            TextMeshProUGUI titleText = Cond.Instance.Get<TextMeshProUGUI>(statusBar, "标题");
            // 获取返回主页按钮
            Button backToHomeBtn = Cond.Instance.Get<Button>(statusBar, "返回主页");
            // 任务页相关按钮（可能在部分UI版本中不存在）
            _createTaskBtn = TryGetButton(statusBar, "创建任务");
            _cancelCreateTaskBtn = TryGetButton(statusBar, "取消创建任务");

            if (_createTaskBtn != null) {
                _createTaskBtn.gameObject.SetActive(false);
            }

            if (_cancelCreateTaskBtn != null) {
                _cancelCreateTaskBtn.gameObject.SetActive(false);
            }

            // 注册"我"按钮点击事件
            ButtonRegister.RemoveAllListener(meBtn);
            ButtonRegister.AddListener(meBtn, () => {
                homePage.gameObject.SetActive(false);
                meModule.gameObject.SetActive(true);
                taskManagementModule.gameObject.SetActive(false);
                // 更新标题为当前页面中文名称
                titleText.text = "我";
                backToHomeBtn.gameObject.SetActive(true);

                if (_createTaskBtn != null) {
                    _createTaskBtn.gameObject.SetActive(false);
                }

                if (_cancelCreateTaskBtn != null) {
                    _cancelCreateTaskBtn.gameObject.SetActive(false);
                }
            });

            // 注册"任务"按钮点击事件
            ButtonRegister.RemoveAllListener(taskManagementBtn);
            ButtonRegister.AddListener(taskManagementBtn, () => {
                homePage.gameObject.SetActive(false);
                meModule.gameObject.SetActive(false);
                taskManagementModule.gameObject.SetActive(true);
                // 更新标题为当前页面中文名称
                titleText.text = "任务";
                backToHomeBtn.gameObject.SetActive(true);
                RefreshAllTaskInfo();

                if (_createTaskBtn != null) {
                    _createTaskBtn.gameObject.SetActive(true);
                }

                if (_cancelCreateTaskBtn != null) {
                    _cancelCreateTaskBtn.gameObject.SetActive(false);
                }
            });
            
            // 状态栏：创建任务（进入创建表单态）
            if (_createTaskBtn != null) {
                ButtonRegister.RemoveAllListener(_createTaskBtn);
                ButtonRegister.AddListener(_createTaskBtn, () => {
                    if (_createTaskBtn != null) {
                        _createTaskBtn.gameObject.SetActive(false);
                    }

                    if (_cancelCreateTaskBtn != null) {
                        _cancelCreateTaskBtn.gameObject.SetActive(true);
                    }

                    ShowCreateTaskForm();
                });
            }

            // 状态栏：取消创建任务（回到任务列表态）
            if (_cancelCreateTaskBtn != null) {
                ButtonRegister.RemoveAllListener(_cancelCreateTaskBtn);
                ButtonRegister.AddListener(_cancelCreateTaskBtn, () => {
                    SwitchToTaskBrowseButtons();
                    RefreshAllTaskInfo();
                });
            }

            ButtonRegister.RemoveAllListener(backToHomeBtn);
            ButtonRegister.AddListener(backToHomeBtn, () => {
                homePage.gameObject.SetActive(true);
                meModule.gameObject.SetActive(false);
                taskManagementModule.gameObject.SetActive(false);
                // 返回主页时标题显示“修行手册”
                titleText.text = "修行手册";
                backToHomeBtn.gameObject.SetActive(false);

                if (_createTaskBtn != null) {
                    _createTaskBtn.gameObject.SetActive(false);
                }

                if (_cancelCreateTaskBtn != null) {
                    _cancelCreateTaskBtn.gameObject.SetActive(false);
                }
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
            // 从指定路径加载用户设置资源
            MySetting mySetting = Loader.LoadAsset<MySetting>(AssetType.ASSET, "Setting/MySetting");

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
            TextMeshProUGUI briefInteractPoint = Cond.Instance.Get<TextMeshProUGUI>(userBrief, "交互点");
            briefInteractPoint.text = $"交互点:{GetAllInteractPoint()}";

            // // 获取当前日期并格式化为中文格式
            // string chineseDate = DateTime.Now.ToString("yyyy年MM月dd日"); // 2024年01月15日
            // // 获取日期文本组件并更新内容
            // TextMeshProUGUI date = Cond.Instance.Get<TextMeshProUGUI>(userInfo, "日期");
            // date.text = chineseDate;
        }

        #endregion

        #region 成就信息

        /// <summary>
        /// 刷新所有成就信息的方法
        /// </summary>
        private void RefreshAllAchievementInfo() {
            // 加载成就设置资源
            AchievementSetting achievementSetting =
                Loader.LoadAsset<AchievementSetting>(AssetType.ASSET, "Setting/AchievementSetting");

            // 获取场景流程组件
            Flo.Instance.GetFlow(out Flow_SceneA flow);

            // 获取界面组件
            Comp interfaceComp = Cond.Instance.Get<Comp>(flow.GetUI(), "界面");

            // 获取成就信息组件并激活
            Comp achievementInfo = Cond.Instance.Get<Comp>(interfaceComp, "成就信息");
            achievementInfo.gameObject.SetActive(true);

            // 获取成就列表的父级Transform
            Transform achievementParent = Cond.Instance.Get<Transform>(achievementInfo, "父物体");

            // 清空所有现有的成就子物体
            foreach (Transform tmp in achievementParent) {
                GameObject.Destroy(tmp.gameObject);
            }

            // 遍历所有成就数据并刷新显示
            foreach (var tmpAchievement in achievementSetting.AchievementDatas) {
                RefreshAchievementInfo(tmpAchievement);
            }
        }

        /// <summary>
        /// 刷新成就信息的方法
        /// </summary>
        /// <param name="tmpAchievement">传入的成就数据对象</param>
        private void RefreshAchievementInfo(AchievementData tmpAchievement) {
            // 关闭所有已打开的成就信息
            CloseAllInfo();

            // 获取Flow场景实例中的UI界面组件
            Flo.Instance.GetFlow(out Flow_SceneA flow);
            Comp interfaceComp = Cond.Instance.Get<Comp>(flow.GetUI(), "界面");

            // 获取并激活成就信息界面组件
            Comp achievementInfo = Cond.Instance.Get<Comp>(interfaceComp, "成就信息");
            achievementInfo.gameObject.SetActive(true);

            // 获取成就信息的父物体和模板组件
            Transform achievementParent = Cond.Instance.Get<Transform>(achievementInfo, "父物体");
            Comp achievementTemplate = Cond.Instance.Get<Comp>(achievementInfo, "成就模板");

            // 如果成就已完成，则直接返回
            if (tmpAchievement.IsAchievementFinished) {
                return;
            }

            // 实例化成就模板并激活
            Comp achievementInstance = GameObject.Instantiate(achievementTemplate, achievementParent);
            achievementInstance.gameObject.SetActive(true);

            // 设置成就名称、内容和奖励文本
            TextMeshProUGUI achievementTile = Cond.Instance.Get<TextMeshProUGUI>(achievementInstance, "成就名");
            achievementTile.text = tmpAchievement.AchievementTitle;

            TextMeshProUGUI achievementContent = Cond.Instance.Get<TextMeshProUGUI>(achievementInstance, "成就内容");
            achievementContent.text = tmpAchievement.AchievementContent;

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
                tmpAchievement.IsAchievementFinished = true;

#if UNITY_EDITOR
                AchievementSetting achievementSettingForSave =
                    Loader.LoadAsset<AchievementSetting>(AssetType.ASSET, "Setting/AchievementSetting");
                if (achievementSettingForSave != null) {
                    UnityEditor.EditorUtility.SetDirty(achievementSettingForSave);
                    UnityEditor.AssetDatabase.SaveAssets();
                }
#endif

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
            // 加载成就设置资源
            AchievementSetting achievementSetting =
                Loader.LoadAsset<AchievementSetting>(AssetType.ASSET, "Setting/AchievementSetting");

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
            foreach (var tmpAchievement in achievementSetting.AchievementDatas) {
                foreach (var tmpTask in tmpAchievement.TaskDatas) {
                    RefreshTaskInfo(tmpAchievement, tmpTask);
                }
            }
        }

        /// <summary>
        /// 刷新任务信息界面
        /// </summary>
        /// <param name="tmpAchievement">成就数据对象</param>
        private void RefreshTaskInfo(AchievementData tmpAchievement) {
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
            foreach (var tmpTask in tmpAchievement.TaskDatas) {
                RefreshTaskInfo(tmpAchievement, tmpTask, taskTemplate, taskParent);
            }
        }

        /// <summary>
        /// 刷新任务信息的显示
        /// </summary>
        /// <param name="tmpAchievement">成就数据对象</param>
        /// <param name="tmpTask">任务数据对象</param>
        private void RefreshTaskInfo(AchievementData tmpAchievement, TaskData tmpTask) {
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
            AchievementData tmpAchievement,
            TaskData tmpTask,
            Comp taskTemplate,
            Transform taskParent
        ) {
            // 如果任务已完成，则直接返回
            if (tmpTask.IsTaskFinished) {
                return;
            }

            // 实例化任务模板并激活
            Comp taskInstance = GameObject.Instantiate(taskTemplate, taskParent);
            taskInstance.gameObject.SetActive(true);

            // 设置任务名称文本
            TextMeshProUGUI taskTile = Cond.Instance.Get<TextMeshProUGUI>(taskInstance, "任务名");
            taskTile.text = tmpTask.TaskTitle;

            // 设置任务内容文本
            TextMeshProUGUI taskContent = Cond.Instance.Get<TextMeshProUGUI>(taskInstance, "任务内容");
            taskContent.text = tmpTask.TaskContent;

            // 设置任务所属成就文本（如果有配置）
            try {
                TextMeshProUGUI taskBelongAchievement =
                    Cond.Instance.Get<TextMeshProUGUI>(taskInstance, "任务所属成就");
                taskBelongAchievement.text = tmpAchievement?.AchievementTitle ?? string.Empty;
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
                tmpTask.IsTaskFinished = true;

#if UNITY_EDITOR
                AchievementSetting achievementSettingForSave =
                    Loader.LoadAsset<AchievementSetting>(AssetType.ASSET, "Setting/AchievementSetting");
                if (achievementSettingForSave != null) {
                    UnityEditor.EditorUtility.SetDirty(achievementSettingForSave);
                    UnityEditor.AssetDatabase.SaveAssets();
                }
#endif

                RefreshUserInfo();
                RefreshAllTaskInfo();
            });
        }

        /// <summary>
        /// 获取所有交互点的总和
        /// </summary>
        /// <returns>返回总交互点数</returns>
        private int GetAllInteractPoint() {
            // 从指定路径加载游戏设置
            MySetting mySetting = Loader.LoadAsset<MySetting>(AssetType.ASSET, "Setting/MySetting");
            // 从指定路径加载成就设置
            AchievementSetting achievementSetting =
                Loader.LoadAsset<AchievementSetting>(AssetType.ASSET, "Setting/AchievementSetting");

            // 初始化任务交互点数为0
            int taskInteractPoint = 0;
            // 遍历所有成就数据
            foreach (var tmpAchievement in achievementSetting.AchievementDatas) {
                // 如果成就已完成，则添加其奖励交互点
                if (tmpAchievement.IsAchievementFinished) {
                    taskInteractPoint += tmpAchievement.RewardInteractPoint;
                }

                // 遍历当前成就下的所有任务数据
                foreach (var tmpTask in tmpAchievement.TaskDatas) {
                    // 如果任务已完成，则添加其奖励交互点
                    if (tmpTask.IsTaskFinished) {
                        taskInteractPoint += tmpTask.RewardInteractPoint;
                    }
                }
            }

            // 返回基础交互点与任务交互点的总和
            return mySetting.InteractPoint + taskInteractPoint;
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

        private Button TryGetButton(Comp parent, string sign) {
            try {
                return Cond.Instance.Get<Button>(parent, sign);
            } catch (Exception) {
                return null;
            }
        }

        private TMP_InputField TryGetTMPInputField(Comp parent, string sign) {
            try {
                return Cond.Instance.Get<TMP_InputField>(parent, sign);
            } catch (Exception) {
                return null;
            }
        }

        private TextMeshProUGUI TryGetTMPText(Comp parent, string sign) {
            try {
                return Cond.Instance.Get<TextMeshProUGUI>(parent, sign);
            } catch (Exception) {
                return null;
            }
        }

        private string GetInputOrText(Comp parent, string sign) {
            TMP_InputField input = TryGetTMPInputField(parent, sign);
            if (input != null) {
                return input.text;
            }

            TextMeshProUGUI text = TryGetTMPText(parent, sign);
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
            AchievementSetting achievementSetting =
                Loader.LoadAsset<AchievementSetting>(AssetType.ASSET, "Setting/AchievementSetting");
            var titles = new System.Collections.Generic.List<string>();
            var seen = new System.Collections.Generic.HashSet<string>();
            if (achievementSetting != null && achievementSetting.AchievementDatas != null) {
                foreach (var a in achievementSetting.AchievementDatas) {
                    if (a == null) {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(a.AchievementTitle)) {
                        continue;
                    }

                    if (seen.Add(a.AchievementTitle)) {
                        titles.Add(a.AchievementTitle);
                    }
                }
            }

            if (titles.Count == 0) {
                Debug.LogError("当前没有可选成就（AchievementSetting 为空或未配置成就名），无法创建任务");
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

                AddTaskToConfig(achievementTitle, taskTitle, taskContent, reward);
                SwitchToTaskBrowseButtons();
                RefreshAllTaskInfo();
            });
        }

        private void SwitchToTaskBrowseButtons() {
            if (_createTaskBtn != null) {
                _createTaskBtn.gameObject.SetActive(true);
            }

            if (_cancelCreateTaskBtn != null) {
                _cancelCreateTaskBtn.gameObject.SetActive(false);
            }
        }

        private void AddTaskToConfig(
            string achievementTitle,
            string taskTitle,
            string taskContent,
            int rewardInteractPoint
        ) {
            AchievementSetting achievementSetting =
                Loader.LoadAsset<AchievementSetting>(AssetType.ASSET, "Setting/AchievementSetting");

            if (achievementSetting.AchievementDatas == null) {
                achievementSetting.AchievementDatas = new System.Collections.Generic.List<AchievementData>();
            }

            AchievementData targetAchievement = null;
            foreach (var a in achievementSetting.AchievementDatas) {
                if (a != null && a.AchievementTitle == achievementTitle) {
                    targetAchievement = a;
                    break;
                }
            }

            if (targetAchievement == null) {
                targetAchievement = new AchievementData {
                    AchievementTitle = achievementTitle,
                    AchievementContent = "任務預設歸屬（新添加的任務將先放在此成就名下）",
                    IsAchievementFinished = false,
                    RewardInteractPoint = 0
                };
                achievementSetting.AchievementDatas.Add(targetAchievement);
            }

            if (targetAchievement.TaskDatas == null) {
                targetAchievement.TaskDatas = new System.Collections.Generic.List<TaskData>();
            }

            targetAchievement.TaskDatas.Add(new TaskData {
                TaskTitle = taskTitle,
                TaskContent = taskContent,
                RewardInteractPoint = rewardInteractPoint,
                IsTaskFinished = false
            });

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(achievementSetting);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }

        #endregion

        public override void DelayedExecute() {
        }

        public override void Clear() {
            base.Clear();
        }
    }
}