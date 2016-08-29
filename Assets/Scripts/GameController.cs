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

    public void scoreRight()
    {
        rightScore++;
        rightRacket.transform.position = new Vector2(rightRacket.transform.position.x, 0);
        leftRacket.transform.position = new Vector2(leftRacket.transform.position.x, 0);
    }

    public void scoreLeft()
    {
        leftScore++;
        rightRacket.transform.position = new Vector2(rightRacket.transform.position.x, 0);
        leftRacket.transform.position = new Vector2(leftRacket.transform.position.x, 0);
    }

    public void StartGame()
    {
        paused = false;
        thisBall.StartBall();
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(rightScore);
            stream.SendNext(leftScore);
        }
        else
        {
            this.rightScore = (int)stream.ReceiveNext();
            this.leftScore = (int)stream.ReceiveNext();
        }
    }
}
