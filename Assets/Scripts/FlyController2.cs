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
    public Transform flyBody;
    public static Vector3 flyVelocity;
    public static Vector3 flyAcceleration;

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


        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        
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

        //Vector3 xAxis = transform.right;
        //Vector3 zAxis = transform.forward;

        Vector3 zAxis = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up);
        Vector3 xAxis = cam.transform.right;

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

            float newXVelocity = Mathf.MoveTowards(currentXVelocity, desiredVelocity.x, accelerationScaled);
            float newZVelocity = Mathf.MoveTowards(currentZVelocity, desiredVelocity.z, accelerationScaled);

            velocity += xAxis * (newXVelocity - currentXVelocity) + zAxis * (newZVelocity - currentZVelocity);

            float cameraAngleCompensation = Vector3.Cross(cam.transform.forward, transform.forward).magnitude;

            if(jumpInput != 0 && velocity.y != 0)
            {
                velocity.y = Mathf.MoveTowards(velocity.y, Mathf.Clamp(jumpInput, -1, 1) * maxYSpeed, Time.unscaledDeltaTime * Yacceleration * 10f);

                
            }
            else if(velocity.y > 0)
            {
                //velocity.y -= Time.unscaledDeltaTime*4;
                velocity.y += 0.1f*(Mathf.PerlinNoise(68, 562 + Time.time * 5000 * Time.unscaledDeltaTime) - 0.5f);
                velocity.y *= 0.97f;
            }
            else
            {
                velocity.y -= Time.unscaledDeltaTime * 0.2f;
                velocity.y += 0.1f*(Mathf.PerlinNoise(68, 562 + Time.time * 5000 * Time.unscaledDeltaTime) - 0.5f);
                velocity.y *= 0.97f;
            }


            LocalRotation(currentXVelocity, currentZVelocity);

        }
    }

    private void LocalRotation(float currentXVelocity, float currentZVelocity)
    {
        CalculateAcceleration(currentXVelocity, currentZVelocity);

        float newXrotation = Mathf.MoveTowardsAngle(flyBody.transform.localRotation.eulerAngles.x, -desiredVelocity.x * 4.5f, Time.unscaledDeltaTime * 80);
        float newZrotation = Mathf.MoveTowardsAngle(flyBody.transform.localRotation.eulerAngles.z, -desiredVelocity.z * 4.5f, Time.unscaledDeltaTime * 80);
        print(newXrotation);
        flyBody.transform.localRotation = Quaternion.Euler(newXrotation, cam.transform.localRotation.y - 90, newZrotation);
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

        if (desiredVelocityY == 0)
        {
            yDiff = 0;
        }

        Vector3 velocityDiff = new Vector3(xDiff, yDiff, zDiff);
        flyAcceleration = velocityDiff;
    }
}
