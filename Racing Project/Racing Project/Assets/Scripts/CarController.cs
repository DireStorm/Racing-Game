using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public enum Axel
{
    Front,
    Rear
}
[Serializable]
public struct wheel
{
    public GameObject model;
    public WheelCollider collider;
    public Axel axel;
}
public class CarController : MonoBehaviour
{
    [SerializeField]
    private float maxAcceleration = 20.0f;
    [SerializeField]
    private float turnSensitivty = 1.0f;
    [SerializeField]
    private float maxSteerAngle = 45.0f;
    [SerializeField]
    private List<wheel> wheels;
    [SerializeField]
    private Vector3 COM;

    private float inputX, inputY;
    //References
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // HELPS THE CAR FROM NOT FLIPPING OVER
        rb.centerOfMass = COM;
    }

    private void Update()
    {
        getInputs();
        animateWheels();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        Turn();
    }

    private void getInputs()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");
    }

    private void Move()
    {
        foreach(var wheel in wheels)
        {
            wheel.collider.motorTorque = inputY * maxAcceleration * 500 * Time.deltaTime;
        }
    }

    private void Turn()
    {
        foreach(var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                var steerAngle = inputX * turnSensitivty * maxSteerAngle;
                wheel.collider.steerAngle = Mathf.Lerp(wheel.collider.steerAngle, steerAngle, .5f);
            }
        }
    }

    private void animateWheels()
    {
        foreach(var wheel in wheels)
        {
            Quaternion wheelRotation;
            Vector3 wheelPos;
            wheel.collider.GetWorldPose(out wheelPos, out wheelRotation);
            wheel.model.transform.position = wheelPos;
            wheel.model.transform.rotation = wheelRotation;
        }
    }
}
