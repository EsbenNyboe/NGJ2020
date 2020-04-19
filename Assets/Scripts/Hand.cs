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
    Vector3 lastFrameTargetPos;
    FlyController2 fly;
    public float attackTimeModifier;
    public float homingUntilDistance;
    public float swatTime;
    public float cooldownTime;
    Projector shadow;
    Transform restPos;
    public Animator animator;
   public bool resetting;
    GameManager gameManager;

    
   
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        hand = gameObject.GetComponent<Transform>();
        spine = GameObject.FindObjectOfType<Spine>().GetComponent<Transform>();
        target = GameObject.FindObjectOfType<Target>().GetComponent<Transform>();
        readyPosVert = GameObject.FindObjectOfType<readyPos>().GetComponent<Transform>();
        readyPosHori = GameObject.FindObjectOfType<readyPosHorizontal>().GetComponent<Transform>();
        lastFramePos = hand.position;
        lastFrameTargetPos = target.position;
        restPos = GameObject.FindObjectOfType<restPos>().GetComponent<Transform>();
        fly = GameObject.FindObjectOfType<FlyController2>();
        shadow = gameObject.GetComponentInChildren<Projector>();
    }

    // Update is called once per frame
    void Update()
    {
        shadow.transform.LookAt(target.transform);
        distanceToTarget = Vector3.Distance(target.position, spine.position);
        shadow.orthographicSize = 10f - distanceToTarget;
        
        if (resetting)
        {
            animator.SetInteger("HandState", 3);
            //hand.right = (hand.position - lastFramePos);
            t += swatSpeed * attackTimeModifier * Time.unscaledDeltaTime;
            hand.position = BezierCurve(startPos, restPos.position, t * t);
            if(Vector3.Distance(restPos.position, hand.position) < 0.3f)
            {
                animator.SetInteger("HandState", 0);
                resetting = false;
            }
        }
        else
        {

            if (distanceToTarget < distanceThreshold)
            {
                // The step size is equal to speed times frame time.
                var step = rotationSpeed * attackTimeModifier * Time.unscaledDeltaTime;

                Quaternion newRotation = Quaternion.LookRotation(spine.position - target.position);
                // Rotate our transform a step closer to the target's.
                spine.rotation = Quaternion.RotateTowards(spine.rotation, newRotation, step);
                if (!coolDown)
                    LiftArm();
                
            }
            if (swat)
            {
                animator.SetInteger("HandState", 2);
                hand.right = -(hand.position - lastFramePos);

                //homing until within certain distance
                if (Vector3.Distance(target.position, hand.position) > homingUntilDistance && !reachedTarget)
                {
                    swatPos = lastFrameTargetPos;
                    overShoot = lastFrameTargetPos + (lastFrameTargetPos - hand.position).normalized;
                }


                t += swatSpeed * attackTimeModifier * Time.unscaledDeltaTime;
                if (currentReadyPos == readyPosVert) //Vertical curves
                {
                    if (!reachedTarget)
                        hand.position = BezierCurve(startPos, swatPos, t * t);
                    else
                        hand.position = BezierCurve2(swatPos, overShoot, 2 * t);
                }
                else if (currentReadyPos == readyPosHori) //Horizontal curves
                {
                    if (!reachedTarget)
                        hand.position = BezierCurve3(startPos, swatPos, t * t);
                    else
                        hand.position = BezierCurve2(swatPos, overShoot, 2 * t);
                }
                //hvornår har den nået target
                if (Vector3.Distance(hand.position, swatPos) < 0.01f && !reachedTarget) 
                {

                   
                    reachedTarget = true;
                    t = 0;
                    StopAllCoroutines();
                    StartCoroutine(SwatCoroutine());
                }
                //hvor tæt er den på overshoot position så den kan resette
                if (Vector3.Distance(hand.position, overShoot) < 0.01f && reachedTarget && !resetting)
                {
                    resetting = true;
                    t = 0;
                    startPos = hand.position;
                    StopAllCoroutines();
                    StartCoroutine(CoolDownCoroutine());
                }



            }
        }
        lastFramePos = hand.position;
        lastFrameTargetPos = target.position;
        
    }
    
    public void OnCollisionEnter(Collision collision)
    {

      
        if (collision.gameObject.isStatic && !resetting && reachedTarget)
        {
           
            t = 0;
            resetting = true;
            startPos = hand.position;
            StopAllCoroutines();
            StartCoroutine(CoolDownCoroutine());   
        }
        if(collision.gameObject.GetComponent<Target>() != null)
        {
            GameManager.Death();
        }
        if (collision.gameObject.GetComponentInChildren<AudioEvent>() != null && collision.gameObject.tag =="Collision")
        {
            

            List<ContactPoint> contactPoints = new List<ContactPoint>();
            collision.GetContacts(contactPoints);
            bool play = false;
            
            foreach (ContactPoint c in contactPoints)
            {
                
                if(c.thisCollider.gameObject.tag != null || c.thisCollider.gameObject != null)
                {
                    if (c.thisCollider.gameObject.tag == "AudioCollider")
                    {
                       //print("audio collision detected");
                        play = true;
                        break;
                    }
                }
            }
                
            
            if (play && gameManager.AddToScore(collision)) //and add to score
            {
                AudioEvent audioEvent = collision.gameObject.GetComponentInChildren<AudioEvent>();
                FindObjectOfType<SoundManager>().PlayCollision(audioEvent);
                Renderer ren =collision.gameObject.GetComponentInChildren<Renderer>();
                ren.material.color = new Color(ren.material.color.r, ren.material.color.g*1.3f, ren.material.color.b);
               
            }
           
            
        }


    }
    //public void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Collision")
    //    {
    //        print("audio collision detected");
    //        AudioEvent audioEvent = other.gameObject.GetComponentInChildren<AudioEvent>();
    //        SoundManager.PlayCollision(audioEvent);
    //    }
    //}

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

        Vector3 b = a - 0.5f * Vector3.left;
        Vector3 c = d - 0.5f * Vector3.left;

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
        animator.SetInteger("HandState", 1);
        Vector3 readyDirNorm;
        if (!swat)
        {
            if (FlyController2.grounded == true) // Vertical swat
            {
                currentReadyPos = readyPosVert;
            }
            else //Horizontal swat
                currentReadyPos = readyPosHori;
        }
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
        StartCoroutine(CoolDownCoroutine());

    }
    IEnumerator CoolDownCoroutine()
    {
        //startPos = hand.position;
        float scale = Time.timeScale;
        swat = false;
        reachedTarget = false;
        coolDown = true;

        yield return new WaitForSeconds((cooldownTime / attackTimeModifier) * scale);
        coolDown = false;
        swatSpeed = 1;
    }

}
