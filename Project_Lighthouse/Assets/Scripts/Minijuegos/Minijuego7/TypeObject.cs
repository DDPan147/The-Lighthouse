using UnityEngine;
using UnityEngine.UI;

public class TypeObject : MonoBehaviour
{
    public enum ObjectType { Important, Nonimportant }

    [SerializeField] private ObjectType currentType;

    public bool isImportantObject => currentType == ObjectType.Important;
    
    [SerializeField][TextArea(3,10)] private string objectDescription;
    [SerializeField] private float emotionalWeight;
    [SerializeField] private Image iconObject;
    
    public string GetObjectDescription() => objectDescription;
    public Image GetObjectIcon() => iconObject;
}