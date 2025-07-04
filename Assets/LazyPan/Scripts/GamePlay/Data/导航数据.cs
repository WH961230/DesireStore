using System;
using System.Collections.Generic;

namespace LazyPan {
    [Serializable]
    public class 导航数据 {
        public List<导航信息> 所有导航;//导航栏
    }

    [Serializable]
    public enum 导航信息 {
        无,
        灵谋,
        修行,
        蓄势,
        琳琅,
        博弈,
    }
}