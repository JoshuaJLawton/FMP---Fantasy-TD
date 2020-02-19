using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public GameObject CurrentUnit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SelectUnit();
    }

    // When a unit is clicked, it will be set as the selected unit for the player
    void SelectUnit()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit ObjectInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out ObjectInfo);

            if (hit)
            {
                if (ObjectInfo.transform.gameObject.tag == "Player")
                {
                    Debug.Log("Current Unit is " + ObjectInfo.transform.gameObject.name);
                    CurrentUnit = ObjectInfo.transform.gameObject;
                }
            }
        }
    }
}
