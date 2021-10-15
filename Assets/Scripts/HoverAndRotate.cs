using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverAndRotate : MonoBehaviour
{
    public float RotationSpeed = 0.5f;
    public float hoverMagnitude = 0.2f;

    private Vector3 initialPos;
    void Start()
    {
        initialPos = transform.localPosition;
    }
    void Update()
    {
        transform.localPosition = initialPos + hoverMagnitude * Vector3.up * Mathf.Cos(2*Time.time);
        transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime, Space.Self);
    }
}
