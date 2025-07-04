using System;
using System.Collections.Generic;

namespace LazyPan {
    [Serializable]
    public class 修行数据 {
        public List<用户修行信息> 用户修行信息数据;
    }

    [Serializable]
    public class 用户修行信息 {
        public string 用户名;//用户名
        public List<修行信息> 所有修行数据;
    }

    [Serializable]
    public class 修行信息 {
        public string 标题;
        public string 内容;
        public bool 完成;
    }
}