using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LazyPan {
    public class Behaviour_Event_SceneAUI : Behaviour {
        public Behaviour_Event_SceneAUI (Entity entity, string behaviourSign) : base(entity, behaviourSign) {
            #region 用户信息

            RefreshUserInfo();

            #endregion

            #region 任务信息

            RefreshTaskInfo();
            
            #endregion
        }

        private void RefreshUserInfo() {
            MySetting mySetting = Loader.LoadAsset<MySetting>(AssetType.ASSET, "Setting/MySetting");

            Flo.Instance.GetFlow(out Flow_SceneA flow);
            Comp interfaceComp = Cond.Instance.Get<Comp>(flow.GetUI(), "界面");
            
            Comp userInfo = Cond.Instance.Get<Comp>(interfaceComp, "用户信息");
            TextMeshProUGUI name = Cond.Instance.Get<TextMeshProUGUI>(userInfo, "名字");
            name.text = mySetting.Name;
            
            TextMeshProUGUI interactPoint = Cond.Instance.Get<TextMeshProUGUI>(userInfo, "交互点");
            interactPoint.text = string.Concat("交互点:" + GetAllInteractPoint());
        }

        private void RefreshTaskInfo() {
            AchievementSetting achievementSetting = Loader.LoadAsset<AchievementSetting>(AssetType.ASSET, "Setting/AchievementSetting");

            Flo.Instance.GetFlow(out Flow_SceneA flow);
            Comp interfaceComp = Cond.Instance.Get<Comp>(flow.GetUI(), "界面");
            
            Comp taskInfo = Cond.Instance.Get<Comp>(interfaceComp, "任务信息");
            Transform taskParent = Cond.Instance.Get<Transform>(taskInfo, "父物体");
            Comp taskTemplate = Cond.Instance.Get<Comp>(taskInfo, "任务模板");

            foreach (Transform tmp in taskParent) {
                GameObject.Destroy(tmp.gameObject);
            }

            foreach (var tmpAchievement in achievementSetting.AchievementDatas) {
                foreach (var tmpTask in tmpAchievement.TaskDatas) {
                    if (tmpTask.IsTaskFinished) {
                        continue;
                    }
                    Comp taskInstance = GameObject.Instantiate(taskTemplate, taskParent);
                    taskInstance.gameObject.SetActive(true);
                    TextMeshProUGUI taskTile = Cond.Instance.Get<TextMeshProUGUI>(taskInstance, "任务名");
                    taskTile.text = tmpTask.TaskTitle;
                    TextMeshProUGUI taskContent = Cond.Instance.Get<TextMeshProUGUI>(taskInstance, "任务内容");
                    taskContent.text = tmpTask.TaskContent;
                    TextMeshProUGUI taskReward = Cond.Instance.Get<TextMeshProUGUI>(taskInstance, "任务奖励交互点");
                    taskReward.text = string.Concat("任务奖励点:", tmpTask.RewardInteractPoint);
                    Button taskFinishBtn = Cond.Instance.Get<Button>(taskInstance, "任务完成");
                    ButtonRegister.RemoveAllListener(taskFinishBtn);
                    ButtonRegister.AddListener(taskFinishBtn, () => {
                        tmpTask.IsTaskFinished = true;
                        RefreshUserInfo();
                        RefreshTaskInfo();
                    });
                }
            }
        }

        private int GetAllInteractPoint() {
            MySetting mySetting = Loader.LoadAsset<MySetting>(AssetType.ASSET, "Setting/MySetting");
            AchievementSetting achievementSetting = Loader.LoadAsset<AchievementSetting>(AssetType.ASSET, "Setting/AchievementSetting");

            int taskInteractPoint = 0;
            foreach (var tmpAchievement in achievementSetting.AchievementDatas) {
                foreach (var tmpTask in tmpAchievement.TaskDatas) {
                    if (tmpTask.IsTaskFinished) {
                        taskInteractPoint += tmpTask.RewardInteractPoint;
                    }
                }
            }

            return mySetting.InteractPoint + taskInteractPoint;
        }

        public override void DelayedExecute() {
            
        }

        public override void Clear() {
            base.Clear();
        }
    }
}