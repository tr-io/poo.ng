using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : Photon.PunBehaviour
{

    public GameObject playerPaddle;
    public GameController gc;

    void Start()
    {
        gc.paused = true;

        if (playerPaddle != null)
        {
            if (PhotonNetwork.isMasterClient)
            {
                PhotonNetwork.Instantiate(this.playerPaddle.name, new Vector3(-85, 0, 0), Quaternion.identity, 0);
            }
            else
            {
                PhotonNetwork.Instantiate(this.playerPaddle.name, new Vector3(85, 0, 0), Quaternion.identity, 0);
            }
        }
    }

    void Update()
    {
        
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        if (PhotonNetwork.playerList.Length == PhotonNetwork.room.maxPlayers)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        gc.StartGame();
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        PhotonNetwork.LeaveRoom();
    }

    void LoadPong()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            Debug.LogError("Trying to load game but not master client!");
        }
        PhotonNetwork.LoadLevel("main-game");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
