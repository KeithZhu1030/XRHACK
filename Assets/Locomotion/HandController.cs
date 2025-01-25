using System;
using UnityEngine;

// Physics hand controller using PID
// https://en.wikipedia.org/wiki/Proportional%E2%80%93integral%E2%80%93derivative_controller
public class HandController : MonoBehaviour
{
    Rigidbody rigidBody;
    Vector3 previousPosition;
    bool isColliding;

    [SerializeField] Rigidbody playerRigidBody;
    [SerializeField] Transform target;
    [SerializeField] Hand hand;
    [SerializeField] float frequency = 50f;
    [SerializeField] float damping = 1f;
    [SerializeField] float rotationFrequency = 100f;
    [SerializeField] float rotationDamping = 0.9f;

    [SerializeField] float climbForce = 1000f;
    [SerializeField] float climbDrag = 1000f;

    void Start()
    {
        transform.position = target.position;
        transform.rotation = target.rotation;

        rigidBody = GetComponent<Rigidbody>();
        rigidBody.maxAngularVelocity = float.PositiveInfinity;
        previousPosition = transform.position;
    }

    void FixedUpdate()
    {
        PIDMovement();
        PIDRotation();
        if (isColliding) 
            HookesLaw();
    }

    void PIDMovement()
    {
        float kp = (6f * frequency) * (6f * frequency) * 0.25f;
        float kd = 4.5f * frequency * damping;
        float g = 1 / (1 + kd * Time.fixedDeltaTime + kp * Time.fixedDeltaTime * Time.fixedDeltaTime);
        float ksg = kp * g;
        float kdg = (kd + kp * Time.fixedDeltaTime) * g;
        Vector3 force = (target.position -  transform.position) * ksg + (playerRigidBody.linearVelocity - rigidBody.linearVelocity) * kdg;
        // Debug.Log(force);
        rigidBody.AddForce(force, ForceMode.Acceleration);
    }

    void PIDRotation()
    {
        float kp = (6f * rotationFrequency) * (6f * rotationFrequency) * 0.25f;
        float kd = 4.5f * rotationFrequency * rotationDamping;
        float g = 1 / (1 + kd * Time.fixedDeltaTime + kp * Time.fixedDeltaTime * Time.fixedDeltaTime);
        float ksg = kp * g;
        float kdg = (kd + kp * Time.fixedDeltaTime) * g;
        Quaternion q = target.rotation * Quaternion.Inverse(transform.rotation);
        if (q.w < 0)
        {
            q.x = -q.x;
            q.y = -q.y;
            q.z = -q.z;
            q.w = -q.w;
        }
        q.ToAngleAxis(out float angle, out Vector3 axis);
        axis.Normalize();
        axis *= Mathf.Deg2Rad;
        Vector3 torque = ksg * axis * angle + -rigidBody.angularVelocity * kdg;
        rigidBody.AddTorque(torque, ForceMode.Acceleration);
    }

    void HookesLaw()
    {
        Vector3 displacementFromResting = transform.position - target.position;
        Vector3 force = displacementFromResting * climbForce;
        float drag = GetDrag();
        
        Debug.Log(force);

        playerRigidBody.AddForce(force, ForceMode.Acceleration);
        playerRigidBody.AddForce(drag * -playerRigidBody.linearVelocity * climbDrag, ForceMode.Acceleration);
    }

    float GetDrag()
    {
        Vector3 handVelocity = (target.localPosition - previousPosition) / Time.fixedDeltaTime;
        float drag = 1 / handVelocity.magnitude + 0.01f;
        if (drag > 1)
            drag = 1;
        else if (drag < 0.03f)
            drag = 0.03f;
        previousPosition = transform.position;
        return drag;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        isColliding = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isColliding = false;
    }
}
