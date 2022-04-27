using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using TMPro;

public class VRUIInput : MonoBehaviour
{
    public SteamVR_Action_Boolean TriggerOn;

    public SteamVR_Input_Sources handType;
    public GameObject cameraRig;
    private LineRenderer laserLine;
    private Transform handPosition;

    private static int counter;

    private int scale = 20;
    private RaycastHit hit;
    private bool hitCollider = false;

    void Start()
    {
        laserLine = GetComponent<LineRenderer>();
        handPosition = transform;
        laserLine.positionCount = 0;
        laserLine.SetWidth(0.01f, 0.01f);
        TriggerOn.AddOnStateDownListener(TriggerHold, handType);
        TriggerOn.AddOnStateUpListener(TriggerUp, handType);
    }

    public void TriggerHold(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if(laserLine != null)
        {
            laserLine.positionCount = 2;

        }
        hitCollider = true;
    }

    public void TriggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (laserLine != null)
        {
            laserLine.positionCount = 0;
        }
        hitCollider = false;
    }


    private void LateUpdate()
    {
        if (hitCollider && laserLine != null)
        {
            laserLine.SetPosition(0, handPosition.position);
            laserLine.SetPosition(1, handPosition.forward * scale + handPosition.position);
            if (Physics.Raycast(handPosition.position, transform.TransformDirection(handPosition.forward * scale), out hit, 50))
            {
                if (hit.collider.gameObject.CompareTag("StartButton"))
                {
                    //Destroy(cameraRig);
                    GameManager.LoadMainGameClick();
                }
            }
        }
    }

}


