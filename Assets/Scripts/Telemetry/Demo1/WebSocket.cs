using UnityEngine;
using WebSocketSharp;

public class WebSocketClient : MonoBehaviour
{

    public static WebSocketClient sharedInstance;
    private WebSocket ws;

    void Awake()
    {
        if (sharedInstance == null)
        {
            sharedInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (sharedInstance != this)
        {
            Debug.LogWarning("Duplicate WebSocketClient instance found on " + gameObject.name + ", but not destroying it to avoid breaking logic.");
            this.enabled = false;
        }
    }

    void Start()
    {
        ws = new WebSocket("ws://127.0.0.1:8000/ws");

        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket connected!");
        };
        ws.OnError += (sender, e) =>
        {
            Debug.LogError("WebSocket Error: " + e.Message);
        };
        ws.OnMessage += (sender, e) => {
            Debug.Log("Received from server: " + e.Data);
        }
        ;

        ws.Connect();
    }

    public static WebSocketClient getSharedInstance()
    {
        if (sharedInstance == null)
        {
            Debug.LogError("Shared instance is null");
        }
        return sharedInstance;
    }

    public void SendData(string message)
    {
        if (ws == null || ws.ReadyState != WebSocketState.Open)
            return;

        if (ws.IsAlive) 
        {
            ws.Send(message);
        }
    }

    void OnDestroy()
    {
        if(ws !=null)
        {
            ws.Close();
        }
        if (sharedInstance == this)
        {
            sharedInstance = null;
        }
    }
}
