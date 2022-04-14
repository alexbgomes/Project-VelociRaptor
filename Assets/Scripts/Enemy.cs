using UnityEngine;

public class Enemy : MonoBehaviour {
    int health;
    protected int maxHealth;
    public int HP {
        get { return health; }
    }

    public virtual void Start() {
        health = maxHealth;
        tag = "Enemy";
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

    public virtual void OnTriggerEnter(Collider other) { }
}