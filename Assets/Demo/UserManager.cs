using TMPro;
using UnityEngine;

public class UserManager : MonoBehaviour {
    public static UserManager Instance;

    public string userName = "Player";
    public int coins = 0;
    
    [Header("UI References")]
    public TextMeshProUGUI userNameText;
    public TextMeshProUGUI coinsText;

    private void Awake() {
        if (Instance == null) Instance = this;
    }

    public void AddCoins(int amount) {
        coins += amount;
        Debug.Log("Coins: " + coins);
    }

    public bool SpendCoins(int amount) {
        if (coins >= amount) {
            coins -= amount;
            return true;
        }

        return false;
    }

    public void DisplayUI() {
        if (userNameText != null) userNameText.text = userName;
        if (coinsText != null) coinsText.text = "Coins: " + coins;
    }
}