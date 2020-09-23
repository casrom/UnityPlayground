using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSubDevice : MonoBehaviour
{

    Vector3 targetLocation;

    float speed = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator MoveCoroutine()
    {
        while (transform.localPosition != targetLocation) {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocation, speed * Time.deltaTime);
            yield return null;
        }
    }

    public void MoveToTargeLocation()
    {
        StartCoroutine(MoveCoroutine());
    }


    public void SetTargetLocation(Vector3 location)
    {
        targetLocation = location;
    }


}
