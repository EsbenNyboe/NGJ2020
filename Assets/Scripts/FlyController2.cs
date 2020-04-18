using System;
using UnityEngine;

public class FlyController2 : MonoBehaviour
{

    public enum AccelerationType
    {
        noAcc, constAcc, LinAccBoost, QuadAccBoost
    }

    //grounded på loft - hvad gør space?

    //grounded orientering.

    [SerializeField, Range(0f, 10f)]
    float maxSpeed = 0.10f;

    [SerializeField, Range(0f, 50f)]
    float acceleration = 0.10f;

    [SerializeField, Range(0f, 10f)]
    float maxYSpeed = 1f;

    [SerializeField, Range(0f, 50f)]
    float Yacceleration = 0.10f;

    [SerializeField]
    AccelerationType accType = default;

    [SerializeField]
    public float staminaChargeTime = 4f;

    [SerializeField]
    public float staminaFlyTime = 20f;

    public static float currentStamina = 1f;
    
    public Transform flyBody;
    
    public static Vector3 flyAcceleration, flyVelocity;

    public static float speed = 0;

    Vector3 velocity, desiredVelocity, forwardDir, landedForward;
    //Quaternion landedRotation;

    float startAcceleration, jumpInput, desiredVelocityY;

    RaycastHit hit;
    Rigidbody body;
    Camera cam;

    bool turnFly = false;
    public static bool grounded = false;


    void Awake()
    {
        body = GetComponent<Rigidbody>();
        startAcceleration = acceleration;
        cam = Camera.main;
        flyVelocity = Vector3.zero;
    }

