using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NameInputField : MonoBehaviour
{
    // Setting Nicknames for users
    public void SetPlayerName(string name)
{
    if (string.IsNullOrEmpty(name))
    {
        Debug.LogError("Player name is empty");
        return;
    }
    PhotonNetwork.NickName = name;
}
}
