using System.Collections.Generic;
using LazyPan;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour {
    public static ShopManager Instance;

    public List<Item> items = new List<Item>();
    private int nextId = 1;

    [Header("UI References")] public Transform itemListContent;
    public GameObject itemPrefab;

    public Button _shopAdd;
    public Comp _shopCreator;
    
    private void Awake() {
        if (Instance == null) Instance = this;
        
        _shopAdd.onClick.AddListener(() => {
            _shopCreator.gameObject.SetActive(true);
            TMP_InputField name = Cond.Instance.Get<TMP_InputField>(_shopCreator, "名字"); 
            name.text = "";
            TMP_InputField price = Cond.Instance.Get<TMP_InputField>(_shopCreator, "价格");
            price.text = "";
            Button add = Cond.Instance.Get<Button>(_shopCreator, "创建");
            add.onClick.RemoveAllListeners();
            add.onClick.AddListener(() => {
                CreateShop(name.text, int.Parse(price.text));
                _shopCreator.gameObject.SetActive(false);
            });
        });
    }

    // 创建任务
    public void CreateShop(string name, int price) {
        Item newTask = new Item {
            id = nextId++,
            name = name,
            isOwned = false,
            price = price,
        };
        items.Add(newTask);
        DisplayUI();
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

        int count = 0;
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

            count++;
        }

        Vector2 sizeDelta = itemListContent.GetComponent<RectTransform>().sizeDelta;
        sizeDelta = new Vector2(sizeDelta.x, 300 * count);
        itemListContent.GetComponent<RectTransform>().sizeDelta = sizeDelta;
    }
}