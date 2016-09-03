using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : Photon.PunBehaviour, IPunObservable
{
    public int rightScore = 0;
    public int leftScore = 0;

    public GameObject rightRacket;
    public GameObject leftRacket;

    public Ball ballPrefab;

    public Ball thisBall;

    public bool paused = false;

    public Text scoreText;

    void Start()
    {
        //PhotonNetwork.offlineMode = true; // debug only
    }

    void Update()
    {
        scoreText.text = leftScore + " | " + rightScore;
    }

    void OnGUI()
    {
        GUILayout.Label("Room Name: " + PhotonNetwork.room.name);
    }

    [PunRPC]
    public void scoreRight()
    {
        if (PhotonNetwork.offlineMode)
        {
            rightScore++;
            rightRacket.GetComponent<RacketMovement>().ResetPos();
            leftRacket.GetComponent<RacketMovement>().ResetPos();
        }
        else
        {
            if (PhotonNetwork.isMasterClient)
            {
                rightScore++;
                //rightRacket.transform.position = new Vector2(rightRacket.transform.position.x, 0);
                //leftRacket.transform.position = new Vector2(leftRacket.transform.position.x, 0);
                rightRacket.GetComponent<PhotonView>().RPC("ResetPos", PhotonTargets.All);
                leftRacket.GetComponent<PhotonView>().RPC("ResetPos", PhotonTargets.All);
            }
        }
    }

    [PunRPC]
    public void SetRightRacket(int playerID)
    {
        rightRacket = PhotonView.Find(playerID).gameObject;
        rightRacket.name = "RightRacket";
        rightRacket.tag = "RightRacket";
    }

    [PunRPC]
    public void SetLeftRacket(int playerID)
    {
        leftRacket = PhotonView.Find(playerID).gameObject;
        leftRacket.name = "LeftRacket";
        leftRacket.tag = "LeftRacket";
    }

    [PunRPC]
    public void scoreLeft()
    {
        if (PhotonNetwork.offlineMode)
        {
            leftScore++;
            rightRacket.GetComponent<RacketMovement>().ResetPos();
            leftRacket.GetComponent<RacketMovement>().ResetPos();
        }
        else
        {
            if (PhotonNetwork.isMasterClient)
            {
                leftScore++;
                //rightRacket.transform.position = new Vector2(rightRacket.transform.position.x, 0);
                //leftRacket.transform.position = new Vector2(leftRacket.transform.position.x, 0);
                rightRacket.GetComponent<PhotonView>().RPC("ResetPos", PhotonTargets.All);
                leftRacket.GetComponent<PhotonView>().RPC("ResetPos", PhotonTargets.All);
            }
        }
    }

    [PunRPC]
    public void StartGame()
    {
        if (PhotonNetwork.offlineMode)
        {
            paused = false;
            thisBall = (Ball)Instantiate(ballPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            thisBall.gc = this;
        }
        else
        {
            if (PhotonNetwork.isMasterClient)
            {
                paused = false;

                GameObject ballObj = PhotonNetwork.InstantiateSceneObject("Ball", new Vector3(0, 0, 0), Quaternion.identity, 0, null);

                thisBall = ballObj.GetComponent<Ball>();

                thisBall.GetComponent<PhotonView>().RPC("setGC", PhotonTargets.AllBuffered, photonView.viewID);
                //PhotonView ballView = thisBall.GetComponent<PhotonView>();
                //ballView.RPC("StartBall", PhotonTargets.All);
            }
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) //Online syncing
    {
        if (stream.isWriting)
        {
            stream.SendNext(rightScore);
            stream.SendNext(leftScore);
            stream.SendNext(paused);
        }
        else
        {
            this.rightScore = (int)stream.ReceiveNext();
            this.leftScore = (int)stream.ReceiveNext();
            this.paused = (bool)stream.ReceiveNext();
        }
    }
}
