using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material.SetColor("_UnlitColor", Color.red);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
