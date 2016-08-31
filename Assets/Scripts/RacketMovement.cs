using UnityEngine;
using System.Collections;

public class RacketMovement : Photon.PunBehaviour, IPunObservable
{
    //Offline playing
    public string axis = "VerticalLeft"; // can only be VerticalLeft or VerticalRight

    //Online playing (lag compensation)
    public Vector2 realPosition = Vector2.zero;
    public Vector2 positionAtLastPacket = Vector2.zero;
    public float currentTime = 0f;
    public float currentPacketTime = 0f;
    public float lastPacketTime = 0f;
    public float timeToReachGoal = 0f;

    public Vector2 newVelocity = Vector2.zero;

    //General
    public float speed = 40f;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (PhotonNetwork.offlineMode)
        {
            float v = Input.GetAxisRaw(axis);
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, v) * speed;
        }

        else
        {
            if (photonView.isMine)
            {
                float v = Input.GetAxisRaw("Vertical");
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, v) * speed;
            }

            else if (!photonView.isMine)
            {
                timeToReachGoal = currentPacketTime - lastPacketTime;
                currentTime += Time.deltaTime;
                GetComponent<Rigidbody2D>().velocity = newVelocity / timeToReachGoal;
            }
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext((Vector2)(GetComponent<Rigidbody2D>().velocity));
        }
        else
        {
            currentTime = 0f;
            positionAtLastPacket = transform.position;
            realPosition = (Vector2)stream.ReceiveNext();
            lastPacketTime = currentPacketTime;
            currentPacketTime = (float)info.timestamp;

            newVelocity = (Vector2)stream.ReceiveNext();
        }
    }
}
