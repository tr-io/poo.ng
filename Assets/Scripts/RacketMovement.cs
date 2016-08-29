using UnityEngine;
using System.Collections;

public class RacketMovement : Photon.PunBehaviour
{
    //Offline playing
    public string axis = "VerticalLeft"; // can only be VerticalLeft or VerticalRight

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
        }
    }
}
