using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using System;
using TMPro;
using System.Collections.Generic;

public class RoboflowClient : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;

    public string serverIP = "127.0.0.1"; // or your Python IP
    public int port = 5050;
    public int sendIntervalMs = 500;

    Prediction prediction;
    public GameObject redCubePrefab;
    public GameObject greenCubePrefab;
    public GameObject yellowCubePrefab;
    public float scaleFactor = 0.01f; // Adjust depending on your coordinate scale
    public Transform spawnParent;

    bool isSend = false;

    private Dictionary<int, GameObject> activeCubes = new Dictionary<int, GameObject>();

    public MarkerMapper mapper;
    private void Start()
    {
        mapper = FindObjectOfType<MarkerMapper>();
        mapper.Initialize();
    }
    public void ConnectToServer()
    {
        try
        {
            client = new TcpClient(serverIP, port);
            stream = client.GetStream();

            Debug.Log("Connected to Python server");

            receiveThread = new Thread(ReceiveData);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }
        catch (SocketException e)
        {
            Debug.LogError("Socket error: " + e);
        }
    }

    public void SendMessageToServer(string message)
    {
        if (stream != null)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
            Debug.Log("Sent: " + message);
            isSend = true;
        }
    }

    private void ReceiveData()
    {
        while (true)
        {
            if (stream == null) break;

            // Read 4-byte length prefix
            byte[] lengthBytes = new byte[4];
            int read = stream.Read(lengthBytes, 0, 4);
            if (read == 0) break;

            int messageLength = BitConverter.ToInt32(lengthBytes, 0);

            // Read the actual message
            byte[] messageBytes = new byte[messageLength];
            int totalRead = 0;

            while (totalRead < messageLength)
            {
                int chunkRead = stream.Read(messageBytes, totalRead, messageLength - totalRead);
                if (chunkRead == 0) break;
                totalRead += chunkRead;
            }

            string response = Encoding.UTF8.GetString(messageBytes);
            Debug.Log("Received: " + response);

            prediction = JsonUtility.FromJson<Prediction>(response);
        }
    }

    private void Update()
    {
        if (isSend)
        {
            VisualizeCubes();
        }
    }

    public void VisualizeCubes()
    {
        HashSet<int> currentTrackIds = new HashSet<int>();

        foreach (PredictionItem item in prediction.object_result.predictions)
        {
            currentTrackIds.Add(item.tracker_id);

            GameObject prefab = item.class_id switch
            {
                0 => greenCubePrefab,
                1 => redCubePrefab,
                2 => yellowCubePrefab,
                _ => null
            };

            print("prefab: "+ prefab);
            print("item.obb: " + item.obb); 
            print("item.obb.Count: " + item.obb.Count);
            if (prefab == null || item.obb == null || item.obb.Count != 4) continue;
            
            // Convert OBB image points to Unity world coordinates using marker mapping
            Vector3[] worldPoints = new Vector3[4];
            for (int i = 0; i < 4; i++)
            {
                Vector2 imagePoint = new Vector2(item.obb[i].x, item.obb[i].y);
                worldPoints[i] = mapper.MapImagePointToWorld(imagePoint);
            }

            // Center and rotation
            Vector3 center = (worldPoints[0] + worldPoints[1] + worldPoints[2] + worldPoints[3]) / 4f;
            Vector3 dir = worldPoints[1] - worldPoints[0];
            float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

            if (activeCubes.ContainsKey(item.tracker_id))
            {
                var cube = activeCubes[item.tracker_id];
                Vector3 currentPos = cube.transform.position;
                cube.transform.position = new Vector3(center.x, currentPos.y, center.z);
                cube.transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }
            else
            {
                var cube = Instantiate(prefab, center, Quaternion.Euler(0f, angle, 0f), spawnParent);
                cube.name = $"Cube_{item.tracker_id}";
                activeCubes[item.tracker_id] = cube;
            }
        }

        // Cleanup old cubes
        List<int> toRemove = new List<int>();
        foreach (int oldId in activeCubes.Keys)
        {
            if (!currentTrackIds.Contains(oldId))
            {
                Destroy(activeCubes[oldId]);
                toRemove.Add(oldId);
            }
        }
        foreach (int id in toRemove)
        {
            activeCubes.Remove(id);
        }
    }

}
