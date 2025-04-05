using TMPro;
using UnityEngine;

public class IngredienteList : MonoBehaviour
{
    public string nombreComida;
    private string[] estadosText = new string[4];
    private TMP_Text[] estadoComida = new TMP_Text[4]; // 0: Cortado; 1: Rebozado; 2: Pelado; 3: Extra
    private TMP_Text nombre;
    private string cortadoText;
    private TMP_Text estadoCortado;
    private string rebozadoText;
    private TMP_Text estadoRebozado;
    public Comida comida;

    private void Awake()
    {
        
        
    }

    private void OnEnable()
    {
        if (comida == null)
        {
            comida = GameObject.Find(nombreComida).GetComponent<Comida>();
        }
        nombre = transform.GetChild(0).GetComponent<TMP_Text>();
        switch (comida.tipoComida)
        {
            case Comida.TipoComida.Patata:
                estadoComida[0] = transform.GetChild(1).GetComponent<TMP_Text>();
                //estadoComida[1] = transform.GetChild(2).GetComponent<TMP_Text>();
                estadoComida[2] = transform.GetChild(3).GetComponent<TMP_Text>();
                break;
            case Comida.TipoComida.Zanahoria:
                estadoComida[0] = transform.GetChild(1).GetComponent<TMP_Text>();
                //estadoComida[1] = transform.GetChild(2).GetComponent<TMP_Text>();
                estadoComida[2] = transform.GetChild(3).GetComponent<TMP_Text>();
                break;
            case Comida.TipoComida.Pescado:
                estadoComida[0] = transform.GetChild(1).GetComponent<TMP_Text>();
                estadoComida[1] = transform.GetChild(2).GetComponent<TMP_Text>();
                break;
            case Comida.TipoComida.RestosPescado:
                break;
        }
    }

    private void Start()
    {
        switch (comida.tipoComida)
        {
            case Comida.TipoComida.Patata:
                estadosText[0] = estadoComida[0].text;
                //estadosText[1] = estadoComida[1].text;
                estadosText[2] = estadoComida[2].text;
                break;
            case Comida.TipoComida.Zanahoria:
                estadosText[0] = estadoComida[0].text;
                //estadosText[1] = estadoComida[1].text;
                estadosText[2] = estadoComida[2].text;
                break;
            case Comida.TipoComida.Pescado:
                estadosText[0] = estadoComida[0].text;
                estadosText[1] = estadoComida[1].text;
                break;
            case Comida.TipoComida.RestosPescado:
                break;
        }
        
        //estadosText[3] = estadoComida[3].text;
        //nombre.text = nombreComida;
    }

    private void Update()
    {
        
        switch (comida.tipoComida)
        {
            case Comida.TipoComida.Patata:
                PelarYCortar();
                break;
            case Comida.TipoComida.Zanahoria:
                PelarYCortar();
                break;
            case Comida.TipoComida.Pescado:
                CortarYRebozar();
                break;
            case Comida.TipoComida.RestosPescado:
                break;
            default:
                Debug.LogError("Pero que cojones mi manin");
                break;
        }
    }

    private void CortarYRebozar()
    {
        if (comida.isCutted)
        {
            estadoComida[0].color = Color.green;
            estadoComida[0].text = "<s>" + estadosText[0] + "</s>";
        }
        if (comida.isRebozado)
        {
            estadoComida[1].color = Color.green;
            estadoComida[1].text = "<s>" + estadosText[1] + "</s>";
        }
        if (comida.isCutted && comida.isRebozado)
        {
            nombre.color = Color.green;
            nombre.text = "<s>" + nombreComida + "</s>";
        }
    }
    private void PelarCortarYRebozar()
    {
        if (comida.isCutted)
        {
            estadoComida[0].color = Color.green;
            estadoComida[0].text = "<s>" + estadosText[0] + "</s>";
        }
        if (comida.isRebozado)
        {
            estadoComida[1].color = Color.green;
            estadoComida[1].text = "<s>" + estadosText[1] + "</s>";
        }
        if (comida.isPelado)
        {
            estadoComida[2].color = Color.green;
            estadoComida[2].text = "<s>" + estadosText[2] + "</s>";
        }
        if (comida.isCutted && comida.isRebozado && comida.isPelado)
        {
            nombre.color = Color.green;
            nombre.text = "<s>" + nombreComida + "</s>";
        }
    }

    private void PelarYCortar()
    {
        if (comida.isCutted)
        {
            estadoComida[0].color = Color.green;
            estadoComida[0].text = "<s>" + estadosText[0] + "</s>";
        }
        if (comida.isPelado)
        {
            estadoComida[2].color = Color.green;
            estadoComida[2].text = "<s>" + estadosText[2] + "</s>";
        }
        if(comida.isPelado && comida.isCutted)
        {
            nombre.color = Color.green;
            nombre.text = "<s>" + nombreComida + "</s>";
        }
    }
}
