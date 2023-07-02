using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * This class handles the pre-task exercises. Part 1 is a callibration exercise
 * where the user drags the robot through a series of waypoints and Part 2 has
 * user pick and place objects by dragging the robot from fridge to table.
 */
public class calibPoint : MonoBehaviour
{
    private bool isEnter = false;
    // Number of waypoints the robot has been dragged through
    private int count = 0;
    private int prevCount = -1;
    // Count of the number of ingredients place for Part 2
    private int rep = 0;
    // Object name for ingredient needed for Part 2
    private string whichOne = "";
    // Name of the ingredients already done for Part 2
    private List<string> nameArr = new List<string> { };
    // Object that is held by the robot for Part 2
    private GameObject grippedObj;
    // Holds the rendering information for the robot
    Component[] renderers;
    // Initializing game object for the training session
    private GameObject g = null;
    // Index for the location of each ingredient in the game hierarchy for Part 2
    private int index = 1000;
    // Count for the number of done button clicks in the training session (should be 2)
    private int doneCount = 0;
    private bool toggleDone = false;
    // Index of waypoints to switch to
    private int nextPath = 0;
    private int prevrep = 0;
    // Array of instructions shown during training session; Part 1 and part 2
    private List<string> recipArr = new List<string> { "Drag the purple sphere, which guides the robot in the training process, to the red circles. " +
        "If the robot is calibrated correctly the circles will turn green. If the robot is stuck, click reset robot.", "To do this, select any ingredient from the menu and then guide the robot " +
        "through the numbered path. Once the ingredient is at the table level, click on the place button to set the ingredient down. Then repeat until all of the ingredients are placed. If " +
        "the robot is stuck, click Reset Robot."};
    // Name of current ingredient
    public static string ingred = "";

    // Original position of the game object
    private Vector3 oldPos;

    [SerializeField] private GameObject gripperTip;
    [SerializeField] private GameObject rec;
    [SerializeField] private GameObject screenTarget;

    private Camera mainCamera;

    private void Awake()
    {
        oldPos = gameObject.transform.position;
        // Game object robot for the training session
        GameObject g = GameObject.Find("Training").transform.GetChild(2).gameObject;
        g = g.transform.GetChild(0).gameObject;
        renderers = g.GetComponentsInChildren(typeof(MeshRenderer));
        // For each rendering of the robot change it to the selected ingroup color
        foreach (MeshRenderer r in renderers)
        {
            foreach (var m in r.materials)
            {
                m.color = Run.roboC;
            }
        }
    }

    private void Start()
    {
        // Set the game camera to the training camera
        mainCamera = GameObject.Find("trainCamera").GetComponent<Camera>();
    }

    private void Update()
    {
        // If the game round is for the training session (aka round 1 or 2)
        if (Run.round < 2)
        {
            // Set the text for the instructions to be the training session instructions
            rec.GetComponent<Text>().text = recipArr[Run.round];
        }

        if (rep == 5 && prevrep!= rep)
        {
            GameObject.Find("Dock Menu (1)").transform.GetChild(0).gameObject.SetActive(false);
            GameObject.Find("Dock Menu (1)").transform.GetChild(1).gameObject.SetActive(false);
            GameObject.Find("Dock Menu (1)").transform.GetChild(2).gameObject.SetActive(false);
            GameObject.Find("Dock Menu (1)").transform.GetChild(3).gameObject.SetActive(true);
            prevrep = rep;

        }
        // Check if the first calibration path is completed
        if (count == 5 && count != prevCount)
        {
            prevCount = count;
            GameObject.Find("Pt1").SetActive(false);
            nameArr.Clear();
            isEnter = false;
            nextPath = 9;
            StartCoroutine(delay());
        }
        // Check if the second calibration path is completed
        else if (count == 10 && count != prevCount)
        {
            prevCount = count;
            GameObject.Find("Pt2").SetActive(false);
            nameArr.Clear();
            isEnter = false;
            nextPath = 10;
            StartCoroutine(delay());
        }
        // Check if it is part 2 of the training session
        else if (count == 11 && count != prevCount && rep > 0)
        {
            if (rep > 0)
            {
                // Get index of the ingredient that was requested
                prevCount = count;
                if (ingred.Equals("Dough"))
                {
                    index = 5;
                }
                else if (ingred.Equals("Cheese"))
                {
                    index = 7;
                }
                else if (ingred.Equals("Mushroom"))
                {
                    index = 6;
                }
                else if (ingred.Equals("Pepperoni"))
                {
                    index = 8;
                }

                // Create the ingredient game object, set the location of where
                // it appears, and attach with the gripper
                g = GameObject.Find("Game (1)").transform.GetChild(index).gameObject;
                grippedObj = Instantiate(g) as GameObject;
                grippedObj.transform.localScale = g.transform.localScale * 5;
                grippedObj.SetActive(true);
                grippedObj.transform.SetParent(gripperTip.transform);
                grippedObj.transform.localPosition = new Vector3(0f, 0f, -0.50f);
            }
        }
        // Check if the entire training session is complete or ingredient has
        // been dragged through all of the waypoint
        else if (count == 15 && count != prevCount)
        {
            prevCount = count;
            // Check if training session is done.
            if (rep == 0)
            {
                // Set the done button as active
                GameObject.Find("DoneButton").GetComponent<Button>().interactable = true;
            }

            else
            {
                // Set the place button as active for placing the ingredient
                GameObject.Find("Place Button").GetComponent<Button>().interactable = true;
            }
        }

    }

    // Check if the mouse is clicked
    private void OnMouseDown()
    {
        // Play sound while the robot is moving
        gameObject.GetComponent<AudioSource>().Play();
    }

