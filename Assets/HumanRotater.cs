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
        Vector3 dir = target.position - transform.position;
        dir.y = 0; // keep the direction strictly horizontal
        Quaternion rot = Quaternion.LookRotation(dir);
        // slerp to the desired rotation over time
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.unscaledDeltaTime*attackTimeModifier);

    }
}
