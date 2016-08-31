using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : Photon.PunBehaviour, IPunObservable
{
    public int rightScore = 0;
    public int leftScore = 0;

    public GameObject rightRacket;
    public GameObject leftRacket;
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

    [PunRPC]
    public void scoreRight()
    {
        if (PhotonNetwork.isMasterClient)
        {
            rightScore++;
            rightRacket.transform.position = new Vector2(rightRacket.transform.position.x, 0);
            leftRacket.transform.position = new Vector2(leftRacket.transform.position.x, 0);
        }
    }

    [PunRPC]
    public void scoreLeft()
    {
        if (PhotonNetwork.isMasterClient)
        {
            leftScore++;
            rightRacket.transform.position = new Vector2(rightRacket.transform.position.x, 0);
            leftRacket.transform.position = new Vector2(leftRacket.transform.position.x, 0);
        }
    }

    [PunRPC]
    public void StartGame()
    {
        if (PhotonNetwork.isMasterClient)
        {
            paused = false;
            PhotonView ballView = thisBall.GetComponent<PhotonView>();
            ballView.RPC("StartBall", PhotonTargets.All);
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) //Online syncing
    {
        if (stream.isWriting)
        {
            stream.SendNext(rightScore);
            stream.SendNext(leftScore);
            stream.SendNext(paused);
            stream.SendNext(leftRacket);
        }
        else
        {
            this.rightScore = (int)stream.ReceiveNext();
            this.leftScore = (int)stream.ReceiveNext();
            this.paused = (bool)stream.ReceiveNext();
            this.leftRacket = (GameObject)stream.ReceiveNext();
        }
    }
}
