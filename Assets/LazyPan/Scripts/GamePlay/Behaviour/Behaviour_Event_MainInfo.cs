using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LazyPan {
    public class Behaviour_Event_MainInfo : Behaviour {
        //流程
        private Flow_SceneA _流程;

        //数据
        private 用户数据 _用户数据;
        private 导航数据 _导航数据;

        private 灵谋数据 _灵谋数据;

        //组件
        private Comp _主面板数据组件;
        private Comp _用户信息组件;
        private Comp _内容信息组件;
        private Comp _详细信息组件;
        private Comp _模板库组件;
        private Transform _内容父物体;

        //实例
        private Comp _导航栏实例;
        private Comp _导航栏子实例;
        private Comp _灵谋内容实例;
        private List<Comp> _导航栏按钮实例;
        private List<Comp> _灵谋子标题按钮示例;

        public Behaviour_Event_MainInfo(Entity entity, string behaviourSign) : base(entity, behaviourSign) {
            Flo.Instance.GetFlow(out _流程);
            //加载
            _主面板数据组件 = Cond.Instance.Get<Comp>(_流程.GetUI(), "主面板信息");
            _用户信息组件 = Cond.Instance.Get<Comp>(_主面板数据组件, "用户信息");
            _内容信息组件 = Cond.Instance.Get<Comp>(_主面板数据组件, "内容信息");
            _内容父物体 = Cond.Instance.Get<Transform>(_内容信息组件, "内容父物体");
            _模板库组件 = Cond.Instance.Get<Comp>(_内容信息组件, "模板库");
            获取数据();

            //初始化
            _主面板数据组件.gameObject.SetActive(true);
            _用户信息组件.gameObject.SetActive(true);
            _内容信息组件.gameObject.SetActive(true);
            _模板库组件.gameObject.SetActive(true);
            
            //清理父物体残留物体
            foreach (Transform tmp in _内容父物体) {
                GameObject.Destroy(tmp.gameObject);
            }
            
            //清理完后挪进去
            _详细信息组件 = GameObject.Instantiate(Cond.Instance.Get<Comp>(_内容信息组件, "详细信息"), _内容父物体);
            _详细信息组件.gameObject.SetActive(true);
            Transform 内容信息父物体 = Cond.Instance.Get<Transform>(_详细信息组件, "内容信息父物体");
            foreach (Transform tmp in 内容信息父物体.transform) {
                GameObject.Destroy(tmp.gameObject);
            }
            
            //延时加载
            ClockUtil.Instance.AlarmAfter(0.01f, DelayedExecute);
        }

        private void 获取数据() {
            _用户数据 = SaveLoad.Instance.Load<用户数据>("用户数据");
            _导航数据 = SaveLoad.Instance.Load<导航数据>("导航数据");
            if (_导航数据 == default) {
                _导航数据 = new 导航数据() {
                    所有导航 = new List<导航信息>() {
                        导航信息.灵谋,
                        导航信息.修行,
                        导航信息.蓄势,
                        导航信息.琳琅,
                        导航信息.博弈,
                    }
                };
                SaveLoad.Instance.Save("导航数据", _导航数据);
            }

            _灵谋数据 = SaveLoad.Instance.Load<灵谋数据>("灵谋数据");
            if (_灵谋数据 == default) {
                _灵谋数据 = new 灵谋数据() {
                    所有灵谋数据 = new List<灵谋信息>() {
                        new 灵谋信息() {
                            标题 = "测试",
                            内容 = "灵谋内容",
                        }
                    }
                };
                SaveLoad.Instance.Save("灵谋数据", _灵谋数据);
            }

            _导航栏按钮实例 = new List<Comp>();
            _灵谋子标题按钮示例 = new List<Comp>();
        }

        public override void DelayedExecute() {
            //逻辑
            用户信息();
            内容信息();
        }

        private void 用户信息() {
            //数据赋值
            Cond.Instance.Get<TextMeshProUGUI>(_用户信息组件, "昵称").text = _用户数据.用户名;
            Cond.Instance.Get<TextMeshProUGUI>(_用户信息组件, "金钱").text = _用户数据.用户钱.ToString();
            Cond.Instance.Get<TextMeshProUGUI>(_用户信息组件, "欲望币").text = _用户数据.用户欲望币.ToString();
            Cond.Instance.Get<TextMeshProUGUI>(_用户信息组件, "勋章").text = _用户数据.用户勋章.ToString();
            //按钮
            Button 返回登录按钮 = Cond.Instance.Get<Button>(_用户信息组件, "返回登录");
            ButtonRegister.RemoveAllListener(返回登录按钮);
            ButtonRegister.AddListener(返回登录按钮, () => { _流程.Login(); });
        }

        private void 内容信息() {
            导航栏信息();
        }

        private void 导航栏信息() {
            //获取导航栏数据
            List<导航信息> 导航栏 = _导航数据.所有导航;
            //模板库
            if (导航栏.Count > 0) {
                Comp 导航栏模板 = Cond.Instance.Get<Comp>(_模板库组件, "导航栏模板");
                _导航栏实例 = GameObject.Instantiate(导航栏模板, _内容父物体);
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
                    Button 导航栏按钮 = Cond.Instance.Get<Button>(导航栏按钮实例, "导航栏按钮");
                    ButtonRegister.RemoveAllListener(导航栏按钮);
                    ButtonRegister.AddListener(导航栏按钮, 导航栏子标题, tmpNavigation);
                }
            }
        }

        private void 导航栏子标题(导航信息 导航信息) {
            switch (导航信息) {
                case 导航信息.灵谋:
                    灵谋子标题();
                    break;
            }
        }

        private void 灵谋子标题() {
            //获取导航栏数据
            List<灵谋信息> 灵谋 = _灵谋数据.所有灵谋数据;
            //模板库
            if (灵谋.Count > 0) {
                if (_灵谋子标题按钮示例.Count == 0) {
                    Comp 导航栏模板 = Cond.Instance.Get<Comp>(_模板库组件, "导航栏模板");
                    if (_导航栏子实例 == null) {
                        _导航栏子实例 = GameObject.Instantiate(导航栏模板, _内容父物体);
                    }

                    _导航栏子实例.gameObject.SetActive(true);
                    _导航栏子实例.transform.SetSiblingIndex(1);
                    Comp 导航栏按钮模板 = Cond.Instance.Get<Comp>(_模板库组件, "导航栏按钮模板");
                    Transform 导航栏按钮父物体 = Cond.Instance.Get<Transform>(_导航栏子实例, "导航栏父物体");
                    _灵谋子标题按钮示例.Clear();
                    foreach (var tmp in 灵谋) {
                        Comp 导航栏按钮实例 = GameObject.Instantiate(导航栏按钮模板, 导航栏按钮父物体);
                        导航栏按钮实例.gameObject.SetActive(true);
                        Cond.Instance.Get<TextMeshProUGUI>(导航栏按钮实例, "导航栏按钮文本").text = tmp.标题;
                        _灵谋子标题按钮示例.Add(导航栏按钮实例);
                        Button 导航栏按钮 = Cond.Instance.Get<Button>(导航栏按钮实例, "导航栏按钮");
                        ButtonRegister.RemoveAllListener(导航栏按钮);
                        ButtonRegister.AddListener(导航栏按钮, 详细内容, tmp);
                    }
                } else {
                    _导航栏子实例.gameObject.SetActive(false);
                    foreach (var tmp in _灵谋子标题按钮示例) {
                        GameObject.Destroy(tmp.transform.gameObject);
                    }

                    _灵谋子标题按钮示例.Clear();
                    详细内容(null);
                }
            }
        }

        private void 详细内容(灵谋信息 灵谋信息) {
            if (灵谋信息 == null) {
                if (_灵谋内容实例 != null) {
                    _灵谋内容实例.gameObject.SetActive(false);
                }
            } else {
                Comp 灵谋内容模板 = Cond.Instance.Get<Comp>(_模板库组件, "灵谋内容模板");
                if (_灵谋内容实例 == null) {
                    Transform 内容信息父物体 = Cond.Instance.Get<Transform>(_详细信息组件, "内容信息父物体");
                    _灵谋内容实例 = GameObject.Instantiate(灵谋内容模板, 内容信息父物体.transform);
                } else if (_灵谋内容实例.gameObject.activeSelf) {
                    _灵谋内容实例.gameObject.SetActive(false);
                    return;
                }

                _灵谋内容实例.gameObject.SetActive(true);
                Cond.Instance.Get<TextMeshProUGUI>(_灵谋内容实例, "标题").text = 灵谋信息.标题;
                Cond.Instance.Get<TextMeshProUGUI>(_灵谋内容实例, "内容").text = 灵谋信息.内容;
            }
        }

        public override void Clear() {
            base.Clear();
            _主面板数据组件.gameObject.SetActive(false);
            _用户信息组件.gameObject.SetActive(false);
            _内容信息组件.gameObject.SetActive(false);
            _详细信息组件.gameObject.SetActive(false);
            _模板库组件.gameObject.SetActive(false);
        }
    }
}