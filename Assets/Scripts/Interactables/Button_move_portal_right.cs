using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_move_portal_right : Interactable {
    public Transform portal;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Action() {
        portal.Rotate(0, 0, -1);
    }
}
