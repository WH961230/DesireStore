using LazyPan;
using TMPro;
using UnityEngine;

public class UserManager : MonoBehaviour {
    public static UserManager Instance;
    public const string USERDATAFILENAME = "用户信息";
    public User _userData;
    
    [Header("UI References")]
    public TextMeshProUGUI userNameText;
    public TextMeshProUGUI coinsText;

    private void Awake() {
        if (Instance == null) Instance = this;
        //初始化用户信息
        _userData = SaveLoad.Instance.Load<User>(USERDATAFILENAME);
        if (_userData == default) {
            _userData = new User();
            _userData.name = "Evoreek 野鹤";
            _userData.coin = 0;
            SaveLoad.Instance.Save(USERDATAFILENAME, _userData);
        }
    }

    public void AddCoins(int amount) {
        _userData.coin += amount;
        SaveLoad.Instance.Save(USERDATAFILENAME, _userData);
    }

    public bool SpendCoins(int amount) {
        if (_userData.coin >= amount) {
            _userData.coin -= amount;
            SaveLoad.Instance.Save(USERDATAFILENAME, _userData);
            return true;
        }

        return false;
    }

    public void DisplayUI() {
        if (userNameText != null) userNameText.text = string.Concat("昵称: ", _userData.name);
        if (coinsText != null) coinsText.text = string.Concat("欲望币: ", _userData.coin);
    }
}