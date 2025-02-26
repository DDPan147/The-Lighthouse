using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject player; // Referencia al jugador
    [SerializeField] private Camera cam; // Referencia a la cámara
    [SerializeField] private Transform cameraHolder; // Asignar manualmente en el inspector
    [Range(1, 150)] public float sensitivity = 15f;
    public float limitRotationCamera = 90f; // Limita la rotación vertical

    private Vector2 lookInput;
    private float vertRot;

    void Update()
    {
        RotateVertical();
        RotateHorizontal();
    }

    public void OnLookCallback(InputAction.CallbackContext context)
    {
        // Leer la entrada del ratón
        lookInput = context.ReadValue<Vector2>();

        // Calcular la rotación vertical
        vertRot -= lookInput.y * sensitivity * Time.deltaTime; // Usar el eje Y para la rotación vertical
        vertRot = Mathf.Clamp(vertRot, -limitRotationCamera, limitRotationCamera); // Limitar la rotación vertical
    }

    protected virtual void RotateVertical()
    {
        // Aplicar la rotación vertical al cameraHolder (solo afecta a la cámara)
        cameraHolder.localRotation = Quaternion.Euler(vertRot, 0f, 0f);
    }

    protected virtual void RotateHorizontal()
    {
        // Aplicar la rotación horizontal al jugador (gira en el eje Y)
        player.transform.Rotate(Vector3.up * lookInput.x * sensitivity * Time.deltaTime);
    }
}