using UnityEngine;
using Unity.Cinemachine;
using System;
using System.Collections.Generic;
using Input = UnityEngine.Input;

[Serializable]
public class CameraController : MonoBehaviour
{
  // Global variables
  // TODO: Figure this lerping thing out
  // [SerializeField] float smoothing = 0.95f;
  Camera cam;
  CinemachineOrbitalFollow _orbitalFollow;
  [SerializeField] Transform cameraTarget;

  // Panning
  // TODO: Figure out why panning also rotates the camera
  [Header("Panning")]
  [SerializeField] float panSpeed = 20f;
  [SerializeField] Transform maxBounds;
  [SerializeField] Transform minBounds;
  [SerializeField] float sprintMultiplier = 1.8f;

  // Zooming
  [Header("Zooming")]
  [SerializeField] float scrollSpeed = 2f;
  [SerializeField] float maxZoomDistance = 30f;
  [SerializeField] float minZoomDistance = 6.5f;
  [SerializeField] float maxVerticalAngle = 20f;
  [SerializeField] float minVerticalAngle = 0f;

  // Rotating
  [Header("Rotating")]
  CinemachineHardLookAt _lookAt;
  [SerializeField] float rotationSpeed = 2.0f;

  // "Sprinting"

  // Input
  Vector3 input;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    cam = GetComponent<Camera>();
  }

  private void Awake()
  {
    _orbitalFollow = GetComponent<CinemachineOrbitalFollow>();
    _lookAt = GetComponent<CinemachineHardLookAt>();
  }

  // Update is called once per frame
  void Update()
  {
    // TODO: Make a timer to go back to the original camera position after a while

    // UpdateRotation();
    UpdateZooming();
    // UpdatePanning();
  }

  // Panning
  // TODO: Change panning logic if object is selected
  void UpdatePanning()
  {

    Vector3 velocity = Vector3.zero;
    input = new Vector3(Input.GetAxisRaw("Horizontal"), 
                        Input.GetAxisRaw("Vertical"));


    // Assess relative vectors
    Vector3 newPosition = cameraTarget.position;
    Vector3 forward = transform.forward;
    Vector3 right = transform.right;
    forward.y = 0;
    right.y = 0;
    forward.Normalize();
    right.Normalize();

    // Forward & back
    if (input.y > 0)
      velocity += forward;
    else if (input.y < 0)
      velocity += -forward;

    // Right & left
    if (input.x > 0)
      velocity += right;
    else if (input.x < 0)
      velocity += -right;

    velocity.Normalize();

    if (Input.GetKey(KeyCode.LeftShift))
      newPosition += velocity * panSpeed * sprintMultiplier * Time.deltaTime;
    else
      newPosition += velocity * panSpeed * Time.deltaTime;

    // Boundary Check
    newPosition.x = Mathf.Clamp(newPosition.x, minBounds.position.x, maxBounds.position.x);
    newPosition.z = Mathf.Clamp(newPosition.z, minBounds.position.z, maxBounds.position.z);

    // TODO: Add lerping or smoothing
    //newPosition = Vector3.Lerp(newPosition, cameraTarget.transform.position, Time.deltaTime * smoothing);
    
    cameraTarget.transform.position = newPosition;
  }

  void UpdateZooming()
  {
    // Update zoom distance
    float newZoomDistance = _orbitalFollow.Radius;
    newZoomDistance -= Input.mouseScrollDelta.y * scrollSpeed * Time.deltaTime;

    // Update vertical offset
    float newVerticalOffset = _orbitalFollow.TargetOffset.y;
    newVerticalOffset -= Input.mouseScrollDelta.y * scrollSpeed * Time.deltaTime;

    // Clamping
    newZoomDistance = Mathf.Clamp(newZoomDistance, minZoomDistance, maxZoomDistance);
    newVerticalOffset = Mathf.Clamp(newVerticalOffset, minVerticalAngle, maxVerticalAngle);

    // Reassign the original values
    _orbitalFollow.Radius = newZoomDistance;
    _orbitalFollow.TargetOffset.y = newVerticalOffset;
  }

  void UpdateRotation()
  {
    //_orbitalFollow.HorizontalAxis.Value += rotationSpeed * Time.deltaTime;
    //return;

    // TODO: Add if you have time Andy
    float rotateDelta = 0f;
    
    // If middle clicked
    if (Input.GetMouseButton(2))
    {
      rotateDelta = Input.mousePositionDelta.x;
    }

    // Use real delta time not virtual delta time
    _orbitalFollow.HorizontalAxis.Value += rotateDelta * rotationSpeed * Time.deltaTime;
  }
}
