using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectState
{
    Gripped,
    Placed,
    Dragged,
    Dropped
}

/*
 * This class enables users to drag objects during the simulation and be added
 * to the pizzas when the user lets go. Also, it adapts to the object in case 
 * the item needs to be shaken based on the game.
 */
public class Drag : MonoBehaviour
{
    // Time alotted for objects that need to be shaken
    public static float actTime = 0;

    // Game object that needs to be active after the current one is dragged
    public GameObject next;

    // Game object that needs to be removed after the current one is 
    public GameObject prev;

    // Original position of the game object
    private Vector3 oldPos;

    // Camera used in the game
    private Camera mainCamera;

    // Flag to determine if the objects that need to be moved are done
    public static bool isShakeDone = false;

    // Indicator of whether the object is currently moving
    private bool shake;

    Vector2 oldMouseAxis;

    // Tracks what time the shaking stopped for an item that needs to be shaken
    private float stopTime = 0;

    // Origional position of the game object before the drag
    private Vector3 originalPos;

    // Flag to determine if object has collided with something
    [SerializeField] private bool isCollide = false;

    private RobotBehavior robot;
    // Object for the robot state machine
    private Animator animator;
    private Run run;
    private Vector3 droppedAttraction;
    private AudioSource audioSource;
    private Rigidbody rb;
    float planeY = 0;
    Plane p;

    [SerializeField]
    private ObjectState state = ObjectState.Placed;

    //States for the game objects
    public ObjectState State
    {
        get { return state; }
        set
        {
            var prevState = state;
            state = value;
            if (rb != null)
            {
                switch (value)
                {
                    case ObjectState.Gripped:
                        rb.isKinematic = true;
                        break;
                    case ObjectState.Dropped:
                        rb.isKinematic = false;
                        break;
                    case ObjectState.Placed:
                        rb.isKinematic = true;
                        break;
                    case ObjectState.Dragged:
                        rb.isKinematic = true;
                        // Check the object has already been placed. If so it
                        // can't be dragged
                        if (prevState != ObjectState.Placed)
                        {
                            state = prevState;
                            Debug.LogWarningFormat("[{0}] You can only drag placed item.", gameObject.name);
                        }
                        break;
                }
            }
        }
    }

    private void Start()
    {
        // Initialize the camera
        mainCamera = GameObject.Find("Camera").GetComponent<Camera>();
        // Initialize the sound
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        robot = GameObject.Find("Target").GetComponent<RobotBehavior>();
        animator = GameObject.Find("Target").GetComponent<Animator>();
        run = GameObject.Find("Initial").GetComponent<Run>();
        // Set place where the dropped objects should land
        droppedAttraction = GameObject.Find("Hall of Shame").transform.position;
        planeY = transform.position.y;
        p = new Plane(Vector3.up, Vector3.up * planeY);
    }

    private void OnValidate()
    {
        State = state; // handle change from inspector
    }

    // If the object has collided as a result of being dropped, play a sound
    // and have it fly across the screen.
    void OnCollisionEnter(Collision collision)
    {
    }

