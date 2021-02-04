using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinCam : MonoBehaviour
{
    public Vector3 StartPos, StartRot, EndPos, EndRot;
    bool MoveCam = false;

    private Quaternion StartRotation = Quaternion.Euler(16.5f, 135, 0);
    private Quaternion EndRotation = Quaternion.Euler(-13.25f, 135, 0);

    public Quaternion CurrentAngle;

    float RotStart = 16.5f;
    float RotEnd = -13.25f;



    // Start is called before the first frame update
    void Start()
    {
        //this.transform.position = StartPos;
        //this.transform.eulerAngles = StartRot;

        StartPos = new Vector3(67.44f, 8, -242.44f);
        EndPos = new Vector3(25.5f, 3.75f, -200.5f);

        StartRot = new Vector3(16.5f, 135, 0);
        EndRot = new Vector3(-13.25f, 135, 0);

        CurrentAngle = StartRotation;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MoveCam = true;
        }

        if (MoveCam)
        {
            transform.position = Vector3.Lerp(transform.position, EndPos, 0.1f * Time.deltaTime);
            transform.eulerAngles = Vector3.Lerp(StartRot, EndRot, 2 * Time.deltaTime);

            //transform.rotation = Quaternion.Slerp(StartRotation, EndRotation, 0.1f);
        }
    }
}
