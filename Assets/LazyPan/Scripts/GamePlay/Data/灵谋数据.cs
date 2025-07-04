using System;
using System.Collections.Generic;

namespace LazyPan {
    [Serializable]
    public class 灵谋数据 {
        public List<灵谋信息> 所有灵谋数据;
    }

    [Serializable]
    public class 灵谋信息 {
        public string 标题;
        public string 内容;
    }
}