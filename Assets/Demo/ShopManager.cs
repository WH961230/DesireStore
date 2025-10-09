using System.Collections.Generic;
using LazyPan;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour {
    public static ShopManager Instance;

    public const string SHOPDATAFILENAME = "商品信息";

    public ItemData _itemData;

    [Header("UI References")] public Transform itemListContent;
    public GameObject itemPrefab;

    public Button _shopAdd;
    public Comp _shopCreator;
    
    private void Awake() {
        if (Instance == null) Instance = this;
        
        //初始化用户信息
        _itemData = SaveLoad.Instance.Load<ItemData>(SHOPDATAFILENAME);
        if (_itemData == default) {
            _itemData = new ItemData();
            _itemData.item = new List<Item>();
            SaveLoad.Instance.Save(SHOPDATAFILENAME, _itemData);
        }
        
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
            id = ++_itemData.nextItemIndex,
            name = name,
            isOwned = false,
            price = price,
        };
        _itemData.item.Add(newTask);
        SaveLoad.Instance.Save(SHOPDATAFILENAME, _itemData);
        DisplayUI();
    }

    // 购买商品
    public void BuyItem(int id) {
        Item item = _itemData.item.Find(x => x.id == id);
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
        foreach (var item in _itemData.item) {
            if (item.isOwned) {
                continue;
            }
            GameObject obj = Instantiate(itemPrefab, itemListContent);

            obj.transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = string.Concat("商品名:", item.name);
            obj.transform.Find("PriceText").GetComponent<TextMeshProUGUI>().text = string.Concat("价格:", item.price.ToString());

            Button buyBtn = obj.transform.Find("BuyButton").GetComponent<Button>();
            buyBtn.onClick.RemoveAllListeners();
            buyBtn.onClick.AddListener(() => {
                BuyItem(item.id);
                DisplayUI(); // 购买后刷新界面
            });
            
            // 如果钱不够，禁用按钮
            if (UserManager.Instance._userData.coin < item.price) {
                buyBtn.interactable = false;
            }

            count++;
        }

        Vector2 sizeDelta = itemListContent.GetComponent<RectTransform>().sizeDelta;
        sizeDelta = new Vector2(sizeDelta.x, 300 * count);
        itemListContent.GetComponent<RectTransform>().sizeDelta = sizeDelta;
    }
}