using UnityEngine;
using SocketIOClient;
using System.Collections.Generic;

public class Net : MonoBehaviour
{
    private SocketIO clientSocket;

    // Store arcs
    private Vector2[][] arcs;

    void Start()
    {
        clientSocket = new SocketIO("http://localhost:3000");

        // Listen for connection success
        clientSocket.OnConnected += (sender, err) => { Debug.Log("Connection to server is established!"); };

        // Listen for connection error
        clientSocket.OnError += (sender, err) => { Debug.Log("Socket error: " + err); };

        ConnectToServer();
        CreateTestArcs();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendScan();
        }
    }

    void CreateTestArcs()
    {
        arcs = ArcGenerator.GenerateArcWithOffsets(new Vector2(-725f, 600f), new Vector2(725f, 600f), 900f, false, 36, 25f, 2);
        Debug.Log($"Num arcs generated: { arcs.Length }");
    }

    async void ConnectToServer()
    {
        Debug.Log("Attempting to connect...");
        await clientSocket.ConnectAsync();
    }

    // ------------------------------------------------------------------------------------------------

    [System.Serializable]
    public class Payload
    {
        public List<List<Point>> arcs { get; set; }
        public Payload(List<List<Point>> arcsInput) { arcs = arcsInput; }
    }

    [System.Serializable]
    public class Point
    {
        public float x { get; set; }
        public float y { get; set; }

        public Point(float xValue, float yValue) { x = xValue; y = yValue; }
    }

    async void SendScan()
    {
        if (clientSocket == null)
        {
            Debug.LogError("Client was not created");
            return;
        }

        if (!clientSocket.Connected)
        {
            Debug.LogError("Client not connected to Server");
            return;
        }

        if (arcs == null)
        {
            Debug.LogError("There are no arcs to send");
            return;
        }

        List<List<Point>> formattedArcs = new List<List<Point>>();

        foreach (Vector2[] arc in arcs)
        {
            List<Point> arcPoints = new List<Point>();

            foreach (Vector2 point in arc)
            {
                arcPoints.Add(new Point(point.x, point.y));
            }

            formattedArcs.Add(arcPoints);
        }

        Payload payLoad = new Payload(formattedArcs);
        await clientSocket.EmitAsync("arm.scan", payLoad);
        Debug.Log("Scan was sent to Robot");
    }
}
