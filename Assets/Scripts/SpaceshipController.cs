using RengeGames.HealthBars;
using System.Collections;
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
    public bool invulnerable = false;
    int health;
    public int maxHealth;
    public int HP {
        get { return health; }
    }
    int shield;
    public int maxShield;
    public float shieldGateDuration = 3.0f;
    public int SP {
        get { return shield; }
    }
    private BulletPool bulletPool;
    public GameObject shieldGameObject;
    private PlayerShieldShaderController playerShieldShaderController;
    public GameObject interiorParentGameObject;
    private UltimateCircularHealthBar healthBar;
    private UltimateCircularHealthBar shieldBar;
    
    void Start() {
        health = maxHealth;
        shield = maxShield;
        roll = transform.eulerAngles;
        pitch = transform.eulerAngles;
        rollPitch = transform.eulerAngles;

        flightStickController = flightStick.GetComponent<FlightStickController>();
        playerShieldShaderController = shieldGameObject.GetComponent<PlayerShieldShaderController>();
        playerShieldShaderController.alphaDuration = shieldGateDuration;

        bulletPool = GetComponent<BulletPool>();
        shieldBar = interiorParentGameObject.transform
                        .Find("CockpitEquipments_Gauge1/CockpitEquipments_Gauge1-Screen/SP_Canvas/RadialSegmentedHealthBarImage")
                        .GetComponent<UltimateCircularHealthBar>();
        shieldBar.SetSegmentCount(maxShield);
        healthBar = interiorParentGameObject.transform
                        .Find("CockpitEquipments_Gauge2/CockpitEquipments_Gauge2-Screen/HP_Canvas/RadialSegmentedHealthBarImage")
                        .GetComponent<UltimateCircularHealthBar>();
        healthBar.SetSegmentCount(maxHealth);
    }

    void Update() {
        UpdateMovement();
    }

    void UpdateMovement() {
        Vector3 position = transform.position;
        
        position.x += rollSpeed * flightStickController.Value.X;
        position.x = Mathf.Clamp(position.x, -GameManager.MaxXBoundary, GameManager.MaxXBoundary);

        position.y += rollSpeed * flightStickController.Value.Y;
        position.y = Mathf.Clamp(position.y, -GameManager.MaxYBoundary, GameManager.MaxYBoundary);

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
            if (shield == 0) {
                TakeHealthDamage(bullet.damage, bullet.Source);
            } else {
                TakeShieldDamage(bullet.damage, bullet.Source);
            }
            bullet.Expire();
        }

        if (other.tag == "Pickup") {
            PickupsController pickupsController = other.GetComponent<PickupsController>();
            PickupType pickupType = pickupsController.pickupType;
            switch(pickupType) {
                case PickupType.Shield:
                    pickupsController.PickupShield(ref shield, maxShield, pickupsController.value);
                    break;
                case PickupType.Invul:
                    pickupsController.PickupInvul(InvulnerableFor, pickupsController.duration);
                    break;
                case PickupType.Multiplier:
                    pickupsController.PickupMultiplier(bulletPool, pickupsController.value, pickupsController.duration);
                    break;
            }
            Debug.Log($"Picked up {other.name}");
            Destroy(other.gameObject);
        }
    }

    public void TakeHealthDamage(int value, GameObject source) {
        health -= value;
        health = Mathf.Clamp(health, 0, maxHealth);
        StartCoroutine(LerpHealthBarOver(maxHealth - health, 0.5f));
        if (health == 0) {
            healthBar.RemovedSegments++; // push it over so there is no residual line
            OnDeath(source);
        }
    }

    public void TakeShieldDamage(int value, GameObject source) {
        shield -= value;
        shield = Mathf.Clamp(shield, 0, maxShield);
        StartCoroutine(LerpHShieldBarOver(maxShield - shield, 0.5f));
        if (shield == 0) {
            shieldBar.RemovedSegments++;
            ShieldGate(1.0f);
            StartCoroutine(playerShieldShaderController.Break());
        }
    }

    public void ShieldGate(float duration) {
        Debug.Log($"Your shield broke! You will start taking health damage after {shieldGateDuration}s.");
        InvulnerableFor(shieldGateDuration);
    }

    protected virtual void OnDeath(GameObject cause) {
        Collider[] collider = GetComponents<Collider>();
        foreach (Collider c in collider) {
            c.enabled = false;
        }
        Debug.Log($"You died by {cause.name}.");
    }

    public void InvulnerableFor(float duration) {
        invulnerable = true;
        StartCoroutine(LerpHealthBarInvulnerableColorOver(duration));
        StartCoroutine(SetVulnerableAfter(duration));
    }

    public IEnumerator SetVulnerableAfter(float duration) {
        yield return new WaitForSeconds(duration);
        invulnerable = false;
        yield return null;
    }

    public IEnumerator LerpHShieldBarOver(float targetValue, float duration) {
        float elapsed = 0.0f;

        while (elapsed < duration) {
            shieldBar.RemovedSegments = Mathf.Lerp(shieldBar.RemovedSegments, targetValue, elapsed / duration);
            elapsed += Time.deltaTime;

            yield return null;
        }
        shieldBar.RemovedSegments = targetValue;
        yield return null;
    }

    public IEnumerator LerpHealthBarOver(float targetValue, float duration) {
        float elapsed = 0.0f;

        while (elapsed < duration) {
            healthBar.RemovedSegments = Mathf.Lerp(healthBar.RemovedSegments, targetValue, elapsed / duration);
            elapsed += Time.deltaTime;

            yield return null;
        }
        healthBar.RemovedSegments = targetValue;
        yield return null;
    }

    public IEnumerator LerpHealthBarInvulnerableColorOver(float duration) {
        float elapsed = 0.0f;
        Color color = healthBar.Color;
        while (elapsed < duration) {
            if (elapsed < 2.0f / 3.0f * duration) {
                color.g = Mathf.Lerp(color.g, 1.0f, elapsed / duration / 2.0f);
                color.b = Mathf.Lerp(color.b, 1.0f, elapsed / duration / 2.0f);
            }
            if (elapsed >= 2.0f / 3.0f * duration) {
                color.g = Mathf.Lerp(color.g, 0.0f, elapsed / duration);
                color.b = Mathf.Lerp(color.b, 0.0f, elapsed / duration);
            }
            healthBar.Color = color;
            color = healthBar.Color;
            elapsed += Time.deltaTime;

            yield return null;
        }
        color.g = 0.0f;
        color.b = 0.0f;
        healthBar.Color = color;
        yield return null;
    }
}
