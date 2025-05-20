using UnityEngine;

public class OBBSpawner : MonoBehaviour
{
    public GameObject cubePrefab;
    public Transform spawnParent;
    MarkerMapper mapper;

    // Called from a UI button
    public void SpawnOBBCube()
    {
        // Define 4 points of a rotated rectangle (clockwise or counter-clockwise)
        Vector2[] imagePoints = new Vector2[4]
        {
            new Vector2(323.9f, 214.7f),
            new Vector2(331.5f, 183.8f),
            new Vector2(300.1f, 176.1f),
            new Vector2(292.5f, 207.0f)
        }; 
        mapper = FindObjectOfType<MarkerMapper>();
        mapper.Initialize();

        // Convert to world space (XZ plane)
        Vector3[] worldPoints = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            worldPoints[i] = mapper.MapImagePointToWorld(imagePoints[i]);
        }

        // Calculate center
        Vector3 center = (worldPoints[0] + worldPoints[1] + worldPoints[2] + worldPoints[3]) / 4f;

        // Calculate rotation angle from edge 0->1
        Vector3 dir = worldPoints[1] - worldPoints[0];
        float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

        // Spawn cube
        GameObject cube = Instantiate(cubePrefab, center, Quaternion.identity, spawnParent);
        cube.transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }
}
