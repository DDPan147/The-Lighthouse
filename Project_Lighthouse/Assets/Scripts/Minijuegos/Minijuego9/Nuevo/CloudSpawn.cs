using UnityEngine;

public class CloudSpawn : MonoBehaviour
{
    private BoxCollider boxCollider;

    [Header("Prefabs de Nubes")]
    public GameObject cloudObj;
    public GameObject badCloudObj;
    [Header("Variables")]
    public float spawnTimeClouds;
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();

        InvokeRepeating("SpawnCloud", 0.5f, spawnTimeClouds);
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
        int random = Random.Range(1, 7);
        if(random == 4)
        {
            Instantiate(badCloudObj, GetRandomPointInCollider(), Quaternion.identity, transform);
        }
        else
        {
            Instantiate(cloudObj, GetRandomPointInCollider(), Quaternion.identity, transform);
        }
        
    }
}
