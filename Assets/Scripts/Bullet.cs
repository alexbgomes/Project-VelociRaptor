using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public int damage;
    public GameObject Source { get; set; }
    float timeToLive = 5000f;
    public float speed = 0.5f;
    public Direction direction = Direction.Outbound;

    void Start() {
        tag = "Bullet";
        Expire(); // start inactive, instantiated within the bullet pool
    }

    void Update() {
        Vector3 position = transform.position;
        position.z += (int)direction * speed;
        transform.position = position;
    }

    void OnEnabled() {
        Invoke("Expire", timeToLive);
    }

    public void Expire() {
        gameObject.SetActive(false);
    }

    public void Incept() {
        RectTransform blasterRectTransform = (RectTransform)Source.transform;
        Debug.Log($"Blaster dims: {blasterRectTransform.rect.width}x{blasterRectTransform.rect.height} (wxh)");
        // use above to place at the tip of the blaster
        Vector3 position = Source.transform.position;
        gameObject.SetActive(true);
    }
}

public enum Direction : int {
    Outbound = 1,
    Inbound = -1
}