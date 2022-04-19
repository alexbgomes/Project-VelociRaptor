using UnityEngine;

public class SpaceshipController : MonoBehaviour {
    public float moveSpeed = 0.3f;
    public float rollSpeed = 0.05f;
    public bool moving = false;
    public GameObject flightStick;
    FlightStickController flightStickController;
    private Vector3 roll;
    private Vector3 pitch;
    private Vector3 rollPitch;
    private float maxRollAngle = 20.0f;
    private float maxPitchAngle = 15.0f;
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
        rollPitch = transform.eulerAngles;
    }

    void Update() {
        // Movement
        Vector3 position = transform.position;
        
        position.x += rollSpeed * flightStickController.Value.X;
        position.x = Mathf.Clamp(position.x, -GameManager.MaxXBoundary, GameManager.MaxXBoundary);

        position.y += rollSpeed * flightStickController.Value.Y;
        position.y = Mathf.Clamp(position.y, -GameManager.MaxXBoundary, GameManager.MaxXBoundary);

        if (flightStickController.Value.X == 0.00f) {
            rollPitch.z = Mathf.Lerp(rollPitch.z, 0.00f, 1 / 0.05f * Time.deltaTime);
        } else {
            rollPitch.z = maxRollAngle * -flightStickController.Value.X;
        }

        if (flightStickController.Value.Y == 0.00f) {
            rollPitch.x = Mathf.Lerp(rollPitch.x, 0.00f, 1 / 0.05f * Time.deltaTime);
        } else {
            rollPitch.x = maxPitchAngle * -flightStickController.Value.Y;
        }

        if (moving) {
            position.z += moveSpeed;    
        }

        transform.position = position;
        transform.localRotation = Quaternion.Euler(rollPitch);
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
