using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tilt : MonoBehaviour 
{
    public LayerMask mask;

    void Update () 
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 2f, mask)) 
        {
            transform.position = new Vector3(transform.position.x,hit.point.y,transform.position.z);
        }
    }
}
