﻿using System;
using System.Collections.Generic;

namespace LazyPan {
    [Serializable]
    public class 灵谋数据 {
        public List<用户灵谋信息> 用户灵谋信息数据;
    }

    [Serializable]
    public class 用户灵谋信息 {
        public string 用户名;//用户名
        public List<灵谋信息> 所有灵谋数据;
    }

    [Serializable]
    public class 灵谋信息 {
        public string 标题;
        public string 内容;
    }
}