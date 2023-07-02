using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/*
 * This class if the main class for the simulation. All of the other code
 * stems and is connected through this class.
 */

public class Run : MonoBehaviour
{
    // Current round number
    public static int round;
    // Objects in list can be dragged
    public static List<string> dragObj;
    // Objects that need to shaked before added to the pizza
    public static List<string> needShake = new List<string> { "Rolling Pin", "Tomato"};
    // List of objects in the order that needs to be brought by the robot
    public static List<string> triggerList;
    // List of all game objects
    public static List<GameObject> allGameObj;
    // Current recipe for the round
    public static string curRecipe;
    // 0 for ingroup robot first and 1 for outgroup first 
    public static int conditionGroup;
    // 0 for no error, 1 low severity error, 2 high severity error
    public static int conditionError;
    // ID for the user
    public static string userID;
    public static string robotName;
    public static int curGroup;
    public static string[] roundTimes = { "0", "0", "0", "0", "0", "0", "0", "0", "0" };
    public static string[] otherTeamTimes = { "0", "0", "0", "0", "0", "0", "0", "0", "0" };
    public static string[] nonIdleTimes = { "0", "0", "0", "0", "0", "0", "0", "0", "0" };
    public static string trainTime = "";
    public static string satisfaction = "";

    // Variable of text to display the order
    public Text pizzaOrder;

    public Text robotText;

    public Text recipeInstruct;

    public Text calibrationText;

    public static Color roboC;

    private int pizzaCount = 0;

    private bool isPractice = false;

    private bool isOvenClick = false;

    public static int practiceCount = 0;

    // List of all of the pizza orders
    private string[] pizzaOrderList = {"Training Pt. 1", "Training Pt. 2", "Cheese", "Mushroom Pepper", "Mushroom Sausage",
        "Pepper Pepperoni", "Pepperoni Sausage", "Sausage Pepper", "Pepperoni Mushroom"};
    private int prevRound;
    // List of the possible condition combinations for the user study
    private string[] conditionList = { "00", "01", "02", "10", "11", "12" };
    private List<List<string>> orderList = new List<List<string>> { new List<string> { }, new List<string> {"doughBall", "Mushroom", "CheeseBlock", "Pepperoni"}, new List<string> { "doughBall", "Rolling Pin", "Tomato", "CheeseBlock"}, new List<string> { "doughBall", "Rolling Pin", "Tomato", "CheeseBlock", "Mushroom", "Pepper"},
    new List<string> { "doughBall", "Rolling Pin", "Tomato", "CheeseBlock", "Mushroom", "Sausage"}, new List<string> { "doughBall", "Rolling Pin", "Tomato", "CheeseBlock", "Pepper", "Pepperoni"},
    new List<string> { "doughBall", "Rolling Pin", "Tomato", "CheeseBlock", "Pepperoni", "Sausage"},
    new List<string> { "doughBall", "Rolling Pin", "Tomato", "CheeseBlock", "Sausage", "Pepper"}, new List<string> { "doughBall", "Rolling Pin", "Tomato", "CheeseBlock", "Pepperoni", "Mushroom"}};
    private ColorBlock temp;

    private RobotBehavior robotBehavior;

    private string curCondition;

    private Timer timer;

    private float remainingTime;

    private float rTime;

