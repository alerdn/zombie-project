using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace ZombieProject.Core
{
    public class Player : NetworkBehaviour
    {
        public NetworkVariable<bool> IsShooting = new NetworkVariable<bool>();
        public PlayerInventory InventoryComponent { get; private set; }

        [Header("Input")]
        [SerializeField] private InputReader _inputReader;

        [Header("Movement")]
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Vector3 _foward;
        [SerializeField] private Vector3 _strafe;
        [SerializeField] private Vector3 _vertical;
        [SerializeField] private float _fowardSpeed = 5f;
        [SerializeField] private float _strafeSpeed = 5f;
        [SerializeField] private Transform _groundCheckTransform;

        [Header("Jump")]
        [SerializeField] private float _gravity;
        [SerializeField] private float _jumpSpeed;
        [SerializeField] private float _jumpMaxHeight = 2f;
        [SerializeField] private float _timeToMaxHeight = .5f;
        [SerializeField] private bool _isGrounded;
        [SerializeField] private LayerMask _groundLayers;

        [Header("Vision")]
        [SerializeField] private Transform _viewPoint;
        [SerializeField] private Vector2 _mouseInput;
        [SerializeField] private float _mouseSensitivity = 1f;
        [SerializeField] private float _verticalRotStore;
        [SerializeField] private CinemachineVirtualCamera _vcam;
        [SerializeField] private bool InvertLook = false;

        private Vector2 _movementDirection;
        private Vector2 _mouseDirection;
        private bool _jump;
        private bool _isRuning;

        [Header("Survival")]
        [SerializeField] private PlayerUIReferences _playerUI;
        private bool _hasShiftUp = false;

        [Header("Gun")]
        [SerializeField] private List<Gun> _allGuns;
        private int _currentGun;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;

            _playerUI.gameObject.SetActive(true);

            Cursor.lockState = CursorLockMode.Locked;

            _characterController = GetComponent<CharacterController>();
            InventoryComponent = GetComponent<PlayerInventory>();

            _vcam.Priority = 15;

            _gravity = (-2 * _jumpMaxHeight) / (_timeToMaxHeight * _timeToMaxHeight);
            _jumpSpeed = (2 * _jumpMaxHeight) / _timeToMaxHeight;

            _inputReader.MovementEvent += HandleMovementInputs;
            _inputReader.LookEvent += HandleLookInput;
            _inputReader.ShootEvent += HandleShootInput;
            _inputReader.JumpEvent += HandleJumpInput;
            _inputReader.RunEvent += HandleRunInput;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner) return;

            _inputReader.MovementEvent -= HandleMovementInputs;
            _inputReader.LookEvent -= HandleLookInput;
            _inputReader.ShootEvent -= HandleShootInput;
            _inputReader.JumpEvent -= HandleJumpInput;
            _inputReader.RunEvent -= HandleRunInput;
        }

        private void Update()
        {
            if (!IsOwner) return;

            HandleMovement();
            HandleJump();
            HandleVision();
            HandleRun();
        }

        private void HandleMovement()
        {
            float forwardInput = _movementDirection.y;
            float strafeInput = _movementDirection.x;

            if (_isRuning)
            {
                _fowardSpeed = 8f;
                _strafeSpeed = 8f;
            }
            else
            {
                _fowardSpeed = 5f;
                _strafeSpeed = 5f;
            }

            _foward = forwardInput * _fowardSpeed * transform.forward;
            _strafe = strafeInput * _strafeSpeed * transform.right;

            Vector3 finalVelocity = _foward + _strafe + _vertical;

            _characterController.Move(finalVelocity * Time.deltaTime);
        }

        private void HandleJump()
        {
            _vertical += _gravity * Time.deltaTime * Vector3.up;

            if (_characterController.isGrounded)
            {
                _vertical = Vector3.down;
            }

            _isGrounded = Physics.Raycast(_groundCheckTransform.position, Vector3.down, .25f, _groundLayers);

            // Alterar para controle nos Inputs
            if (_jump && _isGrounded)
            {
                _vertical = _jumpSpeed * Vector3.up;
            }
        }

        private void HandleVision()
        {
            _mouseInput = new Vector2(_mouseDirection.x * _mouseSensitivity, _mouseDirection.y) * _mouseSensitivity;

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + _mouseInput.x, transform.rotation.eulerAngles.z);

            _verticalRotStore += _mouseInput.y;
            _verticalRotStore = Mathf.Clamp(_verticalRotStore, -50f, 60f);

            if (InvertLook) _viewPoint.rotation = Quaternion.Euler(_verticalRotStore, _viewPoint.rotation.eulerAngles.y, _viewPoint.rotation.eulerAngles.z);
            else _viewPoint.rotation = Quaternion.Euler(-_verticalRotStore, _viewPoint.rotation.eulerAngles.y, _viewPoint.rotation.eulerAngles.z);
        }

        private void HandleMovementInputs(Vector2 direction)
        {
            _movementDirection = direction.normalized;
        }

        private void HandleLookInput(Vector2 direction)
        {
            _mouseDirection = direction;
        }

        private void HandleShootInput(bool isShooting)
        {
            if (!_allGuns[_currentGun].IsAutomatic)
            {
                if (!isShooting) return;
            }
            _allGuns[_currentGun].Shoot(_viewPoint.position, _viewPoint.forward);
        }

        private void HandleJumpInput(bool jump)
        {
            _jump = jump;
        }

        private void HandleRunInput(bool isRuning)
        {
            _hasShiftUp = !isRuning;
            if (!_hasShiftUp) Eat();
            if (!_playerUI.Stammina.HasStammina) return;
            _isRuning = isRuning;
        }

        private void HandleRun()
        {
            if (!_playerUI.Stammina.HasStammina || _movementDirection == Vector2.zero) _isRuning = false;

            _playerUI.Stammina.HandleStammina(_isRuning, _hasShiftUp);
        }

        private void Eat()
        {
            StartCoroutine(_playerUI.Hungry.ReplenishHungry(50));
        }
    }
}