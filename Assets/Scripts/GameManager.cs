using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Text;
using TMPro;
public class GameManager : MonoBehaviour
{

    public string debugStartMessage;

    public GameObject lineWinner;

    public GameObject player1;
    public GameObject player2;
    public GameObject player1Hover;
    public GameObject player2Hover;
    public int AIR;
    public int heightOfBoard = 6;
    public int lengthOfBoard = 7;
    public GameObject WinScreen;
    public GameObject[] spawnLoc;
    GameObject fallingPiece;

    bool IsMyTurn = true;

    public int[,] gameState; //zero is empyt, 1 is player1, 2 is player2

    //0 0 0 0 0 0 0 this is the game state
    //0 0 0 0 0 0 0
    //0 0 0 0 0 0 0
    //0 0 0 0 0 0 0
    //0 0 2 0 0 0 0
    //0 0 1 1 0 0 0

    #region Photon event handling and synchronisation
    
    

    #endregion


    // Start is called before the first frame update
    void Start()
    {
        IsMyTurn = PhotonNetwork.IsMasterClient;//Master Client get the first turn
        Debug.LogError(PhotonNetwork.IsMasterClient);//Now we can tell if we are player one or two based on this Bool
        Debug.Log("Debug message");
        gameState = new int[lengthOfBoard, heightOfBoard];

        player1Hover.SetActive(false);
        player2Hover.SetActive(false);

    }
    
    public void HoverColumn(int columnNum)
    {
        if (IsMyTurn)// is it currently my  turn?
        {
            if (gameState[columnNum, heightOfBoard - 1] == 0 && (fallingPiece == null || fallingPiece.GetComponent<Rigidbody>().velocity == Vector3.zero))  //if the piece doesn't exit or execute the code
            {                                                                                                                                                 //only shows the hover piece when the velcoity is 0                      
                if (PhotonNetwork.IsMasterClient)
                {
                    player1Hover.SetActive(true);
                    player1Hover.transform.position = spawnLoc[columnNum].transform.position;
                }
                else
                {
                    player2Hover.SetActive(true);
                    player2Hover.transform.position = spawnLoc[columnNum].transform.position;
                }
            }
        }
    }
    public void selectColumn(int columnNum)
    {
        if (IsMyTurn)
        {
            if (gameState[columnNum, heightOfBoard - 1] == 0 && (fallingPiece == null || fallingPiece.GetComponent<Rigidbody>().velocity == Vector3.zero)) //
            {
                Debug.Log("GameManager Column" + columnNum);
                TakeTurn(columnNum);
                HoverColumn(columnNum);
                //updateGameState(columnNum);
            }
        }
    }

