using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public int damage;
    public GameObject Source { get; set; }
    float timeToLive = 5.00f;
    public float speed = 0.5f;
    public Direction direction = Direction.Outbound;
    public BulletOrigin bulletOrigin;

    void Start() {
        tag = bulletOrigin.ToString();
    }

    void Update() {
        Vector3 position = transform.position;
        position.z += (int)direction * speed;
        transform.position = position;
    }

    void OnEnable() {
        Invoke("Expire", timeToLive);
    }

    public void Expire() {
        gameObject.SetActive(false);
    }

    public void Incept() {
        Vector3 blasterSize = Source.GetComponent<BoxCollider>().bounds.size;
        //Debug.Log($"Blaster dims: {blasterSize}");
        // use above to place at the tip of the blaster
        Vector3 position = Source.transform.position;
        Quaternion rotation = Source.transform.rotation;
        Debug.Log($"Spawning bullet at: {position}");
        transform.position = position;
        transform.rotation = rotation;
        gameObject.SetActive(true);
    }
}

public enum Direction : int {
    Outbound = 1,
    Inbound = -1
}

public enum BulletOrigin {
    PlayerBullet,
    EnemyBullet
}