using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LazyPan {
    /// <summary>
    /// 负责任务列表、任务完成、创建任务表单的 UI 模块。
    /// 只处理 UI 展示与交互，不负责场景导航。
    /// </summary>
    public class SceneATaskModule {
        private PlayerSaveV1 _save;

        private Comp _interfaceComp;
        private Comp _detailComp;

        private Comp _taskPage;
        private Transform _taskParent;
        private Comp _templateRoot;
        private Comp _taskTemplate;
        private Comp _addTaskTemplate;

        /// <summary>
        /// 使用指定存档数据创建任务 UI 模块。
        /// </summary>
        /// <param name="save">玩家存档数据引用。</param>
        public SceneATaskModule(PlayerSaveV1 save) {
            _save = save;
        }

        /// <summary>
        /// 对任务 UI 模块进行初始化，仅进行一次 Comp 结点缓存。
        /// </summary>
        /// <param name="interfaceComp">场景界面根 Comp，例如“界面”。</param>
        /// <param name="detailComp">详情区域根 Comp，例如“详情”。</param>
        public void Init(Comp interfaceComp, Comp detailComp) {
            _interfaceComp = interfaceComp;
            _detailComp = detailComp;

            _taskPage = Cond.Instance.Get<Comp>(_detailComp, "任务");
            _taskParent = Cond.Instance.Get<Transform>(_taskPage, "父物体");
            _templateRoot = Cond.Instance.Get<Comp>(_interfaceComp, "模板");
            _taskTemplate = Cond.Instance.Get<Comp>(_templateRoot, "任务模板");
            _addTaskTemplate = Cond.Instance.Get<Comp>(_templateRoot, "添加任务模板");
        }

        /// <summary>
        /// 打开任务页并根据当前存档刷新所有任务条目。
        /// </summary>
        public void RefreshAllTasks() {
            _save ??= PlayerSaveStore.LoadOrCreate();

            if (_taskPage == null || _taskParent == null || _taskTemplate == null) {
                return;
            }

            _taskPage.gameObject.SetActive(true);
            UICompUtil.ClearChildren(_taskParent);

            if (_save.Achievements == null) {
                return;
            }

            foreach (var tmpAchievement in _save.Achievements) {
                if (tmpAchievement?.Tasks == null) {
                    continue;
                }

                foreach (var tmpTask in tmpAchievement.Tasks) {
                    RefreshTaskItem(tmpAchievement, tmpTask);
                }
            }
        }

        /// <summary>
        /// 仅显示某个成就下的任务列表。
        /// </summary>
        /// <param name="achievement">要展示任务的成就对象。</param>
        public void RefreshTasksOfAchievement(PlayerAchievementV1 achievement) {
            _save ??= PlayerSaveStore.LoadOrCreate();

            if (_taskPage == null || _taskParent == null || _taskTemplate == null || achievement == null) {
                return;
            }

            _taskPage.gameObject.SetActive(true);
            UICompUtil.ClearChildren(_taskParent);

            if (achievement.Tasks == null) {
                return;
            }

            foreach (var tmpTask in achievement.Tasks) {
                RefreshTaskItem(achievement, tmpTask);
            }
        }

        /// <summary>
        /// 显示“创建任务”表单并处理表单提交逻辑。
        /// </summary>
        public void ShowCreateTaskForm() {
            _save ??= PlayerSaveStore.LoadOrCreate();

            if (_taskPage == null || _taskParent == null || _templateRoot == null || _addTaskTemplate == null) {
                return;
            }

            _taskPage.gameObject.SetActive(true);
            UICompUtil.ClearChildren(_taskParent);

            Comp instance = GameObject.Instantiate(_addTaskTemplate, _taskParent);
            instance.gameObject.SetActive(true);

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

            var titles = new List<string>();
            var seen = new HashSet<string>();
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
                string taskTitle = UICompUtil.GetInputOrText(instance, "任务名").Trim();
                string taskContent = UICompUtil.GetInputOrText(instance, "任务内容").Trim();
                string rewardText = UICompUtil.GetInputOrText(instance, "任务奖励交互点").Trim();

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
                BackToList();
            });
        }

        /// <summary>
        /// 从“创建任务状态”返回到任务浏览列表。
        /// </summary>
        public void BackToList() {
            RefreshAllTasks();
        }

        /// <summary>
        /// 使用任务模板实例化并渲染单个任务条目。
        /// </summary>
        /// <param name="tmpAchievement">任务所属成就。</param>
        /// <param name="tmpTask">要展示的任务数据。</param>
        private void RefreshTaskItem(PlayerAchievementV1 tmpAchievement, PlayerTaskV1 tmpTask) {
            if (_taskTemplate == null || _taskParent == null || tmpTask == null) {
                return;
            }

            if (tmpTask.IsFinished) {
                return;
            }

            Comp taskInstance = GameObject.Instantiate(_taskTemplate, _taskParent);
            taskInstance.gameObject.SetActive(true);

            TextMeshProUGUI taskTile = Cond.Instance.Get<TextMeshProUGUI>(taskInstance, "任务名");
            taskTile.text = tmpTask.Title;

            TextMeshProUGUI taskContent = Cond.Instance.Get<TextMeshProUGUI>(taskInstance, "任务内容");
            taskContent.text = tmpTask.Content;

            try {
                TextMeshProUGUI taskBelongAchievement =
                    Cond.Instance.Get<TextMeshProUGUI>(taskInstance, "任务所属成就");
                taskBelongAchievement.text = tmpAchievement?.Title ?? string.Empty;
            } catch (Exception) {
                // 可选显示：如果模板里没这个文案，则忽略
            }

            TextMeshProUGUI taskReward = Cond.Instance.Get<TextMeshProUGUI>(taskInstance, "任务奖励交互点");
            taskReward.text = string.Concat("任务奖励点:", tmpTask.RewardInteractPoint);

            Button taskFinishBtn = Cond.Instance.Get<Button>(taskInstance, "任务完成");
            ButtonRegister.RemoveAllListener(taskFinishBtn);
            ButtonRegister.AddListener(taskFinishBtn, () => {
                tmpTask.IsFinished = true;
                PlayerSaveStore.Save(_save);
            });
        }

        /// <summary>
        /// 将新任务数据写入存档中指定成就下，并立即保存存档。
        /// 如目标成就不存在则自动创建一个占位成就。
        /// </summary>
        private void AddTaskToSave(
            string achievementTitle,
            string taskTitle,
            string taskContent,
            int rewardInteractPoint
        ) {
            _save ??= PlayerSaveStore.LoadOrCreate();

            if (_save.Achievements == null) {
                _save.Achievements = new List<PlayerAchievementV1>();
            }

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
                    Tasks = new List<PlayerTaskV1>()
                };
                _save.Achievements.Add(targetAchievement);
            }

            if (targetAchievement.Tasks == null) {
                targetAchievement.Tasks = new List<PlayerTaskV1>();
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
    }
}

