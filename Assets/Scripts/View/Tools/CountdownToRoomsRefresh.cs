using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownToRoomsRefresh : MonoBehaviour {

    static float coundownStarted;
    static int currentCount;

    int countdownStart = 30;
    public bool isActive;
    Text countdownText;

	// Use this for initialization
	public void Start()
    {
        isActive = true;
        countdownText = transform.GetComponent<Text>();

        Reset();
    }

    public static void Reset()
    {
        coundownStarted = Time.time;
        currentCount = 0;
    }
	
	// Update is called once per frame
	void Update()
    {
		if (Time.time > coundownStarted + currentCount)
        {
            currentCount++;
            countdownText.text = (countdownStart - currentCount).ToString();
            if (currentCount == countdownStart)
            {
                isActive = false;
                MainMenu.CurrentMainMenu.BrowseMatches();
            }
        }
	}
}