    void Update()
    {
        Debug.DrawRay(transform.position, -Vector3.up*0.01f);
        HandleInput();

        speed = body.velocity.magnitude;
        flyVelocity = body.velocity;

        if(currentStamina == 0)
        {
            GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.red);
        }
        else
        {
            GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.green);
        }
    }

    private void HandleInput()
    {
        Vector2 playerInput;
        playerInput.x = Input.GetAxisRaw("Horizontal");
        playerInput.y = Input.GetAxisRaw("Vertical");

        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        if (grounded )
        {
            desiredVelocity = Vector3.zero;
        }
        else if(currentStamina == 0)
        {
            desiredVelocity = 0.1f* new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
        }
        else
        {
            desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
        }

        if (Input.GetAxis("Jump") != 0)
        {
            jumpInput = Input.GetAxis("Jump");
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            jumpInput = -1;
        }
        else
        {
            jumpInput = 0;
        }

        if (Input.GetMouseButton(1) && grounded == false)
        {
            turnFly = true;

        }
        else { turnFly = false; }
    }

    private void FixedUpdate()
    {
        if (grounded)
        {
            currentStamina = Mathf.Clamp(currentStamina + Time.unscaledDeltaTime / staminaChargeTime,0,1);
        }else
        {
            currentStamina = Mathf.Clamp(currentStamina - Time.unscaledDeltaTime / staminaFlyTime, 0, 1);
        }

        
        Grounding();

        MoveSphere();

        if (turnFly)
        {
            forwardDir = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up);
            transform.forward = forwardDir;
        }
        
        UpdateState();
    }

    private void Grounding()
    {

        grounded = false;
        

        for (int i = 0; i < 24; i++)
        {
            float xangle = Mathf.Cos(i * 30*Mathf.PI*2/360);
            float yangle = Mathf.Sin(i * 30 * Mathf.PI * 2 / 360);
            
            Vector3 dir = new Vector3(xangle, -yangle, 0);
            if(i > 11)
            {
                dir = new Vector3(0, -yangle, xangle);
            }

            if(Physics.Raycast(transform.position, dir, out hit, 0.03f) && grounded == false)
            {
                //print(hit.normal*100);

                landedForward = -transform.right;

                grounded = true;
            }
        }
    }

    private void UpdateState()
    {
        body.velocity = velocity;
    }

    private void MoveSphere()
    {
        velocity = body.velocity;

        float accelerationScaled = 0;

        acceleration = startAcceleration;

        switch (accType)
        {
            case AccelerationType.constAcc:
                accelerationScaled = acceleration * Time.unscaledDeltaTime;
                break;
            case AccelerationType.LinAccBoost:
                accelerationScaled = acceleration * (0.1f + 5 * Vector3.Distance(desiredVelocity, velocity) / (maxSpeed * 2)) * Time.unscaledDeltaTime;
                break;
            case AccelerationType.QuadAccBoost:
                accelerationScaled = 5 * acceleration * (0.1f + 10 * Mathf.Pow(Vector3.Distance(desiredVelocity, velocity) / (maxSpeed * 2), 4)) * Time.unscaledDeltaTime;
                break;
            case AccelerationType.noAcc:
                accelerationScaled = 0;
                break;
        }


        //Vector3 zAxis = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up);
        //Vector3 xAxis = cam.transform.right;
        Vector3 zAxis = transform.forward;
        Vector3 xAxis = transform.right;

        float currentXVelocity = Vector3.Dot(velocity, xAxis);
        float currentZVelocity = Vector3.Dot(velocity, zAxis);

        if (accType == AccelerationType.noAcc)
        {
            float oldY = velocity.y;
            velocity = xAxis * desiredVelocity.x + zAxis * desiredVelocity.z;
            velocity += new Vector3(0, oldY, 0);
        }
        else
        {
            CalculateXZVelocity(accelerationScaled, zAxis, xAxis, currentXVelocity, currentZVelocity);
            CalculateYVelocity();

            RotateChild(currentXVelocity, currentZVelocity);

        }
    }

    private void CalculateXZVelocity(float accelerationScaled, Vector3 zAxis, Vector3 xAxis, float currentXVelocity, float currentZVelocity)
    {
        if (currentStamina == 0)
        {
            desiredVelocity.x += 3 * (Mathf.PerlinNoise(68, 11562 + Time.time * 2000 * Time.unscaledDeltaTime) - 0.5f);
            desiredVelocity.z += 3 * (Mathf.PerlinNoise(68, -2462 + Time.time * 3000 * Time.unscaledDeltaTime) - 0.5f);
        }

        float newXVelocity = Mathf.MoveTowards(currentXVelocity, desiredVelocity.x, accelerationScaled);
        float newZVelocity = Mathf.MoveTowards(currentZVelocity, desiredVelocity.z, accelerationScaled);

        velocity += xAxis * (newXVelocity - currentXVelocity) + zAxis * (newZVelocity - currentZVelocity);
    }

    private void CalculateYVelocity()
    {
        //float cameraAngleCompensation = Vector3.Cross(cam.transform.forward, transform.forward).magnitude;

        if (jumpInput != 0 && velocity.y != 0 && currentStamina != 0)
        {
            velocity.y = Mathf.MoveTowards(velocity.y, Mathf.Clamp(jumpInput, -1, 1) * maxYSpeed, Time.unscaledDeltaTime * Yacceleration * 10f);
        }
        else if (velocity.y > 0)
        {
            //ingen stamina: daler ned. TIL ALLE SIDER!
            //
            velocity.y += 0.1f * (Mathf.PerlinNoise(68, 562 + Time.time * 5000 * Time.unscaledDeltaTime) - 0.5f);

            if(currentStamina == 0)
            {
            velocity.y -= Time.unscaledDeltaTime * 4 + 0.1f*(Mathf.PerlinNoise(68, 1562 + Time.time * 5000 * Time.unscaledDeltaTime) - 0.5f);
            }
            else
            {
                velocity.y *= 0.97f;
            }

        }
        else
        {
            velocity.y -= Time.unscaledDeltaTime * 0.2f;
            velocity.y += 0.1f * (Mathf.PerlinNoise(68, 562 + Time.time * 5000 * Time.unscaledDeltaTime) - 0.5f);

            if (currentStamina == 0)
            {
                velocity.y -= Time.unscaledDeltaTime * 2;
            }
            else
            {
                velocity.y *= 0.97f;
            }
        }
    }

    private void RotateChild(float currentXVelocity, float currentZVelocity)
    {
        CalculateAcceleration(currentXVelocity, currentZVelocity);

        float desiredAngleX;
        float desiredAngleZ;
        float desiredAngleY;

        if (grounded)
        {
            Quaternion landedOrientation = Quaternion.LookRotation(landedForward, hit.normal); //mærkelig "forward" fordi fluen vender mærkeligt (langs +x)
            //print(hit.normal);

            desiredAngleX = landedOrientation.eulerAngles.x;
            desiredAngleY = body.transform.localRotation.y - 90;
            desiredAngleZ = landedOrientation.eulerAngles.z;

            Debug.DrawRay(transform.position, hit.normal);
            Debug.DrawRay(transform.position, landedForward, Color.yellow);
            
        }
        else
        {
            desiredAngleX = -desiredVelocity.x * 4.5f;
            desiredAngleZ = -desiredVelocity.z * 4.5f;
            desiredAngleY = cam.transform.localRotation.y - 90;
        }

        float newXrotation = Mathf.MoveTowardsAngle(flyBody.transform.localRotation.eulerAngles.x, desiredAngleX, Time.unscaledDeltaTime * 80);
        float newZrotation = Mathf.MoveTowardsAngle(flyBody.transform.localRotation.eulerAngles.z, desiredAngleZ, Time.unscaledDeltaTime * 80);
       
        flyBody.transform.localRotation = Quaternion.Euler(newXrotation, desiredAngleY, newZrotation);
    }

    private void CalculateAcceleration(float currentXVelocity, float currentZVelocity)
    {
        float xDiff;
        float zDiff;
        float yDiff;

        if (desiredVelocity.x > currentXVelocity)
        {
            xDiff = desiredVelocity.x - currentXVelocity;
        }
        else
        {
            xDiff = currentXVelocity - desiredVelocity.x;
        }

        if (desiredVelocity.z > currentZVelocity)
        {
            zDiff = desiredVelocity.z - currentZVelocity;
        }
        else
        {
            zDiff = -desiredVelocity.z + currentZVelocity;
        }

        if (desiredVelocity.z == 0)
        {
            zDiff = 0;
        }

        if (desiredVelocity.x == 0)
        {
            xDiff = 0;
        }

        yDiff = jumpInput*maxYSpeed - velocity.y;

        if (desiredVelocity.y == 0)
        {
            yDiff = 0;
        }

        Vector3 velocityDiff = new Vector3(xDiff, yDiff, zDiff);
        flyAcceleration = velocityDiff;
    }
}
