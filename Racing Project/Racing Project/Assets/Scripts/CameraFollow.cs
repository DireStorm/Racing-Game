using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //Refernces
    //public Transform player;
    //public Vector3 offset;
    public Transform cameraTarget;
    public float cSpeed = 10.0f;
    public Vector3 dist;
    public Transform lookTarget;

    // Update is called once per frame
    void Update()
    {
        ////Debug.Log(player.position);
        //transform.position = player.position + offset;
        Vector3 dPos = cameraTarget.position + dist;
        Vector3 sPos = Vector3.Lerp(transform.position, dPos, cSpeed * Time.deltaTime);
        transform.position = sPos;
        transform.LookAt(lookTarget.position);
    }
}
