using System;
using System.Collections.Generic;

namespace LazyPan {
    [Serializable]
    public class NavigationData {
        public List<Navigation> Navigation;//导航栏
    }
    
    [Serializable]
    public enum Navigation {
        无,
        灵谋,
        修行,
        蓄势,
        琳琅,
        博弈,
    }
}