    private List<string> recipeList = new List<string> {"", "", "Cheese Pizza Recipe: \n \n Ingredients/Tools \n Pizza Dough \n Rolling Pin \n Tomato Sauce \n " +
        "Cheese \n \n Instructions \n 1. Place the pizza dough on top of the pizza stone. \n 2. Use the rolling pin to roll out the dough. \n 3. Spread tomato sauce onto the pizza. \n " +
        "4. Sprinkle cheese on top. \n 5. Put the pizza into the oven.", "Mushroom Pepper Pizza Recipe: \n \n Ingredients/Tools \n Pizza Dough \n " +
        "Rolling Pin \n Tomato Sauce \n Cheese \n Mushrooms \n Peppers \n \n Instructions \n 1. Place the pizza dough on top of the pizza stone. \n 2. Use the rolling pin to roll out the dough. \n " +
        "3. Spread tomato sauce onto the pizza. \n 4. Sprinkle cheese on top. \n 5. Place mushrooms on the pizza. \n 6. Place peppers on the pizza. \n 7. Put the pizza into the oven.",
        "Mushroom Sausage Pizza Recipe: \n \n Ingredients/Tools \n Pizza Dough \n Rolling Pin \n Tomato Sauce \n Cheese \n Mushrooms \n Sausages \n \n Instructions \n 1. Place the pizza dough on top of the pizza stone. \n " +
        "2. Use the rolling pin to roll out the dough. \n 3. Spread tomato sauce onto the pizza. \n 4. Sprinkle cheese on top. \n 5. Place mushrooms on the pizza. \n 6. Place sausage on the pizza. \n 7. Put the pizza into the oven.",
        "Pepper Pepperoni Pizza Recipe: \n \n Ingredients/Tools \n Pizza Dough \n Rolling Pin \n Tomato Sauce \n Cheese \n Peppers \n Pepperoni \n \n Instructions \n 1. Place the pizza dough on top of the pizza stone. \n " +
        "2. Use the rolling pin to roll out the dough. \n 3. Spread tomato sauce onto the pizza. \n 4. Sprinkle cheese on top. \n 5. Place peppers on the pizza. \n 6. Place pepperoni on the pizza. \n 7. Put the pizza into the oven.",
        "Pepperoni Sausage Pizza Recipe: \n \n Ingredients/Tools \n Pizza Dough \n Rolling Pin \n Tomato Sauce \n Cheese \n Pepperoni \n Sausage \n \n Instructions \n 1. Place the pizza dough on top of the pizza stone. \n " +
        "2. Use the rolling pin to roll out the dough. \n 3. Spread tomato sauce onto the pizza. \n 4. Sprinkle cheese on top. \n 5. Place pepperoni on the pizza. \n 6. Place sausage on the pizza. \n 7. Put the pizza into the oven.",
        "Sausage Pepper Pizza Recipe: \n \n Ingredients/Tools \n Pizza Dough \n Rolling Pin \n Tomato Sauce \n Cheese \n Sausage \n Peppers \n \n Instructions \n 1. Place the pizza dough on top of the pizza stone. \n " +
        "2. Use the rolling pin to roll out the dough. \n 3. Spread tomato sauce onto the pizza. \n 4. Sprinkle cheese on top. \n 5. Place sausage on the pizza. \n 6. Place peppers on the pizza. \n 7. Put the pizza into the oven.",
        "Pepperoni Mushroom Pizza Recipe: \n \n Ingredients/Tools \n Pizza Dough \n Rolling Pin \n Tomato Sauce \n Cheese \n Pepperoni \n Mushrooms \n \n Instructions \n 1. Place the pizza dough on top of the pizza stone. \n " +
        "2. Use the rolling pin to roll out the dough. \n 3. Spread tomato sauce onto the pizza. \n 4. Sprinkle cheese on top. \n 5. Place pepperoni on the pizza. \n 6. Place mushrooms on the pizza. \n 7. Put the pizza into the oven."};


    void Awake()
    {
        Random.InitState((int)DateTime.Now.Ticks);
        // Load all of the game objects in the list
        allGameObj = GetAllObjectsOnlyInScene();

        robotBehavior = GameObject.Find("RobotBase").GetComponentInChildren<RobotBehavior>();

        timer = GameObject.Find("Canvas").GetComponent<Timer>();

        robotName = "";

        roboC = Color.blue;

    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the round number
        round = 0;
        robotText.text = " ";
        curGroup = 0;

        // Initialize the draggable list
        dragObj = new List<string>();
        // Get the initial recipe for pizza making
        curRecipe = pizzaOrderList[round];
        prevRound = round;

        recipeInstruct.text = recipeList[0];
        recipeInstruct.fontSize = 50;

        // Initialize the gamebobject list of the for the robot to drag
        triggerList = new List<string>();

        curCondition = conditionList[Random.Range(0, 6)];
        conditionGroup = curCondition[0] - '0';
        conditionError = curCondition[1] - '0';
        Debug.LogFormat("Condition group: {0}, Error: {1}", conditionGroup, conditionError);

        Guid g = Guid.NewGuid();
        userID = g.ToString();
        robotBehavior.enabled = true;

        GameObject.Find("Oven Button").GetComponent<Button>().interactable = false;
        GameObject.Find("Done Button").GetComponent<Button>().interactable = false;
    }

