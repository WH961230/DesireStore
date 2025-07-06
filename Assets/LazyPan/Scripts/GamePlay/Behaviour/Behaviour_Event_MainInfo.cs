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
        private 修行数据 _修行数据;
        private 蓄势数据 _蓄势数据;
        private 用户灵谋信息 _用户灵谋信息;
        private 用户修行信息 _用户修行信息;
        private 用户蓄势信息 _用户蓄势信息;

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
        private 导航信息 _导航信息;
        private Comp _灵谋内容实例;
        private 灵谋信息 _灵谋信息实例;
        private Comp _修行内容实例;
        private 修行信息 _修行信息实例;
        private Comp _蓄势内容实例;
        private 灵谋信息 _蓄势信息实例;
        private List<Comp> _导航栏按钮实例;
        private List<Comp> _子标题按钮示例;

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
            #region 用户数据

            _用户数据 = SaveLoad.Instance.Load<用户数据>("用户数据");

            #endregion

            #region 导航数据

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
            
            _导航栏按钮实例 = new List<Comp>();
            _子标题按钮示例 = new List<Comp>();

            #endregion

            #region 灵谋数据

            _灵谋数据 = SaveLoad.Instance.Load<灵谋数据>("灵谋数据");
            if (_灵谋数据 == default) {
                _灵谋数据 = new 灵谋数据() {
                    用户灵谋信息数据 = new List<用户灵谋信息>()
                };
            }

            bool 存在用户 = false;
            foreach (var tmp in _灵谋数据.用户灵谋信息数据) {
                if (tmp.用户名 == _用户数据.用户名) {
                    _用户灵谋信息 = tmp;
                    存在用户 = true;
                    break;
                }
            }

            if (!存在用户) {
                _用户灵谋信息 = new 用户灵谋信息() {
                    用户名 = _用户数据.用户名,
                    所有灵谋数据 = new List<灵谋信息>() {
                        new 灵谋信息() {
                            标题 = "测试一",
                            内容 = $"我叫{_用户数据.用户名} 这是第一篇测试",
                        },
                        new 灵谋信息() {
                            标题 = "测试二",
                            内容 = $"我叫{_用户数据.用户名} 这是第二篇测试",
                        },
                    }
                };

                _灵谋数据.用户灵谋信息数据.Add(_用户灵谋信息);
            }

            SaveLoad.Instance.Save("灵谋数据", _灵谋数据);
            
            #endregion

            #region 修行数据

            _修行数据 = SaveLoad.Instance.Load<修行数据>("修行数据");
            if (_修行数据 == default) {
                _修行数据 = new 修行数据() {
                    用户修行信息数据 = new List<用户修行信息>()
                };
            }

            存在用户 = false;
            foreach (var tmp in _修行数据.用户修行信息数据) {
                if (tmp.用户名 == _用户数据.用户名) {
                    _用户修行信息 = tmp;
                    存在用户 = true;
                    break;
                }
            }

            if (!存在用户) {
                _用户修行信息 = new 用户修行信息() {
                    用户名 = _用户数据.用户名,
                    所有修行数据 = new List<修行信息>() {
                        new 修行信息() {
                            标题 = "修行任务一",
                            内容 = $"我叫{_用户数据.用户名} 这是第一篇测试",
                            完成 = true,
                        },
                        new 修行信息() {
                            标题 = "修行任务二",
                            内容 = $"我叫{_用户数据.用户名} 这是第二篇测试",
                            完成 = false,
                        },
                    }
                };

                _修行数据.用户修行信息数据.Add(_用户修行信息);
            }

            SaveLoad.Instance.Save("修行数据", _修行数据);

            #endregion

            #region 蓄势数据

            _蓄势数据 = SaveLoad.Instance.Load<蓄势数据>("蓄势数据");
            if (_蓄势数据 == default) {
                _蓄势数据 = new 蓄势数据() {
                    用户蓄势信息数据 = new List<用户蓄势信息>()
                };
            }

            存在用户 = false;
            foreach (var tmp in _蓄势数据.用户蓄势信息数据) {
                if (tmp.用户名 == _用户数据.用户名) {
                    _用户蓄势信息 = tmp;
                    存在用户 = true;
                    break;
                }
            }

            if (!存在用户) {
                _用户蓄势信息 = new 用户蓄势信息() {
                    用户名 = _用户数据.用户名,
                    所有蓄势数据 = new List<蓄势信息>() {
                        new 蓄势信息() {
                            标题 = "蓄势任务一",
                            内容 = $"我叫{_用户数据.用户名} 这是第一篇测试",
                            完成 = true,
                        },
                        new 蓄势信息() {
                            标题 = "蓄势任务二",
                            内容 = $"我叫{_用户数据.用户名} 这是第二篇测试",
                            完成 = false,
                        },
                    }
                };

                _蓄势数据.用户蓄势信息数据.Add(_用户蓄势信息);
            }

            SaveLoad.Instance.Save("蓄势数据", _蓄势数据);

            #endregion

            #region 琳琅数据

            

            #endregion

            #region 博弈数据

            

            #endregion
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

        #region 导航

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
            灵谋详细内容(null);
            修行详细内容(null);
            switch (导航信息) {
                case 导航信息.灵谋:
                    灵谋子标题();
                    break;
                case 导航信息.修行:
                    修行子标题();
                    break;
            }
        }

        #endregion

        #region 灵谋

        private void 灵谋子标题() {
            if (_导航信息 == 导航信息.灵谋) {
                return;
            }
            _导航信息 = 导航信息.灵谋;
            //获取导航栏数据
            List<灵谋信息> 灵谋 = _用户灵谋信息.所有灵谋数据;
            //模板库
            if (灵谋.Count > 0) {
                foreach (var tmp in _子标题按钮示例) {
                    GameObject.Destroy(tmp.transform.gameObject);
                }
                _子标题按钮示例.Clear();

                ClockUtil.Instance.AlarmAfter(0.01f, () => {//延后加载
                    if (_子标题按钮示例.Count == 0) {
                        Comp 导航栏模板 = Cond.Instance.Get<Comp>(_模板库组件, "导航栏模板");
                        if (_导航栏子实例 == null) {
                            _导航栏子实例 = GameObject.Instantiate(导航栏模板, _内容父物体);
                        }

                        _导航栏子实例.gameObject.SetActive(true);
                        _导航栏子实例.transform.SetSiblingIndex(1);
                        Comp 导航栏按钮模板 = Cond.Instance.Get<Comp>(_模板库组件, "导航栏按钮模板");
                        Transform 导航栏按钮父物体 = Cond.Instance.Get<Transform>(_导航栏子实例, "导航栏父物体");
                        _子标题按钮示例.Clear();
                        foreach (var tmp in 灵谋) {
                            Comp 导航栏按钮实例 = GameObject.Instantiate(导航栏按钮模板, 导航栏按钮父物体);
                            导航栏按钮实例.gameObject.SetActive(true);
                            Cond.Instance.Get<TextMeshProUGUI>(导航栏按钮实例, "导航栏按钮文本").text = tmp.标题;
                            _子标题按钮示例.Add(导航栏按钮实例);
                            Button 导航栏按钮 = Cond.Instance.Get<Button>(导航栏按钮实例, "导航栏按钮");
                            ButtonRegister.RemoveAllListener(导航栏按钮);
                            ButtonRegister.AddListener(导航栏按钮, 灵谋详细内容, tmp);
                        }

                        if (灵谋.Count > 0) {
                            灵谋详细内容(灵谋[0]);
                        }
                    }
                });
            }
        }

        private void 灵谋详细内容(灵谋信息 灵谋信息) {
            //如果标题一致则不变 如果标题不一样直接替换 如果信息为空直接删除
            if (灵谋信息 == null) {
                //如果信息为空直接删除
                if (_灵谋内容实例 != null) {
                    GameObject.Destroy(_灵谋内容实例.gameObject);
                    _灵谋内容实例 = null;
                    _灵谋信息实例 = null;
                }
            } else {
                Comp 灵谋内容模板 = Cond.Instance.Get<Comp>(_模板库组件, "灵谋内容模板");
                if (_灵谋内容实例 == null) {
                    //当前没有实例直接生成
                    Transform 内容信息父物体 = Cond.Instance.Get<Transform>(_详细信息组件, "内容信息父物体");
                    _灵谋内容实例 = GameObject.Instantiate(灵谋内容模板, 内容信息父物体.transform);
                    _灵谋内容实例.gameObject.SetActive(true);
                    _灵谋信息实例 = 灵谋信息;
                } else if (_灵谋信息实例.标题 != 灵谋信息.标题) {
                    //新的标题直接刷新
                    _灵谋信息实例 = 灵谋信息;
                }

                Cond.Instance.Get<TextMeshProUGUI>(_灵谋内容实例, "标题").text = _灵谋信息实例.标题;
                Cond.Instance.Get<TextMeshProUGUI>(_灵谋内容实例, "内容").text = _灵谋信息实例.内容;
            }
        }

        #endregion

        #region 修行

        private void 修行子标题() {
            if (_导航信息 == 导航信息.修行) {
                return;
            }
            _导航信息 = 导航信息.修行;
            //获取导航栏数据
            List<修行信息> 修行 = _用户修行信息.所有修行数据;
            //模板库
            if (修行.Count > 0) {
                foreach (var tmp in _子标题按钮示例) {
                    GameObject.Destroy(tmp.transform.gameObject);
                }
                _子标题按钮示例.Clear();

                ClockUtil.Instance.AlarmAfter(0.01f, () => {
                    if (_子标题按钮示例.Count == 0) {
                        Comp 导航栏模板 = Cond.Instance.Get<Comp>(_模板库组件, "导航栏模板");
                        if (_导航栏子实例 == null) {
                            _导航栏子实例 = GameObject.Instantiate(导航栏模板, _内容父物体);
                        }

                        _导航栏子实例.gameObject.SetActive(true);
                        _导航栏子实例.transform.SetSiblingIndex(1);
                        Comp 导航栏按钮模板 = Cond.Instance.Get<Comp>(_模板库组件, "导航栏按钮模板");
                        Transform 导航栏按钮父物体 = Cond.Instance.Get<Transform>(_导航栏子实例, "导航栏父物体");
                        _子标题按钮示例.Clear();
                        foreach (var tmp in 修行) {
                            Comp 导航栏按钮实例 = GameObject.Instantiate(导航栏按钮模板, 导航栏按钮父物体);
                            导航栏按钮实例.gameObject.SetActive(true);
                            Cond.Instance.Get<TextMeshProUGUI>(导航栏按钮实例, "导航栏按钮文本").text = tmp.标题;
                            _子标题按钮示例.Add(导航栏按钮实例);
                            Button 导航栏按钮 = Cond.Instance.Get<Button>(导航栏按钮实例, "导航栏按钮");
                            ButtonRegister.RemoveAllListener(导航栏按钮);
                            ButtonRegister.AddListener(导航栏按钮, 修行详细内容, tmp);
                        }

                        if (修行.Count > 0) {
                            修行详细内容(修行[0]);
                        }
                    }
                });
            }
        }

        private void 修行详细内容(修行信息 修行信息) {
            //如果标题一致则不变 如果标题不一样直接替换 如果信息为空直接删除
            if (修行信息 == null) {
                //如果信息为空直接删除
                if (_修行内容实例 != null) {
                    GameObject.Destroy(_修行内容实例.gameObject);
                    _修行内容实例 = null;
                    _修行信息实例 = null;
                }
            } else {
                Comp 修行内容模板 = Cond.Instance.Get<Comp>(_模板库组件, "修行内容模板");
                if (_修行内容实例 == null) {
                    //当前没有实例直接生成
                    Transform 内容信息父物体 = Cond.Instance.Get<Transform>(_详细信息组件, "内容信息父物体");
                    _修行内容实例 = GameObject.Instantiate(修行内容模板, 内容信息父物体.transform);
                    _修行内容实例.gameObject.SetActive(true);
                    _修行信息实例 = 修行信息;
                } else if (_修行信息实例.标题 != 修行信息.标题) {
                    //新的标题直接刷新
                    _修行信息实例 = 修行信息;
                }

                Cond.Instance.Get<TextMeshProUGUI>(_修行内容实例, "标题").text = _修行信息实例.标题;
                Cond.Instance.Get<TextMeshProUGUI>(_修行内容实例, "内容").text = _修行信息实例.内容;
            }
        }

        #endregion

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