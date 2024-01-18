using System;
using DefaultNamespace;
using UnityEngine;

public class SimpleController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform stencilTransform;
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float stencilScaleSpeed;
    [SerializeField] private float stencilMaxScale;
    
    [Header("Raycast")] 
    [SerializeField] private LayerMask rayLayer;
    [SerializeField] private float rayDistance;
    [SerializeField] private Vector3 rayOffset;
    
    private CharacterController _controller;

    private Vector2 _lastKeyboardInput;
    private Vector2 _lastMouseInput;
    
    private int _stencilScaleDirection = 0;

    private bool _collectRequest;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + rayOffset, transform.forward * rayDistance);
    }

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float delta = Time.deltaTime;
        UpdateInput();
        ApplyCameraRotation(delta);
    }

    private void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;
        ApplyCharacterMovement(delta);
        ApplyGravity(delta);
        TryCollect();
    }

    private void TryCollect()
    {
        if (!_collectRequest)
            return;

        _collectRequest = false;

        Ray ray = new Ray(transform.position + rayOffset, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, rayLayer))
        {
            if (hit.collider.TryGetComponent(out ICollectbile collectible))
            {
                collectible.OnCollect();
            }
        }
    }

    private void ApplyGravity(float t)
    {
        _controller.Move(Physics.gravity * t);
    }

    private void ApplyCameraRotation(float t)
    {
        cameraTransform.localEulerAngles += new Vector3(
            -_lastMouseInput.y * (rotateSpeed * t), _lastMouseInput.x * (rotateSpeed * t), 0);
    }
    
    private void ApplyCharacterMovement(float t)
    {
        Vector3 moveVector = cameraTransform.forward * _lastKeyboardInput.y + cameraTransform.right * _lastKeyboardInput.x;
        _controller.Move(moveVector * (moveSpeed * t));
        
        
        // scale can't be negative
        if (_stencilScaleDirection == -1 && stencilTransform.localScale.x <= 0.0f)
        {
            _stencilScaleDirection = 0;
            stencilTransform.localScale = Vector3.zero;
        }
        
        // scale can't be bigger than max scale
        if (_stencilScaleDirection == 1 && stencilTransform.localScale.x >= stencilMaxScale)
            _stencilScaleDirection = 0;
        Vector3 s = stencilTransform.localScale + Vector3.one * (_stencilScaleDirection * stencilScaleSpeed * t);
        
        s.y = s.x / 1.5f;
        stencilTransform.localScale = s;
    }

    private void UpdateInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        _lastKeyboardInput = new Vector2(horizontal, vertical).normalized;
        
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        _lastMouseInput = new Vector2(mouseX, mouseY).normalized;
        
        if (Input.GetKey(KeyCode.C))
            _stencilScaleDirection = 1;
        else if (Input.GetKey(KeyCode.V))
            _stencilScaleDirection = -1;
        else
            _stencilScaleDirection = 0;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _collectRequest = true;
        }
    }
}