    // Check if the mouse is dragging an object
    void OnMouseDrag()
    {
        // Check if the mouse has entered the area of one of the projected waypoints
        if(isEnter && count >= 5)
        {
            // Check specific waypoint is entered
            if (whichOne.Equals((count - 4).ToString()))
            {
                // Change the color of that waypoint
                GameObject.Find(whichOne).GetComponent<Image>().color = new Color32(0, 255, 0, 120);
                count++;
                nameArr.Add(whichOne);
            }

            else if (whichOne.Equals((count - 9).ToString()))
            {
                GameObject.Find(whichOne).GetComponent<Image>().color = new Color32(0, 255, 0, 120);
                count++;
                nameArr.Add(whichOne);
            }
        }

        else if (isEnter && !nameArr.Contains(whichOne))
        {
            GameObject.Find(whichOne).GetComponent<Image>().color = new Color32(0, 255, 0, 120);
            count++;
            nameArr.Add(whichOne);
        }
    }

    // Check if the mouse is no longer clicked
    private void OnMouseUp()
    {
        // Stop playing sound of the robot moving
        gameObject.GetComponent<AudioSource>().Stop();
    }

    // Function determines if the mouse is hovering over the inputted object
    public void Entered(Object obj)
    {
        isEnter = true;
        whichOne = obj.name;
    }

    // Function determines if the mouse has left the object
    public void Exited()
    {
        isEnter = false;
        whichOne = "";
    }

    // Function is called when the place button is clicked to place the
    // ingredient. 
    public void PlaceIngred()
    {
        // Destroys the object that is gripped by the robot
        for (int i = 0; i < gripperTip.transform.childCount; i++)
        {
            Destroy(gripperTip.transform.GetChild(i).gameObject);
        }
        // Set the ingredient's corresponding
        GameObject.Find("Game (1)").transform.GetChild(index).gameObject.SetActive(true);
    }

    // Function resets the positioning of the robot as well as the the color
    // for all of the waypoints. This function is called by the done button
    public void ResetRobot()
    {
        // Reset position of robot
        transform.position = oldPos;
        screenTarget.transform.position = mainCamera.WorldToScreenPoint(gameObject.transform.position);
        // Reset the color of the waypoints
        for (int i = 0; i < 5; i++)
        {
            GameObject.Find("" + (i + 1)).GetComponent<Image>().color = new Color32(255, 0, 0, 90);
        }
        // Reset the count to indicate a reset in progress of the training session
        if (count >= 5 && count < 10)
        {
            count = 5;
            prevCount = 5;
        }
        else if (count >= 0 && count < 5)
        {
            count = 0;
            prevCount = 0;
        }
        else if (count >= 10)
        {
            count = 10;
            prevCount = 10;
        }
        // Deactivate the clickability of the done button
        GameObject.Find("DoneButton").GetComponent<Button>().interactable = false;
        // If active, deactivate the clickability of the place button
        if (GameObject.Find("Dock Menu (1)").transform.GetChild(1).gameObject.activeSelf)
        {
            GameObject.Find("Place Button").GetComponent<Button>().interactable = false;
        }
        nameArr.Clear();
    }

    // Function resets the the color for all of the waypoints. This function is
    // called by the reset button
    public void ResetPath()
    {
        transform.position = oldPos;
        screenTarget.transform.position = mainCamera.WorldToScreenPoint(gameObject.transform.position);
        // Reset the color of the waypoints
        for (int i = 0; i < 5; i++)
        {
            GameObject.Find("" + (i+1)).GetComponent<Image>().color = new Color32(255, 0, 0, 90);
            
        }
        count = 10;
        prevCount = 10;
        ingred = "";
        rep++;
        GameObject.Find("DoneButton").GetComponent<Button>().interactable = false;
        GameObject.Find("Dock Menu (1)").transform.GetChild(0).gameObject.SetActive(true);
        
        GameObject.Find("Dock Menu (1)").transform.GetChild(1).gameObject.SetActive(true);
        GameObject.Find("Place Button").GetComponent<Button>().interactable = false;

        GameObject.Find("Path").SetActive(false);

    }

    // This function call delays the appearance of the next set of waypoints
    // the robot should be dragged through by one second.
    IEnumerator delay()
    {
        yield return new WaitForSeconds(1);
        GameObject.Find("Trace").transform.GetChild(nextPath).gameObject.SetActive(true);
        transform.position = oldPos;
        screenTarget.transform.position = mainCamera.WorldToScreenPoint(gameObject.transform.position);
    }

    public void removeToggle()
    {
        if (index != 1000)
        {
            GameObject.Find("T " + index).SetActive(false);
            GameObject curGroup = GameObject.Find("Items").transform.GetChild(1).gameObject;
            // Iterate through the toggles to reset them after clicking submit
            for (int j = 0; j < 3; j++)
            {
                if (curGroup.transform.GetChild(j).gameObject.activeInHierarchy)
                {
                    if (!toggleDone)
                    {
                        curGroup.transform.GetChild(j).GetComponent<Toggle>().isOn = true;
                        toggleDone = true;
                    }
                    else
                    {
                        curGroup.transform.GetChild(j).GetComponent<Toggle>().isOn = false;
                    }
                }
            }
        }
        toggleDone = false;
    }

    // This function is called by the done button. It determines if both parts
    // of the training session is done.
    public void doneTwo()
    {
        doneCount++;
        if (doneCount == 2)
        {
            GameObject.Find("Trace").transform.GetChild(11).gameObject.SetActive(false);
            GameObject.Find("Trace").transform.GetChild(12).gameObject.SetActive(true);
        }
    }

}
