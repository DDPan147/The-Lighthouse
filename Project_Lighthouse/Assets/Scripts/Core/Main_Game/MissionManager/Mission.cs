using UnityEngine;

[System.Serializable]
public class Mission
{
    
    [Tooltip("Sirve para poder identificar las misiones internamente. No se va a mostrar en el juego.")] public string missionName;
    [Tooltip("Es lo que se muestra al jugador en el juego para que sepa qu� hacer")][TextArea(3,10)] public string missionDesc;
    public bool isCompleted;
    public bool isDiscovered;
}
