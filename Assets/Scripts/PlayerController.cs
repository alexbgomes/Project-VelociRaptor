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

    void OnTriggerEnter(Collider other) {

    }

    void OnTriggerExit(Collider other) {

    }

    void OnTriggerStay(Collider other) {

    }

    public void InvokeHapticPulse(float duration) {
        Haptics.Execute(0.0f, duration, 150.0f, 0.75f, HandType);
    }

    public void InvokeHapticPulse(float duration, float frequency, float strength) {
        Haptics.Execute(0.0f, duration, frequency, strength, HandType);
    }

    void Update() {
        
    }
}
