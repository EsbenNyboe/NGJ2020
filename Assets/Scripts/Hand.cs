using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{

    Transform hand;
    Transform spine;
    Transform target;
    Transform readyPos;
    public float distanceToTarget;
    public float distanceThreshold;
    public float rotationSpeed;
    public float liftHandSpeed;
    public float swatSpeed;
    public bool swat;
    public bool coolDown;
    Vector3 swatPos;

    // Start is called before the first frame update
    void Start()
    {
        hand = gameObject.GetComponent<Transform>();
        spine = GameObject.FindObjectOfType<Spine>().GetComponent<Transform>();
        target = GameObject.FindObjectOfType<Target>().GetComponent<Transform>();
        readyPos = GameObject.FindObjectOfType<readyPos>().GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        distanceToTarget = Vector3.Distance(target.position, spine.position);
        if(distanceToTarget< distanceThreshold)
        {
            // The step size is equal to speed times frame time.
            var step = rotationSpeed * Time.deltaTime;

            Quaternion newRotation = Quaternion.LookRotation(spine.position - target.position);
            // Rotate our transform a step closer to the target's.
            spine.rotation = Quaternion.RotateTowards(spine.rotation, newRotation, step);
            if(!coolDown)
            LiftArm();
        }
        if(swat)
        {
            Vector3 targetDir = (swatPos - hand.position).normalized;

            hand.position = hand.position + targetDir * swatSpeed * Time.deltaTime;
            StartCoroutine(SwatCoroutine());
        }

        
    }

    void LiftArm()
    {
       
        Vector3 readyDirNorm =(readyPos.position - hand.position).normalized;

        hand.position = hand.position + readyDirNorm * liftHandSpeed * Time.deltaTime;

        if(Vector3.Distance(hand.position, readyPos.position) < 0.05f) //magic number indtil videre
        {
            swat = true;
            swatPos = target.position;
        }
    }
    IEnumerator SwatCoroutine()
    {
        
        yield return new WaitForSeconds(2); //magic number
        swat = false;
        coolDown = true;
        yield return new WaitForSeconds(2);
        coolDown = false;
    }
 
}
