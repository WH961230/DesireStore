using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LazyPan {
    public class Behaviour_Event_MainInfo : Behaviour {
        //流程
        private Flow_SceneA _流程;
        //数据
        private UserData _用户数据;
        private NavigationData _导航数据;
        //组件
        private Comp _主面板数据组件;
        private Comp _用户信息组件;
        private Comp _内容信息组件;
        private Comp _详细信息组件;
        private Comp _模板库组件;
        //实例
        private Comp _导航栏实例;
        private List<Comp> _导航栏按钮实例;

        public Behaviour_Event_MainInfo(Entity entity, string behaviourSign) : base(entity, behaviourSign) {
            Flo.Instance.GetFlow(out _流程);
            //加载
            _主面板数据组件 = Cond.Instance.Get<Comp>(_流程.GetUI(), "主面板信息");
            _用户信息组件 = Cond.Instance.Get<Comp>(_主面板数据组件, "用户信息");
            _内容信息组件 = Cond.Instance.Get<Comp>(_主面板数据组件, "内容信息");
            _详细信息组件 = Cond.Instance.Get<Comp>(_内容信息组件, "详细信息");
            _模板库组件 = Cond.Instance.Get<Comp>(_内容信息组件, "模板库");
            获取数据();
            
            //初始化
            _主面板数据组件.gameObject.SetActive(true);
            _用户信息组件.gameObject.SetActive(true);
            _内容信息组件.gameObject.SetActive(true);
            _详细信息组件.gameObject.SetActive(true);
            _模板库组件.gameObject.SetActive(true);
            
            //逻辑
            用户信息();
            内容信息();
        }

        private void 获取数据() {
            _用户数据 = SaveLoad.Instance.Load<UserData>("UserData");
            _导航数据 = SaveLoad.Instance.Load<NavigationData>("NavigationData");
            if (_导航数据 == default) {
                _导航数据 = new NavigationData() {
                    Navigation = new List<Navigation>() {
                        Navigation.灵谋,
                        Navigation.修行,
                        Navigation.蓄势,
                        Navigation.琳琅,
                        Navigation.博弈,
                    }
                };
                SaveLoad.Instance.Save("NavigationData", _导航数据);
            }

            _导航栏按钮实例 = new List<Comp>();
        }

        private void 用户信息() {
            //数据赋值
            Cond.Instance.Get<TextMeshProUGUI>(_用户信息组件, "昵称").text = _用户数据.UserName;
            Cond.Instance.Get<TextMeshProUGUI>(_用户信息组件, "金钱").text = _用户数据.UserMoney.ToString();
            Cond.Instance.Get<TextMeshProUGUI>(_用户信息组件, "欲望币").text = _用户数据.UserDesireCoin.ToString();
            Cond.Instance.Get<TextMeshProUGUI>(_用户信息组件, "勋章").text = _用户数据.UserMedal.ToString();
            //按钮
            Button 返回登录按钮 = Cond.Instance.Get<Button>(_用户信息组件, "返回登录");
            ButtonRegister.RemoveAllListener(返回登录按钮);
            ButtonRegister.AddListener(返回登录按钮, () => {
                _流程.Login();
            });
        }

        private void 内容信息() {
            //获取导航栏数据
            List<Navigation> 导航栏 = _导航数据.Navigation;
            //模板库
            if (导航栏.Count > 0) {
                Comp 导航栏模板 = Cond.Instance.Get<Comp>(_模板库组件, "导航栏模板");
                _导航栏实例 = GameObject.Instantiate(导航栏模板, _内容信息组件.transform);
                _导航栏实例.gameObject.SetActive(true);
                _导航栏实例.transform.SetAsFirstSibling();
                Comp 导航栏按钮模板 = Cond.Instance.Get<Comp>(_模板库组件, "导航栏按钮模板");
                Transform 导航栏按钮父物体 = Cond.Instance.Get<Transform>(_导航栏实例, "导航栏父物体");
                _导航栏按钮实例.Clear();
                foreach (var tmpNavigation in 导航栏) {
                    Comp 导航栏按钮实例 = GameObject.Instantiate(导航栏按钮模板, 导航栏按钮父物体);
                    导航栏按钮实例.gameObject.SetActive(true);
                    Cond.Instance.Get<TextMeshProUGUI>(导航栏按钮实例, "导航栏按钮文本").text = tmpNavigation.ToString();
                    _导航栏按钮实例.Add(导航栏按钮实例);
                }
            }
            //详细信息
        }

        public override void DelayedExecute() {
            
        }

        public override void Clear() {
            base.Clear();
            _主面板数据组件.gameObject.SetActive(false);
            _用户信息组件.gameObject.SetActive(false);
            _内容信息组件.gameObject.SetActive(false);
            _详细信息组件.gameObject.SetActive(false);
            _模板库组件.gameObject.SetActive(false);
            foreach (Comp tmp in _导航栏按钮实例) {
                Object.Destroy(tmp.gameObject);
            }
            Object.Destroy(_导航栏实例.gameObject);
        }
    }
}