    // Update is called once per frame
    void Update()
    {

        // Check if the next round has started
        if (round != prevRound)
        {
            if (round == 2)
            {
                trainTime = ((int)(timer.trainT / 60)).ToString("00") + ":" + (timer.trainT % 60f).ToString("00");
                isPractice = true;
            }
            prevRound = round;
        }

        // Change the displayed pizza order
        if (!isPractice)
        {
            pizzaOrder.text = "Current Pizza Order: " + curRecipe + " Pizza";
            pizzaOrder.fontSize = 75;
            if (round == 1)
            {
                pizzaOrder.text = "Demonstration";
                pizzaOrder.fontSize = 75;
                GameObject.Find("Canvas").transform.GetChild(30).gameObject.SetActive(false);
            }
            else if (GameObject.Find("Canvas").transform.GetChild(30).gameObject.activeSelf && round % 3 == 0 && round < 9)
            {
                for (int i = 0; i < 2; i++)
                {
                    GameObject.Find("Order List").transform.GetChild(i+1).gameObject.GetComponent<Text>().text = pizzaOrderList[round + i + 1] + " Pizza";
                }
            }
                
        }
        else
        {
            pizzaOrder.text = " Tutorial Pizza Order: " + curRecipe + " Pizza";
            pizzaOrder.fontSize = 75;
            GameObject.Find("Canvas").transform.GetChild(30).gameObject.SetActive(false);
        }

         
        if (round == 3 && GameObject.Find("PizzaDone").transform.GetChild(1).gameObject.activeSelf)
        {
            GameObject.Find("Done Button").GetComponent<Button>().interactable = true;
            GameObject.Find("Robot Request Button").GetComponent<Button>().interactable = false;
        }
        else if (GameObject.Find("PizzaDone").transform.GetChild(3).gameObject.activeSelf)
        {
            GameObject.Find("Done Button").GetComponent<Button>().interactable = true;
        }
        if (round > 3 && GameObject.Find("PizzaDone").transform.GetChild(3).gameObject.activeSelf)
        {
            GameObject.Find("Robot Request Button").GetComponent<Button>().interactable = false;
        }
        

        GameObject temp = GameObject.Find("End Pizza");

        if (round < 9)
        {
            temp = GameObject.Find("End Pizza").transform.GetChild(round).gameObject;

            // Check the progress of pizza to make baking available
            if (round == 2 && temp.activeSelf) 
            {
                // Turn on the button to be available to be clicked
                GameObject.Find("Oven Button").GetComponent<Button>().interactable = true;
            }

            if (round > 2 && temp.transform.GetChild(1).gameObject.activeSelf)
            {
                if (!GameObject.Find("Dock Menu").transform.GetChild(3).gameObject.activeSelf && !isOvenClick)
                {
                    // Turn on the button to be available to be clicked
                    GameObject.Find("Oven Button").GetComponent<Button>().interactable = true;
                }
            }
        }
    }

