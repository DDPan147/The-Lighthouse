using System;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.VFX;

public class SkyboxChange : MonoBehaviour
{
    public static SkyboxChange instance;
    [Header("Water Settings")]
    public Color[] dayWaterColour;
    public Color[] nightWaterColour;
    [Header("SkyBox Settings")]
    public Material daySkybox; 
    public Material nightSkybox;
    public Material MJ9Skybox;
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
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
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

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetNightColours();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetDayColours();
        }
    }*/

    public void SetDayColours()
    {
        waterShaderMat.SetColor("_DeepWaterColor", dayWaterColour[0]);
        waterShaderMat.SetColor("_WaterColor", dayWaterColour[1]);
        waterShaderMat.SetColor("_ShallowWaterColor", dayWaterColour[2]);
        UnityEngine.RenderSettings.skybox = daySkybox;
        directionalLight.transform.eulerAngles = dayRotation;
        _directionalLight.color = dayLight;
        _directionalLight.intensity = 2;
        _directionalLight.shadows = LightShadows.Soft;
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
        _directionalLight.intensity = 0.5f;
        _directionalLight.shadows = LightShadows.None;
        fogVFX.SetVector4("FogColor", nightFogColor);
    }

    public void SetMinigame9Skybox()
    {
        UnityEngine.RenderSettings.skybox = MJ9Skybox;
    }

}
