using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public enum ErrorType
{
    None,
    Hesitation,
    WrongObject,
    SideTaskWrong,
    SideTaskHesitate
}

public enum ErrorState
{
    Inactive,
    Triggered,
    Happened
}

public enum RobotModel
{
    Blue,
    Yellow
}

/*
 * This class handles all of the robot behavior and error triggering.
 */
public class RobotBehavior : MonoBehaviour
{

    // Array containg tools and ingredients requested
    public Queue<string> foodQueue = new Queue<string>();
    public float speed = 2f;
    public bool isDone;
    // workarounds for debugging
    public int setRound;
    public int setCondition;
    public bool placed;
    public Color c;
    
    [SerializeField] public ErrorType currRoundError;
    [SerializeField] public ErrorState errorState;

    private Run run;
    private Animator animator;
    [SerializeField] private GameObject gripperTip;
    [SerializeField] private string currOrder;
    [SerializeField] private ErrorType currError;
    [SerializeField] private string wrongItem;
    private int currRound;
    private GameObject grippedObj;
    private List<ErrorType> minorErrorList;// side task error
    private List<ErrorType> majorErrorList; // main task error
    private List<int> noErrorRounds; //round with no error 0, 1, 5
    // Flag for if the study is paused
    private bool pause = true;

    // This function is handles the robot behavior is paused due to it being in
    // between rounds or if the study is paused.
    public bool Pause
    {
        get { return pause; }
        set
        {
            pause = value;
            if (value)
            {
                animator.speed = 0;
                GameObject.Find("RobotBase").GetComponent<AudioSource>().enabled = false;
            }
            else
            {
                animator.speed = speed;
                GameObject.Find("RobotBase").GetComponent<AudioSource>().enabled = true;
            }
        }
    }

    [SerializeField]
    private RobotModel model;
    public RobotModel selectedModel;
    // This sets the color of the robot object.
    public RobotModel Model
    {
        get { return model; }
        set
        {
            model = value;
            GameObject g = GameObject.Find("RobotBase").transform.GetChild(0).gameObject;
            switch (value)
            { 
                case RobotModel.Blue:
                    ChangeColor(g, Color.blue);
                    break;
                case RobotModel.Yellow:
                    ChangeColor(g, Color.yellow);
                    break;
            }
        }
    }

    // This function sets the color of the robot based on which robo model should
    // be shown during a particular round
    public void SetRobotModel(string m)
    {
        GameObject canvas = GameObject.Find("Canvas").transform.GetChild(0).gameObject;
        canvas.transform.GetChild(0).GetComponent<Button>().enabled = false;
        ColorBlock start = canvas.transform.GetChild(0).GetComponent<Button>().colors;

        // Check if the robot should be blue
        if (m.Equals("Blue"))
        {
            Model = RobotModel.Blue;
            // Set displayed name of robot
            Run.robotName = "Ocean";
            Run.roboC = Color.blue;

            start.normalColor = new Color32(82, 151, 245, 255);
            start.selectedColor = new Color32(61, 19, 231, 255);
            start.highlightedColor = new Color32(48, 222, 248, 255);
            start.pressedColor = new Color32(61, 19, 231, 255);
            start.colorMultiplier = 1;

        }
        else if (m.Equals("Yellow"))
        {
            Model = RobotModel.Yellow;
            Run.robotName = "Sun";
            Run.roboC = Color.yellow;

            start.normalColor = new Color32(245, 234, 82, 255);
            start.selectedColor = new Color32(142, 129, 48, 255);
            start.highlightedColor = new Color32(231, 236, 161, 255);
            start.pressedColor = new Color32(142, 129, 48, 255);
            start.colorMultiplier = 1;
        }

         canvas.transform.GetChild(0).GetComponent<Button>().colors = start;
         canvas.transform.GetChild(0).GetComponent<Button>().enabled = true;
         selectedModel = Model;
    }

    // This function allows for switching of the robots colors when it needs
    // to be transitioned to the other robot (ingroup -> outgroup or vice versa).
    public void SwitchTeam()
    {
        switch (Model)
        {
            case RobotModel.Blue:
                Model = RobotModel.Yellow;
                break;
            case RobotModel.Yellow:
                Model = RobotModel.Blue;
                break;
        }
    }

