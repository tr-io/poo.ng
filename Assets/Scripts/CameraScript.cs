using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{
    public float widthToBeSeen = 1920f;

    void Update()
    {
        Camera.main.orthographicSize = widthToBeSeen * Screen.height / Screen.width * 0.5f;
    }
}
