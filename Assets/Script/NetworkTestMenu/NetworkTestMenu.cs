using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkTestMenu : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private UnityTransport transport;

    // 127.0.0.1 indica il PC su cui stai eseguendo il programma.
    [SerializeField] private string serverAddress = "127.0.0.1";
    [SerializeField] private ushort port = 7777;

    private string status = "Disconnesso";

    private void OnEnable()
    {
        networkManager.OnClientConnectedCallback += OnClientConnected;
        networkManager.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnDisable()
    {
        networkManager.OnClientConnectedCallback -= OnClientConnected;
        networkManager.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    public void StartServer()
    {
        if (networkManager.IsListening)
            return;

        // Il server ascolta sulle interfacce di rete del PC.
        transport.SetConnectionData(serverAddress, port, "0.0.0.0");

        bool started = networkManager.StartServer();

        status = started
            ? "Server avviato"
            : "Avvio del server fallito";
    }

    public void StartClient()
    {
        if (networkManager.IsListening)
            return;

        transport.SetConnectionData(serverAddress, port);

        // Imposta il messaggio prima dell'avvio:
        // i callback aggiorneranno lo stato della connessione.
        status = "Connessione in corso...";

        if (!networkManager.StartClient())
            status = "Avvio del client fallito";
    }

    public void StopConnection()
    {
        networkManager.Shutdown();
        status = "Disconnesso";
    }

    private void OnClientConnected(ulong clientId)
    {
        if (networkManager.IsServer)
        {
            Debug.Log($"Client {clientId} collegato.");
        }
        else
        {
            status = "Collegato al server";
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (networkManager.IsServer)
        {
            Debug.Log($"Client {clientId} scollegato.");
        }
        else
        {
            status = "Disconnesso dal server";
        }
    }

    // Menu temporaneo per provare la connessione nell'Editor e nella build.
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(20, 20, 300, 260), GUI.skin.box);

        GUILayout.Label(status);

        if (!networkManager.IsListening)
        {
            GUILayout.Label("Indirizzo del server");
            serverAddress = GUILayout.TextField(serverAddress);

            if (GUILayout.Button("Avvia server"))
                StartServer();

            if (GUILayout.Button("Collegati come client"))
                StartClient();
        }
        else
        {
            if (networkManager.IsServer)
            {
                GUILayout.Label(
                    $"Client collegati: " +
                    $"{networkManager.ConnectedClients.Count}");
            }

            if (GUILayout.Button("Disconnetti"))
                StopConnection();
        }

        GUILayout.EndArea();
    }
}