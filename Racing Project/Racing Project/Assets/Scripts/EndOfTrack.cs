using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfTrack : MonoBehaviour
{
    // This is where you are going to enter the code for what the AI does once it reaches the end... 
    // The code that goes here varies based on what you plan on doing.
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("End Game!");
    }
}
