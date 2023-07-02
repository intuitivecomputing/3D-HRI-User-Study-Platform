using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * This class handles the timer in the upper right hand corner of the game. It
 * calculates the time from the start and allows for pausing, resuming, and
 * reseting of the timer. The pause, resume, and reset functions are used in
 * button presses.
 */
public class Timer : MonoBehaviour
{
    // Timer text object on Canvas
    public Text timerText;
    // Indicate if the timer has been paused
    private float timerTime;
    // Countdown timer in upper right hand corner
    public float countDownTime;
    // Timer for the amount of non-idle mouse time
    public float nonIdleTime;
    public float trainT;

    private float mins, secs;
    private float count;
    // Indicate whether the mouse is moving
    private bool shake;

    public Text compText;

    private int prevRound = 0;

    private bool isTrain = false;

    Vector2 oldMouseAxis;

    private float newPizzaCount;

    private int times;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize text object
        timerText = GameObject.Find("TimerText").GetComponent<Text>();
        // Initalize time lapse variable
        resetTime();
        shake = false;
        times = 0;
    }

    // Update is called once per frame
    void Update()
    {
        isMouseMoving();

        count += (Time.deltaTime * 1.1f);
        newPizzaCount += (Time.deltaTime * 1.1f);

        // Calculate time lapsed
        timerTime += Time.deltaTime;
        countDownTime -= Time.deltaTime;

        // Check if still shake then udpate the offical time
        if (shake)
        {
            nonIdleTime = timerTime;
        }
        else
        {
            // Pause the incremental timing 
            timerTime = nonIdleTime;
        }

        if (isTrain)
        {
            trainT += Time.deltaTime;
        }

        // Divide time lapsed into minutes and seconds
        mins = (int)(countDownTime / 60);
        secs = (countDownTime % 60f);

        // If object is not null (needed to not trigger a NullReferenceException)
        // and timer not paused
        if (timerText != null)
        {
            // Update the displayed time on game
            timerText.text = "Time: " + mins.ToString("00") + ":" + secs.ToString("00");
        }

        if (count >= 1 && countDownTime < 0)
        {
            count = 0;
            timerText.enabled = !timerText.enabled;
        }

        if (times > 6)
        {
            newPizzaCount = 0;
            prevRound = Run.round;
            GameObject.Find("Pizza Order").GetComponent<Text>().text = "Current Pizza Order: " + Run.curRecipe + " Pizza";
            GameObject.Find("Pizza Order").GetComponent<Text>().fontSize = 75;
            GameObject.Find("Pizza Order").GetComponent<Text>().enabled = true;
            times = 0;
        }
        else if (count >= 1 && Run.round != prevRound && Run.round > 3 && Run.round != 6)
        {
            GameObject.Find("Pizza Order").GetComponent<Text>().text = "New Pizza Order";
            GameObject.Find("Pizza Order").GetComponent<Text>().fontSize = 75;
            GameObject.Find("Pizza Order").GetComponent<Text>().enabled = !GameObject.Find("Pizza Order").GetComponent<Text>().enabled;
            newPizzaCount = 0;
            times += 1;
        }

    }
    /*
     * This function, in this case, is triggered by a button press and sets
     * the time scale to 0 so that the time won't pass.
     */
    public void pauseTime()
    {
        Time.timeScale = 0f;
    }

    /*
     * This function resets the timer after a button press.
     */
    public void resetTime()
    {
        timerTime = 0.0f;
        nonIdleTime = 0.0f;
        countDownTime = 200.0f;
        Time.timeScale = 1.0f;
        count = 0.0f;
        timerText.enabled = true;
    }


    public void startComp()
    {
        StartCoroutine(readySetGoTimer());
    }

    private IEnumerator readySetGoTimer()
    {
        GameObject.Find("Countdown Panel").transform.GetChild(0).gameObject.GetComponent<Button>().interactable = false;
        compText.text = "Ready";
        yield return new WaitForSeconds(2);
        compText.text = "Set";
        yield return new WaitForSeconds(2);
        compText.text = "Go!";
        yield return new WaitForSeconds(1);
        GameObject.Find("Countdown Panel").transform.GetChild(0).gameObject.GetComponent<Button>().interactable = true;
        compText.text = "";
        GameObject.Find("Countdown Panel").SetActive(false);
        resetTime();
        GameObject.Find("Canvas").GetComponent<AudioSource>().enabled = true;
    }

    private void isMouseMoving()
    {
        // Get axis of the mouse when selected on the object
        Vector2 mouseAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        // Check to see the mouse is moving
        shake = Mathf.Sign(mouseAxis.x) != Mathf.Sign(oldMouseAxis.x) ||
        Mathf.Sign(mouseAxis.y) != Mathf.Sign(oldMouseAxis.y);
        oldMouseAxis = mouseAxis;
    }

    public void robotTraining()
    {
        isTrain = !isTrain;
    }
}
