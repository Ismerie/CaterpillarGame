using UnityEngine;

public class LeafSpawner : MonoBehaviour
{
    public GameObject leafPrefab;

    public float spawnInterval = 0.3f;
    public float minX = -8f;
    public float maxX = 8f;
    public float spawnY = 5f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;

            Vector3 spawnPos = new Vector3(
                Random.Range(minX, maxX),
                spawnY,
                0f
            );

            Instantiate(leafPrefab, spawnPos, Quaternion.identity);
        }
    }
}

