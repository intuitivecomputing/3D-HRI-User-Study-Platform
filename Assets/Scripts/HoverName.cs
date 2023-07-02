using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * This class handles providing labels on objects when the mouse hovers over them
 * so that the user knows what these objects are.
 */
public class HoverName : MonoBehaviour
{
    // Text box to write the labels into
    public Text tooltipText;
    // Camera used for the game
    public Camera mainCamera;

    // Handles placing the label at the tooltip
    private void Update()
    {
        // Calculating mouse point location and placing the label there
        Vector2 local;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GameObject.Find("Tooltip").transform.parent.GetComponent<RectTransform>(),
            Input.mousePosition, mainCamera, out local);
        GameObject.Find("Tooltip").transform.localPosition = local;
    }

    /*
     * When the mouse is over the game object then set the label to the name of
     * the game object and activate the label.
     */
    void OnMouseOver()
    {
        // Set the text in the label to the game object's label
        if (!gameObject.name.Equals("Pineapple(2)"))
        {
            tooltipText.text = gameObject.name;
        }
        else
        {
            tooltipText.text = "Pineapple";
        }
        
        // Set the label active
        GameObject.Find("Tooltip").transform.GetChild(0).gameObject.SetActive(true);
        GameObject.Find("Tooltip").transform.GetChild(1).gameObject.SetActive(true);
        // Adjust the location of the label at the tooltip
        Update();
    }

    /*
     * When the mouse is no longer over the tooltip, turn off of the label.
     */
    private void OnMouseExit()
    {
        GameObject.Find("Tooltip").transform.GetChild(0).gameObject.SetActive(false);
        GameObject.Find("Tooltip").transform.GetChild(1).gameObject.SetActive(false);
    }
}
