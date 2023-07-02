using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * The class handles camera switches during the rounds of the study to toggle
 * amoung the training area, robot selection area, and actual study area.
 */

public class TeamSelection : MonoBehaviour
{
    public Camera mainCamera;
    public Camera selectionCamera;
    public Camera trainingCamera;

    public void ShowMainCamera()
    {
        mainCamera.enabled = true;
        selectionCamera.enabled = false;
        trainingCamera.enabled = false;
    }

    public void ShowSelectionCamera()
    {
        mainCamera.enabled = false;
        selectionCamera.enabled = true;
        trainingCamera.enabled = false;
    }

    public void ShowTrainCamera()
    {
        mainCamera.enabled = false;
        selectionCamera.enabled = false;
        trainingCamera.enabled = true;
    }
}