    // Function deals with placing an object that is gripped by the robot and sets collision
    // to true when a dragged object.
    private void OnTriggerEnter(Collider other)
    {
        // If the object is gripped by the robot and collided with the place target
        if (State == ObjectState.Gripped && other.gameObject.name == "Place Trigger")
        {
            // Set state of object to placed
            State = ObjectState.Placed;
            animator.SetTrigger("Item Placed");

            gameObject.SetActive(false);
            robot.PlaceEvent();
        }
        // Check what the object that was dragged collided with 
        else if (State == ObjectState.Dragged && other.tag == "Objects")
        {
            // Check if it is a valid collision that would move forward the pizza making
            if (Run.triggerList.Contains(other.name) || Run.triggerList.Contains(gameObject.name))
            {
                isCollide = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Objects" && State == ObjectState.Dragged)
        {
            isCollide = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Objects" && State == ObjectState.Dragged)
        {
            isCollide = false;
        }
    }

    private void OnEnable()
    {
        isCollide = false;
        if (State == ObjectState.Placed)
            originalPos = transform.position;
    }

    private void OnDisable()
    {
        State = ObjectState.Placed;
        isCollide = false;
        transform.position = originalPos;
    }

    // This function plays the audio clip once when trigger by the robot error
    private static AudioSource PlayClipAtPoint(AudioClip clip, Vector3 position)
    {
        var go = new GameObject("PlayAndForget");
        go.transform.position = position;
        var audioSource = go.AddComponent<AudioSource>();
        audioSource.clip = clip;
        Destroy(go, clip.length);
        audioSource.Play();
        return audioSource;
    }

    /*
     * This function runs when the mouse is down. It checks to see if the selected
     * object is draggable and then calculates the offset of the gameobject to the mouse.
     */
    void OnMouseDown()
    {
        State = ObjectState.Dragged;

        if (State == ObjectState.Dragged && Run.dragObj.Contains(gameObject.name))
        {
            // Turn on the trigger for the box collider of the object
            gameObject.GetComponent<Collider>().isTrigger = true;

            // Store the orginal position of the gameObject
            oldPos = gameObject.transform.position;

            // Store whether the object needs to be shaken to apply
            isShakeDone = !Run.needShake.Contains(gameObject.name);
        }
    }

    /*
     * When the mouse is dragging the object, calculating the new position of the
     * object and then check if the object needs to be shaken. In the case if the objects
     * need to be shaken, then detecting whether they are shaken for 3 sec.
     */
    void OnMouseDrag()
    {

        // Check to see if object is draggable
        if (State == ObjectState.Dragged && Run.dragObj.Contains(gameObject.name))
        {
            Debug.LogFormat("[{0}] Dragging at {1}, collide {2}, time {3}", gameObject.name, transform.position, isCollide, actTime);
            // Calculate new position of the game object based on mouse position 
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            float distance;
            if(p.Raycast(ray, out distance))
            {
                transform.position = ray.GetPoint(distance);
            }

            // Check if object is collided with other object and needs to be shaken
            if (isCollide && !isShakeDone)
            {
                // Get axis of the mouse when selected on the object
                Vector2 mouseAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                // Check to see the mouse is moving
                shake = Mathf.Sign(mouseAxis.x) != Mathf.Sign(this.oldMouseAxis.x) ||
                Mathf.Sign(mouseAxis.y) != Mathf.Sign(this.oldMouseAxis.y);
                oldMouseAxis = mouseAxis;

                // Set progress bar to be active
                GameObject.Find("Canvas").transform.GetChild(1).gameObject.SetActive(true);

                // Increment the time shaken
                actTime += Time.deltaTime;

                // Check if still shake then update the offical time
                if (shake)
                {
                    stopTime = actTime;
                }
                else
                {
                    // Pause the incremental timing
                    actTime = stopTime;
                }

                // 2 secs of shaking then need for shaking is done
                if (actTime % 60f >= 1.5f)
                {
                    isShakeDone = true;
                    GameObject.Find("Canvas").transform.GetChild(1).gameObject.SetActive(false);
                    actTime = 0;
                }
            }

            // Check if shaking is done and but had needed to shake
            if (isShakeDone && Run.needShake.Contains(gameObject.name) == true)
            {
                if (gameObject.name != "Knife")
                {
                    next.SetActive(true);
                }
                // Set next object in the sequence active
                if (gameObject.name == "Rolling Pin")
                {
                    Vector3 prevPos = new Vector3(oldPos.x, prev.transform.position.y, oldPos.z);
                    prev.transform.position = prevPos;
                    GameObject.Find("doughBall").SetActive(false);
                }

                // Set current object inactive
                transform.position = oldPos;
                // Reset all of the flags for next round
                isShakeDone = false;
                actTime = 0;
                stopTime = 0;

                if (gameObject.name != "Knife")
                {
                    gameObject.SetActive(false);
                }

                GameObject.Find("Tooltip").transform.GetChild(0).gameObject.SetActive(false);
                GameObject.Find("Tooltip").transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    /*
     * When the mouse is released, either 
     */
    void OnMouseUp()
    {
        if (State == ObjectState.Dragged)
            State = ObjectState.Placed;
        else
            return;

        // Check if object has collided, there is a next object needed, and didn't need to shake
        if (isCollide && next != null && Run.needShake.Contains(gameObject.name) == false)
        {
            // Set next object active
            if (gameObject.name == "CheeseBlock" && GameObject.Find("Game").transform.GetChild(15).gameObject.activeSelf)
            {
                GameObject.Find("TomatoPlaced").SetActive(false);
                next.SetActive(true);
                transform.position = oldPos;
                gameObject.SetActive(false);
                GameObject.Find("Tooltip").transform.GetChild(0).gameObject.SetActive(false);
                GameObject.Find("Tooltip").transform.GetChild(1).gameObject.SetActive(false);
            }
            else if (gameObject.name == "CheeseBlock" && !GameObject.Find("Game").transform.GetChild(15).gameObject.activeSelf)
            {
                transform.position = oldPos;
            }
            else
            {
                GameObject temp = GameObject.Find("End Pizza").transform.GetChild(Run.round).gameObject;
                temp.transform.Find(next.name).gameObject.SetActive(true);
                transform.position = oldPos;
                gameObject.SetActive(false);
                GameObject.Find("Tooltip").transform.GetChild(0).gameObject.SetActive(false);
                GameObject.Find("Tooltip").transform.GetChild(1).gameObject.SetActive(false);
            }
        }

        // Check if object has finished being shaken or it hasn't collided
        if (!isShakeDone || !isCollide)
        {
            // Reset object to the original placement position
            if (!gameObject.name.Contains("(Clone"))
            {
                transform.position = oldPos;
            }
        }

    }
}

