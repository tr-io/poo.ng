using UnityEngine;
using System.Collections;

public class Ball : Photon.PunBehaviour, IPunObservable
{
    public float speed = 40f;

    public GameController gc;

    public Vector2 dir;

    //Networking variables
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
                //GetComponent<Rigidbody2D>().velocity = Vector2.right * speed;
                dir = Vector2.right;
            }
            else
            {
                //GetComponent<Rigidbody2D>().velocity = Vector2.left * speed;
                dir = Vector2.left;
            }
        }
        else
        {
            if (PhotonNetwork.isMasterClient)
            {
                float r = Random.value;

                if (r >= 0.5f)
                {
                    //GetComponent<Rigidbody2D>().velocity = Vector2.right * speed;
                    dir = Vector2.right;
                }
                else
                {
                    //GetComponent<Rigidbody2D>().velocity = Vector2.left * speed;
                    dir = Vector2.left;
                }
            }
        }
    }

    void Update()
    {
        if (PhotonNetwork.offlineMode)
        {
            if (gc.paused)
            {
                //GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                dir = Vector2.zero;
            }
            else
            {
                transform.Translate(dir * speed * Time.deltaTime);
            }
        }
        else
        {
            if (PhotonNetwork.isMasterClient)
            {
                if (gc.paused)
                {
                    //GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    dir = Vector2.zero;
                }

                else
                {
                    transform.Translate(dir * speed * Time.deltaTime);
                }
            }

            if (!PhotonNetwork.isMasterClient)
            {
                timeToReachGoal = currentPacketTime - lastPacketTime;
                currentTime += Time.deltaTime;
                //transform.position = Vector2.Lerp(positionAtLastPacket, realPosition, currentTime / timeToReachGoal);
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
                //GetComponent<Rigidbody2D>().velocity = Vector2.right * speed;
                dir = Vector2.right;
            }
            else
            {
                //GetComponent<Rigidbody2D>().velocity = Vector2.left * speed;
                dir = Vector2.left;
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
                    //GetComponent<Rigidbody2D>().velocity = Vector2.right * speed;
                    dir = Vector2.right;
                }
                else
                {
                    //GetComponent<Rigidbody2D>().velocity = Vector2.left * speed;
                    dir = Vector2.left;
                }
            }
            else
            {
                timeToReachGoal = currentPacketTime - lastPacketTime;
                currentTime += Time.deltaTime;
                transform.position = Vector2.Lerp(positionAtLastPacket, realPosition, currentTime / timeToReachGoal);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (PhotonNetwork.offlineMode)
        {
            if (col.gameObject.CompareTag("LeftRacket"))
            {
                float y = hitFactor(transform.position,
                                    col.transform.position,
                                    col.collider.bounds.size.y);

                dir = new Vector2(1, y).normalized;

                speed += 5.0f;

                //GetComponent<Rigidbody2D>().velocity = dir * speed;
            }

            if (col.gameObject.CompareTag("RightRacket"))
            {
                float y = hitFactor(transform.position,
                                    col.transform.position,
                                    col.collider.bounds.size.y);

                dir = new Vector2(-1, y).normalized;

                speed += 5.0f;

                //GetComponent<Rigidbody2D>().velocity = dir * speed;
            }

            if (col.gameObject.CompareTag("TopWall") || col.gameObject.CompareTag("BottomWall"))
            {
                dir = new Vector2(dir.x, -dir.y).normalized;
            }

            if (col.gameObject.CompareTag("LeftWall"))
            {
                gc.scoreRight();
                transform.position = new Vector2(0, 0);
                speed = 40.0f;
                dir = Vector2.left;
            }

            if (col.gameObject.CompareTag("RightWall"))
            {
                gc.scoreLeft();
                transform.position = new Vector2(0, 0);
                speed = 40.0f;
                dir = Vector2.right;
            }
        }
        else
        {
            if (PhotonNetwork.isMasterClient)
            {
                if (col.gameObject.CompareTag("LeftRacket")) // Hits left racket
                {
                    float y = hitFactor(transform.position,
                                        col.transform.position,
                                        col.collider.bounds.size.y);

                    dir = new Vector2(1, y).normalized;

                    speed += 5.0f;

                    //GetComponent<Rigidbody2D>().velocity = dir * speed;
                }

                if (col.gameObject.CompareTag("RightRacket")) // Hits right racket
                {
                    float y = hitFactor(transform.position,
                                        col.transform.position,
                                        col.collider.bounds.size.y);

                    dir = new Vector2(-1, y).normalized;

                    speed += 5.0f;

                    //GetComponent<Rigidbody2D>().velocity = dir * speed;
                }

                if (col.gameObject.CompareTag("TopWall") || col.gameObject.CompareTag("BottomWall"))
                {
                    dir = new Vector2(dir.x, -dir.y).normalized;
                }

                if (col.gameObject.CompareTag("LeftWall"))
                {
                    PhotonView gcView = gc.GetComponent<PhotonView>();
                    gcView.RPC("scoreRight", PhotonTargets.All);
                    transform.position = new Vector2(0, 0);
                    speed = 40.0f;
                    dir = Vector2.left;
                }

                if (col.gameObject.CompareTag("RightWall"))
                {
                    PhotonView gcView = gc.GetComponent<PhotonView>();
                    gcView.RPC("scoreLeft", PhotonTargets.All);
                    transform.position = new Vector2(0, 0);
                    speed = 40.0f;
                    dir = Vector2.right;
                }
            }
        }
    }

    [PunRPC]
    public void setGC(int ID)
    {
        gc = PhotonView.Find(ID).GetComponent<GameController>();
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
            stream.SendNext((float)speed);
        }
        else
        {
            currentTime = 0f;
            positionAtLastPacket = transform.position;
            realPosition = (Vector2)stream.ReceiveNext();
            speed = (float)stream.ReceiveNext();
            lastPacketTime = currentPacketTime;
            currentPacketTime = (float)info.timestamp;
        }
    }
}
