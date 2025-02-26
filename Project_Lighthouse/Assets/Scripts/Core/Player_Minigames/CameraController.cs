using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject player; // Referencia al jugador
    [SerializeField] private Camera cam; // Referencia a la c�mara
    [SerializeField] private Transform cameraHolder; // Asignar manualmente en el inspector
    [Range(1, 150)] public float sensitivity = 15f;
    public float limitRotationCamera = 90f; // Limita la rotaci�n vertical

    private Vector2 lookInput;
    private float vertRot;

    void Update()
    {
        RotateVertical();
        RotateHorizontal();
    }

    public void OnLookCallback(InputAction.CallbackContext context)
    {
        // Leer la entrada del rat�n
        lookInput = context.ReadValue<Vector2>();

        // Calcular la rotaci�n vertical
        vertRot -= lookInput.y * sensitivity * Time.deltaTime; // Usar el eje Y para la rotaci�n vertical
        vertRot = Mathf.Clamp(vertRot, -limitRotationCamera, limitRotationCamera); // Limitar la rotaci�n vertical
    }

    protected virtual void RotateVertical()
    {
        // Aplicar la rotaci�n vertical al cameraHolder (solo afecta a la c�mara)
        cameraHolder.localRotation = Quaternion.Euler(vertRot, 0f, 0f);
    }

    protected virtual void RotateHorizontal()
    {
        // Aplicar la rotaci�n horizontal al jugador (gira en el eje Y)
        player.transform.Rotate(Vector3.up * lookInput.x * sensitivity * Time.deltaTime);
    }
}