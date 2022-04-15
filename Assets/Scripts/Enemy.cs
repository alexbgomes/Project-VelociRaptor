using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    int health;
    protected int maxHealth;
    public int HP {
        get { return health; }
    }

    public bool IsDead {
        get { return health == 0; }
    }

    public virtual void Start() {
        health = maxHealth;
        tag = "Enemy";
        List<GameObject> enemies = GameManager.EnemyGameObjects;
        enemies.Add(gameObject);
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
        Debug.Log($"{this.name} died by {cause.name}.");
    }

    public virtual void Reset() {
        Collider[] collider = GetComponents<Collider>();
        foreach (Collider c in collider) {
            c.enabled = true;
        }

        Animator animator = GetComponent<Animator>();
        if (animator is not null) {
            //animator.StopPlayback();
        }

        health = maxHealth;
    }

    public virtual void OnTriggerEnter(Collider other) { }
}