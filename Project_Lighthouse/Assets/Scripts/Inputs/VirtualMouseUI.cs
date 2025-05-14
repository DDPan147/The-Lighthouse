using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using UnityEngine.Playables;

public class VirtualMouseUI : MonoBehaviour
{
    [SerializeField] private RectTransform canvasRectTransform;

    [HideInInspector]public VirtualMouseInput virtualMouseInput;

    private void Awake()
    {
        virtualMouseInput = FindAnyObjectByType<VirtualMouseInput>();
    }

    private void Update()
    {
        transform.localScale = Vector3.one * 1f/canvasRectTransform.localScale.x;
        transform.SetAsLastSibling();
    }

    private void LateUpdate()
    {
        Vector2 virtualMousePosition = virtualMouseInput.virtualMouse.position.value;
        virtualMousePosition.x = Math.Clamp(virtualMousePosition.x, 0f, Screen.width);
        virtualMousePosition.y = Math.Clamp(virtualMousePosition.y, 0f, Screen.height);
        InputState.Change(virtualMouseInput.virtualMouse.position, virtualMousePosition);
    }
}
