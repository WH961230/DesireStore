using TMPro;
using UnityEngine.UI;

namespace LazyPan {
    public class Behaviour_Event_SceneAUI : Behaviour {
        private UserData userData;
        private Flow_SceneA flow;
        private Comp userInfo;
        private Comp userCreation;
        public Behaviour_Event_SceneAUI (Entity entity, string behaviourSign) : base(entity, behaviourSign) {
            Flo.Instance.GetFlow(out flow);
            //如果有用户数据显示读取用户数据 反之 创建用户数据
            userData = SaveLoad.Instance.Load<UserData>("UserData");
            bool hasUserData = userData != default;
            userCreation = Cond.Instance.Get<Comp>(flow.GetUI(), "创建用户信息");
            userCreation.gameObject.SetActive(!hasUserData);
            userInfo = Cond.Instance.Get<Comp>(flow.GetUI(), "用户信息");
            userInfo.gameObject.SetActive(false);
            if (!hasUserData) {
                Button userSubmit = Cond.Instance.Get<Button>(userCreation, "提交用户信息");
                ButtonRegister.AddListener(userSubmit, SubmitUserInfo, userCreation);
            } else {
                DisplayUserInfo();
            }
        }

        private void SubmitUserInfo(Comp userCreation) {
            userData = new UserData();
            userData.UserName = Cond.Instance.Get<TMP_InputField>(userCreation, "昵称").text;
            userData.UserMoney = int.Parse(Cond.Instance.Get<TMP_InputField>(userCreation, "金钱").text);
            SaveLoad.Instance.Save("UserData", userData);
            userCreation.gameObject.SetActive(false);
            DisplayUserInfo();
        }

        private void DisplayUserInfo() {
            userInfo.gameObject.SetActive(true);
            Cond.Instance.Get<TextMeshProUGUI>(userInfo, "昵称").text = userData.UserName;
            Cond.Instance.Get<TextMeshProUGUI>(userInfo, "金钱").text = userData.UserMoney.ToString();
        }

        public override void DelayedExecute() {
            
        }

        public override void Clear() {
            base.Clear();
        }
    }
}