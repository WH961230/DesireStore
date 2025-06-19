using System;

namespace LazyPan {
    [Serializable]
    public class UserData {
        public string UserName;
        public int UserMoney;
        public int UserDesireCoin;
        public Medal UserMedal;
    }

    [Serializable]
    public enum Medal {
        CleverMan,
        VirtualCoinMaster,
    }
}