using TMPro;
using UnityEngine.UI;


namespace LazyPan {
    public class Behaviour_Event_UserInfo : Behaviour {
        private UserData _用户数据;
        private Flow_SceneA _流程;
        private Comp _创建用户组件;
        private bool _测试_默认无用户数据 = true;

        public Behaviour_Event_UserInfo(Entity entity, string behaviourSign) : base(entity, behaviourSign) {
            Flo.Instance.GetFlow(out _流程);
            //加载
            _用户数据 = SaveLoad.Instance.Load<UserData>("UserData");
            _创建用户组件 = Cond.Instance.Get<Comp>(_流程.GetUI(), "创建用户信息");
            
            //初始化
            _创建用户组件.gameObject.SetActive(_用户数据 == default || _测试_默认无用户数据);
            
            //逻辑
            if (_用户数据 == default || _测试_默认无用户数据) {
                Button 用户提交按钮 = Cond.Instance.Get<Button>(_创建用户组件, "提交用户信息");
                ButtonRegister.RemoveAllListener(用户提交按钮);
                ButtonRegister.AddListener(用户提交按钮, 提交用户信息, _创建用户组件);
            } else {
                _流程.Main();
            }
        }
        
        private void 提交用户信息(Comp 创建用户组件) {
            _用户数据 = new UserData {
                UserName = Cond.Instance.Get<TMP_InputField>(创建用户组件, "昵称").text,
                UserMoney = int.Parse(Cond.Instance.Get<TMP_InputField>(创建用户组件, "金钱").text)
            };
            SaveLoad.Instance.Save("UserData", _用户数据);
            创建用户组件.gameObject.SetActive(false);
            _流程.Main();
        }

        public override void DelayedExecute() {
            
        }

        public override void Clear() {
            base.Clear();
            _创建用户组件.gameObject.SetActive(false);
        }
    }
}