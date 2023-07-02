using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/*
 * This class handles writing all the user responses to various google sheets
 * so they can be analyzed. These functions are trigger by the submit button
 * of these forms unity submitting values in google forms.
 * 
 * You may notice that many of that there are XXXX in the code. We had to remove
 * links to forms to protect data covered under our IRB. To run the simulation
 * you must fill in the urls and then correspond entry numbers for each question
 */
public class WriteToFile : MonoBehaviour
{
    // Base url to access all of the google forms
    [SerializeField]
    private string BASE_URL = "https://docs.google.com/forms/d/e/";

    void Awake()
    {
        formBehavior.SubmitFormEvent += this.OnSubmitEvent;
    }

    public void OnSubmitEvent(List<string> x, string formName)
    {
        StartCoroutine(Post(x, formName));
    }

    IEnumerator Post(List<string> x, string formName)
    {
        string curCondition = "";
        if (Run.conditionGroup == 0 && (Run.round - 1) > 1 && (Run.round - 1) <= 5)
        {
            curCondition = "ingroup";
        }
        else if (Run.conditionGroup == 1 && (Run.round - 1) > 5)
        {
            curCondition = "ingroup";
        }
        else if (Run.conditionGroup == 0 && (Run.round - 1) > 5)
        {
            curCondition = "outgroup";
        }
        else if (Run.conditionGroup == 1 && (Run.round - 1) > 1 && (Run.round - 1) <= 5)
        {
            curCondition = "outgroup";
        }

        WWWForm form = new WWWForm();

        // Depending on the form each one has a different url and entry values
        // for the form.
        if (formName.Equals("Name"))
        {
            BASE_URL = BASE_URL + "XXXX";

            // Id for the partipant
            form.AddField("XXXX", Run.userID);
            // Conditions for the study: first digit is 0 for ingroup first and
            // 1 for outgroup first; second digit is 0 for no error, 1 for low
            // severity error
            form.AddField("XXXX", Run.conditionGroup.ToString() + Run.conditionError.ToString());
            // Given robot name for the ingroup robot
            form.AddField("XXXX", x[0]);
            form.AddField("XXXX", Run.robotName);
            form.AddField("XXXX", Run.practiceCount);
            form.AddField("XXXX", Run.trainTime);
            form.AddField("XXXX", Run.satisfaction);
        }

        // Manipulation check for group membership
        else if (formName.Equals("Manipulation"))
        {
            BASE_URL = BASE_URL + "XXXX";

            // ID for the participant
            form.AddField("XXXX", Run.userID);
            // Conditions for this study
            form.AddField("XXXX", Run.conditionGroup.ToString() + Run.conditionError.ToString());
            // Team name
            form.AddField("XXXX", x[0]);
            // Robot name
            form.AddField("XXXX", x[1]);
        }

        else if (formName.Equals("Demographics"))
        {
            BASE_URL = BASE_URL + "XXXX";

            // ID for participant
            form.AddField("XXXX", Run.userID);
            // Age
            form.AddField("XXXX", x[0]);
            // Gender
            form.AddField("XXXX", x[1]);
            // Highest level of education
            form.AddField("XXXX", x[2]);
            // Current industry/major
            form.AddField("XXXX", x[3]);
            // Experience with tech
            form.AddField("XXXX", x[4]);
            // Experience with robot
            form.AddField("XXXX", x[5]);
            // Experience robot teaming
            form.AddField("XXXX", x[6]);
        }

        else if (formName.Equals("Tolerance"))
        {

            BASE_URL = BASE_URL + "XXXX";

            form.AddField("XXXX", Run.userID);
            form.AddField("XXXX", x[0]);
            form.AddField("XXXX", x[1]);
            form.AddField("XXXX", x[2]);
            form.AddField("XXXX", x[3]);
            form.AddField("XXXX", x[4]);
            form.AddField("XXXX", x[5]);
            form.AddField("XXXX", x[6]);
            form.AddField("XXXX", x[7]);
        }
        else if (formName.Equals("pref"))
        {
            BASE_URL = BASE_URL + "XXXX";

            form.AddField("XXXX", Run.userID);
            form.AddField("XXXX", Run.conditionGroup.ToString() + Run.conditionError.ToString());
            form.AddField("XXXX", x[0]);
            form.AddField("XXXX", Run.roundTimes[1]);
            form.AddField("XXXX", Run.roundTimes[2]);
            form.AddField("XXXX", Run.roundTimes[3]);
            form.AddField("XXXX", Run.roundTimes[4]);
            form.AddField("XXXX", Run.roundTimes[5]);
            form.AddField("XXXX", Run.roundTimes[6]);
            form.AddField("XXXX", Run.nonIdleTimes[1]);
            form.AddField("XXXX", Run.nonIdleTimes[2]);
            form.AddField("XXXX", Run.nonIdleTimes[3]);
            form.AddField("XXXX", Run.nonIdleTimes[4]);
            form.AddField("XXXX", Run.nonIdleTimes[5]);
            form.AddField("XXXX", Run.nonIdleTimes[6]);
            form.AddField("XXXX", x[1]);
        }

        // Percieve Capability
        else if (formName.Equals("Capability"))
        {
            BASE_URL = BASE_URL + "XXXX";

            form.AddField("XXXX", Run.userID);
            form.AddField("XXXX", curCondition);
            form.AddField("XXXX", x[0]);
            form.AddField("XXXX", x[1]);
            form.AddField("XXXX", x[2]);
            form.AddField("XXXX", x[3]);
            form.AddField("XXXX", x[4]);
            form.AddField("XXXX", x[5]);
        }

        else if (formName.Equals("Trust"))
        {
            BASE_URL = BASE_URL + "XXXX";

            form.AddField("XXXX", Run.userID);
            form.AddField("XXXX", curCondition);
            form.AddField("XXXX", x[0]);
            form.AddField("XXXX", x[1]);
            form.AddField("XXXX", x[2]);
            form.AddField("XXXX", x[3]);
            form.AddField("XXXX", x[4]);
        }

        else if (formName.Equals("Teamwork"))
        {
            BASE_URL = BASE_URL + "XXXX";

            form.AddField("XXXX", Run.userID);
            form.AddField("XXXX", curCondition);
            form.AddField("XXXX", x[0]);
            form.AddField("XXXX", x[1]);
            form.AddField("XXXX", x[2]);
            form.AddField("XXXX", x[3]);
            form.AddField("XXXX", x[4]);
            form.AddField("XXXX", x[5]);
            form.AddField("XXXX", x[6]);
            form.AddField("XXXX", x[7]);
            form.AddField("XXXX", x[8]);
            form.AddField("XXXX", x[9]);
        }

        else if (formName.Equals("Email"))
        {
            BASE_URL = BASE_URL + "XXXX";

            form.AddField("XXXX", x[0]);
            form.AddField("XXXX", x[2]);
            form.AddField("XXXX", x[3]);

        }

        else if (formName.Equals("Item"))
        {
            calibPoint.ingred = x[0];
        }

        else if (formName.Equals("Error"))
        {
            BASE_URL = BASE_URL + "XXXX";

            form.AddField("XXXX", Run.userID);
            form.AddField("XXXX", curCondition);
            form.AddField("XXXX", x[0]);
            form.AddField("XXXX", x[1]);
            form.AddField("XXXX", x[2]);
            form.AddField("XXXX", x[3]);

            if (Run.round < 8)
            {
                GameObject.Find("Transition Panel").GetComponent<AudioSource>().enabled = true;
            }
        }

        if (!formName.Equals("Item"))
        {
            UnityWebRequest www = UnityWebRequest.Post(BASE_URL, form);
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
        BASE_URL = "https://docs.google.com/forms/d/e/";
    }
}