using System;

namespace LazyPan {
    [Serializable]
    public class UserData {
        public string UserName;//用户名
        public int UserMoney;//用户钱
        public int UserDesireCoin;//用户欲望币
        public Medal UserMedal;//用户勋章
    }

    [Serializable]
    public enum Medal {
        无,
        勋章一,
        勋章二,
    }
}