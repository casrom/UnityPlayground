using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeDisplay : MonoBehaviour
{

    public LineRenderer line;

    public Vector3 srcPos;
    public Vector3 dstPos;

    public Vector3 srcHandle, dstHandle, control;
    public float handleStrength = 2f;

    // Start is called before the first frame update
    void Start()
    {
        line = transform.GetComponent<LineRenderer>();
        line.startWidth = 0.002f;
        line.startWidth = 0.002f;
    }

    // Update is called once per frame
    void Update()
    {
        control = (srcPos + dstPos) / 2 + (srcHandle - srcPos) + (dstHandle - dstPos) * 2f;
        DrawForthOrderBezierCurve(srcPos, srcHandle, control, dstHandle, dstPos);
    }


    void DrawCubicBezierCurve(Vector3 point0, Vector3 point1, Vector3 point2, Vector3 point3) {
        //https://www.codinblack.com/how-to-draw-lines-circles-or-anything-else-using-linerenderer/

        line.positionCount = 200;
        float t = 0f;
        Vector3 B = new Vector3(0, 0, 0);
        for (int i = 0; i < line.positionCount; i++) {
            B = (1 - t) * (1 - t) * (1 - t) * point0 + 3 * (1 - t) * (1 - t) *
                t * point1 + 3 * (1 - t) * t * t * point2 + t * t * t * point3;

            line.SetPosition(i, B);
            t += (1 / (float)line.positionCount);
        }
    }


    void DrawForthOrderBezierCurve(Vector3 point0, Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4) {
        //https://www.codinblack.com/how-to-draw-lines-circles-or-anything-else-using-linerenderer/

        line.positionCount = 200;
        float t = 0f;
        Vector3 B = new Vector3(0, 0, 0);
        for (int i = 0; i < line.positionCount; i++) {
            B = 1 * (1 - t) * (1 - t) * (1 - t) * (1 - t) * point0 + 
                4 * (1 - t) * (1 - t) * (1 - t) * t * point1 + 
                6 * (1 - t) * (1 - t) * t * t * point2 + 
                4 * (1 - t) * t * t * t * point3 + 
                1 * t * t * t * t * point4;

            line.SetPosition(i, B);
            t += (1 / (float)line.positionCount);
        }
    }

}