    /*
     * This functions handles all of the resets for the next order
     */
    public void nextRound()
    {
        if (round == 3 || round == 6)
        {
            remainingTime = 200f;
        }

        if (round > 1)
        {
            rTime = Math.Abs(remainingTime - timer.countDownTime);
            roundTimes[round-2] = ((int)(rTime / 60)).ToString("00") + ":" + (rTime % 60f).ToString("00");
            float otherTime = Math.Abs(rTime - UnityEngine.Random.Range(2, 15));
            otherTeamTimes[round-2] = ((int)(otherTime / 60)).ToString("00") + ":" + (otherTime % 60f).ToString("00");
            nonIdleTimes[round-2] = ((int)(timer.nonIdleTime / 60)).ToString("00") + ":" + (timer.nonIdleTime % 60f).ToString("00");
            remainingTime = timer.countDownTime;
        }

        round += 1;
        if (round < 9)
        {
            if (round == 1)
            {
                GameObject.Find("Dock Menu").SetActive(false);
                robotBehavior.isDone = false;
                dragObj.Clear();

                triggerList = orderList[round];
                GameObject.Find("PizzaStone").SetActive(false);
            }

            isOvenClick = false;

            // Reset for the next round
            if (round > 1)
            {
                GameObject.Find("End Pizza").transform.GetChild(round - 1).gameObject.SetActive(false);
                GameObject.Find("Oven Button").GetComponent<Button>().interactable = false;
                GameObject.Find("Done Button").GetComponent<Button>().interactable = false;
                if (!isPractice)
                {
                    GameObject.Find("Dock Menu").transform.GetChild(0).gameObject.GetComponent<Button>().interactable = true;
                    curRecipe = pizzaOrderList[round];
                }
                
                
                robotBehavior.isDone = false;
                dragObj.Clear();

                triggerList = orderList[round];
                recipeInstruct.fontSize = 50;
                recipeInstruct.text = recipeList[round];
            }

            
            if (round != 2)
            {
                GameObject.Find("End Pizza").transform.GetChild(round).gameObject.SetActive(true);
            }
        }

    }

