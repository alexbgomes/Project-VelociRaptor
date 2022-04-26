using System.Collections;
using System.Collections.Generic;
using VolumetricLines;
using UnityEngine;

public class Destroyer : Enemy {
    public bool moving = true;
    public float moveSpeed = GameManager.PlayerMovespeed;
    public float trackingRate = 4.0f;
    public float shootingRange = 100.0f;
    public float fireRate = 2.0f;
    public float laserCooldown = 15.0f;
    public float laserDuration = 5.0f;
    public bool trackPlayer = true;
    private bool canShoot = true;
    private bool canLaser = false;
    private BulletPool bulletPool;
    DissolveShaderController dissolveShaderController;
    private GameObject laserObject;
    VolumetricLineBehavior volumetricLineBehavior;
    public bool TEST;
    
    public override void Start() {
        MaxHP = 500;
        ScoreValue = 5000;
        bulletPool = GetComponent<BulletPool>();
        dissolveShaderController = GetComponent<DissolveShaderController>();
        base.Start();
        List<Drop> dropTable = new List<Drop> { };
        SetDropTable(dropTable);
        Invoke("ResetLaserCooldown", laserCooldown);
        moving = GameManager.PlayerMoving;
        laserObject = transform.Find("DestroyerLaser").gameObject;
        volumetricLineBehavior = laserObject.GetComponent<VolumetricLineBehavior>();
        laserObject.GetComponent<Laser>().Source = gameObject;
    }

    void LateUpdate() {
        if (IsAlive) {
            if (trackPlayer) {
            TrackPlayer();
            }
            if (canShoot) {
                if (canLaser) {
                    DeathLaser();
                } else {
                    ShootPlayer();
                }
            }

            if (TEST) {
                TakeDamage(9999, GameManager.Spaceship.gameObject);
            }

            if (moving) {
                Vector3 position = transform.position;
                position.z += moveSpeed;
                transform.position = position;
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

    protected override void OnDeath(GameObject cause, bool canDrop) {
        base.OnDeath(cause, canDrop);
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
                if (hit.transform.tag == "Enemy" && hit.transform != transform) {
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

    void DeathLaser() {
        Debug.Log("Death laser firing!");
        StartCoroutine(StartLerpLaserWidth(0.5f, 20.0f, 1.0f));
    }

    IEnumerator StartLerpLaserWidth(float a, float b, float duration) {
        float elapsed = 0.0f;
        float width = a;
        canLaser = false;
        trackPlayer = false;
        canShoot = false;
        laserObject.SetActive(true);
        while (elapsed < duration) {
            width = Mathf.Lerp(width, b, elapsed / duration);
            volumetricLineBehavior.LineWidth = width;
            elapsed += Time.deltaTime;
            yield return null;
        }
        volumetricLineBehavior.LineWidth = b;
        laserObject.GetComponent<Collider>().enabled = true;
        yield return new WaitForSeconds(laserDuration);
        laserObject.GetComponent<Collider>().enabled = false;
        elapsed = 0.0f;
        while (elapsed < duration) {
            width = Mathf.Lerp(width, a, elapsed / duration);
            volumetricLineBehavior.LineWidth = width;
            elapsed += Time.deltaTime * 0.5f;
            yield return null;
        }
        volumetricLineBehavior.LineWidth = width;
        laserObject.SetActive(false);
        trackPlayer = true;
        Invoke("ResetShootingCooldown", 1.0f);
        Invoke("ResetLaserCooldown", laserCooldown);
        yield return null;
    }



    void ResetShootingCooldown() {
        canShoot = true;
    }

    void ResetLaserCooldown() {
        canLaser = true;
    }
}