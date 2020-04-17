using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlyController : MonoBehaviour
{
    public float maxSpeed;
    public float minSpeed;

    GameObject lookTarget;
    
    Vector3 startRight;
    bool changeRight = true;

    Camera cam;
    Vector3 newPos;
    float speed = 0.06f;
    float horizontalSpeed = 0f;
    public float maxHorizontalSpeed;
    public float horizontalAcceleration;
    public float turnSpeed = 180f;
    public static Vector3 currentSpeed;

    Rigidbody rb;
    float targetXRot;
    float targetYRot;
    float targetZRot;
    float lastXRot;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        newPos = Vector3.zero;
        targetXRot = transform.localEulerAngles.x;
        targetYRot = transform.localEulerAngles.y;
        targetZRot = transform.localEulerAngles.z;
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0)
        {

            speed = speed + Input.GetAxis("Vertical") * Time.deltaTime;

            if (speed > maxSpeed)
            {
                speed = maxSpeed;
            }
            else if (speed < minSpeed)
            {
                speed = minSpeed;
            }

        }
        horizontalSpeed = horizontalSpeed * 0.95f;
        //strafe controls. skal flyttes ind i controller

        if (Input.GetAxis("Horizontal") < 0)
        {

            horizontalSpeed -= horizontalAcceleration;


            if (horizontalSpeed < maxHorizontalSpeed * -1)
                horizontalSpeed = -maxHorizontalSpeed;

        }

        /*
        if (Input.GetKeyUp(KeyCode.A))
        {
            horizontalSpeed += 10 * horizontalSpeed;
        }
        */

        if (Input.GetAxis("Horizontal") > 0)
        {

            horizontalSpeed += horizontalAcceleration;

            if (horizontalSpeed > maxHorizontalSpeed)
                horizontalSpeed = maxHorizontalSpeed;
        }

        transform.Translate(currentSpeed, Space.World);
        /*
       if (Input.GetKeyUp(KeyCode.D))
       {
           horizontalSpeed -= 10 * horizontalSpeed;
       }
       */

        //flytte flyet frem:

        //transform.position = transform.position + transform.forward * speed * Time.deltaTime + transform.right *horizontalSpeed *Time.deltaTime;
        //currentSpeed = transform.forward * speed * Time.deltaTime + cam.transform.right * horizontalSpeed * Time.deltaTime;
        currentSpeed = transform.forward * speed * Time.deltaTime;

        float xRot = cam.ScreenToViewportPoint(Input.mousePosition).y - 0.5f;
        xRot *= 2;


        if (Mathf.Abs(xRot) > 0.4f)
        {
            xRot = 0.4f * Mathf.Sign(xRot);
        }

        float yRot = 2 * (cam.ScreenToViewportPoint(Input.mousePosition).x - 0.5f);
        float zRot = yRot;




        if (Mathf.Abs(yRot) > 0.4f)
        {
            zRot = yRot;
            yRot = 0.4f * Mathf.Sign(yRot);
        }


        targetXRot = Mathf.Clamp(targetXRot += -turnSpeed * xRot * Time.deltaTime * (maxSpeed + 20) / (speed + 20), -80, 80);

        targetYRot += turnSpeed * yRot * Time.deltaTime * (maxSpeed + 20) / (speed + 20);

        transform.localEulerAngles = new Vector3(targetXRot, targetYRot, 0);

        //targetZRot = zRot * 100 * (maxSpeed + 20) / (speed + 20);
        //zRot = Mathf.Lerp(zRot, targetZRot, 0.02f);

        //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, -zRot);


        float finalTarget = 1f * (turnSpeed * (maxSpeed + 20) / (speed + 20));


        targetZRot = Mathf.Lerp(targetZRot, zRot * finalTarget + horizontalSpeed * 15, 0.02f);
        targetZRot = Mathf.Clamp(targetZRot, -110, 110);

        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, -targetZRot);
    }


    void LateUpdate()
    {
            // flytte kameraet et offset fra flyets bagside;
            Vector3 newCameraPos = transform.position - transform.forward * 0.1f + transform.up * 0.03f;

    }
}
