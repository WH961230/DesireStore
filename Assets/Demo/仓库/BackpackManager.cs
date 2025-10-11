using System.Collections.Generic;
using LazyPan;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackpackManager : MonoBehaviour {
    public static BackpackManager Instance;

    public const string BACKPACKDATAFILENAME = "背包信息";

    public ItemData _itemData;

    [Header("UI References")] public Transform itemListContent;
    public GameObject itemPrefab;
    
    private void Awake() {
        if (Instance == null) Instance = this;
        
        //初始化用户信息
        _itemData = SaveLoad.Instance.Load<ItemData>(BACKPACKDATAFILENAME);
        if (_itemData == default) {
            _itemData = new ItemData();
            _itemData.item = new List<Item>();
            SaveLoad.Instance.Save(BACKPACKDATAFILENAME, _itemData);
        }
    }

    public void Get(Item item) {
        _itemData.item.Add(item);
        SaveLoad.Instance.Save(BACKPACKDATAFILENAME, _itemData);
        DisplayUI();
    }

    public void DisplayUI() {
        foreach (Transform child in itemListContent) {
            Destroy(child.gameObject);
        }

        int count = 0;
        foreach (var item in _itemData.item) {
            GameObject obj = Instantiate(itemPrefab, itemListContent);

            obj.transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = string.Concat("物品名:", item.name);
            obj.transform.Find("PriceText").GetComponent<TextMeshProUGUI>().text = string.Concat("价值币:", item.price.ToString());

            Button buyBtn = obj.transform.Find("BuyButton").GetComponent<Button>();
            buyBtn.gameObject.SetActive(false);

            count++;
        }

        Vector2 sizeDelta = itemListContent.GetComponent<RectTransform>().sizeDelta;
        sizeDelta = new Vector2(sizeDelta.x, 320 * count);
        itemListContent.GetComponent<RectTransform>().sizeDelta = sizeDelta;
    }
}