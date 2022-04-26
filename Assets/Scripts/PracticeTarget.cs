using UnityEngine;

public class PracticeTarget : Enemy {
    public bool CanReset = true;
    private Transform LegFX;
    public override void Start() {
        MaxHP = 1;
        LegFX = transform.Find("Leg FX");
        base.Start();
    }

    void Update() {
        
    }

    public override void Reset() {
        base.Reset();
        Vector3 position = transform.position;
        position.z += 250.0f;
        transform.position = position;
        Animator animator = GetComponent<Animator>();
        animator.enabled = true;
        animator.ResetTrigger("Die");
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.x = -90.0f;
        transform.rotation = Quaternion.Euler(rotation);
        LegFX.gameObject.SetActive(true);
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
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("Die");
    }

}