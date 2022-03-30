using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
public class ChatSystem : MonoBehaviour
{
    public TMP_Text Box;
    public TMP_InputField inputField;
    public string Banedwords;
    public string[] BadString;
    public string[] debug;
    // Start is called before the first frame update
    public void Start()
    {
        BadString = Banedwords.Split(char.Parse(","));
    }
    public void OnSend()
    {
        string message = inputField.text;
        string safe = "";
        debug = message.Split(' ');
        
        foreach (string Word in message.Split(' '))
        {
            bool bad = false;
            foreach (string Ban in BadString)
            {
                if (Word.ToLower() == Ban.ToLower())
                {
                    safe = safe + " *****";
                    bad = true;
                }

            }
            if(bad == false) { safe = safe + " " + Word; }
        }
        this.GetComponent<PhotonView>().RPC("OnRecive", RpcTarget.All, PhotonNetwork.NickName + " Says: " + safe);
        inputField.text = "";
    }
    [PunRPC]
    public void OnRecive(string message)
    {
        Box.text = message;
        Debug.LogError("message");
    }
}
