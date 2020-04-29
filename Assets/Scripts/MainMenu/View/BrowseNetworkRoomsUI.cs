using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrowseNetworkRoomsUI : MonoBehaviour
{
    public static BrowseNetworkRoomsUI Instance;

    public GameObject BottomControls;
    public GameObject LoadingMessage;
    public GameObject NoRoomsPanel;

    public GameObject RoomListPanel;
    public GameObject RoomInfoPrefab;

    void Awake()
    {
        Instance = this;
    }

    public void ShowLoading()
    {
        BottomControls.SetActive(false);
        NoRoomsPanel.SetActive(false);
        LoadingMessage.SetActive(true);
    }

    public void ShowRooms()
    {
        BottomControls.SetActive(true);
        NoRoomsPanel.SetActive(false);
        LoadingMessage.SetActive(false);

        ShowListofRooms();
    }

    private void ShowListofRooms()
    {
        int roomsCount = 1;

        RectTransform parentRect = RoomListPanel.transform.GetComponent<RectTransform>();
        parentRect.sizeDelta = new Vector2(parentRect.sizeDelta.x, 120f * (roomsCount) + 20f);

        for (int i = 0; i < roomsCount; i++)
        {
            GameObject newRoom = GameObject.Instantiate(RoomInfoPrefab, RoomListPanel.transform);
            newRoom.name = "Room" + i;

            newRoom.transform.localPosition = new Vector3(newRoom.transform.localPosition.x, -20f * (i + 1) - 100f * i, newRoom.transform.localPosition.z);

            newRoom.GetComponentInChildren<Button>().onClick.AddListener(
                delegate { Network.JoinRoom(null);
            });
        }
    }

    public void ShowNoRooms()
    {
        BottomControls.SetActive(true);
        NoRoomsPanel.SetActive(true);
        LoadingMessage.SetActive(false);

        CountdownToRoomsRefresh.Reset();
    }
}
