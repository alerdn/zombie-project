using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

namespace ZombieProject.Core
{
    public class PlayerFirstVision : NetworkBehaviour
    {
        [Header("Movement")]
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Vector3 _foward;
        [SerializeField] private Vector3 _strafe;
        [SerializeField] private Vector3 _vertical;
        [SerializeField] private float _fowardSpeed = 5f;
        [SerializeField] private float _strafeSpeed = 5f;

        [Header("Jump")]
        [SerializeField] private float _gravity;
        [SerializeField] private float _jumpSpeed;
        [SerializeField] private float _jumpMaxHeight = 2f;
        [SerializeField] private float _timeToMaxHeight = .5f;

        [Header("Vision")]
        [SerializeField] private Transform _viewPoint;
        [SerializeField] private Vector2 _mouseInput;
        [SerializeField] private float _mouseSensitivity = 5f;
        [SerializeField] private float _verticalRotStore;
        [SerializeField] private Camera _cam;
        [SerializeField] private bool InvertLook = false;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;

            Cursor.lockState = CursorLockMode.Locked;

            _characterController = GetComponent<CharacterController>();

            _cam = Camera.main;

            _gravity = (-2 * _jumpMaxHeight) / (_timeToMaxHeight * _timeToMaxHeight);
            _jumpSpeed = (2 * _jumpMaxHeight) / _timeToMaxHeight;
        }

        private void Update()
        {
            if (!IsOwner) return;

            HandleMovement();
            HandleJump();
            HandleVision();
        }

        private void LateUpdate()
        {
            if (!IsOwner) return;

            if (_cam == null) _cam = Camera.main;

            _cam.transform.SetPositionAndRotation(_viewPoint.position, _viewPoint.rotation);
        }

        private void HandleMovement()
        {
            float forwardInput = Input.GetAxisRaw("Vertical");
            float strafeInput = Input.GetAxisRaw("Horizontal");

            _foward = forwardInput * _fowardSpeed * transform.forward;
            _strafe = strafeInput * _strafeSpeed * transform.right;

            Vector3 finalVelocity = _foward + _strafe + _vertical;

            _characterController.Move(finalVelocity * Time.deltaTime);
        }

        private void HandleJump()
        {
            _vertical += _gravity * Time.deltaTime * Vector3.up;

            if (Input.GetKeyDown(KeyCode.Space) && _characterController.isGrounded)
            {
                _vertical = _jumpSpeed * Vector3.up;
            }

            if (_characterController.isGrounded)
            {
                _vertical = Vector3.down;
            }
        }

        private void HandleVision()
        {
            _mouseInput = new Vector2(Input.GetAxisRaw("Mouse X") * _mouseSensitivity, Input.GetAxisRaw("Mouse Y")) * _mouseSensitivity;

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + _mouseInput.x, transform.rotation.eulerAngles.z);

            _verticalRotStore += _mouseInput.y;
            _verticalRotStore = Mathf.Clamp(_verticalRotStore, -60f, 60f);

            if (InvertLook) _viewPoint.rotation = Quaternion.Euler(_verticalRotStore, _viewPoint.rotation.eulerAngles.y, _viewPoint.rotation.eulerAngles.z);
            else _viewPoint.rotation = Quaternion.Euler(-_verticalRotStore, _viewPoint.rotation.eulerAngles.y, _viewPoint.rotation.eulerAngles.z);
        }
    }
}