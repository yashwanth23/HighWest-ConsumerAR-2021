using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***************************************************************************************************************************************
 * This script is to make the wheels rotate around its own axis
 * This is written for the truck wheels
 **************************************************************************************************************************************/

public class WheelRotate : MonoBehaviour
{
    void Update()
    {
        //Rotate the object about its own axis (local axis of the object)
        transform.Rotate(5, 0, 0, Space.Self);
    }
}
