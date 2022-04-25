using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarsController : MonoBehaviour
{
    public float moveSpeed = 0.3f;
    public bool moving = true;
    private SpaceshipController spaceshipController;

    // Start is called before the first frame update
    void Start() {
        spaceshipController = GameManager.Spaceship.GetComponent<SpaceshipController>();
    }

    // Update is called once per frame
    void Update() {
        moving = spaceshipController.moving;

        if (moving) {
            Vector3 position = transform.position;
            position.z += moveSpeed;
            transform.position = position;
        }
    }
}
