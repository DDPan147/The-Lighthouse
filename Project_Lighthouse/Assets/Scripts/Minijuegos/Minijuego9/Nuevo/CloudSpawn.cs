using UnityEngine;

public class CloudSpawn : MonoBehaviour
{
    private BoxCollider boxCollider;
    public GameObject cloudObj;
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();

        InvokeRepeating("SpawnCloud", 0.5f, 2f);
    }

    void Update()
    {
        
    }

    Vector3 GetRandomPointInCollider()
    {
        Vector3 randomPoint;
        Vector3 min = boxCollider.bounds.min;
        Vector3 max = boxCollider.bounds.max;

        float pointX = Random.Range(min.x, max.x);
        float pointY = Random.Range(min.y, max.y);
        float pointZ = Random.Range(min.z, max.z);

        randomPoint = new Vector3(pointX, pointY, pointZ);

        return randomPoint;
    }

    void SpawnCloud()
    {
        Instantiate(cloudObj, GetRandomPointInCollider(), Quaternion.identity, transform);
    }
}
