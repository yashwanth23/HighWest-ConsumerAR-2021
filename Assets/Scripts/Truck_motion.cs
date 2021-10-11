using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

/******************************************************************************************************************************************************
 * This script is to move the truck on a custom path that was created using PathCreator asset from Unity asset store
 *****************************************************************************************************************************************************/

public class Truck_motion : MonoBehaviour
{
    //Using path creator asset from Unity asset store
    public PathCreator pathCreator;
    public float speed = 5;
    float distanceTravelled;
    public GameObject Trailer;
    private float trailer_pos;

    // Start is called before the first frame update
    void Start()
    {
        //Store the distance from the truck to the trailer (not displacement)
        trailer_pos = Vector3.Distance(Trailer.transform.position, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        // Move the truck a certain distance every frame
        distanceTravelled += speed * Time.deltaTime;
        // Find the (x, y, z) position of the path at a distance from the original position and move the truck's position to the new position
        transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
        //Find the rotation vector of the path at a distance from the original position and rotate the truck accordingly 
        transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled);

        //Find the (x, y, z) position of the path at a distance from the original position of the trailer and move the trailer's position to the new position
        Trailer.transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled - trailer_pos);
        //Find the rotation vector of the path at a distance from the original position of the trailer and rotate it accordingly 
        Trailer.transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled - trailer_pos);
     
    }
}
