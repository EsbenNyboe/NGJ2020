using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanRotater : MonoBehaviour
{
    public float rotationSpeed;
    public float attackTimeModifier;
    Transform target;
    // Start is called before the first frame update
    void Start()
    {
        target = FindObjectOfType<Target>().gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        // The step size is equal to speed times frame time.
        var step = rotationSpeed * attackTimeModifier * Time.unscaledDeltaTime;

        Quaternion newRotation = Quaternion.LookRotation( -target.position-transform.position);
        // Rotate our transform a step closer to the target's.
        transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, step);
       
        
        
    }
}
