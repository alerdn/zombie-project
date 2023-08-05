using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Netcode;
using UnityEngine;

public class InventoryLayout : NetworkBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private PlayerInventory _playerInventory;

    [Header("Containers")]
    [SerializeField] private GameObject _content;
    [SerializeField] private InventoryItemLayout _itemLayoutPrefab;
    [SerializeField] private List<InventoryContainerLayout> _containers;

    private bool _isInventoryOpen;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        _inputReader.ToggleInventoryEvent += ToggleInventory;
    }

    public void ToggleInventory()
    {
        if (!IsOwner) return;

        _isInventoryOpen = !_isInventoryOpen;

        if (_isInventoryOpen)
        {
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
        else
        {
            _content.SetActive(false);
            foreach (InventoryContainerLayout containerLayout in _containers)
            {
                if (containerLayout.gameObject.activeInHierarchy)
                {
                    containerLayout.Clear();
                    containerLayout.gameObject.SetActive(false);
                }
            }
        }
    }
}
