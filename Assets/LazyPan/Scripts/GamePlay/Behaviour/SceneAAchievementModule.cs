using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LazyPan {
    /// <summary>
    /// 负责成就列表、成就完成、创建成就表单的 UI 模块。
    /// 只处理成就相关 UI 展示与交互，不负责场景导航。
    /// </summary>
    public class SceneAAchievementModule {
        private PlayerSaveV1 _save;

        private Comp _interfaceComp;
        private Comp _detailComp;

        private Comp _achievementPage;
        private Transform _achievementParent;
        private Comp _templateRoot;
        private Comp _achievementTemplate;
        private Comp _addAchievementTemplate;

        // 当点击“成就所有任务”按钮时回调，由外部总控决定如何显示任务
        private readonly Action<PlayerAchievementV1> _onShowTasks;

        /// <summary>
        /// 使用指定存档数据和任务展示回调创建成就 UI 模块。
        /// </summary>
        /// <param name="save">玩家存档数据引用。</param>
        /// <param name="onShowTasks">当点击“成就所有任务”按钮时要触发的回调。</param>
        public SceneAAchievementModule(PlayerSaveV1 save, Action<PlayerAchievementV1> onShowTasks) {
            _save = save;
            _onShowTasks = onShowTasks;
        }

        /// <summary>
        /// 对成就 UI 模块进行初始化，仅进行一次 Comp 结点缓存。
        /// </summary>
        /// <param name="interfaceComp">场景界面根 Comp，例如“界面”。</param>
        /// <param name="detailComp">详情区域根 Comp，例如“详情”。</param>
        public void Init(Comp interfaceComp, Comp detailComp) {
            _interfaceComp = interfaceComp;
            _detailComp = detailComp;

            _achievementPage = Cond.Instance.Get<Comp>(_detailComp, "成就");
            _achievementParent = Cond.Instance.Get<Transform>(_achievementPage, "父物体");
            _templateRoot = Cond.Instance.Get<Comp>(_interfaceComp, "模板");
            _achievementTemplate = Cond.Instance.Get<Comp>(_templateRoot, "成就模板");
            _addAchievementTemplate = Cond.Instance.Get<Comp>(_templateRoot, "添加成就模板");
        }

        /// <summary>
        /// 打开成就页并根据当前存档刷新所有成就条目。
        /// </summary>
        public void RefreshAllAchievements() {
            _save ??= PlayerSaveStore.LoadOrCreate();

            if (_achievementPage == null) {
                return;
            }

            _achievementPage.gameObject.SetActive(true);

            if (_achievementParent == null) {
                return;
            }

            UICompUtil.ClearChildren(_achievementParent);

            if (_save.Achievements == null) {
                return;
            }

            foreach (var tmpAchievement in _save.Achievements) {
                if (tmpAchievement != null) {
                    RefreshAchievementInfo(tmpAchievement);
                }
            }
        }

        /// <summary>
        /// 从“创建成就状态”返回成就列表浏览。
        /// </summary>
        public void BackToList() {
            RefreshAllAchievements();
        }

        /// <summary>
        /// 显示“创建成就”表单并处理表单提交逻辑。
        /// </summary>
        public void ShowCreateAchievementForm() {
            _save ??= PlayerSaveStore.LoadOrCreate();

            if (_achievementPage == null || _achievementParent == null || _templateRoot == null || _addAchievementTemplate == null) {
                return;
            }

            _achievementPage.gameObject.SetActive(true);
            UICompUtil.ClearChildren(_achievementParent);

            Comp instance = GameObject.Instantiate(_addAchievementTemplate, _achievementParent);
            instance.gameObject.SetActive(true);

            Button createDoneBtn = Cond.Instance.Get<Button>(instance, "创建完成");

            ButtonRegister.RemoveAllListener(createDoneBtn);
            ButtonRegister.AddListener(createDoneBtn, () => {
                // 支持成就模板字段名或任务模板字段名
                string title = UICompUtil.GetInputOrText(instance, "成就名").Trim();
                string content = UICompUtil.GetInputOrText(instance, "成就内容").Trim();
                string rewardText = UICompUtil.GetInputOrText(instance, "成就奖励交互点").Trim();
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

                BackToList();
            });
        }

        /// <summary>
        /// 使用成就模板实例化并渲染单个未完成成就条目。
        /// </summary>
        /// <param name="tmpAchievement">要展示的成就数据。</param>
        private void RefreshAchievementInfo(PlayerAchievementV1 tmpAchievement) {
            if (_interfaceComp == null || _achievementPage == null || _achievementTemplate == null || _achievementParent == null) {
                return;
            }

            // 已完成的成就不再重复展示创建中的 UI
            if (tmpAchievement.IsFinished) {
                return;
            }

            // 关闭其它信息面板（如果有）
            CloseAllInfo();

            _achievementPage.gameObject.SetActive(true);

            Comp achievementInstance = GameObject.Instantiate(_achievementTemplate, _achievementParent);
            achievementInstance.gameObject.SetActive(true);

            TextMeshProUGUI achievementTile = Cond.Instance.Get<TextMeshProUGUI>(achievementInstance, "成就名");
            achievementTile.text = tmpAchievement.Title;

            TextMeshProUGUI achievementContent = Cond.Instance.Get<TextMeshProUGUI>(achievementInstance, "成就内容");
            achievementContent.text = tmpAchievement.Content;

            TextMeshProUGUI achievementReward = Cond.Instance.Get<TextMeshProUGUI>(achievementInstance, "成就奖励交互点");
            achievementReward.text = string.Concat("成就奖励点:", tmpAchievement.RewardInteractPoint);

            Button achievementTasksBtn = Cond.Instance.Get<Button>(achievementInstance, "成就所有任务");
            ButtonRegister.RemoveAllListener(achievementTasksBtn);
            ButtonRegister.AddListener(achievementTasksBtn, () => { _onShowTasks?.Invoke(tmpAchievement); });

            Button achievementFinishBtn = Cond.Instance.Get<Button>(achievementInstance, "完成");
            ButtonRegister.RemoveAllListener(achievementFinishBtn);
            ButtonRegister.AddListener(achievementFinishBtn, () => {
                tmpAchievement.IsFinished = true;
                PlayerSaveStore.Save(_save);
            });
        }

        /// <summary>
        /// 关闭同一界面下的“任务信息”和“成就信息”面板（如果存在）。
        /// </summary>
        private void CloseAllInfo() {
            if (_interfaceComp == null) {
                return;
            }

            try {
                Comp info = Cond.Instance.Get<Comp>(_interfaceComp, "任务信息");
                info.gameObject.SetActive(false);
            } catch (Exception) {
                // ignore
            }

            try {
                Comp info = Cond.Instance.Get<Comp>(_interfaceComp, "成就信息");
                info.gameObject.SetActive(false);
            } catch (Exception) {
                // ignore
            }
        }

        // 其余通用 UI 操作已统一抽到 UICompUtil 中
    }
}

