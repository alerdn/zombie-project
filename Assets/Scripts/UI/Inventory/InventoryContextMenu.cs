using UnityEngine;
using TMPro;
using System;

public class InventoryContextMenu : MonoBehaviour
{
    public event Action<ItemData> OnUseItem;
    public event Action<ItemData> OnDropItem;

    [SerializeField] private TMP_Text _titleText;

    private ItemData _data;

    public void Show(ItemData data, Vector3 position)
    {
        _data = data;
        transform.position = position;

        _titleText.text = _data.Name;

        gameObject.SetActive(true);
    }

    public void UseItem()
    {
        OnUseItem?.Invoke(_data);
    }

    public void DropItem()
    {
        OnDropItem?.Invoke(_data);
    }
}