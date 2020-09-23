using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// Manages time in the game
/// </summary>
public class TimeManager : MonoBehaviour
{

    public int window;


    // Start is called before the first frame update
    void Start()
    {
        window = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        window = (int) Time.realtimeSinceStartup % 12;
        //Debug.Log(System.DateTime.Now);
    }

}
