using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipController : MonoBehaviour {
    public float moveSpeed = 0.3f;
    public float rollSpeed = 0.05f;
    public bool moving = false;
    public GameObject flightStick;
    FlightStickController flightStickController;
    private Vector3 roll;
    private float maxRollAngle = 15.0f;
    
    void Start() {
        flightStickController = flightStick.GetComponent<FlightStickController>();
        roll = transform.eulerAngles;
    }

    void Update() {
        // Movement
        Vector3 position = transform.position;
        position.x += rollSpeed * flightStickController.Value;
        position.x = Mathf.Clamp(position.x, -GameManager.MaxXBoundary, GameManager.MaxXBoundary);

        roll.z = maxRollAngle * -flightStickController.Value;

        if (moving) {
            position.z += moveSpeed;    
        }

        transform.position = position;
        transform.localRotation = Quaternion.Euler(roll);
    }
}
