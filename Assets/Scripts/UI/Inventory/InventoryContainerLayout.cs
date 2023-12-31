using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryContainerLayout : MonoBehaviour
{
    [field: SerializeField] public ItemCategory ItemCategory { get; private set; }
    [SerializeField] private Transform _itemsParent;

    private List<InventoryItemLayout> _items = new();

    public void Show(InventoryContainer container, InventoryItemLayout itemLayoutPrefab)
    {
        if (_items.Count > 0) Clear();

        foreach (InventoryItem item in container.Items)
        {
            InventoryItemLayout itemLayout = Instantiate(itemLayoutPrefab, _itemsParent);
            itemLayout.Init(item.Data.Sprite, item.CurrentQuantity);

            _items.Add(itemLayout);
        }
    }

    public void Clear()
    {
        _items.ForEach(item => Destroy(item.gameObject));
        _items.Clear();
    }
}
