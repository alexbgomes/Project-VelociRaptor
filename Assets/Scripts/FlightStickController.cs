using System.Collections;
using UnityEngine;

public class FlightStickController : Interactable {
    [Range(2.0f, 4.0f)]
    public float sensitivity = 3.0f;
    public float maxHAngle = 45.0f;
    public float maxVAngle = 30.0f;
    private float breakDistance = 0.4f; // empiracally determined
    private float lerpDelay = 0.05f;
    private Quaternion defaultRotation = Quaternion.Euler(Vector3.zero);
    private bool needsReset = false;
    private bool firedOnce = false;
    private BulletPool bulletPool;
    private ComponentValues value;

    // Value returned between -1 to 1, utilise as a vector. 
    // (E.g. 1) -1 -> max intensity in the left direction
    //           0 -> neutral
    //           1 -> max intensity in the left direction
    // (E.g. 2) User moves flight stick 50% in the left direction -> -0.5, apply 50% of throttle/acceleration in the left direction
    public ComponentValues Value {
        get {
            return value;
        }
    }

    void Start() {
        bulletPool = GameManager.Spaceship.GetComponent<BulletPool>();
        value = new ComponentValues(0.0f, 0.0f, maxHAngle, maxVAngle);
    }

    void Update() {
        ComponentValues newValue = new ComponentValues(value.x, value.y);

        if (boundPlayerController) {
            needsReset = true;
            Vector3 controllerPosition = boundPlayerController.TransformOffsetPosition;
            Vector3 stickPosition = Vector3.zero;

            if (Vector3.Distance(boundPlayerController.transform.position, transform.position) > breakDistance) {
                boundPlayerController.InvokeHapticPulse(0.1f);
                Unbind();
                return;
            }
            ComponentValues target = new ComponentValues();
            target.x = -Mathf.Atan((1.0f - stickPosition.y) / (controllerPosition.x - stickPosition.x)) * 180.0f / Mathf.PI;
            target.y = -Mathf.Atan((1.0f - stickPosition.y) / (controllerPosition.z - stickPosition.z)) * 180.0f / Mathf.PI;
            
            if (controllerPosition.x >= stickPosition.x) {
                target.x += 90.0f;
            } else {
                target.x -= 90.0f;
            }

            if (controllerPosition.z >= stickPosition.z) {
                target.y += 90.0f - 15.0f;
            } else {
                target.y -= 90.0f + 15.0f;
            }

            target.x *= sensitivity;
            target.x = -Mathf.Clamp(target.x, -maxHAngle, maxHAngle);

            target.y *= sensitivity;// + 1.0f;
            target.y = Mathf.Clamp(target.y, -maxVAngle, maxVAngle);

            float deltaXAngle = Mathf.Abs(target.x - value.x);
            if (deltaXAngle > 0.01f) {
                newValue.x = Mathf.Lerp(value.x, target.x, (1 / lerpDelay) * Time.deltaTime);
            }
            value.x = newValue.x;

            float deltaYAngle = Mathf.Abs(target.y - value.y);
            if (deltaYAngle > 0.01f) {
                newValue.y = Mathf.Lerp(value.y, target.y, (1 / lerpDelay) * Time.deltaTime);
            }
            value.y = newValue.y;

            Vector3 newRotation = transform.localEulerAngles;
            newRotation.z = value.x;
            newRotation.x = value.y;
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
            value.y = Mathf.Lerp(value.y, 0.0f, elapsed / delay);
            elapsed += Time.deltaTime;

            yield return null;
        }
        value.x = 0.0f;
        value.y = 0.0f;
        yield return null;
    }

    public override void OnHoverEnter(PlayerController playerController) {
        playerController.InvokeHapticPulse(0.1f);
    }

    public override void OnPinchDown() {
        Debug.Log($"Value: {value.ToString()}");
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
        public float x = 0.0f;
        public float y = 0.0f;
        private float maxHAngle = 1.0f;
        private float maxVAngle = 1.0f;

        public ComponentValues(float x, float y, float maxHAngle, float maxVAngle) {
            this.x = x;
            this.y = y;
            this.maxHAngle = maxHAngle;
            this.maxVAngle = maxVAngle;
        }

        public ComponentValues(float x, float y) {
            this.x = x;
            this.y = y;
        }

        public ComponentValues() { }
        
        // Normalized values, translate all range [-n, n] into range [0, 1]
        public float X {
            get {
                return -x / maxHAngle;
            }
        }

        public float Y {
            get {
                return -y / maxVAngle;
            }
        }

        public override string ToString()
        {
            return $"({x}, {y}) =N=> ({X}, {Y})";
        }
    }

}
