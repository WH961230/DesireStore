using TMPro;
using UnityEngine.UI;

namespace LazyPan {
    /// <summary>
    /// 场景 A UI 的事件行为入口，负责主页/我/任务/成就模块的导航和状态栏按钮。
    /// 具体业务与渲染逻辑委托给各自的模块类处理。
    /// </summary>
    public class Behaviour_Event_SceneAUI : Behaviour {
        private Button _createBtn;
        private Button _cancelCreateBtn;
        private TextMeshProUGUI _createText;
        private TextMeshProUGUI _cancelCreateText;
        private string _createContext;
        private PlayerSaveV1 _save;

        private SceneAProfileModule _profileModule;
        private SceneAAchievementModule _achievementModule;
        private SceneATaskModule _taskModule;

        /// <summary>
        /// 构造场景 A UI 行为实例，并立即初始化 UI 与模块。
        /// </summary>
        /// <param name="entity">所属实体。</param>
        /// <param name="behaviourSign">行为标识。</param>
        public Behaviour_Event_SceneAUI(Entity entity, string behaviourSign) : base(entity, behaviourSign) {
            InitUI();
        }

        /// <summary>
        /// 初始化场景 A 的 UI 结构、模块实例和顶层按钮事件绑定。
        /// </summary>
        private void InitUI() {
            _save = PlayerSaveStore.LoadOrCreate();

            InitializeFlowAndModules(
                out Comp interfaceComp,
                out Comp detailComp,
                out Comp homePage,
                out Comp moduleEntrance,
                out Comp meModule,
                out Comp taskManagementModule,
                out Comp achievementModule
            );

            InitializeStatusBarAndTopButtons(
                interfaceComp,
                homePage,
                moduleEntrance,
                meModule,
                taskManagementModule,
                achievementModule
            );
        }

        /// <summary>
        /// 获取场景 A 的基础 UI 结点，并初始化“我/任务/成就”模块。
        /// </summary>
        private void InitializeFlowAndModules(
            out Comp interfaceComp,
            out Comp detailComp,
            out Comp homePage,
            out Comp moduleEntrance,
            out Comp meModule,
            out Comp taskManagementModule,
            out Comp achievementModule
        ) {
            // 使用局部变量避免在 lambda 中直接捕获 out 参数
            Comp localInterfaceComp;
            Comp localDetailComp;
            Comp localHomePage;
            Comp localModuleEntrance;
            Comp localMeModule;
            Comp localTaskManagementModule;
            Comp localAchievementModule;

            // 获取场景A的流程实例
            Flo.Instance.GetFlow(out Flow_SceneA flow);

            // 获取界面与详情组件
            localInterfaceComp = Cond.Instance.Get<Comp>(flow.GetUI(), "界面");
            localDetailComp = Cond.Instance.Get<Comp>(localInterfaceComp, "详情");

            // 获取主页组件并激活
            localHomePage = Cond.Instance.Get<Comp>(localDetailComp, "主页");
            localHomePage.gameObject.SetActive(true);

            // 获取模块入口组件
            localModuleEntrance = Cond.Instance.Get<Comp>(localHomePage, "模块入口");

            // 获取各个模块组件
            localMeModule = Cond.Instance.Get<Comp>(localDetailComp, "我");
            localTaskManagementModule = Cond.Instance.Get<Comp>(localDetailComp, "任务");
            localAchievementModule = Cond.Instance.Get<Comp>(localDetailComp, "成就");

            // 初始化“我”页面模块
            _profileModule = new SceneAProfileModule(_save);
            _profileModule.Init(localMeModule);

            // 初始化任务模块
            _taskModule = new SceneATaskModule(_save);
            _taskModule.Init(localInterfaceComp, localDetailComp);

            // 初始化成就模块，并配置“成就所有任务”按钮的回调
            _achievementModule = new SceneAAchievementModule(_save, achievement => {
                // 切到任务页显示该成就下所有任务
                localHomePage.gameObject.SetActive(false);
                localMeModule.gameObject.SetActive(false);
                localTaskManagementModule.gameObject.SetActive(true);
                localAchievementModule.gameObject.SetActive(false);

                _taskModule.RefreshTasksOfAchievement(achievement);
                _createContext = "任务";
                _createBtn.gameObject.SetActive(true);
                _cancelCreateBtn.gameObject.SetActive(false);
                UpdateCreateButtonsForContext();
            });
            _achievementModule.Init(localInterfaceComp, localDetailComp);

            // 将局部变量赋值给 out 参数
            interfaceComp = localInterfaceComp;
            detailComp = localDetailComp;
            homePage = localHomePage;
            moduleEntrance = localModuleEntrance;
            meModule = localMeModule;
            taskManagementModule = localTaskManagementModule;
            achievementModule = localAchievementModule;
        }

        /// <summary>
        /// 初始化状态栏与顶部导航按钮的事件绑定。
        /// </summary>
        private void InitializeStatusBarAndTopButtons(
            Comp interfaceComp,
            Comp homePage,
            Comp moduleEntrance,
            Comp meModule,
            Comp taskManagementModule,
            Comp achievementModule
        ) {
            // 获取各个按钮
            Button meBtn = Cond.Instance.Get<Button>(moduleEntrance, "我");
            Button taskManagementBtn = Cond.Instance.Get<Button>(moduleEntrance, "任务");
            Button achievementBtn = Cond.Instance.Get<Button>(moduleEntrance, "成就");

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

            BindTopNavButtons(
                meBtn,
                taskManagementBtn,
                achievementBtn,
                homePage,
                meModule,
                taskManagementModule,
                achievementModule,
                titleText,
                backToHomeBtn
            );

            BindCreateButtons(backToHomeBtn, homePage, meModule, taskManagementModule, achievementModule, titleText);
        }

