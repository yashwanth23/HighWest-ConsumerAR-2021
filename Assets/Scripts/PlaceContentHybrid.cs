using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
[RequireComponent(typeof(ARPlaneManager))]
[RequireComponent(typeof(ARRaycastManager))]
public class PlaceContentHybrid : MonoBehaviour
{
    [Header("AR Session")]
    public ARSession ARSession;
    public ARSessionOrigin ARSessionOrigin;
    public static float ARSessionScaleFactor = 4.0f;

    [Header("Image Tracking")]
    public Transform ScanImageUI;
    public Transform ARUI;
    public GameObject ObjectToPlace;
    public Vector3 spawnedObjPosition;

    private bool nextState = false;
    private GameObject _spawnObject;

    [Header("Plane Tracking")]
    public bool isPlaneTracking = false;

    private ARTrackedImageManager _aRTrackedImageManager;
    private ARPlaneManager _arPlaneManager;
    private ARRaycastManager _aRRaycastManager;
    private List<ARRaycastHit> arHits = new List<ARRaycastHit>();
    //private TapToPlaceOnPlane _placeOnPlane;

    private Vector3 imagePosition;

    private void Awake()
    {
        _aRTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
        _arPlaneManager = FindObjectOfType<ARPlaneManager>();
        _aRRaycastManager = FindObjectOfType<ARRaycastManager>();

        _aRTrackedImageManager.enabled = true;
        _arPlaneManager.enabled = true;
        _aRRaycastManager.enabled = true;

        ScanImageUI.gameObject.SetActive(false);
        ARUI.gameObject.SetActive(false);

    }
    public void OnEnable()
    {
        _aRTrackedImageManager.trackedImagesChanged += OnImageChanged;

    }
    public void OnDisable()
    {
        _aRTrackedImageManager.trackedImagesChanged -= OnImageChanged;
        //_placeOnPlane.objectPlaced -= OnARObjectPlaced;
    }
    private void Start()
    {
        showUI(ScanImageUI);
    }

    private void OnImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            imagePosition = newImage.transform.position;

            //_spawnObject = Instantiate(ObjectToPlace, newImage.transform.position, newImage.transform.rotation);
            //_spawnObject.SetActive(false);
            // Start Plane tracking here
            //startPlaneTracking();
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            if(imagePosition != Vector3.zero)
            {
                if (!nextState)
                {
                    nextState = true;
                    hideUI(ScanImageUI, () =>
                    {
                        showUI(ARUI);
                        PlaceContentOnPlane(imagePosition);
                    });

                }
            }
            else
            {
                imagePosition = updatedImage.transform.position;
            }

        }

        foreach (var removedImage in eventArgs.removed)
        {

        }

    }

    private void OnARObjectPlaced()
    {
        showUI(ARUI);
    }

    private void showUI(Transform uiScreen)
    {
        GameObject go = uiScreen.gameObject;
        LeanTween.alphaCanvas(go.GetComponent<CanvasGroup>(), 1, 1.0f).setEase(LeanTweenType.linear);
        go.SetActive(true);
    }

    private void hideUI(Transform uiScreen, Action callback)
    {
        GameObject go = uiScreen.gameObject;
        LeanTween.alphaCanvas(go.GetComponent<CanvasGroup>(), 0, 1.0f).setEase(LeanTweenType.linear).setOnComplete(() =>
        {
            go.SetActive(false);
            callback?.Invoke();
        });
    }

    public void RotateObject()
    {
         _spawnObject.transform.Rotate(Vector3.up, 20.0f * Time.deltaTime, Space.Self);
    }

    public void ScaleObject(float value)
    {
        //Given that maximum scale factor we can is 0.5
        float scaleFactor = ARSessionScaleFactor * (1 - value);

        ARSessionOrigin.transform.localScale = Vector3.one * scaleFactor;
        ARSessionOrigin.MakeContentAppearAt(_spawnObject.transform, spawnedObjPosition);
    }

    private void PlaceContentOnPlane(Vector3 imgPosition)
    {
        Vector3 cameraPoint = Camera.main.transform.position;

        RaycastInWorldSpace(cameraPoint, imgPosition);

    }
    private void RaycastInWorldSpace(Vector3 start, Vector3 end)
    {

        if (_aRRaycastManager.Raycast(start, arHits, TrackableType.Planes))
        {
            var hitPose = arHits[0].pose;


            if (_spawnObject == null)
            {
                _spawnObject = Instantiate(ObjectToPlace, hitPose.position, hitPose.rotation);
                if (ARSessionOrigin != null)
                {
                    ARSessionOrigin.transform.localScale = Vector3.one * PlaceContentOnImage.ARSessionScaleFactor;
                    ARSessionOrigin.MakeContentAppearAt(_spawnObject.transform, hitPose.position);
                }

                spawnedObjPosition = _spawnObject.transform.position;
            }
        }
    }
}
