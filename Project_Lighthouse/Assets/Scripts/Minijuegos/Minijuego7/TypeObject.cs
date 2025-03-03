using UnityEngine;

public class TypeObject : MonoBehaviour
{
    public enum ObjectType { Rafa, Arnau }

    [SerializeField] private ObjectType currentType;

    // Propiedad para verificar el tipo
    public bool currentObjectType => currentType == ObjectType.Rafa;


}
