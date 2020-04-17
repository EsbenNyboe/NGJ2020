using System;
using UnityEngine;

public class FlyController2 : MonoBehaviour
{

    public enum AccelerationType
    {
        noAcc, constAcc, LinAccBoost, QuadAccBoost
    }

    [SerializeField, Range(0f, 1f)]
    float maxSpeed = 0.10f;

    [SerializeField, Range(0f, 5f)]
    float acceleration = 0.10f;

    [SerializeField]
    AccelerationType accType = default;

    Vector3 velocity, desiredVelocity;

    public static Vector3 flyVelocity;

    public static float speed = 0;

    Vector3 forwardDir;

    float startAcceleration;
    float jumpInput;

    int stepsSinceLastGrounded, stepsSinceLastJump;

    Rigidbody body;
    Camera cam;

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
        else
        {
            jumpInput = 0;
        }
        

        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

        if (Input.GetMouseButton(1))
        {
            forwardDir = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up);
            transform.forward = forwardDir;
        }


    }

    private void FixedUpdate()
    {
        MoveSphere();

        UpdateState();
    }

    private void UpdateState()
    {
        stepsSinceLastGrounded += 1;
        stepsSinceLastJump += 1;

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
                accelerationScaled = acceleration * Time.deltaTime;
                break;
            case AccelerationType.LinAccBoost:
                accelerationScaled = acceleration * (0.1f + 5 * Vector3.Distance(desiredVelocity, velocity) / (maxSpeed * 2)) * Time.deltaTime;
                break;
            case AccelerationType.QuadAccBoost:
                accelerationScaled = 5 * acceleration * (0.1f + 10 * Mathf.Pow(Vector3.Distance(desiredVelocity, velocity) / (maxSpeed * 2), 4)) * Time.deltaTime;
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
            float newXVelocity = Mathf.MoveTowards(currentXVelocity, desiredVelocity.x, accelerationScaled);
            float newZVelocity = Mathf.MoveTowards(currentZVelocity, desiredVelocity.z, accelerationScaled);

            velocity += xAxis * (newXVelocity - currentXVelocity) + zAxis * (newZVelocity - currentZVelocity);

            if (jumpInput != 0)
            {
                velocity.y = jumpInput*Time.deltaTime * 60;
            }

            if(velocity.y > 0)
            {
                velocity.y -= 7f * Time.deltaTime;
            }
            else
            {
                velocity.y -= 0.4f * Time.deltaTime;
            }
            
            
            

        }
    }
}
