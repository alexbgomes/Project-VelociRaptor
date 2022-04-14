using UnityEngine;

public class PracticeTarget : Enemy {
    void Start() {
        maxHealth = 1;
    }

    public override void OnTriggerEnter(Collider other) {
        Debug.Log($"{transform.name} collided with {other.name}");
        GameObject otherGameObject = other.gameObject;
        if (otherGameObject is null) {
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