    // This function handles the changes that need to be made between the two
    // group membership factors after they need to be switched.
    public void Change()
    {
        if (conditionGroup == 0 && round > 1 && round <= 4)
        {
            robotBehavior.Model = robotBehavior.selectedModel;
            robotText.text = robotName;
            robotText.color = robotBehavior.selectedModel == RobotModel.Blue ? Color.blue : Color.yellow;


            robotText.fontSize = 80;

            GameObject.Find("Dock Menu").transform.GetChild(0).GetComponent<Button>().enabled = false;
            if (robotBehavior.selectedModel == RobotModel.Blue)
            {
                temp.normalColor = new Color32(82, 151, 245, 255);
                temp.selectedColor = new Color32(61, 19, 231, 255);
                temp.highlightedColor = new Color32(48, 222, 248, 255);
                temp.pressedColor = new Color32(61, 19, 231, 255);
                temp.colorMultiplier = 1;
            }
            else
            {
                temp.normalColor = new Color32(245, 234, 82, 255);
                temp.selectedColor = new Color32(142, 129, 48, 255);
                temp.highlightedColor = new Color32(231, 236, 161, 255);
                temp.pressedColor = new Color32(142, 129, 48, 255);
                temp.colorMultiplier = 1;
            }
            GameObject.Find("Dock Menu").transform.GetChild(0).GetComponent<Button>().colors = temp;
            GameObject.Find("Dock Menu").transform.GetChild(0).GetComponent<Button>().enabled = true;
            curGroup = 0;
        }
        else if (conditionGroup == 1 && round > 4)
        {
            robotBehavior.Model = robotBehavior.selectedModel;
            robotText.text = robotName;
            robotText.color = robotBehavior.selectedModel == RobotModel.Blue ? Color.blue : Color.yellow;

            GameObject.Find("Dock Menu").transform.GetChild(0).GetComponent<Button>().enabled = false;
            if (robotBehavior.selectedModel == RobotModel.Blue)
            {
                temp.normalColor = new Color32(82, 151, 245, 255);
                temp.selectedColor = new Color32(61, 19, 231, 255);
                temp.highlightedColor = new Color32(48, 222, 248, 255);
                temp.pressedColor = new Color32(61, 19, 231, 255);
                temp.colorMultiplier = 1;
            }
            else
            {
                temp.normalColor = new Color32(245, 234, 82, 255);
                temp.selectedColor = new Color32(142, 129, 48, 255);
                temp.highlightedColor = new Color32(231, 236, 161, 255);
                temp.pressedColor = new Color32(142, 129, 48, 255);
                temp.colorMultiplier = 1;
            }

            GameObject.Find("Dock Menu").transform.GetChild(0).GetComponent<Button>().colors = temp;
            GameObject.Find("Dock Menu").transform.GetChild(0).GetComponent<Button>().enabled = true;
            curGroup = 0;
        }
        else if (conditionGroup == 0 && round > 4)
        {
            robotBehavior.Model = robotBehavior.selectedModel == RobotModel.Blue ? RobotModel.Yellow : RobotModel.Blue;
            robotText.text = robotBehavior.selectedModel == RobotModel.Blue ? "Sun" : "Ocean";
            robotText.color = robotBehavior.selectedModel == RobotModel.Blue ? Color.yellow : Color.blue;

            GameObject.Find("Dock Menu").transform.GetChild(0).GetComponent<Button>().enabled = false;
            if (robotBehavior.selectedModel == RobotModel.Blue)
            {
                temp.normalColor = new Color32(245, 234, 82, 255);
                temp.selectedColor = new Color32(142, 129, 48, 255);
                temp.highlightedColor = new Color32(231, 236, 161, 255);
                temp.pressedColor = new Color32(142, 129, 48, 255);
                temp.colorMultiplier = 1;
            }
            else
            {
                temp.normalColor = new Color32(82, 151, 245, 255);
                temp.selectedColor = new Color32(61, 19, 231, 255);
                temp.highlightedColor = new Color32(48, 222, 248, 255);
                temp.pressedColor = new Color32(61, 19, 231, 255);
                temp.colorMultiplier = 1;
            }
            GameObject.Find("Dock Menu").transform.GetChild(0).GetComponent<Button>().colors = temp;
            GameObject.Find("Dock Menu").transform.GetChild(0).GetComponent<Button>().enabled = true;
            curGroup = 1;
        }
        else if (conditionGroup == 1 && round > 1 && round <= 4)
        {
            robotBehavior.Model = robotBehavior.selectedModel == RobotModel.Blue ? RobotModel.Yellow : RobotModel.Blue;
            robotText.text = robotBehavior.selectedModel == RobotModel.Blue ? "Sun" : "Ocean";
            robotText.color = robotBehavior.selectedModel == RobotModel.Blue ? Color.yellow : Color.blue;

            GameObject.Find("Dock Menu").transform.GetChild(0).GetComponent<Button>().enabled = false;
            if (robotBehavior.selectedModel == RobotModel.Blue)
            {
                temp.normalColor = new Color32(245, 234, 82, 255);
                temp.selectedColor = new Color32(142, 129, 48, 255);
                temp.highlightedColor = new Color32(231, 236, 161, 255);
                temp.pressedColor = new Color32(142, 129, 48, 255);
                temp.colorMultiplier = 1;
            }
            else
            {
                temp.normalColor = new Color32(82, 151, 245, 255);
                temp.selectedColor = new Color32(61, 19, 231, 255);
                temp.highlightedColor = new Color32(48, 222, 248, 255);
                temp.pressedColor = new Color32(61, 19, 231, 255);
                temp.colorMultiplier = 1;
            }
            GameObject.Find("Dock Menu").transform.GetChild(0).GetComponent<Button>().colors = temp;
            GameObject.Find("Dock Menu").transform.GetChild(0).GetComponent<Button>().enabled = true;
            curGroup = 1;
        }
    }

    // This function controls the oven behavior.
    public void oven()
    {
        GameObject.Find(curRecipe).SetActive(false);
        GameObject.Find("PizzaDough").SetActive(false);
        GameObject.Find("End Pizza").transform.GetChild(2).gameObject.SetActive(false);
        GameObject.Find("End Pizza").transform.GetChild(round).gameObject.SetActive(false);
        isOvenClick = true;
        // Counts the number of pizzas that have been in the oven
        pizzaCount++;
        // "Cooks pizza in oven for a certain amount of time
        StartCoroutine(delay());
    }

    // This function handles the delay or cook time of the pizza in the oven. Then
    // allows the user to click the oven button
    IEnumerator delay()
    {
        GameObject.Find("Oven Button").GetComponent<Button>().interactable = false;
        yield return new WaitForSeconds(16);
        GameObject.Find("Dock Menu").transform.GetChild(3).gameObject.SetActive(true);
        GameObject.Find("Oven Button").transform.GetChild(0).gameObject.SetActive(false);
        Drag.actTime = 0;
    }

