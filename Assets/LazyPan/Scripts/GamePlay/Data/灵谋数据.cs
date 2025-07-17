using System;
using System.Collections.Generic;

namespace LazyPan {
    [Serializable]
    public class 灵谋数据 {
        public List<用户灵谋信息> 用户灵谋信息数据;
        
        #region 获取

        public bool 获取用户灵谋信息(string 用户名, out 用户灵谋信息 用户灵谋信息) {
            foreach (var tmp in 用户灵谋信息数据) {
                if (tmp.用户名 == 用户名) {
                    用户灵谋信息 = tmp;
                    return true;
                }
            }

            用户灵谋信息 = null;
            return false;
        }
        
        public bool 更新用户灵谋日志(string 用户名, List<灵谋日志> 更新的灵谋日志) {
            foreach (var tmp in 用户灵谋信息数据) {
                if (tmp.用户名 == 用户名) {
                    tmp.所有灵谋日志 = 更新的灵谋日志;
                    return true;
                }
            }

            return false;
        }

        #endregion
    }

    [Serializable]
    public class 用户灵谋信息 {
        public string 用户名;//用户名
        public List<灵谋日志> 所有灵谋日志;

        #region 获取

        public bool 获取第一个灵谋日志(bool 包含待编辑, out 灵谋日志 灵谋日志) {
            foreach (var tmp in 所有灵谋日志) {
                if (!包含待编辑 && tmp.编辑占用) {
                    continue;
                }
 
                灵谋日志 = tmp;
                return true;
            }

            灵谋日志 = null;
            return false;
        }

        public bool 获取所有灵谋日志(bool 包含待编辑, out List<灵谋日志> 返回灵谋日志) {
            返回灵谋日志 = new List<灵谋日志>();
            foreach (var tmp in 所有灵谋日志) {
                if (!包含待编辑 && tmp.编辑占用) {
                    continue;
                }
                
                返回灵谋日志.Add(tmp);
            }
            return false;
        }

        #endregion

        #region 新增

        public void 新增一个灵谋日志(灵谋日志 灵谋日志) {
            所有灵谋日志.Add(灵谋日志);
        }

        #endregion
    }

    [Serializable]
    public class 灵谋日志 {
        public string 标题;
        public string 内容;
        public bool 编辑占用;
    }
}