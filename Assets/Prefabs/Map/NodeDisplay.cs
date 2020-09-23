using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NodeDisplay : MonoBehaviour
{

    public TextMeshPro text;
    public bool Selected { set; get; }
    MeshRenderer selectionBox;
    // Start is called before the first frame update
    void Start()
    {
        selectionBox = transform.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        text.transform.LookAt(Camera.main.transform);
        selectionBox.enabled = Selected;
        
    }
}
