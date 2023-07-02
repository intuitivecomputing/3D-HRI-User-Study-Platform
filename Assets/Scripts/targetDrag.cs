using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * This class handles the localization fo the object with respect to mouse for
 * the training session.
 */

public class targetDrag : MonoBehaviour
{
    private Vector3 mOffset;

    // Camera used in the game
    private Camera mainCamera;

    public GameObject targetScreen;

    private Vector3 posScreen;

    private void Start()
    {
        // Set the camera of the game
        mainCamera = GameObject.Find("trainCamera").GetComponent<Camera>();
        // Set the name of the robot
        GameObject.Find("Robo Name Text").transform.GetComponent<Text>().text = Run.robotName;
        GameObject.Find("Robo Name Text").transform.GetComponent<Text>().color = Run.roboC;
        targetScreen.transform.position = mainCamera.WorldToScreenPoint(gameObject.transform.position);
    }
    /*
     * This function runs when the mouse is down. It checks to see if the selected
     * object is draggable and then calculates the offset of the gameobject to the mouse.
     */
    void OnMouseDown()
    {
        posScreen = mainCamera.WorldToScreenPoint(transform.position);
        mOffset = Input.mousePosition - posScreen;
    }

    void OnMouseDrag()
    {
        // Calculate new position of the game object based on mouse position 
        transform.position = mainCamera.ScreenToWorldPoint(Input.mousePosition - mOffset);
        targetScreen.transform.position = mainCamera.WorldToScreenPoint(transform.position);
    }
}


