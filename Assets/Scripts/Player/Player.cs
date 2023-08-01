using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Cinemachine;


public class Player : NetworkBehaviour
{
    public NetworkVariable<bool> IsShooting = new NetworkVariable<bool>();

    [SerializeField] private float _moveSpeed = 25f;
    [SerializeField] private InputReader _inputReader;

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera _vcam;
    [SerializeField] private int _vcamPriority = 15;

    private Vector2 _movementDirection;
    private CharacterController _charController;
    private float _turnSmoothVelocity;
    private Transform _cam;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        _charController = GetComponent<CharacterController>();
        _cam = Camera.main.transform;

        _vcam.Priority = _vcamPriority;

        _inputReader.MovementEvent += HandleMovementInputs;
        _inputReader.ShootEvent += HandleShootInputServerRpc;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        _inputReader.MovementEvent -= HandleMovementInputs;
        _inputReader.ShootEvent -= HandleShootInputServerRpc;
    }

    private void Update()
    {
        if (!IsOwner) return;

        HandleMovement();
    }

    private void HandleMovementInputs(Vector2 direction)
    {
        _movementDirection = direction;
    }

    [ServerRpc]
    private void HandleShootInputServerRpc(bool isShooting)
    {
        IsShooting.Value = isShooting;
    }

    private void HandleMovement()
    {
        float horizontal = _movementDirection.x;
        float vertical = _movementDirection.y;

        if (_cam == null) _cam = Camera.main.transform;

        float targetAngle = Mathf.Atan2(horizontal, vertical) * Mathf.Rad2Deg + _cam.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, .1f);
        Vector3 speedVector = (Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward).normalized * _moveSpeed;

        // Check if player is moving
        bool isWalking = _movementDirection.magnitude >= .1;
        if (isWalking)
        {
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
        else
        {
            speedVector = Vector3.zero;
        }

        // Check if player is grounded
        if (!_charController.isGrounded)
        {
            speedVector.y -= 9.87f * Time.deltaTime;
        }

        // Moving player
        _charController.Move(speedVector * Time.deltaTime);
    }
}
