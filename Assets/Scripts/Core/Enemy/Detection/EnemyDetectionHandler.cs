using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace ZombieProject.Core
{
    public class EnemyDetectionHandler : NetworkBehaviour
    {
        public event Action<Player> OnSpotPlayer;

        [SerializeField] private List<EnemyDetection> _detections;

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;

            _detections.ForEach(detection => detection.OnSpotPlayer += OnSpotPlayer);
        }
    }
}