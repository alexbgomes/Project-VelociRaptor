using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    int health;
    int maxHealth;
    private List<GameObject> ManagedEnemies;
    private List<int> ManagedLevelScore;
    private int scoreValue;
    public int HP {
        get { return health; }
    }

    public int MaxHP {
        get { 
            return maxHealth; 
        } set {
            maxHealth = value;
        }
    }

    public bool IsDead {
        get { return health == 0; }
    }

    public bool IsAlive {
        get { return health != 0; }
    }

    public int ScoreValue {
        get {
            return scoreValue;
        } set {
            scoreValue = value;
        }
    }

    public virtual void Start() {
        health = maxHealth;
        tag = "Enemy";
        ManagedEnemies = GameManager.EnemyGameObjects;
        ManagedEnemies.Add(gameObject);
        ManagedLevelScore = GameManager.CurrentLevelScore;
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
        Debug.Log($"{name} died by {cause.name}.");
        ManagedLevelScore.Add(scoreValue);
        //GameManager.CurrentLevelScore = ManagedLevelScore;
        ManagedEnemies.Remove(gameObject); // update gm
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