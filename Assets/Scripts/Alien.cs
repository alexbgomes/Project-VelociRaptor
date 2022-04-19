using UnityEngine;

public class Alien : Enemy {
    public float trackingRate = 6.0f;
    public float shootingRange = 50.0f;
    public float fireRate = 2.0f;
    public bool trackPlayer = true;
    private bool canShoot = true;
    private BulletPool bulletPool;
    DissolveShaderController dissolveShaderController;
    public bool TEST = false;
    
    public override void Start() {
        MaxHP = 1;
        ScoreValue = 100;
        bulletPool = GetComponent<BulletPool>();
        dissolveShaderController = GetComponent<DissolveShaderController>();
        base.Start();
    }

    void Update() {
        if (IsAlive) {
            if (trackPlayer) {
            TrackPlayer();
            }
            if (canShoot) {
                ShootPlayer();
            }
        }

        if (IsAlive && TEST) {
            TakeDamage(9999, GameManager.Instance.gameObject);
        }

    }

    public override void OnTriggerEnter(Collider other) {
        GameObject otherGameObject = other.gameObject;
        if (otherGameObject is null) {
            return;
        }


        if (other.tag == BulletOrigin.PlayerBullet.ToString()) {
            Bullet bullet = other.GetComponent<Bullet>();
            TakeDamage(bullet.damage, bullet.Source);
            bullet.Expire();
        }
    }

    protected override void OnDeath(GameObject cause) {
        base.OnDeath(cause);
        bulletPool.DestroyPool();
        StartCoroutine(dissolveShaderController.Dissolve());
    }

    void TrackPlayer() {
        Vector3 position = transform.position;

        float t = trackingRate * Time.deltaTime;
        t = t * t * (3.0f - 2.0f * t);
        position.x = Mathf.Lerp(position.x, GameManager.Spaceship.transform.position.x, t);

        // Prevent enemies from going inside each other
        Collider collider = GetComponent<Collider>();
        float radius = collider.bounds.size.x;
        int layerFilter = ~(1 << LayerMask.GetMask("Ignore Raycast")); // all layers excluding "Ignore Raycast"

        collider.enabled = false;
        if (!Physics.CheckSphere(position, radius, layerFilter)) {
            transform.position = position;
        }
        collider.enabled = true;

    }

    void ShootPlayer() {
        Vector3 position = transform.position;
        float distanceBetween = Mathf.Abs(position.z - GameManager.Spaceship.transform.position.z);
        if (distanceBetween <= shootingRange) {
            // Check if another enemy is in the way, if so, return
            Collider collider = GetComponent<Collider>();
            float rayWidth = collider.bounds.size.x;
            RaycastHit hit;
            if (Physics.SphereCast(position, rayWidth, Vector3.back, out hit, distanceBetween)) {
                if (hit.transform.tag == "Enemy") {
                    return;
                }
            }

            GameObject bulletObject = bulletPool.GetNextBullet();
            if (bulletPool is null) {
                return;
            }
            Bullet bullet = bulletObject.GetComponent<Bullet>();
            bullet.Incept();
            canShoot = false;
            Invoke("ResetShootingCooldown", fireRate);
        }
    }

    void ResetShootingCooldown() {
        canShoot = true;
    }

}