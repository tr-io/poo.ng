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
            if (PhotonNetwork.offlineMode)
            {
                GameObject leftRacket = (GameObject)Instantiate(playerPaddle, new Vector3(-85, 0, 0), Quaternion.identity);
                gc.leftRacket = leftRacket;
                leftRacket.name = "LeftRacket";
                leftRacket.tag = "LeftRacket";
                leftRacket.GetComponent<RacketMovement>().axis = "VerticalLeft";
                GameObject rightRacket = (GameObject)Instantiate(playerPaddle, new Vector3(85, 0, 0), Quaternion.identity);
                gc.rightRacket = rightRacket;
                rightRacket.name = "RightRacket";
                rightRacket.tag = "RightRacket";
                rightRacket.GetComponent<RacketMovement>().axis = "VerticalRight";
                StartGame();
            }
            else
            {
                if (PhotonNetwork.isMasterClient)
                {
                    GameObject leftRacket = PhotonNetwork.Instantiate(this.playerPaddle.name, new Vector3(-85, 0, 0), Quaternion.identity, 0);
                    PhotonView gcView = gc.GetComponent<PhotonView>();
                    gcView.RPC("SetLeftRacket", PhotonTargets.AllBuffered, leftRacket.GetComponent<PhotonView>().viewID);
                }
                else
                {
                    GameObject rightRacket = PhotonNetwork.Instantiate(this.playerPaddle.name, new Vector3(85, 0, 0), Quaternion.identity, 0);
                    PhotonView gcView = gc.GetComponent<PhotonView>();
                    gcView.RPC("SetRightRacket", PhotonTargets.AllBuffered, rightRacket.GetComponent<PhotonView>().viewID);
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        if (PhotonNetwork.playerList.Length == PhotonNetwork.room.maxPlayers)
        {
            if (PhotonNetwork.isMasterClient)
            {
                PhotonView gcView = gc.GetComponent<PhotonView>();
                gcView.RPC("StartGame", PhotonTargets.AllBuffered);
            }
        }
    }

    [PunRPC]
    public void StartGame()
    {
        if (PhotonNetwork.offlineMode)
        {
            gc.StartGame();
        }
        else
        {
            PhotonView gcView = gc.GetComponent<PhotonView>();
            gcView.RPC("StartGame", PhotonTargets.All);
        }
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
