using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;

[RequireComponent(typeof(ARPlaneManager))]
[RequireComponent(typeof(ARRaycastManager))]
public class TapToPlaceOnPlane : MonoBehaviour
{
    private ARPlaneManager _aRPlaneManager;
    private ARRaycastManager _aRRaycastManager;
    
    private List<ARRaycastHit> arHits = new List<ARRaycastHit>();
    private GameObject spawnedObject;

    public GameObject ObjectToPlace;
    public bool isTapToPlace = false;
    public bool isObjectPlaced = false;

    private void Awake()
    {
        _aRPlaneManager = FindObjectOfType<ARPlaneManager>();
        _aRRaycastManager = FindObjectOfType<ARRaycastManager>();

        _aRPlaneManager.enabled = true;
        _aRRaycastManager.enabled = true;

    }

    public void OnEnable()
    {
        _aRPlaneManager.planesChanged += OnPlanesChanged;
    }
    public void OnDisable()
    {
        _aRPlaneManager.planesChanged -= OnPlanesChanged;
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs obj)
    {
        foreach (ARPlane plane in obj.added)
        {

        }
        foreach (ARPlane plane in obj.removed)
        {

        }
    }

    void Update()
    {
        if (isTapToPlace)
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePosition = Input.mousePosition;
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if(Physics.Raycast(ray, out hit, 100.0f))
                {
                    if (spawnedObject == null)
                    {
                        spawnedObject = Instantiate(ObjectToPlace, hit.transform.position, hit.transform.rotation);
                        isObjectPlaced = true;
                    }
                }
            }

#else
        if(Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Vector2 touchPosition = Input.GetTouch(0).position;

                OnTapToPlace(touchPosition);
            }

        }
#endif
        }
    }

    public void OnTapToPlace(Vector2 touchPosition)
    {
        if (_aRRaycastManager.Raycast(touchPosition, arHits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = arHits[0].pose;

            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(ObjectToPlace, hitPose.position, hitPose.rotation);
                isObjectPlaced = true;
            }
        }
    }
}
