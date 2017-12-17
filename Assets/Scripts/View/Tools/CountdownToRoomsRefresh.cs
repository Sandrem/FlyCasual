using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownToRoomsRefresh : MonoBehaviour {

    int countdownStart = 30;
    float coundownStarted;
    int currentCount;
    public bool isActive;
    Text countdownText;

	// Use this for initialization
	public void Start()
    {
        coundownStarted = Time.time;
        currentCount = 0;
        isActive = true;
        countdownText = transform.GetComponent<Text>();
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
                Network.BrowseMatches();
            }
        }
	}
}
