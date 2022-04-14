using System.Collections;
using UnityEngine;

public class FlightStickController : Interactable {
    private float value = 0.0f;
    [Range(2.0f, 4.0f)]
    public float sensitivity = 3.0f;
    public float maxAngle = 45.0f;
    private float breakDistance = 0.4f; // empiracally determined
    private float lerpDelay = 0.05f;
    private Quaternion defaultRotation = Quaternion.Euler(Vector3.zero);
    private bool needsReset = false;
    private bool firedOnce = false;
    public GameObject Spaceship;
    BulletPool bulletPool;

    // Value returned between -1 to 1, utilise as a vector. 
    // (E.g. 1) -1 -> max intensity in the left direction
    //           0 -> neutral
    //           1 -> max intensity in the left direction
    // (E.g. 2) User moves flight stick 50% in the left direction -> -0.5, apply 50% of throttle/acceleration in the left direction
    public float Value {
        get {
            return -value / maxAngle;
        }
    }

    void Start() {
        bulletPool = Spaceship.GetComponent<BulletPool>();
    }

    void Update() {
        float newValue = value;

        if (boundPlayerController) {
            needsReset = true;
            Vector3 controllerPosition = boundPlayerController.TransformOffsetPosition;
            Vector3 stickPosition = Vector3.zero;

            if (Vector3.Distance(boundPlayerController.transform.position, transform.position) > breakDistance) {
                boundPlayerController.InvokeHapticPulse(0.1f);
                Unbind();
                return;
            }
            controllerPosition.y = 1;
            float target = -Mathf.Atan((controllerPosition.y - stickPosition.y) / (controllerPosition.x - stickPosition.x)) * 180 / Mathf.PI;
            
            if (controllerPosition.x >= stickPosition.x) {
                target += 90.0f;
            } else {
                target -= 90.0f;
            }

            target *= sensitivity;
            target = -Mathf.Clamp(target, -maxAngle, maxAngle);

            float deltaAngle = Mathf.Abs(target - value);
            if (deltaAngle > 0.01f) {
                newValue = Mathf.Lerp(value, target, (1 / lerpDelay) * Time.deltaTime);
            }
            value = newValue;

            Vector3 newRotation = transform.localEulerAngles;
            newRotation.z = value;
            transform.localRotation = Quaternion.Euler(newRotation);
        } else {
            if (needsReset) {
                needsReset = false;
                StartCoroutine(SmoothRotation(transform.localRotation, defaultRotation, 0.5F));
                StartCoroutine(SmoothValueReset(0.5f));
            }
        }
    }

    IEnumerator SmoothValueReset(float delay) {
        float elapsed = 0.0f;

        while (elapsed < delay) {
            value = Mathf.Lerp(value, 0.0f, elapsed / delay);
            elapsed += Time.deltaTime;

            yield return null;
        }
        value = 0.0f;
        yield return null;
    }

    public override void OnHoverEnter(PlayerController playerController) {
        playerController.InvokeHapticPulse(0.1f);
    }

    public override void OnPinchDown() {
        if (!firedOnce) {
            GameObject bulletGameObject = bulletPool.GetNextBullet();
            Bullet bullet = bulletGameObject.GetComponent<Bullet>();
            bullet.Incept();
            firedOnce = true;
        }
    }

    public override void OnPinchUp() {
        firedOnce = false;
    }

}
