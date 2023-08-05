using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemLayout : MonoBehaviour
{
    [SerializeField] private Image _sprite;
    [SerializeField] private GameObject _quantityBox;
    [SerializeField] private TMP_Text _quantity;

    public void Init(Sprite sprite, int quantity)
    {
        _sprite.sprite = sprite;

        if (quantity > 1)
        {
            _quantity.text = quantity.ToString();
            _quantityBox.SetActive(true);
        }
        else
        {
            _quantityBox.SetActive(false);
        }
    }
}