        /// <summary>
        /// 绑定“我/任务/成就/返回主页”四个导航按钮的点击行为。
        /// </summary>
        private void BindTopNavButtons(
            Button meBtn,
            Button taskManagementBtn,
            Button achievementBtn,
            Comp homePage,
            Comp meModule,
            Comp taskManagementModule,
            Comp achievementModule,
            TextMeshProUGUI titleText,
            Button backToHomeBtn
        ) {
            // 注册"我"按钮点击事件
            ButtonRegister.RemoveAllListener(meBtn);
            ButtonRegister.AddListener(meBtn, () => {
                homePage.gameObject.SetActive(false);
                taskManagementModule.gameObject.SetActive(false);
                achievementModule.gameObject.SetActive(false);
                meModule.gameObject.SetActive(true);

                // 更新标题为当前页面中文名称
                titleText.text = "我";
                backToHomeBtn.gameObject.SetActive(true);

                _profileModule.Refresh();

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
                taskManagementModule.gameObject.SetActive(true);

                // 更新标题为当前页面中文名称
                titleText.text = "任务";
                backToHomeBtn.gameObject.SetActive(true);
                _taskModule.RefreshAllTasks();
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
                    achievementModule.gameObject.SetActive(true);
                    // 更新标题为当前页面中文名称
                    titleText.text = "成就";
                    backToHomeBtn.gameObject.SetActive(true);
                    _achievementModule.RefreshAllAchievements();
                    _createContext = "成就";

                    _createBtn.gameObject.SetActive(true);
                    _cancelCreateBtn.gameObject.SetActive(false);

                    UpdateCreateButtonsForContext();
                });
            }
        }

        /// <summary>
        /// 绑定状态栏中的“创建/取消创建/返回主页”按钮行为，并设置初始标题与可见性。
        /// </summary>
        private void BindCreateButtons(
            Button backToHomeBtn,
            Comp homePage,
            Comp meModule,
            Comp taskManagementModule,
            Comp achievementModule,
            TextMeshProUGUI titleText
        ) {
            if (_createBtn != null) {
                ButtonRegister.RemoveAllListener(_createBtn);
                ButtonRegister.AddListener(_createBtn, () => {
                    _createBtn.gameObject.SetActive(false);
                    _cancelCreateBtn.gameObject.SetActive(true);

                    UpdateCreateButtonsForContext();

                    if (_createContext == "成就") {
                        _achievementModule.ShowCreateAchievementForm();
                    } else {
                        _taskModule.ShowCreateTaskForm();
                    }
                });
            }

            // 状态栏：取消创建任务（回到任务列表态）
            if (_cancelCreateBtn != null) {
                ButtonRegister.RemoveAllListener(_cancelCreateBtn);
                ButtonRegister.AddListener(_cancelCreateBtn, () => {
                    SwitchToTaskBrowseButtons();
                    if (_createContext == "成就") {
                        _achievementModule.BackToList();
                    } else {
                        _taskModule.BackToList();
                    }
                });
            }

            ButtonRegister.RemoveAllListener(backToHomeBtn);
            ButtonRegister.AddListener(backToHomeBtn, () => {
                meModule.gameObject.SetActive(false);
                taskManagementModule.gameObject.SetActive(false);
                achievementModule.gameObject.SetActive(false);
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
        /// 刷新主页用户信息区域（头像、名字、总交互点）。
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

        #region 成就信息
        /// <summary>
        /// 计算当前存档下成就与任务获得的总交互点数。
        /// </summary>
        /// <returns>基础交互点与所有已完成任务/成就奖励点的总和。</returns>
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

        #region 通用

        /// <summary>
        /// 切换状态栏按钮为“浏览列表模式”：只显示“创建”按钮。
        /// </summary>
        private void SwitchToTaskBrowseButtons() {
            _createBtn.gameObject.SetActive(true);
            _cancelCreateBtn.gameObject.SetActive(false);

            UpdateCreateButtonsForContext();
        }

        /// <summary>
        /// 根据当前上下文（任务/成就）更新“创建/取消创建”按钮的文案。
        /// </summary>
        private void UpdateCreateButtonsForContext() {
            if (_createText == null || _cancelCreateText == null) {
                return;
            }

            switch (_createContext) {
                case "成就":
                    _createText.text = "创建成就";
                    _cancelCreateText.text = "取消创建成就";
                    break;
                case "任务":
                default:
                    _createText.text = "创建任务";
                    _cancelCreateText.text = "取消创建任务";
                    break;
            }
        }

        #endregion

        /// <summary>
        /// 行为的延迟执行入口，当前未使用。
        /// </summary>
        public override void DelayedExecute() {
        }

        /// <summary>
        /// 清理行为持有的资源，当前只调用基类清理逻辑。
        /// </summary>
        public override void Clear() {
            base.Clear();
        }
    }
}