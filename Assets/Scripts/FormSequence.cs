using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * This class determines the ordering of questionnaires that occur during the
 * study.
 */
public class FormSequence : MonoBehaviour
{
    // Logs the number of rounds that the users has finished with
    private int doneClickTimes;
    private int prevDone;

    public Text roundText;
    public Text otherText;
    public Text teamText;
    public Text otherTeamText;
    public Text notTrainText;

    // Start is called before the first frame update
    void Start()
    {
        doneClickTimes = 0;
        prevDone = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Check which robot was selected for training and then display
        // the name of the robot that was not trained by the participant
        if (Run.robotName.Equals("Ocean"))
        {
            notTrainText.text = "The name of this robot is SUN.";
        }
        else
        {
            notTrainText.text = "The name of this robot is OCEAN.";
        }

        // Check if new round has been completed and if so display questionnaires
        // after round 3, 4, 5 
        if (prevDone != doneClickTimes)
        {
            prevDone = doneClickTimes;
            if (doneClickTimes == 4)
            {
                // Results Panel
                roundText.text = "Your Team's Times: \n Pizza Order 1 " + Run.roundTimes[1] +
                    " \n Pizza Order 2 " + Run.roundTimes[2] + " \n Pizza Order 3 " + Run.roundTimes[3];
                otherText.text = "Other Team's Times: \n Pizza Order 1 " + Run.otherTeamTimes[1] +
                    " \n Pizza Order 2 " + Run.otherTeamTimes[2] + " \n Pizza Order 3 " + Run.otherTeamTimes[3];
                GameObject.Find("Canvas").transform.GetChild(24).gameObject.SetActive(true);
                // Capability
                GameObject.Find("Canvas").transform.GetChild(14).gameObject.SetActive(true);
                // Manipulation Check
                GameObject.Find("Canvas").transform.GetChild(19).gameObject.SetActive(true);
                // Percieved Trust
                GameObject.Find("Canvas").transform.GetChild(16).gameObject.SetActive(true);
                // Teamwork
                GameObject.Find("Canvas").transform.GetChild(17).gameObject.SetActive(true);
                // Robot Error
                GameObject.Find("Canvas").transform.GetChild(13).gameObject.SetActive(true);
                // Transition Panel
                GameObject.Find("Canvas").transform.GetChild(10).gameObject.SetActive(true);
                // Other Robot Panel
                if (Run.curGroup == 1)
                {
                    GameObject.Find("Canvas").transform.GetChild(8).gameObject.SetActive(true);
                }
                else
                {
                    GameObject.Find("Canvas").transform.GetChild(9).gameObject.SetActive(true);
                }
            }
            else if (doneClickTimes == 5)
            {
                // Results Panel
                roundText.text = "Your Team's Times: \n Pizza Order 1 " + Run.roundTimes[4] +
                    " \n Pizza Order 2 " + Run.roundTimes[5] + " \n Pizza Order 3 " + Run.roundTimes[6];
                otherText.text = "Other Team's Times: \n Pizza Order 1 " + Run.otherTeamTimes[4] +
                    " \n Pizza Order 2 " + Run.otherTeamTimes[5] + " \n Pizza Order 3 " + Run.otherTeamTimes[6];
                GameObject.Find("Canvas").transform.GetChild(24).gameObject.SetActive(true);
                // Capability
                GameObject.Find("Canvas").transform.GetChild(14).gameObject.SetActive(true);
                // Manipulation Check
                GameObject.Find("Canvas").transform.GetChild(19).gameObject.SetActive(true);
                // Percieved Trust
                GameObject.Find("Canvas").transform.GetChild(16).gameObject.SetActive(true);
                // Teamwork
                GameObject.Find("Canvas").transform.GetChild(17).gameObject.SetActive(true);
                // Robot Error
                GameObject.Find("Canvas").transform.GetChild(13).gameObject.SetActive(true);
                // Preference Form
                if (Run.robotName.Equals("Ocean"))
                {
                    otherTeamText.text = "Sun";
                }
                else
                {
                    otherTeamText.text = "Ocean";
                }
                GameObject.Find("Canvas").transform.GetChild(12).gameObject.SetActive(true);
                // Demographics
                GameObject.Find("Canvas").transform.GetChild(11).gameObject.SetActive(true);
            }
            else if (doneClickTimes == 3)
            {
                // End of Practice panel
                GameObject.Find("Canvas").transform.GetChild(25).gameObject.SetActive(true);

                // Team panel
                teamText.text = "The robot you just trained " + Run.robotName + ". \n Please remember it.";
                GameObject.Find("Canvas").transform.GetChild(21).gameObject.SetActive(true);

                // Other team's robot Panel
                if (Run.curGroup == 1)
                {
                    GameObject.Find("Canvas").transform.GetChild(8).gameObject.SetActive(true);
                }
                else
                {
                    GameObject.Find("Canvas").transform.GetChild(9).gameObject.SetActive(true);
                }
            }
        }
    }

    // Function called by the done button to log the number of times it has been
    // clicked
    public void clicked()
    {
        doneClickTimes += 1;
    }

    // If round is reset then adjust round count and done click times
    public void repeatRound()
    {
        doneClickTimes -= 1;
        Run.round -= 2;
    }
}
