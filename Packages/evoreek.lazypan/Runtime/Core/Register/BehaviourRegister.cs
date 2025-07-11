﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace LazyPan {
    public class BehaviourRegister {
        private static Dictionary<int, List<Behaviour>> BehaviourDic = new Dictionary<int, List<Behaviour>>();

        #region 增

        //增加注册行为
        public static bool RegisterBehaviour(int id, string name, out Behaviour outBehaviour) {
            //是否有实体
            if (EntityRegister.TryGetEntityByID(id, out Entity entity)) {
                if (BehaviourDic.TryGetValue(id, out List<Behaviour> behaviours)) {
                    
                    //判断实体已有当前行为
                    foreach (Behaviour tempBehaviour in behaviours) {
                        BehaviourConfig config = BehaviourConfig.Get(tempBehaviour.BehaviourSign);
                        if (config.Name == name) {
                            outBehaviour = default;
                            return false;
                        }
                    }

                    string sign = "";
                    foreach (var tmpKey in BehaviourConfig.GetKeys()) {
                        BehaviourConfig config = BehaviourConfig.Get(tmpKey);
                        if (config.Name == name) {
                            sign = config.Sign;
                            break;
                        }
                    }

                    //创建行为实体
                    try {
                        Type type = Assembly.Load("Assembly-CSharp").GetType(string.Concat("LazyPan.", sign));
                        Behaviour behaviour = (Behaviour) Activator.CreateInstance(type, entity, sign);
                        outBehaviour = behaviour;
                        behaviours.Add(behaviour);
                    } catch (Exception e) {
                        LogUtil.LogError(name);
                        throw;
                    }

                    return true;
                } else {
                    //创建行为实体
                    try {
                        string sign = "";
                        List<Behaviour> instanceBehaviours = new List<Behaviour>();
                        foreach (var tmpKey in BehaviourConfig.GetKeys()) {
                            BehaviourConfig config = BehaviourConfig.Get(tmpKey);
                            if (config.Name == name) {
                                sign = config.Sign;
                                break;
                            }
                        }
                        Type type = Assembly.Load("Assembly-CSharp").GetType(string.Concat("LazyPan.", sign));
                        Behaviour behaviour = (Behaviour) Activator.CreateInstance(type, entity, sign);
                        outBehaviour = behaviour;
                        instanceBehaviours.Add(behaviour);
                        BehaviourDic.TryAdd(id, instanceBehaviours);
                    } catch (Exception e) {
                        LogUtil.LogError(name);
                        throw;
                    }
                    
                    return true;
                }
            }

            outBehaviour = default;
            return false;
        }

        //删除注册的行为
        public static bool UnRegisterBehaviour(int id, string name) {
            int index = GetBehaviourIndex(id, name);
            //是否有实体
            if (index != -1) {
                if (BehaviourDic.TryGetValue(id, out List<Behaviour> behaviours)) {
                    behaviours[index].Clear();
                    behaviours.RemoveAt(index);
                    return true;
                }
            }

            return false;
        }
        
        //删除注册的行为
        public static bool UnRegisterAllBehaviour(int id) {
            if (BehaviourDic.TryGetValue(id, out List<Behaviour> behaviours)) {
                if (behaviours != null && behaviours.Count > 0) {
                    int index = behaviours.Count - 1;
                    for (int i = index; i >= 0; i--) {
                        UnRegisterBehaviour(id, behaviours[i].BehaviourName);
                    }
                }
                return true;
            }

            return false;
        }

        private static int GetBehaviourIndex(int id, string name) {
            int index = -1;
            //是否有实体
            if (BehaviourDic.TryGetValue(id, out List<Behaviour> behaviours)) {
                //判断实体已有当前行为
                for (var i = 0; i < behaviours.Count; i++) {
                    if (behaviours[i].BehaviourName == name) {
                        index = i;
                        break;
                    }
                }
            }

            return index;
        }

        public static bool GetBehaviour<T>(int id, out T t) where T : Behaviour {
            //是否有实体
            if (BehaviourDic.TryGetValue(id, out List<Behaviour> behaviours)) {
                //判断实体已有当前行为
                for (var i = 0; i < behaviours.Count; i++) {
                    Behaviour behaviour = behaviours[i];
                    if (behaviour.GetType() == typeof(T)) {
                        t = (T)behaviour;
                        return true;
                    }
                }
            }

            t = default;
            return false;
        }
        
        public static bool GetBehaviour<T>(Entity entity, out T t) where T : Behaviour {
            //是否有实体
            if (BehaviourDic.TryGetValue(entity.ID, out List<Behaviour> behaviours)) {
                //判断实体已有当前行为
                for (var i = 0; i < behaviours.Count; i++) {
                    Behaviour behaviour = behaviours[i];
                    if (behaviour.GetType() == typeof(T)) {
                        t = (T)behaviour;
                        return true;
                    }
                }
            }

            t = default;
            return false;
        }

        #endregion
    }
}