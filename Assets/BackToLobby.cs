using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class BackToLobby : MonoBehaviour
{ 
    public void Restart()
    {
        if (!PhotonNetwork.IsMasterClient) { PhotonNetwork.LeaveRoom(); SceneManager.LoadScene(0); } else 
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(0);
        }

    }
}
