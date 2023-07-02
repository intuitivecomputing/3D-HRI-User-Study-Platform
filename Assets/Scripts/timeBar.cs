using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * This class handles the time bar associated with the oven to indicate how
 * much time is left for the pizza to cook. In addition, it dictates the behavior
 * of the time bar for when an item needs to be shaked.
 */

public class timeBar : MonoBehaviour
{
    Image timeIndicator;

    public float timeLimit;

    private float ovenTime = 0;

    private GameObject g;

    // Start is called before the first frame update
    void Start()
    {
        timeIndicator = GetComponent<Image>();
        g = GameObject.Find("Oven Button").transform.GetChild(0).gameObject;
        if (g.activeSelf)
        {
            ovenTime = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (g.activeSelf)
        {
            ovenTime += Time.deltaTime;

            if (ovenTime % 60f < timeLimit)
            {
                timeIndicator.fillAmount = ovenTime / timeLimit;
            }
            else
            {
                ovenTime = 0;
                timeIndicator.fillAmount = ovenTime / timeLimit;
            }
        }
        if (Drag.actTime % 60f < timeLimit && timeIndicator != g.transform.GetChild(0).GetComponent<Image>())
        {
            timeIndicator.fillAmount = Drag.actTime / timeLimit;
        }
        
    }
}
