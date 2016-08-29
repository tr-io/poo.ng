using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(InputField))]
public class PlayerName : MonoBehaviour
{
    static string playerNamePrefKey = "PlayerName";

    void Start()
    {
        string defaultName = "";
        InputField _inputField = this.GetComponent<InputField>();
        if (_inputField != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                _inputField.text = defaultName;
            }
        }

        PhotonNetwork.playerName = defaultName;
    }

    public void SetName(string name)
    {
        if (name == "")
        {
            PhotonNetwork.playerName = "Sample Player";
        }
        else
        {
            PhotonNetwork.playerName = name;
        }

        PlayerPrefs.SetString(playerNamePrefKey, name);
    }
}
