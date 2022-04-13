using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipController : MonoBehaviour {
    public float speed = 0.25f;
    public GameObject flightStick;
    FlightStickController flightStickController;
    
    void Start() {
        flightStickController = flightStick.GetComponent<FlightStickController>();
    }

    void Update() {
        // Movement
        Vector3 position = transform.position;
        position.x += speed * flightStickController.Value;
        position.x = Mathf.Clamp(position.x, -GameManager.MaxXBoundary, GameManager.MaxXBoundary);
        transform.position = position;
    }
}
