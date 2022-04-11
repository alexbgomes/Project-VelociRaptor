using System.Collections;
using UnityEngine;

public class Interactable : MonoBehaviour {
    public PlayerController boundPlayerController { get; private set; }
    public bool Exchangeable = true;

    public bool TryBind(PlayerController playerController) {
        if (playerController.grabbedObject) {
            return false;
        }

        if (boundPlayerController) {
            if (!Exchangeable) {
                return false;
            } else {
                Unbind();
            }
        }

        boundPlayerController = playerController;
        OnInteractionEnter();

        if (!boundPlayerController) {
            return false;
        }

        return true;
    }

    public void Unbind() {
        OnInteractionExit();
        if (boundPlayerController.grabbedObject == this) {
            boundPlayerController.ReleaseObject();
        } else {
            Debug.LogError($"Attempted to unbind {this} while holding {boundPlayerController.grabbedObject}");
        }
        boundPlayerController = null;
    }

    protected IEnumerator SmoothPosition(Vector3 oldPosition, Vector3 newPosition, float delay) {
        float elapsed = 0.0f;

        while (elapsed < delay) {
            transform.localPosition = Vector3.Lerp(oldPosition, newPosition, elapsed / delay);
            elapsed += Time.deltaTime;

            yield return null;
        }
        yield return null;
    }

    protected IEnumerator SmoothRotation(Quaternion oldRotation, Quaternion newRotation, float delay) {
        float elapsed = 0.0f;

        while (elapsed < delay) {
            transform.localRotation = Quaternion.Slerp(oldRotation, newRotation, elapsed / delay);
            elapsed += Time.deltaTime;

            yield return null;
        }
        yield return null;
    }

    // Called when this object is bound to the controller
    protected virtual void OnInteractionEnter() { }

    // Called when this object is unbound from the controller
    protected virtual void OnInteractionExit() { }

    // Called when the controller starts colliding over this object
    public virtual void OnHoverEnter(PlayerController playerController) { }

    // Called when the controller stops colliding over this object
    public virtual void OnHoverExit(PlayerController playerController) { }

    // Called when the controller stays colliding over this object
    public virtual void OnHoverStay(PlayerController playerController) { }
}