using UnityEngine;

public class TypeObject : MonoBehaviour
{
    public enum ObjectType { Important, Nonimportant }

    [SerializeField] private ObjectType currentType;

    public bool isImportantObject => currentType == ObjectType.Important;

    [SerializeField] private string objectDescription;
    [SerializeField] private float emotionalWeight;
    
    public string GetObjectDescription() => objectDescription;
}