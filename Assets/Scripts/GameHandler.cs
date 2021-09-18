using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class GameHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Debug.Log.Message");

        //FunctionPeriodic.Create(() => { CMDebug.TextPopupMouse("Ding!"); }, .300f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
