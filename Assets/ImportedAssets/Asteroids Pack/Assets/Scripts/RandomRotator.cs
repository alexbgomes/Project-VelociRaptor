using UnityEngine;
using System.Collections;

public class RandomRotator : MonoBehaviour
{
    private float tumble = 1;

    void Start()
    {
        GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * tumble;
    }
}