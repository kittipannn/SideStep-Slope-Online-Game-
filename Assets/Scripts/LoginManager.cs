using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using System;
using System.Text;
using UnityEngine.UI;
using MLAPI.Logging;
using MLAPI.Transports.UNET;

public class LoginManager : MonoBehaviour
{
    public GameObject loginPanel;
    public Text playerNameInputField;
    public GameObject leaveButton;
    public string ipAddress = "127.0.0.1";
    UNetTransport transport;

    public void OnIpAddressChanged(string address)
    {
        this.ipAddress = address;
    }

    public GameObject spawnPosHost, spawnPosclient;

    public void Host()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        
        Vector3 spawnPos = spawnPosHost.transform.position;
        Quaternion spawnRot = Quaternion.identity;
        NetworkManager.Singleton.StartHost(spawnPos, spawnRot);
    }
    private void ApprovalCheck(byte[] connectionData, ulong clientId,
        MLAPI.NetworkManager.ConnectionApprovedDelegate callback) //จะถูกเรียกก็ต่อเมื่อ Cilent มาจอยเท่านั้น ตอนstart server จะไม่ถูกเรียกใช้งาน
    {

        string playerName = Encoding.ASCII.GetString(connectionData);
        bool approveConnection = playerName != playerNameInputField.text; // ชื่อต้องไม่เหมือนกัน
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;
        switch (NetworkManager.Singleton.ConnectedClients.Count)
        {
            case 1:
                spawnPos = spawnPosclient.transform.position;
                spawnRot = Quaternion.identity;
                break;
        }
        NetworkLog.LogInfoServer("spawnPos = " + spawnPos.ToString());
        callback(true, null, approveConnection, spawnPos, spawnRot);
    }
    public void Client()
    {
        //transport = NetworkManager.Singleton.GetComponent<UNetTransport>();
        //transport.ConnectAddress = ipAddress;
        NetworkManager.Singleton.NetworkConfig.ConnectionData =
            Encoding.ASCII.GetBytes(playerNameInputField.text);
        NetworkManager.Singleton.StartClient();
    }
    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += HandleServerStart;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
    }
    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null) { return; }
        NetworkManager.Singleton.OnServerStarted -= HandleServerStart;
        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;

    }
    private void HandleClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            loginPanel.SetActive(false);
            leaveButton.SetActive(true);
            setPlayerName(clientId);
        }
    }
    void setPlayerName(ulong cilentId)
    {
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(cilentId, out var networkClient))
        {
            return;
        }
        if (!networkClient.PlayerObject.TryGetComponent<CharacterControl>(out var mainPlayer))
        {
            return;
        }
        string playerName = playerNameInputField.text;
        mainPlayer.SetPlayerNameServerRpc(playerName);
    }
    private void HandleClientDisconnect(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            loginPanel.SetActive(true); ;
            leaveButton.SetActive(false);
        }
    }
    private void HandleServerStart()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HandleClientConnected(NetworkManager.Singleton.LocalClientId);
        }
    }
    public void Leave()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.StopHost();
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.StopClient();
        }
        loginPanel.SetActive(true);
        leaveButton.SetActive(false);
    }
}
