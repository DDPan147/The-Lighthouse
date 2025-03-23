using TMPro;
using UnityEngine;

public class IngredienteList : MonoBehaviour
{
    public string nombreComida;
    private TMP_Text nombre;
    private TMP_Text estadoPelado;
    private TMP_Text estadoRebozado;
    public Comida_Cortada comida;

    private void Awake()
    {
        nombre = transform.GetChild(0).GetComponent<TMP_Text>();
        estadoPelado = transform.GetChild(1).GetComponent<TMP_Text>();
        estadoRebozado = transform.GetChild(2).GetComponent<TMP_Text>();
        if (comida == null)
        {
            comida = GameObject.Find(nombreComida).GetComponent<Comida_Cortada>();
        }
    }

    private void Start()
    {
        nombre.text = nombreComida;
    }

    private void Update()
    {
        if(comida.isCutted)
        {
            estadoPelado.color = Color.green;
        }
        if (comida.isRebozado)
        {
            estadoRebozado.color = Color.green;
        }
        if(comida.isCutted && comida.isRebozado)
        {
            nombre.color = Color.green;
        }
    }


}
