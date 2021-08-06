using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timerText;
    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float time = Time.time - startTime;
        string minutes = ((int)time / 60).ToString("00");
        string seconds = (time % 60).ToString("00.00");

        timerText.text = minutes + ":" + seconds;
    }
}
