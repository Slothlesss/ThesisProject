using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SpawnedPrefab
{
    public GameObject gameObject;
    public int amount;
    public Color color;
}

public class Spawner : MonoBehaviour
{
    [SerializeField] SpawnedPrefab[] spawnedPrefabs;
    [SerializeField] Transform spawnPool;
    private void Start()
    {
        foreach (SpawnedPrefab prefab in spawnedPrefabs)
        {
            SpawnShapes(prefab);
        }
    }
    void SpawnShapes(SpawnedPrefab prefab)
    {
        for (int i = 0; i < prefab.amount; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(-3f, 3f),
                Random.Range(1f, 5f),
                Random.Range(-3f, 3f)
            );

            GameObject shape = Instantiate(prefab.gameObject, position, Quaternion.identity, spawnPool);

            MeshRenderer rend = shape.GetComponentInChildren<MeshRenderer>();

            rend.material = new Material(rend.material);
            rend.material.color = prefab.color;
        }
    }

}
