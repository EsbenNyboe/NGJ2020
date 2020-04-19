using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKSolverArm : MonoBehaviour
{
    public int legNum;
    
    public GameObject target;
    public GameObject endPoint;
    public GameObject joint1;
    public GameObject joint2;
    public GameObject root;


    [HideInInspector]
    public bool grounded;

    [HideInInspector]
    public bool startWait;

    [HideInInspector]
    public Vector3 homePosOffset;

    public Transform hand;
    Vector3 startOffset;

    public int joint1Vert_min;
    public int joint1Vert_max;

    public int joint1Hori_min;
    public int joint1Hori_max;

    float[] xRots;
    float yRot;

    private void Awake()
    {
        startWait = false;
        grounded = true;
    }
    private void Start()
    {
        xRots = new float[legNum];
        startOffset = endPoint.transform.position - hand.position;
        
    }

    private void Update()
    {
        //endPoint.transform.position = hand.position;
    }

    private void FixedUpdate()
    {
        MoveHorizontal();
        for (int i = 0; i < 2; i++)
        {
            MoveIK(joint2, 1);
            MoveIK(joint1, 0);
        }

    }


    private void MoveIK(GameObject thisJoint, int integer)
    {
        Vector3 jointToTarget = target.transform.position - thisJoint.transform.position;
        Vector3 jointToTargetProjected = Vector3.ProjectOnPlane(jointToTarget, root.transform.forward);

        Vector3 jointToEndPoint = endPoint.transform.position - thisJoint.transform.position;
        float alpha = Vector3.Angle(jointToEndPoint, jointToTargetProjected);
        Vector3 normal = Vector3.Cross(jointToTargetProjected, jointToEndPoint).normalized;

        alpha *= -Vector3.Dot(root.transform.forward, normal);

        
        xRots[integer] += alpha;

        if (xRots[integer] > 180)
        {
            xRots[integer] -= 360;
        }
        else if (xRots[integer] < -180)
        {
            xRots[integer] += 360;
        }

        thisJoint.transform.localRotation = Quaternion.Euler(0, 0, xRots[integer]);

        
    }

    private void MoveHorizontal()
    {
        Vector3 jointToTarget = target.transform.position - joint1.transform.position;
        Vector3 jointToTargetProjected = Vector3.ProjectOnPlane(jointToTarget, root.transform.up);

        Vector3 jointToEndPoint = endPoint.transform.position - joint1.transform.position;
        Vector3 jointToEndPointProjected = Vector3.ProjectOnPlane(jointToEndPoint, root.transform.up);

        float theta = Vector3.Angle(jointToEndPointProjected, jointToTargetProjected);
        Vector3 normal = Vector3.Cross(jointToTargetProjected, jointToEndPointProjected).normalized;

        theta *= -Vector3.Dot(root.transform.up, normal);

        yRot += theta;
        float oppositeSide = (joint1Hori_min + joint1Hori_max) / 2 - 180;

        if (yRot > 180)
        {
            yRot -= 360;
        }
        else if (yRot < -180)
        {
            yRot += 360;
        }

        if (oppositeSide > 180)
        {
            oppositeSide -= 360;
        }
        else if (oppositeSide < -180)
        {
            oppositeSide += 360;
        }

        /*
        if (yRot < joint1Hori_min && yRot > 0 || yRot > oppositeSide && yRot < 0)
        {
            root.transform.localRotation = Quaternion.Euler(0, joint1Hori_min, 0);
            yRot = joint1Hori_min;
            print("1");
        }
        else if (yRot > joint1Hori_max && yRot > 0 || yRot < oppositeSide && yRot < 0)
        {
            root.transform.localRotation = Quaternion.Euler(0, joint1Hori_max, 0);
            print("2");
            yRot = joint1Hori_max;
        }
        else
        {
            print("3");
            root.transform.localRotation = Quaternion.Euler(0, yRot, 0);

        }
        */

        root.transform.localRotation = Quaternion.Euler(0, yRot, 0);

        /*
        if (yRot < joint1Hori_min)
        {
            root.transform.localRotation = Quaternion.Euler(0, joint1Hori_min, 0);
            yRot = joint1Hori_min;
        }
        else if (yRot > joint1Hori_max)
        {
            root.transform.localRotation = Quaternion.Euler(0, joint1Hori_max, 0);
            yRot = joint1Hori_max;
        }
        else
        {
            root.transform.localRotation = Quaternion.Euler(0, yRot, 0);
        }
        */
    }
}
