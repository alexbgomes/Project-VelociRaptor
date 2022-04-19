using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class PlayerController : MonoBehaviour {
    private SteamVR_Action_Boolean GrabGrip;
    private SteamVR_Action_Boolean GrabPinch;
    private SteamVR_Behaviour_Pose Pose;
    private SteamVR_Action_Vibration Haptics;
    private SteamVR_Input_Sources HandType;
    [HideInInspector]
    public Interactable grabbedObject;
    private bool pinchPressedDown = false;
    private bool gripPressedDown = false;

    public Vector3 TransformOffsetPosition {
        get {
            return transform.position - transform.parent.parent.position;
        }
    }

    void Awake() {
        Pose = GetComponent<SteamVR_Behaviour_Pose>();
        GrabGrip = GetComponent<Hand>().grabGripAction;
        GrabPinch = GetComponent<Hand>().grabPinchAction;
        Haptics = GetComponent<Hand>().hapticAction;
        HandType = GetComponent<Hand>().handType;
    }

    void Start() {
        AttachListeners();
    }

    void AttachListeners() {
        GrabGrip.AddOnStateDownListener(OnGripDown, HandType);
        GrabGrip.AddOnStateUpListener(OnGripUp, HandType);
        GrabPinch.AddOnStateDownListener(OnPinchDown, HandType);
        GrabPinch.AddOnStateUpListener(OnPinchUp, HandType);
    }

    void OnGripDown(SteamVR_Action_Boolean action, SteamVR_Input_Sources source) {
        gripPressedDown = true;
    }

    void OnGripUp(SteamVR_Action_Boolean action, SteamVR_Input_Sources source) {
        gripPressedDown = false;
    }

    void OnPinchDown(SteamVR_Action_Boolean action, SteamVR_Input_Sources source) {
        pinchPressedDown = true;
        if (grabbedObject is Interactable) {
            grabbedObject.OnPinchDown();
        }
    }

    void OnPinchUp(SteamVR_Action_Boolean action, SteamVR_Input_Sources source) {
        pinchPressedDown = false;
        if (grabbedObject is Interactable) {
            grabbedObject.OnPinchUp();
        }
    }

    public void GrabObject(Interactable objectToGrab) {
        if (objectToGrab.TryBind(this)) {
            grabbedObject = objectToGrab;
        }
    }

    public void ReleaseObject() {
        grabbedObject = null;
    }

    void OnTriggerEnter(Collider other) {
        Interactable interactable = NewInteractableRigidBody(other);
        if (!interactable) {
            return;
        }

        interactable.OnHoverEnter(this);
    }

    void OnTriggerExit(Collider other) {
        Interactable interactable = NewInteractableRigidBody(other);
        if (!interactable) {
            return;
        }

        interactable.OnHoverExit(this);
    }

    void OnTriggerStay(Collider other) {
        Interactable interactable = NewInteractableRigidBody(other);
        if (!interactable) {
            return;
        }

        if (gripPressedDown) {
            GrabObject(interactable);
        } else {
            interactable.OnHoverStay(this);
        }
    }

    Interactable NewInteractableRigidBody(Collider other) {
        if (!other.attachedRigidbody) {
            return null;
        }

        Interactable interactable = other.attachedRigidbody.GetComponent<Interactable>();
        if (!interactable) {
            return null;
        }

        if (grabbedObject == interactable) {
            return null;
        }
        return interactable;
    }

    public void InvokeHapticPulse(float duration) {
        Haptics.Execute(0.0f, duration, 150.0f, 0.75f, HandType);
    }

    public void InvokeHapticPulse(float duration, float frequency, float strength) {
        Haptics.Execute(0.0f, duration, frequency, strength, HandType);
    }

    public void InvokeStrongHapticPulse(float duration) {
        Haptics.Execute(0.0f, duration, 150.0f, 1.0f, HandType);
    }

    void Update() {
        if (!gripPressedDown && grabbedObject) {
            grabbedObject.Unbind();
        }
    }
}
