using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemLayout : MonoBehaviour, IPointerClickHandler
{
    public event Action<ItemData, Vector3> OnContextMenu;

    [SerializeField] private Image _sprite;
    [SerializeField] private GameObject _quantityBox;
    [SerializeField] private TMP_Text _quantity;

    private ItemData _itemData;

    public void Init(InventoryItem item)
    {
        _itemData = item.Data;
        _sprite.sprite = _itemData.Sprite;

        if (item.CurrentQuantity > 1)
        {
            _quantity.text = item.CurrentQuantity.ToString();
            _quantityBox.SetActive(true);
        }
        else
        {
            _quantityBox.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OpenContextMenu(eventData.position);
        }
    }

    private void OpenContextMenu(Vector3 position)
    {
        OnContextMenu?.Invoke(_itemData, position);
    }
}
