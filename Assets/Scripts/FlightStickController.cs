using System.Collections;
using UnityEngine;

public class FlightStickController : Interactable {
    //private float value = 0.0f;
    [Range(2.0f, 4.0f)]
    public float sensitivity = 3.0f;
    public float maxAngle = 45.0f;
    private float breakDistance = 0.4f; // empiracally determined
    private float lerpDelay = 0.05f;
    private Quaternion defaultRotation = Quaternion.Euler(Vector3.zero);
    private bool needsReset = false;
    private bool firedOnce = false;
    public GameObject Spaceship;
    private BulletPool bulletPool;
    private ComponentValues value;

    // Value returned between -1 to 1, utilise as a vector. 
    // (E.g. 1) -1 -> max intensity in the left direction
    //           0 -> neutral
    //           1 -> max intensity in the left direction
    // (E.g. 2) User moves flight stick 50% in the left direction -> -0.5, apply 50% of throttle/acceleration in the left direction
    public ComponentValues Value {
        // get {
        //     return -value.x / maxAngle;
        // }
        get {
            return value;
        }
    }

    void Start() {
        bulletPool = Spaceship.GetComponent<BulletPool>();
        value = new ComponentValues();
        value.maxAngle = maxAngle;
    }

    void Update() {
        float newValue = value.x;

        if (boundPlayerController) {
            needsReset = true;
            Vector3 controllerPosition = boundPlayerController.TransformOffsetPosition;
            Vector3 stickPosition = Vector3.zero;

            if (Vector3.Distance(boundPlayerController.transform.position, transform.position) > breakDistance) {
                boundPlayerController.InvokeHapticPulse(0.1f);
                Unbind();
                return;
            }
            //controllerPosition.y = 1;
            ComponentValues target = new ComponentValues();
            //target.x = -Mathf.Atan((controllerPosition.y - stickPosition.y) / (controllerPosition.x - stickPosition.x)) * 180 / Mathf.PI;
            target.x = -Mathf.Atan((1.0f - stickPosition.y) / (controllerPosition.x - stickPosition.x)) * 180 / Mathf.PI;
            
            if (controllerPosition.x >= stickPosition.x) {
                target.x += 90.0f;
            } else {
                target.x -= 90.0f;
            }

            target.x *= sensitivity;
            target.x = -Mathf.Clamp(target.x, -maxAngle, maxAngle);

            float deltaAngle = Mathf.Abs(target.x - value.x);
            if (deltaAngle > 0.01f) {
                newValue = Mathf.Lerp(value.x, target.x, (1 / lerpDelay) * Time.deltaTime);
            }
            value.x = newValue;

            Vector3 newRotation = transform.localEulerAngles;
            newRotation.z = value.x;
            transform.localRotation = Quaternion.Euler(newRotation);
        } else {
            if (needsReset) {
                needsReset = false;
                StartCoroutine(SmoothRotation(transform.localRotation, defaultRotation, 0.5f));
                StartCoroutine(SmoothValueReset(0.5f));
            }
        }
    }

    IEnumerator SmoothValueReset(float delay) {
        float elapsed = 0.0f;

        while (elapsed < delay) {
            value.x = Mathf.Lerp(value.x, 0.0f, elapsed / delay);
            elapsed += Time.deltaTime;

            yield return null;
        }
        value.x = 0.0f;
        yield return null;
    }

    public override void OnHoverEnter(PlayerController playerController) {
        playerController.InvokeHapticPulse(0.1f);
    }

    public override void OnPinchDown() {
        if (bulletPool.pooledBullets[0] == null) {
            bulletPool.ReadyPool();
        }

        if (!firedOnce) {
            GameObject bulletGameObject = bulletPool.GetNextBullet();
            Bullet bullet = bulletGameObject.GetComponent<Bullet>();
            bullet.Incept();
            firedOnce = true;
            boundPlayerController.InvokeStrongHapticPulse(0.05f);
        }
    }

    public override void OnPinchUp() {
        firedOnce = false;
    }

    public class ComponentValues {
        public float x, y = 0.0f;
        public float maxAngle = 1.0f;
        
        // Normalized values, translate all range [-n, n] into range [0, 1]
        public float X {
            get {
                return x / maxAngle;
            }
        }

        public float Y {
            get {
                return y / maxAngle;
            }
        }
    }

}
