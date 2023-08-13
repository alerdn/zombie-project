using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class InventoryLayout : NetworkBehaviour, IPointerClickHandler
{
    public event Action<ItemData> OnDropItem;

    [Header("Setup")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private PlayerInventory _playerInventory;
    [SerializeField] private InventoryContextMenu _contextMenu;

    [Header("Containers")]
    [SerializeField] private GameObject _content;
    [SerializeField] private InventoryItemLayout _itemLayoutPrefab;
    [SerializeField] private List<InventoryContainerLayout> _containers;

    private bool _isInventoryOpen;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        _containers.ForEach(container => container.OnContextMenu += OpenContextMenu);
        _contextMenu.OnUseItem += UseItem;
        _contextMenu.OnDropItem += DropItem;

        // Start Inputs
        _inputReader.OpenMenuEvent += OpenInventory;
        _inputReader.CloseMenuEvent += CloseInventory;
        _inputReader.ReturnMenuEvent += ReturnMenu;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        _containers.ForEach(container => container.OnContextMenu -= OpenContextMenu);
        _contextMenu.OnUseItem -= UseItem;
        _contextMenu.OnDropItem -= DropItem;

        _inputReader.OpenMenuEvent -= OpenInventory;
        _inputReader.CloseMenuEvent -= CloseInventory;
        _inputReader.ReturnMenuEvent -= ReturnMenu;
    }

    #region Item Action
    private void UseItem(ItemData data)
    {
        data.Use();
    }

    private void DropItem(ItemData data)
    {
        Vector2 playerPosition = new Vector2(_playerInventory.transform.position.x, _playerInventory.transform.position.z);
        Vector2 dropPosition = playerPosition + Random.insideUnitCircle * 5f;

        Vector3 dropConvertedPosition = new Vector3(dropPosition.x, _playerInventory.transform.position.y, dropPosition.y);
        Instantiate(data.Prefab, dropConvertedPosition, Quaternion.identity);
    }

    #endregion

    #region Context Menu

    private void OpenContextMenu(ItemData item, Vector3 position)
    {
        _contextMenu.Show(item, position);
    }

    private void CloseContextMenu()
    {
        _contextMenu.gameObject.SetActive(false);
    }

    #endregion

    #region Inventory

    private void OpenInventory()
    {
        if (!IsOwner) return;

        _content.SetActive(true);
        foreach (InventoryContainerLayout containerLayout in _containers)
        {
            InventoryContainer container = _playerInventory.Containers.Find(container => container.Data.ItemCategory == containerLayout.ItemCategory);
            if (container != null)
            {
                containerLayout.gameObject.SetActive(true);
                containerLayout.Show(container, _itemLayoutPrefab);
            }
            else
            {
                containerLayout.gameObject.SetActive(false);
            }
        }
    }

    private void CloseInventory()
    {
        if (!IsOwner) return;

        _content.SetActive(false);
        CloseContextMenu();
        foreach (InventoryContainerLayout containerLayout in _containers)
        {
            if (containerLayout.gameObject.activeInHierarchy)
            {
                containerLayout.Clear();
                containerLayout.gameObject.SetActive(false);
            }
        }
    }

    #endregion

    private void ReturnMenu()
    {
        if (_contextMenu.gameObject.activeInHierarchy)
        {
            CloseContextMenu();
            return;
        }

        // Chamando evento de CloseMenu para triggar todos os lugares que estão ouvindo
        _inputReader.CloseMenuEvent?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Clique em qualquer lugar do inventário para fechar o Context Menu
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            CloseContextMenu();
        }
    }
}
