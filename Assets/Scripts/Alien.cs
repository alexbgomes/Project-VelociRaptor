using UnityEngine;

public class Alien : Enemy {
    public float trackingRate = 6.0f;
    public float shootingRange = 3.0f;
    public float fireRate = 2.0f;
    public bool trackPlayer = true;
    private bool canShoot = true;
    private BulletPool bulletPool;
    DissolveShaderController dissolveShaderController;
    
    public override void Start() {
        maxHealth = 1;
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

        StartCoroutine(dissolveShaderController.Dissolve());
    }

    void TrackPlayer() {
        Vector3 position = transform.position;

        float t = trackingRate * Time.deltaTime;
        t = t * t * (3.0f - 2.0f * t);
        position.x = Mathf.Lerp(position.x, GameManager.Spaceship.transform.position.x, t);

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
        if (Vector3.Distance(Vector3.forward * position.x, Vector3.forward * GameManager.Spaceship.transform.position.x) <= shootingRange) {
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