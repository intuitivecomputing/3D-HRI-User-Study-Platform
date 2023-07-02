using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

/*
 * This class imports questions and answers from forms on unity into arrays.
 * These forms on unity need to consist of GameObjects that contains a Text
 * called Question and a GameObject called Answer. This script reads in both
 * InputField questions and ToggleGroup (1 selected box) questions.
 */
public class formBehavior : MonoBehaviour
{
    public delegate void SubmitFormEventDelegate(List<string> x,  string name);
    public static event SubmitFormEventDelegate SubmitFormEvent;

    // Array of GameObjects that contain question groups in form of questions
    // and then answer options
    public GameObject[] questionGroupArr;

    // Name of form that it is supposed to import into
    public string formName;

    // Structure that contains both the questions and answers
    public QAValue[] qaArr;

    public Text prefText;

    // Array list of answers so can store them at the end of the study
    public List<string> qanswerArr;

    // Start is called before the first frame update
    void Start()
    {
        // Initiaze the question answer array
        qaArr = new QAValue[questionGroupArr.Length];
    }

    // Used by submit button on form to get the array of questions and answers.
    // This function doesn't take in any arguments nor returns any. It writes
    // answers to the form in an array and resets all of the questions.
    public void submitAnswer()
    {
        bool isEmpty = false;
        // Check if it is either the preference or demographics form
        if (formName.Equals("pref") || formName.Equals("Demographics"))
        {
            // Iterate through all of the questions
            for (int i = 0; i < qaArr.Length; i++)
            {
                // Populating array with question and answer type by calling importQA function
                qaArr[i] = importQA(questionGroupArr[i]);
                qanswerArr.Add(qaArr[i].Answer);
            }
            // Prevent them from not answering the question
            if (qaArr[0].Answer.Equals("Not Answered") && formName.Equals("pref"))
            {
                qanswerArr.Clear();
            }
            else
            {
                // Once clicking the submit button on the form it sends to
                // writetofile to be logged
                SubmitFormEvent(qanswerArr, formName);
                // Reset the form
                clearForm();
                qanswerArr.Clear();
                if (formName.Equals("pref"))
                {
                    GameObject.Find(formName + " Form").SetActive(false);
                }
            }
        }
        else
        {
            // Iterate through all of the questions
            for (int i = 0; i < qaArr.Length; i++)
            {
                // Populating array with question and answer type by calling importQA function
                qaArr[i] = importQA(questionGroupArr[i]);
                // Check if email entered is the same as the email in the confirmation textbox
                if (formName.Equals("Email") && i > 0 && !qaArr[0].Answer.Equals(qaArr[1].Answer))
                {
                    isEmpty = true;
                    GameObject.Find("Confirm Email").transform.GetChild(0).GetComponent<Text>().color = Color.red;
                    break;
                }
                if (formName.Equals("Error") && i > 0)
                {
                    if (qaArr[0].Answer.Equals("No"))
                    {
                        qanswerArr.Add("");
                        qanswerArr.Add("");
                        qanswerArr.Add("");
                        break;
                    }

                }

                // Check if a question has not been answered and if so change
                // the color of the question text to red
                if (qaArr[i].Answer.Equals("") || qaArr[i].Answer.Equals("Not Answered"))
                {
                    isEmpty = true;
                    questionGroupArr[i].transform.Find("Question").transform.GetComponent<Text>().color = Color.red;
                }
                else
                {
                    questionGroupArr[i].transform.Find("Question").transform.GetComponent<Text>().color = Color.black;
                }
                qanswerArr.Add(qaArr[i].Answer);
            }

            // If all of the answer boxes are not empty then write to log
            if (!isEmpty)
            {
                if (formName.Equals("Name"))
                {
                    Run.robotName = Run.roboC == Color.blue ? "Ocean" : "Sun";
                    prefText.text = Run.robotName;
                }
                else if (formName.Equals("Tolerance"))
                {
                    GameObject.Find("Canvas").transform.GetChild(29).gameObject.SetActive(true);
                }
                // After statisfaction form has been submitted switch view to main
                // game view from training view
                else if (formName.Equals("Satisfaction"))
                {
                    Run.satisfaction = qanswerArr[0];
                    GameObject.Find("Camera").GetComponent<Camera>().enabled = true;
                    GameObject.Find("trainCamera").GetComponent<Camera>().enabled = true;
                    GameObject.Find("Training").transform.GetChild(1).gameObject.SetActive(true);
                    GameObject.Find("DoneButton").GetComponent<Button>().interactable = true;
                    GameObject.Find("Game").transform.GetChild(3).gameObject.SetActive(true);
                    GameObject.Find("Canvas").transform.GetChild(0).gameObject.SetActive(true);
                    GameObject.Find("Game").transform.GetChild(14).gameObject.SetActive(false);
                }
                // Log all of the answers and questions
                SubmitFormEvent(qanswerArr, formName);
                clearForm();
                qanswerArr.Clear();
                GameObject.Find(formName + " Form").SetActive(false);
            }
            else
            {
                qanswerArr.Clear();
            }
        }
    }

    // This function clears all of the answers from the form to be used in
    // subsequent rounds
    public void clearForm()
    {
        for (int i = 0; i < qaArr.Length; i++)
        {
            // Creating instance of one question group
            GameObject curGroup = questionGroupArr[i].transform.Find("Answer").gameObject;
            // Check if current questionGroup is of Toggle type
            if (curGroup.GetComponent<ToggleGroup>() != null)
            {
                // Iterate through the toggles to reset them after clicking submit
                for (int j = 0; j < curGroup.transform.childCount; j++)
                {
                    if (j+1 == curGroup.transform.childCount)
                    {
                        curGroup.transform.GetChild(j).GetComponent<Toggle>().isOn = true;
                    }
                    else
                    {
                        curGroup.transform.GetChild(j).GetComponent<Toggle>().isOn = false;
                    }
                }
            }

            // Check if current question consists of an input field and reset.
            else if (curGroup.GetComponent<InputField>() != null)
            {
                curGroup.GetComponent<InputField>().text = "";
            }
        }
    }

    // Takes a question group, consisting of question and answer and separates
    // them out. It then writes them into a qavalue object to return. This
    // function takes in a question group.
    QAValue importQA(GameObject questionGroup)
    {
        // Instance of object that contains string question and answer
        QAValue result = new QAValue();

        // Look for GameObject question in imported questionGroup and set
        GameObject q = questionGroup.transform.Find("Question").gameObject;
        // Look for GameObject answer in imported questionGroup and set
        GameObject a = questionGroup.transform.Find("Answer").gameObject;

        // Set value of result part of question
        result.Question = q.GetComponent<Text>().text;

        // Check to see if the question group is of toggle type
        if (a.GetComponent<ToggleGroup>() != null)
        {
            // Iterate through the toggles of the toggle group
            for (int i = 0; i < a.transform.childCount; i++)
            {
                // Check to see if particular toggle is on
                if (a.transform.GetChild(i).GetComponent<Toggle>().isOn)
                {
                    // Set the answer of the result to the one toggle value
                    result.Answer = a.transform.GetChild(i).Find("Label").GetComponent<Text>().text;
                    break;
                }
            }
        }

        // Check to see if the answer type is an input field
        else if (a.GetComponent<InputField>() != null)
        {
            // Set the answer to the value of the input field
            result.Answer = a.transform.Find("Text").GetComponent<Text>().text;
        }
        return result;
    }
}

// Form object that stores question and answer for each question group
[System.Serializable]
public class QAValue
{
    public string Question = "";
    public string Answer = "";
}