    private void OnValidate()
    {
        Model = model;
    }

    // workarounds for debugging
    private int GetRound()
    {
        if (setRound == -1)
            return Run.round;
        else
        {
            return setRound;
        }
    }

    private int GetErrorCondition()
    {
        if (setCondition == -1)
            return Run.conditionError;
        else
        {
            return setCondition;
        }
    }

    private void Awake()
    {
        // Set up waiting for an order event
        menuBehavior.OrderEvent += this.OnOrderEvent;

        isDone = false;
        // Set the name of the object for the wrong object error
        wrongItem = "glass";

        setCondition = -1;
        setRound = -1;
        // Initialize the color of the robot
        Model = RobotModel.Blue;
    }


    private void Start()
    {
        // Initialize the robot animator
        animator = GetComponent<Animator>();
        animator.speed = speed;
        // Determine which rounds contain errrors and which don't
        noErrorRounds = new List<int> { 0, 1, 2 };
        noErrorRounds.Add(Random.Range(3, 6));
        noErrorRounds.Add(Random.Range(6, 9));
        minorErrorList = (new List<int>() { 1, 2 }).Cast<ErrorType>().ToList();
        majorErrorList = (new List<int>() { 3, 4 }).Cast<ErrorType>().ToList();

        InitRound();

        run = GameObject.Find("Initial").GetComponent<Run>();

    }

    private void OnGUI()
    {
#if UNITY_EDITOR
        // Debugging ui
        GUI.Label(new Rect(0, 35, 40, 60), "Speed");
        speed = GUI.HorizontalSlider(new Rect(45, 40, 200, 60), speed, 0.0F, 3.0F);
        Pause = GUI.Toggle(new Rect(0, 60, 100, 30), Pause, "Pause");
        if (!Pause)
            animator.speed = speed;

        GUI.Label(new Rect(0, 10, 500, 40), "Round: " + GetRound() + ",Order: " + currOrder + ",Error: " + currRoundError.ToString() + "-" + errorState.ToString() + ",Cond:" + GetErrorCondition());
#endif
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (GetRound() != currRound)
        {
            InitRound();
        }
        // Check the error state for the round
        switch (errorState)
        {
            // If triggered then set animator to do the error action
            case ErrorState.Triggered:
                currError = currRoundError;
                // Check the type of error that has been triggered
                switch (currError)
                {
                    case ErrorType.Hesitation:
                        animator.SetBool("Error " + currError.ToString(), true);
                        break;
                    case ErrorType.WrongObject:
                    case ErrorType.SideTaskWrong:
                        animator.SetBool("Error Side Hesitate", false);
                        break;
                    case ErrorType.SideTaskHesitate:
                        animator.SetBool("Error Side Hesitate", true);
                        break;
                }
                break;
            case ErrorState.Inactive:
            // If error for the round already occurred then no longer flag the
            // need for the error
            case ErrorState.Happened:
                ResetFlags();
                currError = ErrorType.None;
                break;
        }
    }


    private void InitRound()
    {
        // Get round number
        currRound = GetRound();
        // Initialize the errors for the robot
        currError = ErrorType.None;
        currRoundError = ErrorType.None;
        errorState = ErrorState.Inactive;
        // Initialize what the current order is
        currOrder = "";
        // Initialize queue for selected food items needed
        foodQueue = new Queue<string>();

        // Check if round is the last round and reset everything
        if (currRound == 6)
        {
            minorErrorList.Clear();
            majorErrorList.Clear();

            minorErrorList = (new List<int>() { 1, 2 }).Cast<ErrorType>().ToList();
            majorErrorList = (new List<int>() { 3, 4 }).Cast<ErrorType>().ToList();
        }

        // Check if the current round is an error round
        if (!noErrorRounds.Contains(currRound))
        {
            int idx;
            // Check if it is a side task error (minor) or main task error 
            switch (GetErrorCondition())
            {
                // Randomly select an error from the list of errors
                case 1:
                    idx = Random.Range(0, minorErrorList.Count());
                    currRoundError = minorErrorList[idx];
                    minorErrorList.RemoveAt(idx);
                    break;
                case 2:
                    idx = Random.Range(0, majorErrorList.Count());
                    currRoundError = majorErrorList[idx];
                    majorErrorList.RemoveAt(idx);
                    break;
                default:
                    currRoundError = ErrorType.None;
                    break;
            }
        }
        // Pause robot
        if (currRound <= 3 || currRound == 6)
        {
            Pause = true;
        }

    }

