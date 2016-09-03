using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class Launcher : Photon.PunBehaviour
{
    string _gameVersion = "Working" + "1.0"; //first working build; only edit version number when changing versions

    public PhotonLogLevel Loglevel = PhotonLogLevel.Informational;

    public GameObject menuPanel; //UI shit
    public GameObject progressLabel;
    public GameObject joinGamePanel;

    public GameObject unavailableText;
    public Button joinGameButton; //dynamically create join game buttons to join rooms
    public RoomInfo[] roomsList;
    public GameObject joinPanel;

    public Text playerName;

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

    void Update()
    {
        Debug.Log(PhotonNetwork.GetRoomList());
    }

    public void Connect()
    {
        isConnecting = true;
        progressLabel.SetActive(true);
        menuPanel.SetActive(false);
        joinGamePanel.SetActive(false);
        if (PhotonNetwork.connected)
        {
            Debug.Log("Already connected!");
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }
    }

    public void ApplicationQuit()
    {
        Application.Quit();
    }

    public void LoadGameMenu()
    {
        progressLabel.SetActive(false);
        menuPanel.SetActive(false);
        joinGamePanel.SetActive(true);

        if (isConnecting)
        {
            foreach (RoomInfo game in PhotonNetwork.GetRoomList())
            {
                Button joinRoom = (Button)Instantiate(joinGameButton, Vector3.zero, Quaternion.identity);
                joinRoom.GetComponentInChildren<Text>().text = game.name;
                joinRoom.transform.SetParent(joinPanel.transform);
                joinRoom.onClick.AddListener(() => JoinCalledRoom(game.name));
            }

            if (PhotonNetwork.GetRoomList().Length == 0)
            {
                unavailableText.SetActive(true);
            }
            else
            {
                unavailableText.SetActive(false);
            }
        }
        else
        {
            Connect();
        }
    }

    public void backMenu()
    {
        progressLabel.SetActive(false);
        menuPanel.SetActive(true);
        joinGamePanel.SetActive(false);
    }

    public void HostGame()
    {
        if (isConnecting)
        {
            PhotonNetwork.CreateRoom(playerName.text + "'s room", new RoomOptions() { MaxPlayers = MaxPlayersInRoom }, null);
        }
        else
        {
            Connect();
            HostGame();
        }
    }

    public void JoinCalledRoom(string roomName)
    {
        if (isConnecting)
        {
            PhotonNetwork.JoinRoom(roomName);
        }
        else
        {
            Connect();
            JoinCalledRoom(roomName);
        }
    }

    public override void OnConnectedToMaster()
    {
        backMenu();
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
