using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform cameraHolder;
    [Range(1, 150)] public float sensitivity = 15f;
    public float limitRotationCamera = 90f;

    public Vector2 _lookInput;
    public float _vertRot;
    public bool _isCursorLocked = false;

    private void Start()
    {
        // Bloquear el cursor al iniciar
        LockCursor();
    }

    private void Update()
    {
        // Comprobar si se presiona Escape para alternar el bloqueo del cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isCursorLocked)
                UnlockCursor();
            else
                LockCursor();
        }

        if (_isCursorLocked)
        {
            RotateVertical();
            RotateHorizontal();
        }
    }
    
    public void OnLookCallback(InputAction.CallbackContext context)
    {
        if (_isCursorLocked)
        {
            _lookInput = context.ReadValue<Vector2>();
            _vertRot -= _lookInput.y * sensitivity * Time.deltaTime;
            _vertRot = Mathf.Clamp(_vertRot, -limitRotationCamera, limitRotationCamera);
        }
    }

    #region Rotation

    protected virtual void RotateVertical()
    {
        cameraHolder.localRotation = Quaternion.Euler(_vertRot, 0f, 0f);
    }

    protected virtual void RotateHorizontal()
    {
        player.transform.Rotate(Vector3.up * (_lookInput.x * sensitivity * Time.deltaTime));
    }

    #endregion
    
    #region Lock Cursor
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _isCursorLocked = true;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _isCursorLocked = false;
    }
    private void OnDisable()
    {
        // Asegurarse de que el cursor se desbloquee cuando se desactive el script
        UnlockCursor();
    }
    #endregion
    
}