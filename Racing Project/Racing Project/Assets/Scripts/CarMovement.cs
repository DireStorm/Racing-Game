using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    //References
    public Transform centerOfMass;
    private Rigidbody rb;
    public Material mat;

    //Colliders
    public WheelCollider wheelFrontLeft;
    public WheelCollider wheelFrontRight;
    public WheelCollider wheelBackLeft;
    public WheelCollider wheelBackRight;

    public Transform wheelLFront;
    public Transform wheelRFront;
    public Transform wheelLBack;
    public Transform wheelRBack;

    public float motorTorque = 100f;
    public float maxSteer = 20f;

    public bool BreakingTF = false;
    public float brakeTorque = 1500f;

    //Acceleration & Deceleration 
    public float maxSpeed = 6f;
    public float timeZeroToMax = 2.5f;
    public float timeMaxToZero = 6f;
    public float decelRatePerSec;
    private float accelRatePerSec;

    //Audio
    public float topSpeed = 100; // km per hour
    private float currentSpeed = 0;
    private float pitch = 0;
    public AudioSource engineStart;
    public AudioSource Engine;
    public AudioSource skid;
    public AudioClip skidSound;
    public float skidVolume;


                    //Drifting
    //Drift Trails
    public TrailRenderer[] tireMarks;
    //Drift Variables
    private WheelFrictionCurve forwardFriction, sidewaysFriction;
    //Drift Smoke
    public ParticleSystem[] smoke;

    // Calculates Acceleration rate per second
    //private void Awake()
    //{
    //    accelRatePerSec = maxSpeed / timeZeroToMax;
    //    decelRatePerSec = -maxSpeed / timeMaxToZero;
    //}

    void FixedUpdate()
    {
        wheelBackLeft.motorTorque = Input.GetAxis("Vertical") * motorTorque;
        wheelBackRight.motorTorque = Input.GetAxis("Vertical") * motorTorque;
        wheelFrontLeft.steerAngle = Input.GetAxis("Horizontal") * maxSteer;
        wheelFrontRight.steerAngle = Input.GetAxis("Horizontal") * maxSteer;
        Drifting();
        //checkDrift();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass.localPosition;

        //Starts Engine Audio
        engineStart.Play();

        //Calculates Acceleration and Deceleration per second
        accelRatePerSec = maxSpeed / timeZeroToMax;
        decelRatePerSec = -maxSpeed / timeMaxToZero;

    }

    // Update is called once per frame
    void Update()
    {
        Acceleration();
        //Braking();

        //Audio Test
        currentSpeed = Math.Abs(transform.GetComponent<Rigidbody>().velocity.magnitude * 3.6f) + 10;
        //Debug.Log(currentSpeed);
        pitch = currentSpeed / topSpeed;
        Engine.pitch = pitch;
        //Debug.Log(motorTorque);

        var pos = Vector3.zero;
        var rot = Quaternion.identity;

        wheelFrontLeft.GetWorldPose(out pos, out rot);
        wheelLFront.position = pos;
        wheelLFront.rotation = rot * Quaternion.Euler(0, 180, 0);

        wheelFrontRight.GetWorldPose(out pos, out rot);
        wheelRFront.position = pos;
        wheelRFront.rotation = rot;

        wheelBackLeft.GetWorldPose(out pos, out rot);
        wheelLBack.position = pos;
        wheelLBack.rotation = rot * Quaternion.Euler(0, 180, 0);

        wheelBackRight.GetWorldPose(out pos, out rot);
        wheelRBack.position = pos;
        wheelRBack.rotation = rot;
    }

    // Sets forward velocity values for acceleration and deceleration
    void Acceleration()
    {
        if (Input.GetKey("w"))
        {
            motorTorque += accelRatePerSec * Time.deltaTime;
            motorTorque = Mathf.Min(motorTorque, maxSpeed);

        }
        else
        {
            motorTorque += decelRatePerSec * Time.deltaTime;
            motorTorque = Mathf.Max(motorTorque, 0);
        }

        // In this if statement, you also need to add that the car reverses after being fully breaked
        if (Input.GetKey("s"))
        {
            BreakingTF = true;
        } else
        {
            BreakingTF = false;
        }

        if (BreakingTF == true)
        {
            mat.SetColor("_EmissionColor", Color.red * 4);
            wheelBackLeft.brakeTorque = brakeTorque;
            wheelBackRight.brakeTorque = brakeTorque;
            wheelBackLeft.motorTorque = 0;
            wheelBackRight.motorTorque = 0;
            wheelFrontLeft.brakeTorque = brakeTorque;
            wheelFrontRight.brakeTorque = brakeTorque;
            wheelFrontLeft.motorTorque = 0;
            wheelFrontRight.motorTorque = 0;
        }
        else
        {
            mat.SetColor("_EmissionColor", Color.red * 1);
            wheelBackLeft.brakeTorque = 0;
            wheelBackRight.brakeTorque = 0;
            wheelFrontLeft.brakeTorque = 0;
            wheelFrontRight.brakeTorque = 0;
        }
    }

    //void Braking()
    //{
    //    if(Input.GetKey(KeyCode.Space))
    //    {
    //        BreakingTF = true;
    //    } else
    //    {
    //        BreakingTF = false;
    //    }

    //    if(BreakingTF == true)
    //    {
    //        mat.SetColor("_EmissionColor", Color.red * 4);
    //        wheelBackLeft.brakeTorque = brakeTorque;
    //        wheelBackRight.brakeTorque = brakeTorque;
    //        wheelBackLeft.motorTorque = 0;
    //        wheelBackRight.motorTorque = 0;
    //        wheelFrontLeft.brakeTorque = brakeTorque;
    //        wheelFrontRight.brakeTorque = brakeTorque;
    //        wheelFrontLeft.motorTorque = 0;
    //        wheelFrontRight.motorTorque = 0;
    //    } else
    //    {
    //        mat.SetColor("_EmissionColor", Color.red * 1);
    //        wheelBackLeft.brakeTorque = 0;
    //        wheelBackRight.brakeTorque = 0;
    //        wheelFrontLeft.brakeTorque = 0;
    //        wheelFrontRight.brakeTorque = 0;
    //    }
    //}

    // Drifting Test

    void Drifting()
    {
        wheelBackLeft.sidewaysFriction = sidewaysFriction;
        wheelBackRight.sidewaysFriction = sidewaysFriction;
        //wheelFrontLeft.sidewaysFriction = sidewaysFriction;
        //wheelFrontRight.sidewaysFriction = sidewaysFriction;

        if (Input.GetKey(KeyCode.Space))
        {
            sidewaysFriction.stiffness = .85f;
            sidewaysFriction.extremumSlip = .1f;
            sidewaysFriction.extremumValue = 1f;
            sidewaysFriction.asymptoteSlip = .4f;
            sidewaysFriction.asymptoteValue = .75f;
            for (int i = 0; i < 2; i++)
            {
                smoke[i].Play();
                if (smoke[i].isPlaying)
                {
                    Debug.Log("The smoke is playing.");
                }
                if (smoke[i].isEmitting)
                {
                    Debug.Log("The smoke is emitting");
                }
            }
            
            startRenderer();
            //skid.Play();
            skid.PlayOneShot(skidSound, skidVolume);

        } else
        {
            sidewaysFriction.stiffness = 1.1f;
            sidewaysFriction.extremumSlip = .1f;
            sidewaysFriction.extremumValue = 1f;
            sidewaysFriction.asymptoteSlip = .4f;
            sidewaysFriction.asymptoteValue = .75f;
            for (int i = 0; i < 2; i++)
            {
                smoke[i].Stop();
            }
            stopRenderer();
            skid.Stop();
            //AudioFade.FadeOut(skid, .0001f);
        }

    }

    //void checkDrift()
    //{
    //    if (Input.GetKey("a") || Input.GetKey("d"))
    //    {
    //        startRenderer();
    //    } else
    //    {
    //        stopRenderer();
    //    }
    //}

    void startRenderer()
    {
        foreach(TrailRenderer T in tireMarks)
        {
            T.emitting = true;
        }
    }

    void stopRenderer()
    {
        foreach (TrailRenderer T in tireMarks)
        {
            T.emitting = false;
        }
    }

}
