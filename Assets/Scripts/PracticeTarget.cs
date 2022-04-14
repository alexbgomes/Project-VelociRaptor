using UnityEngine;

public class PracticeTarget : Enemy {
    void Start() {
        maxHealth = 1;
    }

    void OnTriggerEnter(Collider other) {
        GameObject otherGameObject = other.gameObject;
        if (!otherGameObject) {
            return;
        }

        if (other.tag == "Bullet") {
            Bullet bullet = other.GetComponent<Bullet>();
            TakeDamage(bullet.damage, bullet.Source);
        }
    }

    protected override void OnDeath(GameObject cause) {
        base.OnDeath(cause);
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("Die");
    }

}