using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;

[RequireComponent(typeof(ARPlaneManager))]
[RequireComponent(typeof(ARRaycastManager))]
//[RequireComponent(typeof(ARAnchorManager))]
public class GroundDetection : MonoBehaviour
{
    public GameObject PlaneMarker;
    private GameObject indicatorObject;

    private ARPlaneManager _aRPlaneManager;
    private ARRaycastManager _aRRaycastManager;

    public GameObject _spawnObject;
    private List<ARRaycastHit> arHits = new List<ARRaycastHit>();

    private float planeArea;

    private Pose hitPose;
    private bool isObjectPlaced;
    private void Awake()
    {
        _aRPlaneManager = FindObjectOfType<ARPlaneManager>();
        _aRRaycastManager = FindObjectOfType<ARRaycastManager>();

        _aRPlaneManager.enabled = true;
        _aRRaycastManager.enabled = true;

        planeArea = 0;
        isObjectPlaced = false;
    }

    public void OnEnable()
    {
        _aRPlaneManager.planesChanged += OnPlanesChanged;
    }
    public void OnDisable()
    {
        _aRPlaneManager.planesChanged -= OnPlanesChanged;
    }

    void Update()
    {
        if(!isObjectPlaced)
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (_aRRaycastManager.Raycast(ray, arHits, TrackableType.PlaneWithinPolygon))
            {
                hitPose = arHits[0].pose;
                if (indicatorObject == null)
                {
                    indicatorObject = Instantiate(PlaneMarker, hitPose.position, hitPose.rotation);
                }
                else
                {
                    indicatorObject.transform.position = hitPose.position;
                }
            }

            if(Input.touchCount > 0 || Input.GetMouseButtonDown(0))
            {
#if UNITY_EDITOR
                OnTouchSpawn();
#else

                if(Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    OnTouchSpawn();
                }

#endif

            }
        }
        
    }

    private void OnTouchSpawn()
    {
        /*
        if (_spawnObject == null)
        {
            _spawnObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _spawnObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            _spawnObject.transform.position = hitPose.position;
            _spawnObject.transform.rotation = hitPose.rotation;
        }
        else
        {
            _spawnObject.transform.position = hitPose.position;
        }*/

        Instantiate(_spawnObject, hitPose.position, hitPose.rotation);
        Destroy(indicatorObject);
        isObjectPlaced = true;
    }
    private void OnPlanesChanged(ARPlanesChangedEventArgs obj)
    {
        foreach (ARPlane plane in obj.added)
        {
            plane.boundaryChanged += OnBoundaryChanged;
        }
        foreach (ARPlane plane in obj.removed)
        {
            plane.boundaryChanged -= OnBoundaryChanged;
        }
    }

    private void OnBoundaryChanged(ARPlaneBoundaryChangedEventArgs obj)
    {
        planeArea = Mathf.Max(planeArea, CalculatePlaneArea(obj.plane));
    }

    private float CalculatePlaneArea(ARPlane plane)
    {
        return plane.size.x * plane.size.y;
    }

    public void OnTapToPlace()
    {
        if (!isObjectPlaced)
        {
            if (indicatorObject == null)
            {
                indicatorObject = Instantiate(PlaneMarker, hitPose.position, hitPose.rotation);
            }
            else
            {
                indicatorObject.transform.position = hitPose.position;
            }
            isObjectPlaced = true;
        }
        else
        {
            Debug.Log("Object Already placed");
        }
        
    }
}
