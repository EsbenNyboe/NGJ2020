using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{

    Transform hand;
    Transform spine;
    Transform target;
    Transform readyPosVert;
    Transform readyPosHori;
    public float distanceToTarget;
    public float distanceThreshold;
    public float rotationSpeed;
    public float liftHandSpeed;
    public float swatSpeed;
    public bool swat;
    public bool coolDown;
    public bool reachedTarget;
    Vector3 swatPos;
    float t;
    Vector3 startPos;
    Vector3 overShoot;
    Transform currentReadyPos;
    Vector3 lastFramePos;
    FlyController2 fly;
    public float attackTimeModifier;
    public float homingUntilDistance;
    public float swatTime;
    public float cooldownTime;
   
    // Start is called before the first frame update
    void Start()
    {
        hand = gameObject.GetComponent<Transform>();
        spine = GameObject.FindObjectOfType<Spine>().GetComponent<Transform>();
        target = GameObject.FindObjectOfType<Target>().GetComponent<Transform>();
        readyPosVert = GameObject.FindObjectOfType<readyPos>().GetComponent<Transform>();
        readyPosHori = GameObject.FindObjectOfType<readyPosHorizontal>().GetComponent<Transform>();
        lastFramePos = hand.position;
        fly = GameObject.FindObjectOfType<FlyController2>();

    }

    // Update is called once per frame
    void Update()
    {
       
        distanceToTarget = Vector3.Distance(target.position, spine.position);
        if(distanceToTarget< distanceThreshold)
        {
            // The step size is equal to speed times frame time.
            var step = rotationSpeed *attackTimeModifier * Time.unscaledDeltaTime;

            Quaternion newRotation = Quaternion.LookRotation(spine.position - target.position);
            // Rotate our transform a step closer to the target's.
            spine.rotation = Quaternion.RotateTowards(spine.rotation, newRotation, step);
            if(!coolDown)
            LiftArm();
        }
        if(swat)
        {
            //Vector3 targetDir = (swatPos - hand.position).normalized;

            // hand.position = hand.position + targetDir * swatSpeed * Time.deltaTime;
            hand.right = -(hand.position - lastFramePos);
            
            if(Vector3.Distance(target.position, hand.position) > homingUntilDistance && !reachedTarget)
            {
                swatPos = target.position;
                overShoot = target.position + (target.position - hand.position).normalized;
            }
           
                t += swatSpeed *attackTimeModifier * Time.unscaledDeltaTime;
            if (currentReadyPos == readyPosVert ) //Vertical curves
            {
                if (!reachedTarget)
                    hand.position = BezierCurve(startPos, swatPos, t * t);
                else
                    hand.position = BezierCurve2(swatPos, overShoot, 2 * t);
            }
            else if(currentReadyPos == readyPosHori) //Horizontal curves
            {
                if (!reachedTarget)
                    hand.position = BezierCurve3(startPos, swatPos, t * t);
                else
                    hand.position = BezierCurve2(swatPos, overShoot, 2 * t);
            }
                if (Vector3.Distance(hand.position, swatPos) < 0.1f && !reachedTarget)
                {

                // hand.position = swatPos;
                   // startPos = swatPos;
                    //swatPos = overShoot;
                reachedTarget = true;
                t = 0;
               
                }
            
        
            StartCoroutine(SwatCoroutine());

        }

        lastFramePos = hand.position;
        
    }

    Vector3 BezierCurve(Vector3 a, Vector3 d , float t) // kan optimereres
    {
        
        Vector3 b = a + 1f * Vector3.up;
        Vector3 c = d + +1f * Vector3.up;

        Vector3 m = Vector3.Lerp(a, b, t);
        Vector3 n = Vector3.Lerp(b, c, t);
        Vector3 o = Vector3.Lerp(c, d, t);
        Vector3 p = Vector3.Lerp(m, n, t);
        Vector3 e = Vector3.Lerp(n, o, t);
        Vector3 pointOnCurve = Vector3.Lerp(p, e, t);

        return pointOnCurve;

    }
    Vector3 BezierCurve2(Vector3 a, Vector3 d, float t) // kan optimereres
    {

        Vector3 b = a + -0.2f * Vector3.up;
        Vector3 c = d + +-0.1f * Vector3.up;

        Vector3 m = Vector3.Lerp(a, b, t);
        Vector3 n = Vector3.Lerp(b, c, t);
        Vector3 o = Vector3.Lerp(c, d, t);
        Vector3 p = Vector3.Lerp(m, n, t);
        Vector3 e = Vector3.Lerp(n, o, t);
        Vector3 pointOnCurve = Vector3.Lerp(p, e, t);

        return pointOnCurve;

    }
    Vector3 BezierCurve3(Vector3 a, Vector3 d, float t) // kan optimereres
    {

        Vector3 b = a + 0.5f * Vector3.left;
        Vector3 c = d + 0.5f * Vector3.left;

        Vector3 m = Vector3.Lerp(a, b, t);
        Vector3 n = Vector3.Lerp(b, c, t);
        Vector3 o = Vector3.Lerp(c, d, t);
        Vector3 p = Vector3.Lerp(m, n, t);
        Vector3 e = Vector3.Lerp(n, o, t);
        Vector3 pointOnCurve = Vector3.Lerp(p, e, t);

        return pointOnCurve;

    }
    Vector3 BezierCurve4(Vector3 a, Vector3 d, float t) // kan optimereres
    {

        Vector3 b = a + -0.2f * Vector3.left;
        Vector3 c = d + -0.2f * Vector3.left;

        Vector3 m = Vector3.Lerp(a, b, t);
        Vector3 n = Vector3.Lerp(b, c, t);
        Vector3 o = Vector3.Lerp(c, d, t);
        Vector3 p = Vector3.Lerp(m, n, t);
        Vector3 e = Vector3.Lerp(n, o, t);
        Vector3 pointOnCurve = Vector3.Lerp(p, e, t);

        return pointOnCurve;

    }
    void LiftArm()
    {
        Vector3 readyDirNorm;

        if (FlyController2.grounded == true) // Vertical swat
        {
            currentReadyPos = readyPosVert;
        }
        else //Horizontal swat
            currentReadyPos = readyPosHori;

            readyDirNorm = (currentReadyPos.position - hand.position).normalized;

        hand.position = hand.position + readyDirNorm * liftHandSpeed * attackTimeModifier* Time.unscaledDeltaTime;

        if(Vector3.Distance(hand.position, currentReadyPos.position) < 0.05f && !swat) //magic number indtil videre
        {
            swat = true;
            swatPos = target.position;
            startPos = hand.position;
            overShoot = target.position + (target.position -hand.position).normalized; 
            t = 0;
        }
    }
    IEnumerator SwatCoroutine()
    {
        
        float scale = Time.timeScale;
        yield return new WaitForSeconds((swatTime/attackTimeModifier)*scale); //magic number
        swat = false;
        reachedTarget = false;
        coolDown = true;
       
        yield return new WaitForSeconds((cooldownTime/attackTimeModifier)*scale);
        coolDown = false;
        
        
    }
 
}
