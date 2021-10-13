using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class PlaceContentOnImage : MonoBehaviour
{
    [Header("AR Session")]
    public ARSession ARSession;
    public ARSessionOrigin ARSessionOrigin;

    [Header("Image Tracking")]
    public GameObject ConfirmationObject;
    public Transform ScanImageUI;
    public Transform TapImageUI;


    private ARTrackedImageManager _aRTrackedImageManager;
    private bool nextState = false;
    private GameObject _spawnObject;

    [Header("Plane Tracking")]
    public GameObject ObjectToPlace;
    public bool isPlaneTracking = false;

    private ARPlaneManager _arPlaneManager;
    private ARRaycastManager _aRRaycastManager;
    private TapToPlaceOnPlane _placeOnPlane;


    private void Awake()
    {
        _aRTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
        _aRTrackedImageManager.enabled = true;

        ScanImageUI.gameObject.SetActive(false);
        TapImageUI.gameObject.SetActive(false);
    }
    public void OnEnable()
    {
        _aRTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }
    public void OnDisable()
    {
        _aRTrackedImageManager.trackedImagesChanged -= OnImageChanged;
    }
    private void Start()
    {
        showUI(ScanImageUI);
    }

    private void OnImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            _spawnObject = Instantiate(ConfirmationObject, newImage.transform.position, newImage.transform.rotation);
            _spawnObject.SetActive(false);
            // Start Plane tracking here
            startPlaneTracking();
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            if (_placeOnPlane.isObjectPlaced)
            {
                _spawnObject.SetActive(false);
                ScanImageUI.gameObject.SetActive(false);
                TapImageUI.gameObject.SetActive(false);
            }
            else
            {
                if (updatedImage.trackingState == TrackingState.Tracking)
                {
                    if (!nextState)
                    {
                        nextState = true;
                        hideUI(ScanImageUI, () =>
                        {
                            showUI(TapImageUI);
                            _spawnObject.SetActive(true);
                            _placeOnPlane.isTapToPlace = true;
                        });
                    }

                    _spawnObject.transform.position = updatedImage.transform.position;
                    _spawnObject.transform.rotation = updatedImage.transform.rotation;
                }
                else if (updatedImage.trackingState == TrackingState.Limited)
                {
                    if (nextState)
                    {
                        hideUI(TapImageUI, () =>
                        {
                            showUI(ScanImageUI);
                            nextState = false;
                            _placeOnPlane.isTapToPlace = false;
                        });
                    }

                    _spawnObject.SetActive(false);
                }
            }

        }

        foreach (var removedImage in eventArgs.removed)
        {

        }

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
        LeanTween.alphaCanvas(go.GetComponent<CanvasGroup>(), 0, 1.0f).setEase(LeanTweenType.linear).setOnComplete( () =>
        {
            go.SetActive(false);
            callback?.Invoke();
        });
    }

    private void startPlaneTracking()
    {
        if (ARSession != null && _arPlaneManager == null)
        {

            _arPlaneManager = ARSessionOrigin.gameObject.AddComponent<ARPlaneManager>();
            _arPlaneManager.requestedDetectionMode = PlaneDetectionMode.Horizontal;
            _arPlaneManager.enabled = true;

            _aRRaycastManager = ARSessionOrigin.gameObject.AddComponent<ARRaycastManager>();
            _aRRaycastManager.enabled = true;

            _placeOnPlane = ARSessionOrigin.gameObject.AddComponent<TapToPlaceOnPlane>();
            _placeOnPlane.ObjectToPlace = ObjectToPlace;
            _placeOnPlane.enabled = true;

        }
    }

    void Update()
    {
        
    }
}
