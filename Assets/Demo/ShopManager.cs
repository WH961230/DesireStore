using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour {
    public static ShopManager Instance;

    public List<Item> items = new List<Item>();

    [Header("UI References")] public Transform itemListContent;
    public GameObject itemPrefab;

    private void Awake() {
        if (Instance == null) Instance = this;
    }

    // 显示商店（测试时用 Debug.Log）
    public void ShowShop() {
        foreach (var item in items) {
            Debug.Log(item.id + ": " + item.name + " Price: " + item.price);
        }
    }

    // 购买商品
    public void BuyItem(int id) {
        Item item = items.Find(x => x.id == id);
        if (item != null) {
            if (UserManager.Instance.SpendCoins(item.price)) {
                item.isOwned = true;
                Debug.Log("Purchased: " + item.name);
            } else {
                Debug.Log("Not enough coins!");
            }
        }
    }

    public void DisplayUI() {
        foreach (Transform child in itemListContent) {
            Destroy(child.gameObject);
        }

        foreach (var item in items) {
            if (item.isOwned) {
                continue;
            }
            GameObject obj = Instantiate(itemPrefab, itemListContent);

            obj.transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = item.name;
            obj.transform.Find("PriceText").GetComponent<TextMeshProUGUI>().text = item.price.ToString();

            Button buyBtn = obj.transform.Find("BuyButton").GetComponent<Button>();
            buyBtn.onClick.RemoveAllListeners();
            buyBtn.onClick.AddListener(() => {
                BuyItem(item.id);
                DisplayUI(); // 购买后刷新界面
            });
            
            // 如果钱不够，禁用按钮
            if (UserManager.Instance.coins < item.price) {
                buyBtn.interactable = false;
            }
        }
    }
}