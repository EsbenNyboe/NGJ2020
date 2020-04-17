using System;
using UnityEngine;

public class FlyController2 : MonoBehaviour
{

    public enum AccelerationType
    {
        noAcc, constAcc, LinAccBoost, QuadAccBoost
    }


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

    Vector3 velocity, desiredVelocity;

    public static Vector3 flyVelocity;
    public static float flyAcceleration;

    public static float speed = 0;

    Vector3 forwardDir;

    float startAcceleration;
    float jumpInput;
    float desiredVelocityY;


    Rigidbody body;
    Camera cam;

    bool turnFly = false;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        startAcceleration = acceleration;
        cam = Camera.main;
        flyVelocity = Vector3.zero;
    }

    void Update()
    {
        Vector2 playerInput;
        playerInput.x = Input.GetAxisRaw("Horizontal");
        playerInput.y = Input.GetAxisRaw("Vertical");

        speed = body.velocity.magnitude;
        flyVelocity = body.velocity;

        if(Input.GetAxis("Jump") != 0)
        {
            jumpInput = Input.GetAxis("Jump")*maxYSpeed;
        }
        else
        {
            jumpInput = 0;
        }


        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        flyAcceleration = playerInput.magnitude + jumpInput;

        desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

        if (Input.GetMouseButton(1))
        {
            turnFly = true;
            
        }
        else { turnFly = false; }


    }

    private void FixedUpdate()
    {
        MoveSphere();
        if (turnFly)
        {
            forwardDir = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up);
            transform.forward = forwardDir;

        }
        
        UpdateState();
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

        Vector3 xAxis = transform.right;
        Vector3 zAxis = transform.forward;

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
            
            print(flyAcceleration);

            float newXVelocity = Mathf.MoveTowards(currentXVelocity, desiredVelocity.x, accelerationScaled);
            float newZVelocity = Mathf.MoveTowards(currentZVelocity, desiredVelocity.z, accelerationScaled);

            velocity += xAxis * (newXVelocity - currentXVelocity) + zAxis * (newZVelocity - currentZVelocity);



            velocity.y = Mathf.MoveTowards(velocity.y, jumpInput, Time.unscaledDeltaTime*Yacceleration*10f);

            /*
            if (jumpInput != 0)
            {
                velocity.y = jumpInput*Time.unscaledDeltaTime * 600;
            }

            if(velocity.y > 0)
            {
                velocity.y -= 70f * Time.unscaledDeltaTime;
            }
            else
            {
                velocity.y -= 4f * Time.unscaledDeltaTime;
            }
            */
            
            

        }
    }

}
