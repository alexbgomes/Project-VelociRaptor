using UnityEngine;

public class Enemy : MonoBehaviour {
    int health;
    protected int maxHealth;
    public int HP {
        get { return health; }
    }

    void Start() {
        health = maxHealth;
    }

    public void TakeDamage(int value, GameObject source) {
        health -= value;
        health = Mathf.Clamp(health, 0, maxHealth);
        if (health == 0) {
            OnDeath(source);
        }
    }

    protected virtual void OnDeath(GameObject cause) {
        Debug.Log($"{this} died by {cause}.");
    }
}