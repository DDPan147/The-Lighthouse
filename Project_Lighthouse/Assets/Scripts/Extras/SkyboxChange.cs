using System;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.VFX;

public class SkyboxChange : MonoBehaviour
{
    [Header("Water Settings")]
    public Color[] dayWaterColour;
    public Color[] nightWaterColour;
    [Header("SkyBox Settings")]
    public Material daySkybox; 
    public Material nightSkybox; 
    private Material waterShaderMat;
    [Header("Light Settings")]
    public GameObject directionalLight;
    private Light _directionalLight;
    public Vector3 dayRotation, nightRotation;
    public Color dayLight;
    public Color nightLight;
    [Header("Fog Settings")]
    public VisualEffect fogVFX;
    public Color dayFogColor;
    public Color nightFogColor;

    private void Awake()
    {
        waterShaderMat = GetComponent<Renderer>().material;
        if(directionalLight == null)
        {
            Debug.LogError("No hay ninguna Directional Light assignada");
        }
        else
        {
            _directionalLight = directionalLight.GetComponent<Light>();
        }
        
        /*Debug.Log(mat.shader.name);
        waterShader = mat.shader;
        waterShader.*/
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetNightColours();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetDayColours();
        }
    }

    public void SetDayColours()
    {
        waterShaderMat.SetColor("_DeepWaterColor", dayWaterColour[0]);
        waterShaderMat.SetColor("_WaterColor", dayWaterColour[1]);
        waterShaderMat.SetColor("_ShallowWaterColor", dayWaterColour[2]);
        UnityEngine.RenderSettings.skybox = daySkybox;
        directionalLight.transform.eulerAngles = dayRotation;
        _directionalLight.color = dayLight;
        fogVFX.SetVector4("FogColor", dayFogColor);
    }
    public void SetNightColours()
    {
        waterShaderMat.SetColor("_DeepWaterColor", nightWaterColour[0]);
        waterShaderMat.SetColor("_WaterColor", nightWaterColour[1]);
        waterShaderMat.SetColor("_ShallowWaterColor", nightWaterColour[2]);
        UnityEngine.RenderSettings.skybox = nightSkybox;
        directionalLight.transform.eulerAngles = nightRotation;
        _directionalLight.color = nightLight;
        fogVFX.SetVector4("FogColor", nightFogColor);
    }

}
