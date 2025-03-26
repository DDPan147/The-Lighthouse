using TMPro;
using UnityEngine;

public class IngredienteList : MonoBehaviour
{
    public string nombreComida;
    private TMP_Text nombre;
    private string cortadoText;
    private TMP_Text estadoCortado;
    private string rebozadoText;
    private TMP_Text estadoRebozado;
    public Comida comida;

    private void Awake()
    {
        nombre = transform.GetChild(0).GetComponent<TMP_Text>();
        estadoCortado = transform.GetChild(1).GetComponent<TMP_Text>();
        estadoRebozado = transform.GetChild(2).GetComponent<TMP_Text>();
        if (comida == null)
        {
            comida = GameObject.Find(nombreComida).GetComponent<Comida>();
        }
    }

    private void Start()
    {
        cortadoText = estadoCortado.text;
        rebozadoText = estadoRebozado.text;
        nombre.text = nombreComida;
    }

    private void Update()
    {
        if(comida.isCutted)
        {
            estadoCortado.color = Color.green;
            estadoCortado.text = "<s>" + cortadoText + "</s>";
        }
        if (comida.isRebozado)
        {
            estadoRebozado.color = Color.green;
            estadoRebozado.text = "<s>" + rebozadoText + "</s>";
        }
        if(comida.isCutted && comida.isRebozado)
        {
            nombre.color = Color.green;
            nombre.text = "<s>" + nombreComida + "</s>";
        }
    }


}
