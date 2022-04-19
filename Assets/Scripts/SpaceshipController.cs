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
    private Vector3 pitch;
    private float maxRollAngle = 15.0f;

    int health;
    public int maxHealth;
    public int HP {
        get { return health; }
    }
    
    void Start() {
        health = maxHealth;
        flightStickController = flightStick.GetComponent<FlightStickController>();
        roll = transform.eulerAngles;
        pitch = transform.eulerAngles;
    }

    void Update() {
        // Movement
        Vector3 position = transform.position;
        position.x += rollSpeed * flightStickController.Value.x;
        position.x = Mathf.Clamp(position.x, -GameManager.MaxXBoundary, GameManager.MaxXBoundary);

        if (flightStickController.Value.x == 0.00f) {
            roll.z = Mathf.Lerp(roll.z, 0.00f, 1/0.05f * Time.deltaTime);
        } else {
            roll.z = maxRollAngle * -flightStickController.Value.x;
        }

        if (moving) {
            position.z += moveSpeed;    
        }

        transform.position = position;
        //transform.localRotation = Quaternion.Euler(roll);
        transform.Rotate(roll, Space.World);
        //transform.Rotate(pitch, Space.World);
    }

    void OnTriggerEnter(Collider other) {
        GameObject otherGameObject = other.gameObject;
        if (otherGameObject is null) {
            return;
        }


        if (other.tag == BulletOrigin.EnemyBullet.ToString()) {
            Bullet bullet = other.GetComponent<Bullet>();
            TakeDamage(bullet.damage, bullet.Source);
            bullet.Expire();
        }
    }

    public void TakeDamage(int value, GameObject source) {
        health -= value;
        health = Mathf.Clamp(health, 0, maxHealth);
        if (health == 0) {
            OnDeath(source);
        }
    }

    protected virtual void OnDeath(GameObject cause) {
        Collider[] collider = GetComponents<Collider>();
        foreach (Collider c in collider) {
            c.enabled = false;
        }
        Debug.Log($"You died by {cause.name}.");
    }
}
