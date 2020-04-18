using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FlyCamera : MonoBehaviour
{

    public enum InnerSmoothingType
    {
        DontFollow, Exponential, Spring, DampedSpring, Instant
    }

    public enum OuterSmoothingType
    {
        Exponential, Spring, DampedSpring, Instant
    }

    [SerializeField]
    public Transform hand = default;

    [SerializeField]
    public Transform player = default;

    [SerializeField, Range(0.01f, 1f)]
    public float distanceToPlayer = 0.1f;

    [SerializeField]
    public bool useTwoSmoothZones;

    [SerializeField]
    public InnerSmoothingType smoothTypeInnerCircle = default;

    [SerializeField]
    public OuterSmoothingType smoothTypeOuterCircle = default;

    [SerializeField, Min(0f)]
    public float circleRadius = 0.01f;

    [SerializeField, Range(0, 10f)]
    public float innerFollowSpeed = 0.05f;

    [SerializeField, Range(0, 10f)]
    public float outerFollowSpeed = 0.05f;

    Vector3 currentLookPoint, nextLookPoint, velocity;

    float circleStartRadius;

    public Transform target;
    public Vector3 targetOffset;
    public float distance = 5.0f;
    public float maxDistance = 20;
    public float minDistance = .6f;
    public float xSpeed = 200.0f;
    public float ySpeed = 200.0f;
    public int yMinLimit = -80;
    public int yMaxLimit = 80;
    public int zoomRate = 40;
    public float panSpeed = 0.3f;
    public float zoomDampening = 5.0f;

    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;

    void Awake()
    {
        currentLookPoint = player.position;
        circleStartRadius = circleRadius;

        if (!useTwoSmoothZones)
        {
            switch (smoothTypeInnerCircle)
            {
                case InnerSmoothingType.DontFollow:
                    smoothTypeOuterCircle = OuterSmoothingType.Instant;
                    break;
                case InnerSmoothingType.Exponential:
                    smoothTypeOuterCircle = OuterSmoothingType.Exponential;
                    break;
                case InnerSmoothingType.Spring:
                    smoothTypeOuterCircle = OuterSmoothingType.Spring;
                    break;
                case InnerSmoothingType.DampedSpring:
                    smoothTypeOuterCircle = OuterSmoothingType.DampedSpring;
                    break;
                case InnerSmoothingType.Instant:
                    smoothTypeOuterCircle = OuterSmoothingType.Instant;
                    break;
            }

            outerFollowSpeed = innerFollowSpeed;
            if (smoothTypeInnerCircle == InnerSmoothingType.DontFollow)
            {
                Debug.Log("CANT USE DONT FOLLOW WITH ONLY ONE SMOOTH ZONE");
            }
        }
    }

    private void Start()
    {
        if (!target)
        {
            GameObject go = new GameObject("Cam Target");
            go.transform.position = transform.position + (transform.forward * distance);
            target = go.transform;
        }

        distance = Vector3.Distance(transform.position, target.position);
        currentDistance = distance;
        desiredDistance = distance;

        //be sure to grab the current rotations as starting points.
        position = transform.position;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;

        xDeg = Vector3.Angle(Vector3.right, transform.right);
        yDeg = Vector3.Angle(Vector3.up, transform.up);
    }

    void LateUpdate()
    {
        Vector3 playerPos = player.position;
        float dt = Time.unscaledDeltaTime;

        Vector3[] twoPoints = UpdateLookPoint(playerPos, smoothTypeInnerCircle, smoothTypeOuterCircle);

        float dist = Vector3.Distance(playerPos, currentLookPoint);


        if (smoothTypeInnerCircle == InnerSmoothingType.DontFollow && smoothTypeOuterCircle == OuterSmoothingType.Instant)
        {
            if (dist <= circleRadius)
            {
                circleRadius = circleStartRadius;
                nextLookPoint = twoPoints[0];
            }
            else
            {
                circleRadius = circleStartRadius;
                nextLookPoint = twoPoints[1];
            }

        }
        else if (smoothTypeInnerCircle == InnerSmoothingType.DontFollow)
        {
            if (dist <= circleRadius)
            {
                circleRadius = circleStartRadius * 1.3f;
                nextLookPoint = twoPoints[0];
            }
            else
            {
                circleRadius = circleStartRadius * 0.8f;
                nextLookPoint = twoPoints[1];
            }

        }
        else
        {
            float frac = dist / circleRadius;

            frac = Mathf.Clamp(frac, circleRadius * 0.7f, circleRadius * 1.4f);
            frac = (frac - 0.7f * circleRadius) / (circleRadius * 0.7f);

            nextLookPoint = Vector3.Lerp(twoPoints[0], twoPoints[1], frac);
        }


        MoveCamera();

        currentLookPoint = nextLookPoint;

    }

    private void MoveCamera()
    {

        //if (Input.GetMouseButton(1) || Input.GetMouseButton(0))
        //{
            xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            ////////OrbitAngle

            //Clamp the vertical axis for the orbit
            yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
            // set camera rotation 
            desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
            currentRotation = transform.rotation;

            rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.unscaledDeltaTime * zoomDampening);
            transform.rotation = rotation;
        //}


        desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.unscaledDeltaTime * zoomRate * Mathf.Abs(desiredDistance);
        //clamp the zoom min/max
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        // For smoothing of the zoom, lerp distance
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.unscaledDeltaTime * zoomDampening);

        // calculate position based on the new currentDistance 
        //position = currentLookPoint - (rotation * Vector3.forward * currentDistance + targetOffset);
        //transform.position = position;

        Vector3 lookDirection = transform.forward;

        transform.localPosition = nextLookPoint - lookDirection * distanceToPlayer;
        //Vector3 lookPosition = nextLookPoint - lookDirection * distanceToPlayer;
        //transform.SetPositionAndRotation(lookPosition, lookRotation);
    }

    private Vector3[] UpdateLookPoint(Vector3 playerPos, InnerSmoothingType it, OuterSmoothingType ot)
    {
        float lookPointToPlayerDist = Vector3.Distance(playerPos, currentLookPoint);
        float dt = Time.unscaledDeltaTime;

        Vector3[] outPut = new Vector3[2];

        switch (it)
        {
            case InnerSmoothingType.DontFollow:
                velocity = Vector3.zero;
                break;

            case InnerSmoothingType.Exponential:
                velocity = playerPos - currentLookPoint;
                break;

            case InnerSmoothingType.Spring:
                var n1 = velocity - (currentLookPoint - playerPos) * (innerFollowSpeed * dt * innerFollowSpeed);
                var n2 = 1 + dt;
                velocity = n1 / (n2 * n2);
                break;

            case InnerSmoothingType.DampedSpring:
                var n3 = velocity - (currentLookPoint - playerPos) * (innerFollowSpeed * innerFollowSpeed * dt * innerFollowSpeed);
                var n4 = 1 + innerFollowSpeed * dt * innerFollowSpeed;
                velocity = n3 / (n4 * n4);
                break;
        }

        if (smoothTypeInnerCircle != InnerSmoothingType.Instant)
        {
            if (velocity.magnitude * dt < Vector3.Distance(playerPos, currentLookPoint))
            {
                outPut[0] = currentLookPoint + velocity * dt * innerFollowSpeed;
            }
            else
            {
                outPut[0] = playerPos;
            }
        }
        else
        {
            outPut[0] = playerPos;
        }

        switch (smoothTypeOuterCircle)
        {

            case OuterSmoothingType.Exponential:
                velocity = playerPos - currentLookPoint;
                break;

            case OuterSmoothingType.Spring:
                var n1 = velocity - (currentLookPoint - playerPos) * (innerFollowSpeed * dt * outerFollowSpeed);
                var n2 = 1 + dt;
                velocity = n1 / (n2 * n2);
                break;

            case OuterSmoothingType.DampedSpring:
                var n3 = velocity - (currentLookPoint - playerPos) * (innerFollowSpeed * innerFollowSpeed * dt * outerFollowSpeed);
                var n4 = 1 + innerFollowSpeed * dt * outerFollowSpeed;
                velocity = n3 / (n4 * n4);
                break;
        }
        if (smoothTypeOuterCircle != OuterSmoothingType.Instant)
        {
            outPut[1] = currentLookPoint + velocity * dt * outerFollowSpeed;
        }
        else
        {
            outPut[1] = Vector3.Lerp(playerPos, currentLookPoint, circleRadius / lookPointToPlayerDist);
        }

        return outPut;
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

}