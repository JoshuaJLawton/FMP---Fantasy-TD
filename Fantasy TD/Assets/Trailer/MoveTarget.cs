using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTarget : MonoBehaviour
{
    public GameObject[] Locations;
    public int pointer = 0;

    public float Speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, Locations[pointer].transform.position, Speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, Locations[pointer].transform.position) < 1)
        {
            if (pointer == 10)
            {
                
            }
            else
            {
                pointer++;
            }
        }
    }
}
