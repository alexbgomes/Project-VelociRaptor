using System.Collections;
using UnityEngine;

public class Alien : Enemy {
    public float trackingRate = 6.0f;
    public float shootingRange = 3.0f;
    public float fireRate = 2.0f;
    public bool trackPlayer = true;
    private bool canShoot = true;
    private BulletPool bulletPool;
    private MeshRenderer meshRenderer;
    private Material[] materials;
    public bool TEST = false;
    
    public override void Start() {
        maxHealth = 1;
        bulletPool = GetComponent<BulletPool>();
        meshRenderer = GetComponent<MeshRenderer>();
        materials = meshRenderer.materials;
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

        materials = new Material[] { materials[1] };
        meshRenderer.materials = materials;
        StartCoroutine(Dissolve(1.0f));
    }

    void TrackPlayer() {
        Vector3 position = transform.position;
        float t = trackingRate * Time.deltaTime;
        t = t * t * (3.0f - 2.0f * t);
        position.x = Mathf.Lerp(position.x, GameManager.Spaceship.transform.position.x, t);
        transform.position = position;
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

    IEnumerator Dissolve(float delay) {
        float elapsed = 0.0f;

        float value = 0.0f;
        materials[0].SetFloat("_Cutoff", value);
        meshRenderer.materials = materials;
        while (elapsed < delay) {
            value = elapsed / delay;
            materials[0].SetFloat("_Cutoff", value);
            meshRenderer.materials = materials;
            elapsed += Time.deltaTime;

            yield return null;
        }
        materials[0].SetFloat("_Cutoff", 1.0f);
        meshRenderer.materials = materials;
        gameObject.SetActive(false);
        yield return null;
    }
}