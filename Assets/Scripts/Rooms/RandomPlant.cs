using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Generates a random plant at the transform
/// </summary>
public class RandomPlant : MonoBehaviour
{

    RoomsManager roomsManager;

    public GameObject plant1;
    public GameObject plant2;

    // Start is called before the first frame update
    void Start()
    {
        roomsManager = GameObject.FindGameObjectWithTag("RoomsManager").GetComponent<RoomsManager>();

        if (roomsManager.returnMode)
            Instantiate(plant1, transform);
        else
            Instantiate(plant2, transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
