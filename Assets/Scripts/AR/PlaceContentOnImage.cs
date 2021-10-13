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
    public bool isPlaneTracking = false;

    private ARPlaneManager _arPlaneManager;
    private ARRaycastManager _aRRaycastManager;


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
            // Start Plane tracking here
            startPlaneTracking();
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            if(updatedImage.trackingState == TrackingState.Tracking || updatedImage.trackingState == TrackingState.Limited)
            {
                if (!nextState)
                {
                    nextState = true;
                    hideUI(ScanImageUI, () =>
                    {
                        showUI(TapImageUI);
                    });
                }

                _spawnObject.transform.position = updatedImage.transform.position;
                _spawnObject.transform.rotation = updatedImage.transform.rotation;
            }
            else
            {
                if (nextState)
                {
                    nextState = false;
                    hideUI(TapImageUI, () =>
                    {
                        showUI(ScanImageUI);
                    });
                }
            }
        }

    }

    private void showUI(Transform uiScreen)
    {
        GameObject go = uiScreen.gameObject;
        go.SetActive(true);
        LeanTween.alphaCanvas(go.GetComponent<CanvasGroup>(), 1, 0.5f).setEase(LeanTweenType.linear);
    }

    private void hideUI(Transform uiScreen, Action callback)
    {
        GameObject go = uiScreen.gameObject;
        LeanTween.alphaCanvas(go.GetComponent<CanvasGroup>(), 0, 0.5f).setEase(LeanTweenType.linear).setOnComplete( () =>
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

        }
    }

    void Update()
    {
        
    }
}
