using UnityEngine;

public class PracticeTarget : Enemy {
    public override void Start() {
        maxHealth = 1;
        base.Start();
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
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("Die");
    }

}