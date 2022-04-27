using RengeGames.HealthBars;
using System.Collections;
using TMPro;
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
    public bool IsAlive {
        get { return health != 0; }
    }
    int shield;
    public int maxShield;
    public float shieldGateDuration = 3.0f;
    public int SP {
        get { return shield; }
    }
    private BulletPool bulletPool;
    private GameObject shieldGameObject;
    private PlayerShieldShaderController playerShieldShaderController;
    public GameObject interiorParentGameObject;
    private UltimateCircularHealthBar healthBar;
    private UltimateCircularHealthBar shieldBar;
    private GameObject warpDrive;
    private PlayerController leftController;
    private PlayerController rightController;
    public TextMeshProUGUI scoreText;
    private int oldScore = 0;
    private int tempScore = 0;
    private Coroutine scoreCoroutine;
    public GameObject deathTextGameObject;
    public GameObject crosshairGameObject;
    public bool enableTriggerReset = false;
    public bool TEST = false;
    
    void Start() {
        health = maxHealth;
        shield = maxShield;
        roll = transform.eulerAngles;
        pitch = transform.eulerAngles;
        rollPitch = transform.eulerAngles;

        shieldGameObject = transform.Find("Shield").gameObject;
        warpDrive = transform.Find("Warp_Drive").gameObject;

        flightStickController = flightStick.GetComponent<FlightStickController>();
        playerShieldShaderController = shieldGameObject.GetComponent<PlayerShieldShaderController>();
        playerShieldShaderController.alphaDuration = shieldGateDuration;
        leftController = transform.Find("Player/SteamVRObjects/LeftHand").GetComponent<PlayerController>();
        rightController = transform.Find("Player/SteamVRObjects/RightHand").GetComponent<PlayerController>();

        bulletPool = GetComponent<BulletPool>();
        shieldBar = interiorParentGameObject.transform
                        .Find("CockpitEquipments_Gauge1/CockpitEquipments_Gauge1-Screen/SP_Canvas/RadialSegmentedHealthBarImage")
                        .GetComponent<UltimateCircularHealthBar>();
        shieldBar.SetSegmentCount(maxShield);
        healthBar = interiorParentGameObject.transform
                        .Find("CockpitEquipments_Gauge2/CockpitEquipments_Gauge2-Screen/HP_Canvas/RadialSegmentedHealthBarImage")
                        .GetComponent<UltimateCircularHealthBar>();
        healthBar.SetSegmentCount(maxHealth);
        scoreText = interiorParentGameObject.transform
                        .Find("CockpitEquipments_Screens/CockpitEquipments_Screen-1/Score_Canvas/Score_Text")
                        .GetComponent<TextMeshProUGUI>();
        deathTextGameObject = transform.Find("HUD/HUDCanvas/Canvas Layer 3").gameObject;
        crosshairGameObject = transform.Find("HUD/HUDCanvas/Canvas Layer 4").gameObject;
    }

    void Update() {
        UpdateMovement();
        if (TEST) {
            TEST = false;
            StartWarpDrive();
        }
        UpdateScore();
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
            if (!invulnerable) {
                if (shield == 0) {
                    TakeHealthDamage(bullet.damage, bullet.Source);
                } else {
                    TakeShieldDamage(bullet.damage, bullet.Source);
                }
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

    void OnTriggerStay(Collider other) {
        if (other.tag == LaserOrigin.EnemyLaser.ToString()) {
            Laser laser = other.GetComponent<Laser>();
            if (!invulnerable) {
                if (shield == 0) {
                    TakeHealthDamage(1, laser.Source);
                } else {
                    TakeShieldDamage(1, laser.Source);
                }
            }
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

    public void StartWarpDrive() {
        warpDrive.SetActive(true);
        try {
            leftController.InvokeHapticPulse(1.0f);
            rightController.InvokeHapticPulse(1.0f);
        } catch {
            Debug.Log("Error invoking haptics, are controllers active?");
        }
        StartCoroutine(LerpSkyboxExposure(3.0f));
    }

    public void ShowDeathScreen() {
        deathTextGameObject.SetActive(true);
        crosshairGameObject.SetActive(false);
        enableTriggerReset = true;
    }

    public IEnumerator LerpSkyboxExposure(float duration) {
        float elapsed = 0.0f;
        Material skyboxMaterial = Instantiate(RenderSettings.skybox);
        Color skyboxTint = skyboxMaterial.GetColor("_Tint");
        ParticleSystem.MainModule warpParticles = warpDrive.GetComponent<ParticleSystem>().main;
        while (elapsed < duration) {
            if (elapsed < duration / 3.0f) {
                skyboxTint = Color.Lerp(skyboxTint, Color.white, elapsed / duration);
                warpParticles.simulationSpeed = 1.0f;
            } 
            if (elapsed >= duration / 3.0f) {
                skyboxTint = Color.Lerp(skyboxTint, Color.black, (elapsed - (duration / 3.0f)) / (2.0f / 3.0f) * duration);
                warpParticles.simulationSpeed = 10.0f;
            }
            skyboxMaterial.SetColor("_Tint", skyboxTint);
            RenderSettings.skybox = skyboxMaterial;

            elapsed += Time.deltaTime;
            yield return null;
        }
        skyboxMaterial.SetColor("_Tint", Color.black);
        RenderSettings.skybox = skyboxMaterial;
        yield return null;
    }

    public void UpdateScore() {
        if (oldScore != GameManager.Instance.GetCurrentScore()) {
            if (scoreCoroutine is null) {
                Debug.Log("First score");
                scoreCoroutine = StartCoroutine(LerpScore(0.5f));
            } else {
                Debug.Log("Subsequent score");
                StopCoroutine(scoreCoroutine);
                scoreCoroutine = StartCoroutine(LerpScore(0.5f));
            }
        }
    }

    IEnumerator LerpScore(float duration) {
        int newScore = GameManager.Instance.GetCurrentScore();
        oldScore = newScore;
        float elapsed = 0.0f;
        while (elapsed < duration) {
            tempScore = (int)Mathf.Lerp((float)tempScore, (float)newScore, elapsed / duration);
            Debug.Log($"Score={tempScore}");
            scoreText.text = tempScore.ToString("00000");
            elapsed += Time.deltaTime;
            yield return null;
        }
        scoreText.text = GameManager.Instance.GetCurrentScore().ToString("00000");
        yield return null;
    }
}
