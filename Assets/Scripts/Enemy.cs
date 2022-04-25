using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    int health;
    int maxHealth;
    private List<GameObject> ManagedEnemies;
    private List<int> ManagedLevelScore;
    private int scoreValue;
    protected List<Drop> DropTable;
    protected float DropGuaranteeThreshold = 0.33f; // 33% of the time, an item will drop
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
        DropTable = new List<Drop>();
    }

    public void TakeDamage(int value, GameObject source) {
        health -= value;
        health = Mathf.Clamp(health, 0, maxHealth);
        if (health == 0) {
            bool canDrop = (source != GameManager.Instance.gameObject);
            OnDeath(source, canDrop);
        }
    }

    protected virtual void OnDeath(GameObject cause, bool canDrop) {
        Collider[] collider = GetComponents<Collider>();
        foreach (Collider c in collider) {
            c.enabled = false;
        }
        Debug.Log($"{name} died by {cause.name}.");
        if (cause.name.StartsWith("Player")) {
            ManagedLevelScore.Add(scoreValue);
        }
        ManagedEnemies.Remove(gameObject); // update gm
        if (canDrop) {
            float rng = Random.Range(0.0f, 1.0f);
            if (rng <= DropGuaranteeThreshold) {
                Debug.Log($"Rolled {rng} [0, {DropTable.Count}]");
                float rollingRate = 0.0f;
                foreach (Drop dropData in DropTable) {
                    Debug.Log($"Rolling rate: {dropData.Chance + rollingRate}");
                    if (dropData.Chance + rollingRate >= rng) {
                        Debug.Log($"Dropped {dropData.PickupType}");
                        GameObject pickup = PickupsController.Create(dropData.PickupType, dropData.Value, dropData.Duration, dropData.Rotating);
                        pickup.transform.position = transform.position;
                        ManagedEnemies.Add(pickup);
                        break;
                    }
                    rollingRate += dropData.Chance;
                }
            }
        }
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

    protected virtual void SetDropTable(List<Drop> table) {
        DropTable = table;
    }

    public virtual void OnTriggerEnter(Collider other) { }
}