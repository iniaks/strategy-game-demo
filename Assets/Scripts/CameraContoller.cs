using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraContoller : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform cameraTransform;
    public Vector3 zoomAmount;
    public Vector3 newZoom;
    private float movementSpeed;
    public float movementTime;
    public float rotationAmount;
    public Vector3 newPosition;
    public Quaternion newRotation;
    
    void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
        //.Log(newZoom);
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseInput();
        HandleMovementInput();
    }
    public float fastSpeed;
    public float normalSpeed;

    void HandleMovementInput() {
        movementSpeed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : normalSpeed ;
        
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            moveCamera("forward", movementSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            moveCamera("forward", -movementSpeed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            moveCamera("right", movementSpeed);
        }
        if (Input.GetKey(KeyCode.L) || Input.GetKey(KeyCode.LeftArrow)) {
            moveCamera("right", -movementSpeed);
        }

        if (Input.GetKey(KeyCode.Q)) {
            rotateCamera(rotationAmount);
        }
        if (Input.GetKey(KeyCode.E)) {
            rotateCamera(-rotationAmount);
        }

        if (Input.GetKey(KeyCode.R)) {
            zoomCamera();
        }
        if (Input.GetKey(KeyCode.F)) {
            zoomCamera(-1f);
        }

        //.Log(newPosition);
        
        limitBounds();
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
    
    public Vector3 dragStartPosition;
    public Vector3 dragCurrentPosition;
    public Vector3 rotateStartPosition;
    public Vector3 rotateCurrentPosition;
    void HandleMouseInput() {
        if (Input.mouseScrollDelta.y != 0) {
            zoomCamera(Input.mouseScrollDelta.y);
        }

        if (Input.GetMouseButtonDown(0)) {
            dragCamera("start");
        }
        if (Input.GetMouseButton(0)) {
            dragCamera("current");
        }

        if (Input.GetMouseButtonDown(1)) {
            rotateStartPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(1)) {
            rotateCurrentPosition = Input.mousePosition;
            Vector3 difference = rotateStartPosition - rotateCurrentPosition;
            rotateStartPosition = rotateCurrentPosition;
            rotateCamera((-difference.x / 20f));
        }
    }

    void limitBounds () {
        float mapSize = 500f;
        if (newPosition.x > mapSize) newPosition.x = mapSize;
        if (newPosition.x < -mapSize) newPosition.x = -mapSize;
        if (newPosition.z > mapSize) newPosition.z = mapSize;
        if (newPosition.z < -mapSize) newPosition.z = -mapSize;
    }

    void moveCamera(string direction, float speed = 1) {
        newPosition += direction == "forward"
            ? (transform.forward * speed)
            : (transform.right * speed);
    }

    void rotateCamera(float amount) {
        newRotation *= Quaternion.Euler(Vector3.up * amount);
    }

    void zoomCamera(float y = 1f) {
        // 限制缩放上下限
        bool ifTooClose = newZoom.y < 10 && y > 0;
        bool ifTooFar = newZoom.y > 120 && y < 0;
        if (!ifTooClose && !ifTooFar) {
            newZoom += y * zoomAmount;
        }
    }

    void dragCamera(string position) {
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float entry;
        if (plane.Raycast(ray, out entry)) {
            if (position == "start") {
                dragStartPosition = ray.GetPoint(entry);
            } else {
                dragCurrentPosition = ray.GetPoint(entry);
                newPosition = transform.position + dragStartPosition - dragCurrentPosition;
            }
        }
    }
}
