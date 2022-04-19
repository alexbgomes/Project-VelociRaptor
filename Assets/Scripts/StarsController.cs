using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarsController : MonoBehaviour
{
    public float moveSpeed = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.position;
        position.z += moveSpeed;
        transform.position = position;
    }
}
