using System;
using UnityEngine;

public class SkyboxChange : MonoBehaviour
{
    public Color[] dayWaterColour;
    public Color[] nightWaterColour;
    public Material daySkybox; 
    public Material nightSkybox; 
    private Material mat;
    private void Awake()
    {
        mat = GetComponent<Renderer>().material;
        
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

    void SetDayColours()
    {
        mat.SetColor("_DeepWaterColor", dayWaterColour[0]);
        mat.SetColor("_WaterColor", dayWaterColour[1]);
        mat.SetColor("_ShallowWaterColor", dayWaterColour[2]);
        RenderSettings.skybox = daySkybox;
    }
    void SetNightColours()
    {
        mat.SetColor("_DeepWaterColor", nightWaterColour[0]);
        mat.SetColor("_WaterColor", nightWaterColour[1]);
        mat.SetColor("_ShallowWaterColor", nightWaterColour[2]);
        RenderSettings.skybox = nightSkybox;
    }

}
