using System;
using System.Collections.Generic;

namespace LazyPan {
    [Serializable]
    public class 蓄势数据 {
        public List<用户蓄势信息> 用户蓄势信息数据;
    }

    [Serializable]
    public class 用户蓄势信息 {
        public string 用户名;//用户名
        public List<蓄势信息> 所有蓄势数据;
    }
    
    [Serializable]
    public class 蓄势信息 {
        public string 标题;
        public string 内容;
    }
}