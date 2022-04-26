using System;
using System.Collections;
using UnityEngine;

public class PickupsController : MonoBehaviour {
    public PickupType pickupType;
    public int value;
    public float duration;
    public bool rotating = true;

    void Start() {
        tag = "Pickup";
    }

    public static GameObject Create(PickupType pickupType, int value, float duration, bool rotating) {
        GameObject instance = null;
        switch(pickupType) {
            case PickupType.Shield:
                instance = Instantiate(GameManager.pickupShieldPrefab);
                break;
            case PickupType.Invul:
                instance = Instantiate(GameManager.pickupInvulPrefab);
                break;
            case PickupType.Multiplier:
                instance = Instantiate(GameManager.pickupMultiplierPrefab);
                break;
        }
        
        if (instance is null) {
            return null;
        }

        PickupsController pickupsController = instance.GetComponent<PickupsController>();
        pickupsController.pickupType = pickupType;
        pickupsController.value = value;
        pickupsController.duration = duration;
        pickupsController.rotating = rotating;
        return instance;
    }

    void Update() {
        if (rotating) {
            transform.Rotate(new Vector3(0.0f, 0.0f, 30.0f) * Time.deltaTime);
        }
    }

    public void PickupShield(ref int shield, int maxShield, int amount) {
        shield = Mathf.Clamp(shield + amount, 0, maxShield);
    }

    public void PickupInvul(Action<float> InvulFunc, float duration) {
        InvulFunc(duration);
    }

    public void PickupMultiplier(BulletPool bulletPool, int multiplier, float duration) {
        int oldDamage = bulletPool.bulletDamage;
        StartCoroutine(CallChangeXForYOverTAndRevert(bulletPool.SetBulletDamage, oldDamage, oldDamage * multiplier, duration));
    }

    IEnumerator CallChangeXForYOverTAndRevert(Action<int> Callable, int oldValue, int newValue, float duration) {
        Callable(newValue);
        yield return new WaitForSeconds(duration);
        Callable(oldValue);
        yield return null;
    }
}

public enum PickupType {
    Shield,
    Invul,
    Multiplier
}