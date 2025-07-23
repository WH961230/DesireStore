using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace LazyPan {
    public class Behaviour_Event_LinMou : Behaviour {
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
        private 灵谋日志 _灵谋信息实例;
        private Comp _修行内容实例;
        private 修行信息 _修行信息实例;
        private Comp _蓄势内容实例;
        private 蓄势信息 _蓄势信息实例;
        private List<Comp> _导航栏按钮实例;
        private List<Comp> _子标题按钮示例;
        
        public Behaviour_Event_LinMou(Entity entity, string behaviourSign) : base(entity, behaviourSign) {
            Flo.Instance.GetFlow(out _流程);
            //加载
            _主面板数据组件 = Cond.Instance.Get<Comp>(_流程.GetUI(), "主面板信息");
            _用户信息组件 = Cond.Instance.Get<Comp>(_主面板数据组件, "用户信息");
            _内容信息组件 = Cond.Instance.Get<Comp>(_主面板数据组件, "内容信息");
            _内容父物体 = Cond.Instance.Get<Transform>(_内容信息组件, "内容父物体");
            _模板库组件 = Cond.Instance.Get<Comp>(_内容信息组件, "模板库");
            
            //清理父物体残留物体
            foreach (Transform tmp in _内容父物体) {
                GameObject.Destroy(tmp.gameObject);
            }
            
            获取数据();
            
            //初始化
            _主面板数据组件.gameObject.SetActive(true);
            _用户信息组件.gameObject.SetActive(true);
            _内容信息组件.gameObject.SetActive(true);
            _模板库组件.gameObject.SetActive(true);
            
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
                    所有灵谋日志 = new List<灵谋日志>() {
                        new 灵谋日志() {
                            标题 = "请输入标题",
                            内容 = "请输入内容",
                            编辑占用 = true,
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
                        },
                        new 蓄势信息() {
                            标题 = "蓄势任务二",
                            内容 = $"我叫{_用户数据.用户名} 这是第二篇测试",
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

        #region 生成一级导航栏

        private void 生成一级导航栏() {
            //获取导航栏枚举数据
            List<导航信息> 导航栏 = _导航数据.所有导航;
            if (导航栏.Count > 0) {
                //导航栏模板
                Comp 导航栏模板 = Cond.Instance.Get<Comp>(_模板库组件, "导航栏模板");
                _导航栏实例 = GameObject.Instantiate(导航栏模板, _内容父物体);
                _导航栏实例.gameObject.SetActive(true);
                _导航栏实例.transform.SetAsFirstSibling();
                
                //导航栏按钮模板
                Comp 导航栏按钮模板 = Cond.Instance.Get<Comp>(_模板库组件, "导航栏按钮模板");
                Transform 导航栏按钮父物体 = Cond.Instance.Get<Transform>(_导航栏实例, "导航栏父物体");
                
                //遍历枚举
                _导航栏按钮实例.Clear();
                foreach (var tmpNavigation in 导航栏) {
                    Comp 导航栏按钮实例 = GameObject.Instantiate(导航栏按钮模板, 导航栏按钮父物体);
                    导航栏按钮实例.gameObject.SetActive(true);
                    Cond.Instance.Get<TextMeshProUGUI>(导航栏按钮实例, "导航栏按钮文本").text = tmpNavigation.ToString();//文本赋值
                    _导航栏按钮实例.Add(导航栏按钮实例);
                    Button 导航栏按钮 = Cond.Instance.Get<Button>(导航栏按钮实例, "导航栏按钮");
                    ButtonRegister.RemoveAllListener(导航栏按钮);
                    ButtonRegister.AddListener(导航栏按钮, 导航栏子标题, tmpNavigation, false);//按钮注册
                    
                    //右键测试
                    Comp 导航栏按钮触发 = Cond.Instance.Get<Comp>(导航栏按钮实例, "按钮触发");
                    导航栏按钮触发.OnPointerClickEvent.RemoveAllListeners();
                    导航栏按钮触发.OnPointerClickEvent.AddListener((eventData) => {
                        if (eventData.button == PointerEventData.InputButton.Right) {
                            添加项目(tmpNavigation);
                        }
                    });
                }
            }
        }

        #endregion
        
        #region 点击一级灵谋按钮

        private void 导航栏子标题(导航信息 导航信息, bool 包含待编辑 = false) {
            //预清除
            灵谋详细内容(null, false, true);
            修行详细内容(null, true);
            蓄势详细内容(null, true);
            switch (导航信息) {
                case 导航信息.灵谋:
                    灵谋子标题(包含待编辑);
                    break;
                case 导航信息.修行:
                    _流程.XiuXing();
                    break;
                case 导航信息.蓄势:
                    _流程.XuShi();
                    break;
            }
        }
        
        private void 灵谋子标题(bool 包含待编辑) {
            _导航信息 = 导航信息.灵谋;

            //移除子标题
            foreach (var tmp in _子标题按钮示例) {
                GameObject.Destroy(tmp.transform.gameObject);
            }
            _子标题按钮示例.Clear();
            if (_导航栏子实例 != null) {
                GameObject.Destroy(_导航栏子实例.gameObject);
                _导航栏子实例 = null;
            }
            
            //模板库
            if (_用户灵谋信息.获取第一个灵谋日志(包含待编辑, out 灵谋日志 灵谋日志)) {
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
                        
                        //获取导航栏数据
                        _用户灵谋信息.获取所有灵谋日志(包含待编辑, out List<灵谋日志> 灵谋);
                        foreach (var tmp in 灵谋) {
                            Comp 导航栏按钮实例 = GameObject.Instantiate(导航栏按钮模板, 导航栏按钮父物体);
                            导航栏按钮实例.gameObject.SetActive(true);
                            Cond.Instance.Get<TextMeshProUGUI>(导航栏按钮实例, "导航栏按钮文本").text = tmp.标题;
                            _子标题按钮示例.Add(导航栏按钮实例);
                            Button 导航栏按钮 = Cond.Instance.Get<Button>(导航栏按钮实例, "导航栏按钮");
                            ButtonRegister.RemoveAllListener(导航栏按钮);
                            ButtonRegister.AddListener(导航栏按钮, 灵谋详细内容, tmp, 包含待编辑, false);
                        }

                        灵谋详细内容(灵谋日志, 包含待编辑);
                    }
                });
            }
        }

        #endregion

        #region 点击二级灵谋按钮

        private void 灵谋详细内容(灵谋日志 灵谋日志, bool 包含编辑 = false, bool deleteInstance = false) {
            //如果标题一致则不变 如果标题不一样直接替换 如果信息为空直接删除
            if (deleteInstance) {
                //如果信息为空直接删除
                if (_灵谋内容实例 != null) {
                    GameObject.Destroy(_灵谋内容实例.gameObject);
                    _灵谋内容实例 = null;
                    _灵谋信息实例 = null;
                }
            }

            if (灵谋日志 != null) {
                string 模板名 = 包含编辑 ? "灵谋内容编辑模板" : "灵谋内容模板";
                Comp 灵谋内容模板 = Cond.Instance.Get<Comp>(_模板库组件, 模板名);
                if (_灵谋内容实例 == null) {
                    //当前没有实例直接生成
                    Transform 内容信息父物体 = Cond.Instance.Get<Transform>(_详细信息组件, "内容信息父物体");
                    _灵谋内容实例 = GameObject.Instantiate(灵谋内容模板, 内容信息父物体);
                    _灵谋内容实例.gameObject.SetActive(true);
                    _灵谋信息实例 = 灵谋日志;
                } else if (_灵谋信息实例.标题 != 灵谋日志.标题) {
                    //新的标题直接刷新
                    _灵谋信息实例 = 灵谋日志;
                }

                if (包含编辑) {
                    Cond.Instance.Get<TMP_InputField>(_灵谋内容实例, "编辑标题").text = _灵谋信息实例.标题;
                    Cond.Instance.Get<TMP_InputField>(_灵谋内容实例, "编辑内容").text = _灵谋信息实例.内容;
                    Button 保存按钮 = Cond.Instance.Get<Button>(_灵谋内容实例, "保存");
                    ButtonRegister.RemoveAllListener(保存按钮);
                    ButtonRegister.AddListener(保存按钮, 保存灵谋);
                } else {
                    Cond.Instance.Get<TextMeshProUGUI>(_灵谋内容实例, "标题").text = _灵谋信息实例.标题;
                    Cond.Instance.Get<TextMeshProUGUI>(_灵谋内容实例, "内容").text = _灵谋信息实例.内容;
                }
            }
        }

        #endregion

        public override void DelayedExecute() {
            生成一级导航栏();
        }
        
        #region 数据处理 - 添加灵谋信息

        private void 添加项目(导航信息 导航信息) {
            switch (导航信息) {
                case 导航信息.灵谋:
                    添加灵谋();
                    break;
                case 导航信息.修行:
                case 导航信息.蓄势:
                    Debug.LogError(导航信息.ToString());
                    break;
            }
        }

        #endregion
        
        #region 灵谋

        private void 添加灵谋() {
            导航栏子标题(导航信息.灵谋, true);
        }

        private void 保存灵谋() {
            _用户灵谋信息.新增一个灵谋日志(new 灵谋日志() {
                标题 = Cond.Instance.Get<TMP_InputField>(_灵谋内容实例, "编辑标题").text,
                内容 = Cond.Instance.Get<TMP_InputField>(_灵谋内容实例, "编辑内容").text,
            });
            _用户灵谋信息.获取所有灵谋日志(true, out List<灵谋日志> _灵谋日志);
            _灵谋数据.更新用户灵谋日志(_用户灵谋信息.用户名, _灵谋日志);
            SaveLoad.Instance.Save("灵谋数据", _灵谋数据);
            _灵谋数据 = SaveLoad.Instance.Load<灵谋数据>("灵谋数据");
            导航栏子标题(导航信息.灵谋);
        }
        
        #endregion
        
        #region 修行

        private void 修行子标题() {
            _导航信息 = 导航信息.修行;
            //获取导航栏数据
            List<修行信息> 修行 = _用户修行信息.所有修行数据;
            
            //移除子标题
            foreach (var tmp in _子标题按钮示例) {
                GameObject.Destroy(tmp.transform.gameObject);
            }
            _子标题按钮示例.Clear();
            if (_导航栏子实例 != null) {
                GameObject.Destroy(_导航栏子实例.gameObject);
                _导航栏子实例 = null;
            }
            
            //模板库
            if (修行.Count > 0) {
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
                            ButtonRegister.AddListener(导航栏按钮, 修行详细内容, tmp, false);
                        }

                        if (修行.Count > 0) {
                            修行详细内容(修行[0]);
                        }
                    }
                });
            }
        }

        private void 修行详细内容(修行信息 修行信息, bool deleteInstance = false) {
            //如果标题一致则不变 如果标题不一样直接替换 如果信息为空直接删除
            if (deleteInstance) {
                //如果信息为空直接删除
                if (_修行内容实例 != null) {
                    GameObject.Destroy(_修行内容实例.gameObject);
                    _修行内容实例 = null;
                    _修行信息实例 = null;
                }
            }

            if (修行信息 != null) {
                Comp 修行内容模板 = Cond.Instance.Get<Comp>(_模板库组件, "修行内容模板");
                if (_修行内容实例 == null) {
                    //当前没有实例直接生成
                    Transform 内容信息父物体 = Cond.Instance.Get<Transform>(_详细信息组件, "内容信息父物体");
                    _修行内容实例 = GameObject.Instantiate(修行内容模板, 内容信息父物体);
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

        #region 蓄势

        private void 蓄势子标题() {
            _导航信息 = 导航信息.蓄势;
            //获取导航栏数据
            List<蓄势信息> 蓄势 = _用户蓄势信息.所有蓄势数据;
            
            //移除子标题
            foreach (var tmp in _子标题按钮示例) {
                GameObject.Destroy(tmp.transform.gameObject);
            }
            _子标题按钮示例.Clear();
            if (_导航栏子实例 != null) {
                GameObject.Destroy(_导航栏子实例.gameObject);
                _导航栏子实例 = null;
            }
            
            //模板库
            if (蓄势.Count > 0) {
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
                        foreach (var tmp in 蓄势) {
                            Comp 导航栏按钮实例 = GameObject.Instantiate(导航栏按钮模板, 导航栏按钮父物体);
                            导航栏按钮实例.gameObject.SetActive(true);
                            Cond.Instance.Get<TextMeshProUGUI>(导航栏按钮实例, "导航栏按钮文本").text = tmp.标题;
                            _子标题按钮示例.Add(导航栏按钮实例);
                            Button 导航栏按钮 = Cond.Instance.Get<Button>(导航栏按钮实例, "导航栏按钮");
                            ButtonRegister.RemoveAllListener(导航栏按钮);
                            ButtonRegister.AddListener(导航栏按钮, 蓄势详细内容, tmp, false);
                        }

                        if (蓄势.Count > 0) {
                            蓄势详细内容(蓄势[0]);
                        }
                    }
                });
            }
        }

        private void 蓄势详细内容(蓄势信息 蓄势信息, bool deleteInstance = false) {
            //如果标题一致则不变 如果标题不一样直接替换 如果信息为空直接删除
            if (deleteInstance) {
                //如果信息为空直接删除
                if (_蓄势内容实例 != null) {
                    GameObject.Destroy(_蓄势内容实例.gameObject);
                    _蓄势内容实例 = null;
                    _蓄势信息实例 = null;
                }
            }
            
            if(蓄势信息 != null) {
                Comp 蓄势内容模板 = Cond.Instance.Get<Comp>(_模板库组件, "蓄势内容模板");
                if (_蓄势内容实例 == null) {
                    //当前没有实例直接生成
                    Transform 内容信息父物体 = Cond.Instance.Get<Transform>(_详细信息组件, "内容信息父物体");
                    _蓄势内容实例 = GameObject.Instantiate(蓄势内容模板, 内容信息父物体);
                    _蓄势内容实例.gameObject.SetActive(true);
                    _蓄势信息实例 = 蓄势信息;
                } else if (_蓄势信息实例.标题 != 蓄势信息.标题) {
                    //新的标题直接刷新
                    _蓄势信息实例 = 蓄势信息;
                }

                Cond.Instance.Get<TextMeshProUGUI>(_蓄势内容实例, "标题").text = _蓄势信息实例.标题;
                Cond.Instance.Get<TextMeshProUGUI>(_蓄势内容实例, "内容").text = _蓄势信息实例.内容;
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