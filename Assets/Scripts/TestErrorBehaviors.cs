using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestErrorBehaviors : MonoBehaviour
{
    // Start is called before the first frame update
    RobotBehavior rb;
    List<string> order;
    List<ErrorType> errorList;
    public int testIdx;

    private void OnGUI()
    {
        GUI.Label(new Rect(500, 100, 1000, 500), "<color=green><size=40>Error: " + rb.currRoundError.ToString() + "-" + rb.errorState.ToString() + "</size></color>" );
    }


    void Start()
    {
        errorList = (new List<int>() {8, 9, 1, 2, 4, 5, 6, 7 }).Cast<ErrorType>().ToList();
        order = new List<string>() { "doughBall", "Rolling Pin", "Tomato", "Knife", "Rolling Pin" };

        testIdx = 0;

        rb = GameObject.Find("Target").GetComponent<RobotBehavior>();
        rb.enabled = true;

        rb.speed = 3;
        GameObject.Find("Start Panel").SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        switch (testIdx)
        {
            case 0:
                switch (rb.errorState)
                {
                    case ErrorState.Inactive:
                        rb.errorState = ErrorState.Triggered;
                        Debug.Log(rb.currRoundError.ToString());
                        break;
                    case ErrorState.Happened:
                        rb.errorState = ErrorState.Inactive;
                        testIdx += 1;
                        break;
                }
                break;

            case 1:
                switch (rb.errorState)
                {
                    case ErrorState.Inactive:
                        rb.errorState = ErrorState.Triggered;
                        Debug.Log(rb.currRoundError.ToString());
                        break;
                    case ErrorState.Happened:
                        rb.errorState = ErrorState.Inactive;
                        testIdx += 1;
                        rb.OnOrderEvent(order);
                        break;
                }
                break;

            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
                switch (rb.errorState)
                {
                    case ErrorState.Inactive:
                        rb.currRoundError = errorList[testIdx];
                        rb.errorState = ErrorState.Triggered;
                        Debug.Log(rb.currRoundError.ToString());
                        break;
                    case ErrorState.Happened:
                        if (rb.placed)
                        {
                            rb.errorState = ErrorState.Inactive;
                            testIdx += 1;
                        }
                        
                        break;
                }
                break;
            case 8:
                GetComponent<Animator>().SetBool("Picking", false);
                break;


        }

    }
}
