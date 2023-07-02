using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * This class if used to rotate the robot objects on the team selection page.
 */
public class RotateObject : MonoBehaviour
{
    public float step = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Rotates the object
        transform.Rotate(0f, step, 0f);
    }
}