    private void GripObject(GameObject obj) // Instantiate object and attach to gripper
    {
        grippedObj = Instantiate(obj);
        // Check if needed object is a rolling pin and scale accordingly
        if (obj.name.Equals("Rolling Pin"))
        {
            grippedObj.transform.localScale = obj.transform.localScale * 0.7f;
        }
        // Place the object int robot gripper
        grippedObj.transform.SetParent(gripperTip.transform);
        grippedObj.transform.localPosition = new Vector3(0f, 0f, -0.50f);
        grippedObj.GetComponent<Drag>().State = ObjectState.Gripped;

    }

    // This function handles the robot being paused for specified amount of time
    public IEnumerator PauseRobot(float time)
    {
        var s = animator.speed;
        speed = 0;
        animator.speed = 0f;
        Debug.Log("Pause");
        yield return new WaitForSecondsRealtime(time);
        animator.speed = s;
        speed = s;
    }

    private void ResetFlags()
    {
        // set all flags to false except the picking one
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Bool && param.name != "Picking")
            {
                animator.SetBool(param.name, false);
            }
        }
    }

    // This function is triggered after the order has been submitted and activates
    // the robot for picking up objects
    public void OnOrderEvent(List<string> x)
    {
        foodQueue = new Queue<string>(x);
        //Activate animation
        animator.SetBool("Picking", true);
        Pause = false;
    }

    // Prepick state of the robot
    public void PrePick()
    {
        // Double check there is nothing in gripper
        placed = false;
        Destroy(grippedObj);
        float randNum = Random.Range(0f, 1f);
        // Check if it is an error and if the error is randomly triggered for this pick
        if (errorState == ErrorState.Inactive && !noErrorRounds.Contains(currRound) && randNum <= 0.7f)
        {
            switch (currRoundError)
            {
                case ErrorType.None:
                    break;
                case ErrorType.SideTaskHesitate:
                    if (!animator.GetBool("Picking"))
                    {
                        errorState = ErrorState.Triggered;
                        Debug.Log(currRoundError + " error triggered.");
                    }
                    break;
                case ErrorType.SideTaskWrong:
                    if (!animator.GetBool("Picking"))
                    {
                        errorState = ErrorState.Triggered;
                        Debug.Log(currRoundError + " error triggered.");
                    }
                    break;
                default:
                    if (animator.GetBool("Picking"))
                    {
                        errorState = ErrorState.Triggered;
                        Debug.Log(currRoundError + " error triggered.");
                    }
                    break;
            }
        }

    }

    // Pick state of the robot
    public void PickEvent()
    {
        // Check if there is an error that occurs or not then behaves accordingly
        switch (currError)
        {
            case ErrorType.Hesitation:
                currOrder = "";
                StartCoroutine(waitEmote(2f, 5f, 1));
                errorState = ErrorState.Happened;
                break;
            case ErrorType.WrongObject:
                currOrder = wrongItem;
                errorState = ErrorState.Happened;
                GripObject(Resources.Load<GameObject>("Prefabs/" + currOrder));
                StartCoroutine(waitEmote(2f, 7.2f, 1));
                break;
            default:
                currOrder = foodQueue.Dequeue();
                GripObject(Resources.Load<GameObject>("Prefabs/" + currOrder));
                break;
        }

        // At the end of the queue, disable picking flag
        if (foodQueue.Count == 0)
        {
            animator.SetBool("Picking", false);

            if (GetRound() == 1)
            {
                StartCoroutine(delay());
                
                isDone = true;

            }
            else if (GetRound() == 5)
            {
                isDone = true;
            }
        }
    }

    private IEnumerator delay()
    {
        yield return new WaitForSeconds(5);
        GameObject.Find("Game").transform.GetChild(6).gameObject.SetActive(false);
        GameObject.Find("Game").transform.GetChild(10).gameObject.SetActive(false);
        GameObject.Find("Game").transform.GetChild(12).gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.GetChild(33).gameObject.SetActive(true);
        Pause = true;
    }

    // Place state of the robot
    public void PlaceEvent()
    {
        if (!placed)
        {
            switch (currError)
            {
                default:
                    GameObject ingredient = run.FindObject(currOrder);
                    ingredient.SetActive(true);
                    Run.dragObj.Add(ingredient.name);
                    grippedObj.GetComponent<Drag>().State = ObjectState.Placed;
                    Destroy(grippedObj);
                    if (errorState == ErrorState.Triggered && currRoundError == ErrorType.Hesitation)
                    {
                        animator.SetBool("Error " + currError.ToString(), false);
                    }
                    break;
            }

            ResetFlags(); // reset animation flags, wouldn't change local flags, will be set again on update

            if (errorState == ErrorState.Triggered && currRoundError != ErrorType.SideTaskWrong && currRoundError != ErrorType.SideTaskHesitate)
            {
                errorState = ErrorState.Happened;
            }
            placed = true;
        }
    }

    public void DropEvent()
    {
    }

    public void PauseEvent()
    {
    }

    public void SideTaskStart()
    {
        string obj = "Plate2";
        if (currError == ErrorType.SideTaskWrong)
        {
            obj = "Pineapple";
            StartCoroutine(waitEmote(0.75f, 4.8f, 1));
        }
        else if (currError == ErrorType.SideTaskHesitate)
        {
            StartCoroutine(waitEmote(1.3f, 5f, 1));
        }

        GripObject(Resources.Load<GameObject>("Prefabs/" + obj));

    }

    public void SideTaskFinish()
    {
        switch (currError)
        {
            case ErrorType.SideTaskWrong:
                errorState = ErrorState.Happened;
                GameObject placedObj = grippedObj;
                grippedObj = new GameObject();
                placedObj.transform.SetParent(GameObject.Find("Game").transform, true);
                break;
            case ErrorType.SideTaskHesitate:
                animator.SetBool("Error Side Hesitate", false);
                errorState = ErrorState.Happened;
                Destroy(grippedObj);
                break;
            default:
                Destroy(grippedObj);
                break;
        }

    }

    public void SideTaskDrop()
    {
    }

    public void ChangeColor(GameObject obj, Color c)
    {
        Component[] renderers;
        GameObject g = obj.transform.GetChild(0).gameObject;
        renderers = g.GetComponentsInChildren(typeof(MeshRenderer));

        foreach (MeshRenderer r in renderers)
        {
            foreach (var m in r.sharedMaterials)
            {
                if (!m.name.Equals("china-plate") && !m.name.Equals("Cheese") && !m.name.Equals("RollingPin") && !m.name.Equals("TomatoSauce") && !m.name.Equals("Mushroom"))
                {
                    m.color = c;
                }
            }
        }
    }

    private IEnumerator waitEmote(float before, float middle, int whichEmote)
    {
        yield return new WaitForSeconds(before);
        GameObject.Find("Canvas").transform.GetChild(31).gameObject.SetActive(true);
        GameObject emotes = GameObject.Find("Error Emotes").gameObject;
        if (whichEmote == 0)
        {
            emotes.transform.GetChild(0).gameObject.GetComponent<Text>().text = "!!";
        }
        else
        {
            emotes.transform.GetChild(0).gameObject.GetComponent<Text>().text = "??";
        }
        yield return new WaitForSeconds(middle);
        GameObject.Find("Error Emotes").gameObject.SetActive(false);
    }

    public void resetErrors()
    {
        minorErrorList = (new List<int>() { 1, 2 }).Cast<ErrorType>().ToList();
        majorErrorList = (new List<int>() { 3, 4 }).Cast<ErrorType>().ToList();
    }
}