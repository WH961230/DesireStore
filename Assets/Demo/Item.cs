using System.Collections.Generic;

[System.Serializable]
public class Item {
    public int id;
    public string name;
    public bool isOwned;
    public int price;
}

[System.Serializable]
public class ItemData {
    public int nextItemIndex;
    public List<Item> item;
}