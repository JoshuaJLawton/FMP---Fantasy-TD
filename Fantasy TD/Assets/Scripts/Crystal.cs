using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    Vector3 Offset = new Vector3();
    Vector3 Position = new Vector3();

    // Use this for initialization
    void Start()
    {
        Offset = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(0, 50 * Time.deltaTime, 0);


        Position = Offset;
        Position.y += Mathf.Sin(Time.fixedTime * Mathf.PI * 1) * 1;

        transform.position = Position;
    }
}
