using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayerController : MonoBehaviour {
    public SteamVR_Action_Boolean GrabGrip;
    public SteamVR_Action_Boolean GrabPinch;
    private SteamVR_Behaviour_Pose Pose;
    public SteamVR_Action_Vibration Haptics;
    public SteamVR_Input_Sources HandType;
    public Interactable grabbedObject;
    private bool pinchPressedDown = false;

    void Awake() {
        Pose = GetComponent<SteamVR_Behaviour_Pose>();
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

    }

    void OnGripUp(SteamVR_Action_Boolean action, SteamVR_Input_Sources source) {

    }

    void OnPinchDown(SteamVR_Action_Boolean action, SteamVR_Input_Sources source) {

    }

    void OnPinchUp(SteamVR_Action_Boolean action, SteamVR_Input_Sources source) {

    }

    void OnTriggerEnter(Collider other)
    {
        Interactable interactable = NewInteractableRigidBody(other);
        if (!interactable)
        {
            return;
        }

        interactable.OnHoverEnter(this);
    }

    void OnTriggerExit(Collider other)
    {
        Interactable interactable = NewInteractableRigidBody(other);
        if (!interactable)
        {
            return;
        }

        interactable.OnHoverExit(this);
    }

    void OnTriggerStay(Collider other)
    {
        Interactable interactable = NewInteractableRigidBody(other);
        if (!interactable)
        {
            return;
        }

        if (pinchPressedDown)
        {
            GrabObject(interactable);
        }
        else
        {
            interactable.OnHoverStay(this);
        }
    }

    Interactable NewInteractableRigidBody(Collider other)
    {
        if (!other.attachedRigidbody)
        {
            return null;
        }

        Interactable interactable = other.attachedRigidbody.GetComponent<Interactable>();
        if (!interactable)
        {
            return null;
        }

        if (grabbedObject == interactable)
        {
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

    public void GrabObject(Interactable objectToGrab)
    {
        if (objectToGrab.TryBind(this))
        {
            grabbedObject = objectToGrab;
        }
    }
    public void ReleaseObject()
    {
        grabbedObject = null;
    }

    void Update() {
        if (!pinchPressedDown && grabbedObject)
        {
            grabbedObject.Unbind();
        }
    }
}
