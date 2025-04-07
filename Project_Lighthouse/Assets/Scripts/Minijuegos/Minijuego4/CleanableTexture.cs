using DG.Tweening.Core.Easing;
using UnityEngine;

public class CleanableTexture : MonoBehaviour
{
    [Header("Texture Settings")]
    [SerializeField] private Texture2D dirtMaskTexture;
    [SerializeField] private int textureSize = 512;
    [SerializeField] private float brushSize = 32f;

    [Header("Cleaning Progress")]
    [SerializeField] private float cleanThreshold = 0.95f; // Consider clean when 75% is cleaned
    [SerializeField] private bool isCleaningComplete = false;
    private int totalPixels;
    private int cleanedPixels = 0;

    [Header("References")]
    [SerializeField] private Material material;
    private CameraController playerCamera;
    private Camera playerCameraObject;

    private void Start()
    {
        // Obtener referencias
        playerCamera = GameObject.Find("Player_Grand").GetComponent<CameraController>();
        playerCameraObject = GameObject.Find("CameraHolder").GetComponent<Camera>();

        if (material == null)
        {
            material = GetComponent<Renderer>().material;
        }

        totalPixels = textureSize * textureSize;
        InitializeTexture();
    }

    private void InitializeTexture()
    {
        try
        {
            // Si no hay textura asignada, crear una nueva
            if (dirtMaskTexture == null)
            {
                dirtMaskTexture = new Texture2D(textureSize, textureSize);
                Color[] colors = new Color[textureSize * textureSize];

                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = Color.black;
                }

                dirtMaskTexture.SetPixels(colors);
                dirtMaskTexture.Apply();
            }

            // Verificar que tenemos el material
            if (material == null)
            {
                Debug.LogError("Material no asignado en " + gameObject.name);
                return;
            }

            // Asignar la textura al material
            material.SetTexture("_DirtMask", dirtMaskTexture);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error inicializando textura: {e.Message}");
        }
    }

    private void OnMouseDown()
    {
        if (playerCamera == null)
        {
            Debug.LogError("PlayerCamera no encontrada");
            return;
        }

        Debug.Log("Textura click");
        playerCamera.UnlockCursor();
        CleanTexture();
    }

    private void OnMouseDrag()
    {
        CleanTexture();
    }

    private void CleanTexture()
    {
        // Verificar que tenemos todas las referencias necesarias
        if (dirtMaskTexture == null || material == null)
        {
            Debug.LogError("Faltan referencias necesarias en " + gameObject.name);
            return;
        }

        if (Physics.Raycast(playerCameraObject.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit))
        {
            Vector2 textureCoord = raycastHit.textureCoord;

            int pixelX = (int)(textureCoord.x * dirtMaskTexture.width);
            int pixelY = (int)(textureCoord.y * dirtMaskTexture.height);

            PaintCircle(new Vector2Int(pixelX, pixelY), (int)brushSize);
        }
    }

    private void PaintCircle(Vector2Int center, int radius)
    {
        int newCleanedPixels = 0;
        for (int x = -radius; x < radius; x++)
        {
            for (int y = -radius; y < radius; y++)
            {
                if (x * x + y * y <= radius * radius)
                {
                    int pixelX = center.x + x;
                    int pixelY = center.y + y;

                    if (pixelX >= 0 && pixelX < dirtMaskTexture.width &&
                        pixelY >= 0 && pixelY < dirtMaskTexture.height)
                    {
                        // Check if pixel was previously dirty
                        Color currentColor = dirtMaskTexture.GetPixel(pixelX, pixelY);
                        if (currentColor.r < 0.5f) // If it's dark/dirty
                            newCleanedPixels++;

                        dirtMaskTexture.SetPixel(pixelX, pixelY, Color.white);
                    }
                }
            }
        }

        dirtMaskTexture.Apply();

        // Update cleaning progress
        cleanedPixels += newCleanedPixels;
        CheckCleaningProgress();
    }

    private void CheckCleaningProgress()
    {
        float cleanedPercentage = (float)cleanedPixels / totalPixels;

        if (!isCleaningComplete && cleanedPercentage >= cleanThreshold)
        {
            isCleaningComplete = true;
            OnCleaningComplete();
        }
    }

    private void OnCleaningComplete()
    {
        Debug.Log("Cleaning complete!");

        MinigameFourManager.Instance.OnCleaningComplete();
    }
}