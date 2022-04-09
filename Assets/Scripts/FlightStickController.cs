using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightStickController : Interactable
{
    // Start is called before the first frame update
    public float leverLeftAngle = -45;
    public float leverRightAngle = 45;
    private HingeJoint leverHingeJoint;
    private Vector3 startingEuler;
    public PlayerController boundPlayerController { get; private set; }


    void Start()
    {
        leverHingeJoint = GetComponent<HingeJoint>();
        JointLimits limits = leverHingeJoint.limits;

        limits.max = Mathf.Max(leverLeftAngle, leverRightAngle);
        limits.min = Mathf.Min(leverLeftAngle, leverRightAngle);
        leverHingeJoint.limits = limits;
        leverHingeJoint.useLimits = true;
        startingEuler = this.transform.localEulerAngles;
        UpdateHingeJoint();

    }

    // Update is called once per frame
    void Update()
    {

        float offDistance = Quaternion.Angle(this.transform.localRotation, OffHingeAngle());
        float onDistance = Quaternion.Angle(this.transform.localRotation, OnHingeAngle());      

        UpdateHingeJoint();
        
    }

    private void UpdateHingeJoint()
    {
        JointSpring spring = leverHingeJoint.spring;

        if (boundPlayerController.grabbedObject)
        {
            leverHingeJoint.useSpring = false;
        }
        /*else
        {
            if (leverIsOn)
            {
                spring.targetPosition = leverOnAngle;
            }
            else
            {
                spring.targetPosition = leverOffAngle;
            }
            leverHingeJoint.useSpring = true;
        }*/

        leverHingeJoint.spring = spring;
    }

    private Quaternion OnHingeAngle()
    {
        return Quaternion.Euler(this.leverHingeJoint.axis * leverLeftAngle + startingEuler);
    }

    private Quaternion OffHingeAngle()
    {
        return Quaternion.Euler(this.leverHingeJoint.axis * leverRightAngle + startingEuler);
    }
}