    void TakeTurn(int columnNum)
    {
        if (updateGameState(columnNum))
        {
            player1Hover.SetActive(false);
            player2Hover.SetActive(false);
            if (PhotonNetwork.IsMasterClient)//Decide wich player is Placing a pice
            {
                fallingPiece = PhotonNetwork.Instantiate("player1", spawnLoc[columnNum].transform.position, Quaternion.identity);//instantiate the physical piece
                fallingPiece.GetComponent<Rigidbody>().velocity = new Vector3(0, 0.1f, 0); //x, y, z of the piece when falling down
                IsMyTurn = false;//Not my turn anymore
                if (playerWin(1))
                {
                    WinScreen.SetActive(true);
                    WinScreen.GetComponentInChildren<TMP_Text>().text = PhotonNetwork.NickName + " wins";
                    Debug.LogError(PhotonNetwork.NickName + " wins");
                    this.GetComponent<PhotonView>().RPC("Lose", RpcTarget.Others, PhotonNetwork.NickName);
                    return;
                }
                if (AIR >= 2) { RandomTurn(columnNum); }
                else
                {
                    this.GetComponent<PhotonView>().RPC("RPCSwitch", RpcTarget.Others, columnNum,8);// send the other player the turn we just did
                    AIR++;
                }
            }
            else
            {
                fallingPiece = PhotonNetwork.Instantiate("player2", spawnLoc[columnNum].transform.position, Quaternion.identity);//instantiate the physical piece
                fallingPiece.GetComponent<Rigidbody>().velocity = new Vector3(0, 0.1f, 0);//x, y, z of the piece when falling down
                IsMyTurn = false;//Not my turn anymore
                if (playerWin(2))
                {
                    WinScreen.SetActive(true);
                    WinScreen.GetComponentInChildren<TMP_Text>().text = PhotonNetwork.NickName + " wins";
                    Debug.LogError(PhotonNetwork.NickName + " wins");
                    this.GetComponent<PhotonView>().RPC("Lose", RpcTarget.Others, PhotonNetwork.NickName);
                    return;
                }
                this.GetComponent<PhotonView>().RPC("RPCSwitch", RpcTarget.Others, columnNum,8);// send the other player the turn we just did

            }
        }
    }
    [PunRPC]
    public void Lose(string name)
    {
        WinScreen.SetActive(true);
        WinScreen.GetComponentInChildren<TMP_Text>().text = name + " wins";
        Debug.LogError(name + " wins");
        return;
    }
    [PunRPC]
    public void RPCSwitch(int columnNum, int AiCol)
    {
        if(AiCol != 8)
        {
            for (int row = 0; row < heightOfBoard; row++)
            {
                if (gameState[AiCol, row] == 0)
                {
                  
                        gameState[AiCol, row] = 3;

                    Debug.Log("Piece is being updated (" + AiCol + ", " + row + ")");
                    debugarray(gameState);//debug the new gamestate
                    break;//end the loop in order not to fill a column
                }

            }
            
        }
        //Construct the new gamestate using the other players turn data
        for (int row = 0; row < heightOfBoard; row++)
        {
            if (gameState[columnNum, row] == 0)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    gameState[columnNum, row] = 2;
                }
                else
                {
                    gameState[columnNum, row] = 1;
                }
                Debug.Log("Piece is being updated (" + columnNum + ", " + row + ")");
                debugarray(gameState);//debug the new gamestate
                break;//end the loop in order not to fill a column
            }

        }
        IsMyTurn = true;//It is now My turn
        Debug.LogError("Switch");
    }
    public void RandomTurn(int Coll)
    {
        int AI = Random.Range(0, 6);
        if(AI == Coll) { AI = Random.Range(0, 6); }
        if (AI == Coll) { AI = Random.Range(0, 6); }
        if (AI == Coll) { AI = Random.Range(0, 6); }
        if (AI == Coll) { AI = Random.Range(0, 6); }
        if (AI == Coll) { AI = Random.Range(0, 6); }
        if (AI == Coll) { this.GetComponent<PhotonView>().RPC("RPCSwitch", RpcTarget.Others, Coll, 8); return; }
        fallingPiece = PhotonNetwork.Instantiate("ai", spawnLoc[AI].transform.position, Quaternion.identity);//instantiate the physical piece
        fallingPiece.GetComponent<Rigidbody>().velocity = new Vector3(0, 0.1f, 0); //x, y, z of the piece when falling down
        for (int row = 0; row < heightOfBoard; row++)
        {
            if (gameState[AI, row] == 0)
            {

                gameState[AI, row] = 3;

                Debug.Log("Piece is being updated (" + AI + ", " + row + ")");
                debugarray(gameState);//debug the new gamestate
                break;//end the loop in order not to fill a column
            }

        }
        this.GetComponent<PhotonView>().RPC("RPCSwitch", RpcTarget.Others, Coll, AI);
    }
    bool updateGameState(int columnNum)
    {
        //update the local gamestate
        for (int row = 0; row < heightOfBoard; row++)
        {
            if (gameState[columnNum, row] == 0)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    gameState[columnNum, row] = 1;
                }
                else
                {
                    gameState[columnNum, row] = 2;
                }
                Debug.Log("Piece is being updated (" + columnNum + ", " + row + ")");
                debugarray(gameState);//debug the new gamestate
                return true;
            }

        }
        Debug.Log("Column " + columnNum + " is full ");
        return false;

    }
    bool playerWin(int playerNum)
    {
        //Horizontal line check
        for (int x = 0; x < lengthOfBoard - 3; x++) //looping through each piece both x and y.
        {
            for (int y = 0; y < heightOfBoard; y++)
            {
                if (gameState[x, y] == playerNum && gameState[x + 1, y] == playerNum && gameState[x + 2, y] == playerNum && gameState[x + 3, y] == playerNum)
                {

                    return true;
                }

            }
        }
        //Vertical line check 
        for (int x = 0; x < lengthOfBoard; x++)
        {
            for (int y = 0; y < heightOfBoard - 3; y++)
            {
                if (gameState[x, y] == playerNum && gameState[x, y + 1] == playerNum && gameState[x, y + 2] == playerNum && gameState[x, y + 3] == playerNum)
                {

                    return true;
                }

            }
        }
        //Diagonal line y=x check 
        for (int x = 0; x < lengthOfBoard - 3; x++)
        {
            for (int y = 0; y < heightOfBoard - 3; y++)
            {
                if (gameState[x, y] == playerNum && gameState[x + 1, y + 1] == playerNum && gameState[x + 2, y + 2] == playerNum && gameState[x + 3, y + 3] == playerNum)
                {
                    return true;
                }

            }
        }
        //Diagonal line y=-x check 
        for (int x = 0; x < lengthOfBoard - 3; x++)
        {
            for (int y = 0; y < heightOfBoard - 3; y++)
            {
                if (gameState[x, y + 3] == playerNum && gameState[x + 1, y + 2] == playerNum && gameState[x + 2, y + 1] == playerNum && gameState[x + 3, y] == playerNum)
                {

                    return true;
                }

            }
        }
        return false;
    }
    // Debug the array properly
    public void debugarray(int[,] arr) 
    {
        StringBuilder sb = new StringBuilder();
        int rowLength = arr.GetLength(0);
        int colLength = arr.GetLength(1);

        for (int j = 0; j < colLength; j++)
        {
            for (int i = 0; i < rowLength; i++)
            {
                sb.Append(string.Format("{0} ", arr[i, j]));
            }
            sb.AppendLine();
        }
        Debug.LogError(sb.ToString());
    }
}



