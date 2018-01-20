using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeCounter : MonoBehaviour {

    private float timeStarted;
    private float lastTime;
    Text timePassedText;

	void Start()
    {
        timeStarted = Time.time;
        timePassedText = transform.GetComponent<Text>();
    }
	
	void Update()
    {
        if (Time.time > lastTime + 1)
        {
            timePassedText.text = FormatTime();
            lastTime = Mathf.Floor(Time.time);
        }
    }

    private string FormatTime()
    {
        string timeString;
        int timePassed = (int) (Time.time - timeStarted);
        int minutes = timePassed / 60;
        int seconds = timePassed - (minutes * 60);
        timeString = string.Format("{0:00}:{1:00}", minutes, seconds);
        return timeString;
    }

}
