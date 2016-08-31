using UnityEngine;
using System.Collections;

public class Ball : Photon.PunBehaviour, IPunObservable
{
    public float speed = 40f;

    public GameController gc;

    public Vector2 realPosition = Vector2.zero;
    public Vector2 positionAtLastPacket = Vector2.zero;
    public float currentTime = 0f;
    public float currentPacketTime = 0f;
    public float lastPacketTime = 0f;
    public float timeToReachGoal = 0f;

    void Start()
    {
        // Offline movement
        if (PhotonNetwork.offlineMode)
        {
            float r = Random.value;

            if (r >= 0.5f)
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.right * speed;
            }
            else
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.left * speed;
            }
        }
        else
        {
            if (PhotonNetwork.isMasterClient)
            {
                float r = Random.value;

                if (r >= 0.5f)
                {
                    GetComponent<Rigidbody2D>().velocity = Vector2.right * speed;
                }
                else
                {
                    GetComponent<Rigidbody2D>().velocity = Vector2.left * speed;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (PhotonNetwork.offlineMode)
        {
            if (gc.paused)
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
        }
        else
        {
            if (PhotonNetwork.isMasterClient)
            {
                if (gc.paused)
                {
                    GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                }
            }

            if (!PhotonNetwork.isMasterClient)
            {
                timeToReachGoal = currentPacketTime - lastPacketTime;
                currentTime += Time.deltaTime;
                transform.position = Vector2.Lerp(positionAtLastPacket, realPosition, currentTime / timeToReachGoal);
            }
        }
    }

    [PunRPC]
    public void StartBall()
    {
        if (PhotonNetwork.offlineMode)
        {
            transform.position = new Vector2(0, 0);

            float r = Random.value;

            if (r >= 0.5f)
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.right * speed;
            }
            else
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.left * speed;
            }
        }
        else
        {
            if (PhotonNetwork.isMasterClient)
            {
                transform.position = new Vector2(0, 0);

                float r = Random.value;

                if (r >= 0.5f)
                {
                    GetComponent<Rigidbody2D>().velocity = Vector2.right * speed;
                }
                else
                {
                    GetComponent<Rigidbody2D>().velocity = Vector2.left * speed;
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("LeftRacket"))
        {
            float y = hitFactor(transform.position,
                                col.transform.position,
                                col.collider.bounds.size.y);

            Vector2 dir = new Vector2(1, y).normalized;

            GetComponent<Rigidbody2D>().velocity = dir * speed;
        }

        if (col.gameObject.CompareTag("RightRacket"))
        {
            float y = hitFactor(transform.position,
                                col.transform.position,
                                col.collider.bounds.size.y);

            Vector2 dir = new Vector2(-1, y).normalized;

            GetComponent<Rigidbody2D>().velocity = dir * speed;
        }

        if (col.gameObject.CompareTag("LeftWall"))
        {
            PhotonView gcView = gc.GetComponent<PhotonView>();
            gcView.RPC("scoreRight", PhotonTargets.All);
            transform.position = new Vector2(0, 0);
        }

        if (col.gameObject.CompareTag("RightWall"))
        {
            PhotonView gcView = gc.GetComponent<PhotonView>();
            gcView.RPC("scoreLeft", PhotonTargets.All);
            transform.position = new Vector2(0, 0);
        }
    }

    float hitFactor(Vector2 ballPos, Vector2 racketPos, float racketHeight)
    {
        return (ballPos.y - racketPos.y) / racketHeight;
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext((Vector2)transform.position);
        }
        else
        {
            currentTime = 0f;
            positionAtLastPacket = transform.position;
            realPosition = (Vector2)stream.ReceiveNext();
            lastPacketTime = currentPacketTime;
            currentPacketTime = (float)info.timestamp;
        }
    }
}
