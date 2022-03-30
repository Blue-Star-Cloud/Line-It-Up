using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputFieldScript : MonoBehaviour
{
    // Start is called before the first frame update
    public int columnNum;

    public GameManager gm;
    void OnMouseDown()
    {
        gm.selectColumn(columnNum);
    }

    void OnMouseOver()
    {
        gm.HoverColumn(columnNum);
        //Debug.Log("Mouse is over GameObject " + columnNum);
    }
}
