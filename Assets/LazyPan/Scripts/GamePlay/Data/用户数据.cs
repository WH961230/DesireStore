using System;

namespace LazyPan {
    [Serializable]
    public class 用户数据 {
        public string 用户名;//用户名
        public int 用户钱;//用户钱
        public int 用户欲望币;//用户欲望币
        public 勋章 用户勋章;//用户勋章
    }

    [Serializable]
    public enum 勋章 {
        无,
        勋章一,
        勋章二,
    }
}