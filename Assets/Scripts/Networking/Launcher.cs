using UnityEngine;
using System.Collections;

public class Launcher : Photon.PunBehaviour
{
    string _gameVersion = "1";

    public PhotonLogLevel Loglevel = PhotonLogLevel.Informational;

    public GameObject menuPanel;
    public GameObject progressLabel;

    [Tooltip("Maximum number of players allowed per room. It has to be 2 or the game will crash.")]
    public byte MaxPlayersInRoom = 2;

    bool isConnecting = false;

    void Awake()
    {
        PhotonNetwork.logLevel = Loglevel;

        PhotonNetwork.autoJoinLobby = false;

        PhotonNetwork.automaticallySyncScene = true;
    }

    void Start()
    {
        progressLabel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void Connect()
    {
        isConnecting = true;
        progressLabel.SetActive(true);
        menuPanel.SetActive(false);
        if (PhotonNetwork.connected)
        {
            Debug.Log("Already connected!");
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }
    }

    public override void OnConnectedToMaster()
    {   
        if (isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnDisconnectedFromPhoton()
    {
        progressLabel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = MaxPlayersInRoom }, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room!");
        PhotonNetwork.LoadLevel("main-game");
    }
}