    // This function determines the buttons shown when the pizza has been taken
    // out of the oven
    public void OutOven()
    {
        GameObject.Find("PizzaDone").transform.GetChild(pizzaCount).gameObject.SetActive(true);
        if (!isPractice)
        {
            if (round == 5 || round == 9)
            {
                GameObject.Find("Dock Menu").transform.GetChild(0).gameObject.GetComponent<Button>().interactable = false;
            }
            else
            {
                GameObject.Find("Dock Menu").transform.GetChild(0).gameObject.GetComponent<Button>().interactable = true;
            }
        }
        else
        {
            GameObject.Find("Dock Menu").transform.GetChild(0).gameObject.GetComponent<Button>().interactable = false;
        }

        if (round > 2 && pizzaCount > 0 && pizzaCount < 3)
        {
            GameObject.Find("Order List").transform.GetChild(pizzaCount).gameObject.SetActive(false);
        }

        Drag.actTime = 0;
    }

    // Once a pizza is done all of the objects that make up a pizza is set as invisible
    public void resetPizza()
    {
        for (int i = pizzaCount; i > 0; i--)
        {
            GameObject.Find("PizzaDone").transform.GetChild(i).gameObject.SetActive(false);
        }
        GameObject.Find("PizzaDone").transform.GetChild(pizzaCount).gameObject.SetActive(false);
        pizzaCount = 0;
        isPractice = false;
        if (round < 9)
        {
            curRecipe = pizzaOrderList[round];
        }
    }

    // Gets the name of all of the objects in the scene
    List<GameObject> GetAllObjectsOnlyInScene()
    {
        List<GameObject> objectsInScene = new List<GameObject>();
        objectsInScene.AddRange(GameObject.FindGameObjectsWithTag("Objects"));
        objectsInScene.ForEach(x => x.SetActive(false));

        return objectsInScene;
    }

    public void activateOrder()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject.Find("Canvas").transform.GetChild(30).transform.GetChild(i+1).gameObject.SetActive(true);
        }
    }

    public GameObject FindObject(string name)
    {
        return allGameObj.Find(x => x.name.Equals(name));
    }

    public void doExitGame()
    {
        Application.Quit();
    }

    public void revertColor()
    {
        robotBehavior.Model = robotBehavior.selectedModel;
        robotText.text = robotName;
        robotText.color = robotBehavior.selectedModel == RobotModel.Blue ? Color.blue : Color.yellow;
    }

    public void addPractice()
    {
        practiceCount += 1;
    }

    public void destroyHall()
    {
        List<GameObject> activeObj = new List<GameObject> { };
        foreach(Transform obj in GameObject.Find("Game").transform)
        {
            if (obj.gameObject.name.Contains("(Clone)"))
            {
                activeObj.Add(obj.gameObject);
            }
        }

        foreach(GameObject o in activeObj)
        {
            Destroy(o);
        }

        if (GameObject.Find("Game").transform.GetChild(21).gameObject.activeSelf)
        {
            GameObject.Find("Game").transform.GetChild(21).gameObject.SetActive(false);
        }
    }

    public void loadingDots()
    {
        StartCoroutine(dots());
    }

    // This function determines the behavior of the loading screen when the software
    // is first run.
    private IEnumerator dots()
    {
        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(1);
            GameObject.Find("Dots").transform.GetChild(0).gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            GameObject.Find("Dots").transform.GetChild(1).gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            GameObject.Find("Dots").transform.GetChild(2).gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            GameObject.Find("Dots").transform.GetChild(0).gameObject.SetActive(false);
            GameObject.Find("Dots").transform.GetChild(1).gameObject.SetActive(false);
            GameObject.Find("Dots").transform.GetChild(2).gameObject.SetActive(false);
        }
        GameObject.Find("Waiting Participant Panel").transform.GetChild(1).GetComponent<Text>().text = "Participant has joined. \n Please hit the next button.";
        GameObject.Find("Waiting Participant Panel").transform.GetChild(1).GetComponent<Text>().color = Color.blue;
        yield return new WaitForSeconds(0.3f);
        GameObject.Find("Waiting Participant Panel").transform.GetChild(0).GetComponent<Button>().interactable = true;
    }
}
