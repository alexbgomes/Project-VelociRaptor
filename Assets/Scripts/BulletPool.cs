using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour {
    [HideInInspector]
    public static BulletPool bulletPoolInstance;
    [HideInInspector]
    public List<GameObject> pooledBullets;
    public GameObject poolTarget;
    public int poolCount;
    public GameObject spawnSource;
    public float bulletSpeed = 2.0f;
    public int bulletDamage = 2;

    void Awake() {
        bulletPoolInstance = this;   
    }

    void Start() {
        ReadyPool();
    }

    public void ReadyPool() {
        pooledBullets = new List<GameObject>();
        GameObject bulletGameObject;

        for (int i = 0; i < poolCount; i++) {
            bulletGameObject = Instantiate(poolTarget);
            bulletGameObject.SetActive(false);
            pooledBullets.Add(bulletGameObject);

            Bullet bullet = bulletGameObject.GetComponent<Bullet>();
            bullet.Source = spawnSource;
            bullet.speed = bulletSpeed;
            bullet.damage = bulletDamage;
        }
    }

    public GameObject GetNextBullet() {
        for (int i = 0; i < poolCount; i++) {
            if (!pooledBullets[i].activeInHierarchy) {
                return pooledBullets[i];
            }
        }

        return null;
    }

    public void DestroyPool() {
        for (int i = 0; i < poolCount; i++) {
            Destroy(pooledBullets[i]);
        }
        pooledBullets = new List<GameObject>();
        poolCount = 0;
        spawnSource = null;
    }

    public void SetBulletDamage(int damage) {
        bulletDamage = damage;
        for (int i = 0; i < poolCount; i++) {
            GameObject bulletGameObject = pooledBullets[i];
            Bullet bullet = bulletGameObject.GetComponent<Bullet>();
            bullet.damage = bulletDamage;
        }
    }
}