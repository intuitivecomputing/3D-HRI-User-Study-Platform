using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class voiceWait : MonoBehaviour
{

    public AudioClip audioClip;
    public GameObject[] buttons;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EnableButtonAudio());
    }

    private IEnumerator EnableButtonAudio()
    {
        float duration = audioClip.length;
        //play audioclip
        yield return new WaitForSeconds(duration);
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponent<Button>().interactable = true;
        }
    }
}
