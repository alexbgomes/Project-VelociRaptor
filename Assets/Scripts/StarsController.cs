using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarsController : MonoBehaviour
{
    public float moveSpeed = GameManager.PlayerMovespeed;
    public bool moving = true;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() {
        moving = GameManager.PlayerMoving;

        if (moving) {
            Vector3 position = transform.position;
            position.z += moveSpeed;
            transform.position = position;
        }
    }
}
