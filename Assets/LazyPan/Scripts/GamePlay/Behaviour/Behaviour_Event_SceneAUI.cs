using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LazyPan {
    public class Behaviour_Event_SceneAUI : Behaviour {
        public Behaviour_Event_SceneAUI(Entity entity, string behaviourSign) : base(entity, behaviourSign) {
            RefreshUserInfo();//用户信息
            RefreshAllTaskInfo();//任务信息
        }

        #region 用户信息

        private void RefreshUserInfo() {
            MySetting mySetting = Loader.LoadAsset<MySetting>(AssetType.ASSET, "Setting/MySetting");

            Flo.Instance.GetFlow(out Flow_SceneA flow);
            Comp interfaceComp = Cond.Instance.Get<Comp>(flow.GetUI(), "界面");

            Comp userInfo = Cond.Instance.Get<Comp>(interfaceComp, "用户信息");
            userInfo.gameObject.SetActive(true);
            
            TextMeshProUGUI name = Cond.Instance.Get<TextMeshProUGUI>(userInfo, "名字");
            name.text = mySetting.Name;

            TextMeshProUGUI interactPoint = Cond.Instance.Get<TextMeshProUGUI>(userInfo, "交互点");
            interactPoint.text = string.Concat("交互点:" + GetAllInteractPoint());

            string chineseDate = DateTime.Now.ToString("yyyy年MM月dd日"); // 2024年01月15日
            TextMeshProUGUI date = Cond.Instance.Get<TextMeshProUGUI>(userInfo, "日期");
            date.text = chineseDate;
        }

        #endregion

        #region 成就信息
        
        private void RefreshAllAchievementInfo() {
            AchievementSetting achievementSetting =
                Loader.LoadAsset<AchievementSetting>(AssetType.ASSET, "Setting/AchievementSetting");

            Flo.Instance.GetFlow(out Flow_SceneA flow);
            Comp interfaceComp = Cond.Instance.Get<Comp>(flow.GetUI(), "界面");

            Comp achievementInfo = Cond.Instance.Get<Comp>(interfaceComp, "成就信息");
            achievementInfo.gameObject.SetActive(true);
            Transform achievementParent = Cond.Instance.Get<Transform>(achievementInfo, "父物体");

            foreach (Transform tmp in achievementParent) {
                GameObject.Destroy(tmp.gameObject);
            }

            foreach (var tmpAchievement in achievementSetting.AchievementDatas) {
                RefreshAchievementInfo(tmpAchievement);
            }
        }

        private void RefreshAchievementInfo(AchievementData tmpAchievement) {
            CloseAllInfo();
            
            Flo.Instance.GetFlow(out Flow_SceneA flow);
            Comp interfaceComp = Cond.Instance.Get<Comp>(flow.GetUI(), "界面");

            Comp achievementInfo = Cond.Instance.Get<Comp>(interfaceComp, "成就信息");
            achievementInfo.gameObject.SetActive(true);
            
            Transform achievementParent = Cond.Instance.Get<Transform>(achievementInfo, "父物体");
            Comp achievementTemplate = Cond.Instance.Get<Comp>(achievementInfo, "成就模板");

            if (tmpAchievement.IsAchievementFinished) {
                return;
            }

            Comp achievementInstance = GameObject.Instantiate(achievementTemplate, achievementParent);
            achievementInstance.gameObject.SetActive(true);
            
            TextMeshProUGUI achievementTile = Cond.Instance.Get<TextMeshProUGUI>(achievementInstance, "成就名");
            achievementTile.text = tmpAchievement.AchievementTitle;
            
            TextMeshProUGUI achievementContent = Cond.Instance.Get<TextMeshProUGUI>(achievementInstance, "成就内容");
            achievementContent.text = tmpAchievement.AchievementContent;
            
            TextMeshProUGUI achievementReward = Cond.Instance.Get<TextMeshProUGUI>(achievementInstance, "成就奖励交互点");
            achievementReward.text = string.Concat("成就奖励点:", tmpAchievement.RewardInteractPoint);
            
            Button achievementTasksBtn = Cond.Instance.Get<Button>(achievementInstance, "成就所有任务");
            ButtonRegister.RemoveAllListener(achievementTasksBtn);
            ButtonRegister.AddListener(achievementTasksBtn, () => {
                RefreshTaskInfo(tmpAchievement);
            });
            
            Button achievementFinishBtn = Cond.Instance.Get<Button>(achievementInstance, "完成");
            ButtonRegister.RemoveAllListener(achievementFinishBtn);
            ButtonRegister.AddListener(achievementFinishBtn, () => {
                tmpAchievement.IsAchievementFinished = true;
                RefreshUserInfo();
                RefreshAllAchievementInfo();
            });
        }

        #endregion

        #region 任务信息

        private void RefreshAllTaskInfo() {
            CloseAllInfo();
            
            AchievementSetting achievementSetting =
                Loader.LoadAsset<AchievementSetting>(AssetType.ASSET, "Setting/AchievementSetting");

            Flo.Instance.GetFlow(out Flow_SceneA flow);
            Comp interfaceComp = Cond.Instance.Get<Comp>(flow.GetUI(), "界面");

            Comp taskInfo = Cond.Instance.Get<Comp>(interfaceComp, "任务信息");
            taskInfo.gameObject.SetActive(true);
            
            Transform taskParent = Cond.Instance.Get<Transform>(taskInfo, "父物体");

            foreach (Transform tmp in taskParent) {
                GameObject.Destroy(tmp.gameObject);
            }

            foreach (var tmpAchievement in achievementSetting.AchievementDatas) {
                foreach (var tmpTask in tmpAchievement.TaskDatas) {
                    RefreshTaskInfo(tmpAchievement, tmpTask);
                }
            }
        }
        
        private void RefreshTaskInfo(AchievementData tmpAchievement) {
            CloseAllInfo();
            
            Flo.Instance.GetFlow(out Flow_SceneA flow);
            Comp interfaceComp = Cond.Instance.Get<Comp>(flow.GetUI(), "界面");

            Comp taskInfo = Cond.Instance.Get<Comp>(interfaceComp, "任务信息");
            taskInfo.gameObject.SetActive(true);
            
            Transform taskParent = Cond.Instance.Get<Transform>(taskInfo, "父物体");

            foreach (Transform tmp in taskParent) {
                GameObject.Destroy(tmp.gameObject);
            }

            foreach (var tmpTask in tmpAchievement.TaskDatas) {
                RefreshTaskInfo(tmpAchievement, tmpTask);
            }
        }

        private void RefreshTaskInfo(AchievementData tmpAchievement, TaskData tmpTask) {
            CloseAllInfo();

            Flo.Instance.GetFlow(out Flow_SceneA flow);
            Comp interfaceComp = Cond.Instance.Get<Comp>(flow.GetUI(), "界面");

            Comp taskInfo = Cond.Instance.Get<Comp>(interfaceComp, "任务信息");
            taskInfo.gameObject.SetActive(true);
            
            Transform taskParent = Cond.Instance.Get<Transform>(taskInfo, "父物体");
            Comp taskTemplate = Cond.Instance.Get<Comp>(taskInfo, "任务模板");

            if (tmpTask.IsTaskFinished) {
                return;
            }

            Comp taskInstance = GameObject.Instantiate(taskTemplate, taskParent);
            taskInstance.gameObject.SetActive(true);
            
            TextMeshProUGUI taskTile = Cond.Instance.Get<TextMeshProUGUI>(taskInstance, "任务名");
            taskTile.text = tmpTask.TaskTitle;
            
            TextMeshProUGUI taskContent = Cond.Instance.Get<TextMeshProUGUI>(taskInstance, "任务内容");
            taskContent.text = tmpTask.TaskContent;
            
            TextMeshProUGUI taskReward = Cond.Instance.Get<TextMeshProUGUI>(taskInstance, "任务奖励交互点");
            taskReward.text = string.Concat("任务奖励点:", tmpTask.RewardInteractPoint);
            
            Button taskInAchievementBtn = Cond.Instance.Get<Button>(taskInstance, "任务所属成就");
            ButtonRegister.RemoveAllListener(taskInAchievementBtn);
            ButtonRegister.AddListener(taskInAchievementBtn, () => {
                RefreshAchievementInfo(tmpAchievement);
            });
            
            Button taskFinishBtn = Cond.Instance.Get<Button>(taskInstance, "任务完成");
            ButtonRegister.RemoveAllListener(taskFinishBtn);
            ButtonRegister.AddListener(taskFinishBtn, () => {
                tmpTask.IsTaskFinished = true;
                RefreshUserInfo();
                RefreshAllTaskInfo();
            });
        }

        private int GetAllInteractPoint() {
            MySetting mySetting = Loader.LoadAsset<MySetting>(AssetType.ASSET, "Setting/MySetting");
            AchievementSetting achievementSetting =
                Loader.LoadAsset<AchievementSetting>(AssetType.ASSET, "Setting/AchievementSetting");

            int taskInteractPoint = 0;
            foreach (var tmpAchievement in achievementSetting.AchievementDatas) {
                if (tmpAchievement.IsAchievementFinished) {
                    taskInteractPoint += tmpAchievement.RewardInteractPoint;
                }
                foreach (var tmpTask in tmpAchievement.TaskDatas) {
                    if (tmpTask.IsTaskFinished) {
                        taskInteractPoint += tmpTask.RewardInteractPoint;
                    }
                }
            }

            return mySetting.InteractPoint + taskInteractPoint;
        }

        #endregion

        #region 通用

        private void CloseAllInfo() {
            Flo.Instance.GetFlow(out Flow_SceneA flow);
            Comp interfaceComp = Cond.Instance.Get<Comp>(flow.GetUI(), "界面");
            Comp info = Cond.Instance.Get<Comp>(interfaceComp, "任务信息");
            info.gameObject.SetActive(false);
            info = Cond.Instance.Get<Comp>(interfaceComp, "成就信息");
            info.gameObject.SetActive(false);
        }

        #endregion

        public override void DelayedExecute() {
        }

        public override void Clear() {
            base.Clear();
        }
    }
}