using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/*
 * This class handles the menu of tools and ingredients this collects the names
 * of the clicked buttons and once submitted will send a list of objects
 * needed to be retrieved by the robot.
 */
public class menuBehavior : MonoBehaviour
{

    public delegate void OrderEventDelegate(List<string> x);
    public static event OrderEventDelegate OrderEvent;


    // Array containg tools and ingredients requested
    public List<string> foodArr = new List<string>();
    // Menu form input indicated through unity 
    public GameObject menu;

    // Store selected and not selected color
    private ColorBlock color;
    private ColorBlock oldColor;

    // Initializes the listener for each button and to see if clicked
    void Start()
    {
        // Set the original non-highlighted and non-selected color of button
        oldColor.normalColor = new Color32(255, 255, 255, 255);
        oldColor.selectedColor = new Color32(161, 157, 157, 255);
        oldColor.highlightedColor = new Color32(245, 245, 245, 255);
        oldColor.pressedColor = new Color32(200, 200, 200, 255);
        oldColor.colorMultiplier = 1;

        // Iterates through all of the buttons in the menu
        for (int i = 0; i < menu.transform.childCount; i++)
        {
            int closureIndex = i; // Prevents the closure problem
            Button curBut = menu.transform.GetChild(closureIndex).GetComponent<Button>();
            color = curBut.colors;
            // Calls the submitFoodRequest function when button is clicked
            curBut.onClick.AddListener(() => submitFoodRequest(closureIndex));
        }
    }

    // Function used when the submit button on the menu is clicked
    public void submitOrder()
    {
        OrderEvent(Run.triggerList);

        // Change button colors back to original not selected color
        for (int i = 0; i < menu.transform.childCount; i++)
        {
            color.colorMultiplier = 1;
            menu.transform.GetChild(i).GetComponent<Button>().colors = oldColor;
        }
        // Clear list for next round of requests
        foodArr.Clear();
    }

    // Function used to reset the menu if the close button is hit
    public void closeOrder()
    {
        // Change button colors back to original not selected color
        revertColor();
        // Clear list for next round of requests
        foodArr.Clear();
    }

    // Function is called on click and takes in the index of the button within
    // the button group. This function adds the name of the selected button to the food
    // array, if not added already, and changes the color. If the button has been
    // selected, then the second click would be to deselect it and remove from array.
    public void submitFoodRequest(int butIndex)
    {
        // Name of button that has been currently clicked on
        string curFood = menu.transform.GetChild(butIndex).Find("Text").GetComponent<Text>().text;
        // Check to see button has already been selected
        if (!foodArr.Contains(curFood))
        {
            // Add requeted item to array
            foodArr.Add(curFood);
            // Change color of button to selected color
            color.normalColor = new Color32(161, 157, 157, 255);
            // Need this line make sure color shows up
            color.colorMultiplier = 1;
            menu.transform.GetChild(butIndex).GetComponent<Button>().colors = color;
        }
        // In the case of deselect an item
        else
        {
            // Remove the button from the item array
            foodArr.Remove(curFood);
            // Need to turn enabled off and on to force color update
            menu.transform.GetChild(butIndex).GetComponent<Button>().enabled = false;
            menu.transform.GetChild(butIndex).GetComponent<Button>().colors = oldColor;

            menu.transform.GetChild(butIndex).GetComponent<Button>().enabled = true;
        }

    }

    // This function changes the color of the button to the deselect color
    private void revertColor()
    {
        // Iterates through this the buttons to change all of the colors back
        for (int i = 0; i < menu.transform.childCount; i++)
        {
            color.colorMultiplier = 1;
            menu.transform.GetChild(i).GetComponent<Button>().colors = oldColor;
        }
    }
}