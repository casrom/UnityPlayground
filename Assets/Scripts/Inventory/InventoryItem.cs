using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class InventoryItem : MonoBehaviour
{

    Vector3 dimension;
    Material selectorMat;
    public bool selected;
    float speed = 2f;
    Color targetColor;

    [ColorUsage(true, true)]
    public Color activeColor;

    // Start is called before the first frame update
    void Start()
    {
        selectorMat = this.GetComponent<MeshRenderer>().material;
        targetColor = Vector4.zero;
    }


    float velocity;
    float targetWFWidth;

    // Update is called once per frame
    void Update()
    {
        if (selected) {
            targetColor = activeColor;
            targetWFWidth = 0.015f;
        } else {
            targetWFWidth = 0f;
        }
        //Color color = selectorMat.GetColor("_BackColor");
        //selectorMat.SetColor("_BackColor", Vector4.Lerp(color, targetColor, Time.deltaTime * speed));

        float wfWidth = selectorMat.GetFloat("_WireframeVal");
        selectorMat.SetFloat("_WireframeVal", Mathf.SmoothDamp(wfWidth, targetWFWidth, ref velocity, 0.3f));
    }
}
