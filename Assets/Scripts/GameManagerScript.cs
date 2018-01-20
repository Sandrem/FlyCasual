using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModes;

public delegate void CallBackFunction();

public class GameManagerScript : MonoBehaviour {

    public PrefabsList PrefabsList;

    public UI UI;
    public ShipMovementScript Movement;
    public ShipPositionManager Position;

    void Start() {
        SetApplicationParameters();
        InitializeScripts();

        //Global.Initialize();

        Board.BoardManager.Initialize();
        Roster.Initialize();
        Roster.Start();
        Selection.Initialize();
        CriticalHitsDeck.InitializeDeck();
        Bombs.BombsManager.Initialize();
        Actions.Initialize();
        Combat.Initialize();
        Triggers.Initialize();

        CheckRemoteSettings();

        GameMode.CurrentGameMode.StartBattle();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UI.ToggleInGameMenu();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            UI.GoNextShortcut();
        }
    }

    private void SetApplicationParameters()
    {
        Application.targetFrameRate = 100;

        Options.UpdateVolume();
    }

    private void InitializeScripts()
    {
        PrefabsList = this.GetComponent<PrefabsList>();
        UI = this.GetComponent<UI>();
        
        Movement = this.GetComponent<ShipMovementScript>();
        Position = this.GetComponent<ShipPositionManager>();
    }

    public void Wait(float seconds, CallBackFunction callBack)
    {
        StartCoroutine(WaitCoroutine(seconds, callBack));
    }

    IEnumerator WaitCoroutine(float seconds, CallBackFunction callBack)
    {
        yield return new WaitForSeconds(seconds);
        callBack();
    }

    private void CheckRemoteSettings()
    {
        bool showNeyYearTree = RemoteSettings.GetBool("ShowNewYearTree", false);
        if (showNeyYearTree)
        {
            GameObject.Find("SceneHolder/Board").transform.Find("NewYearTree").gameObject.SetActive(true);
            GameObject.Find("SceneHolder/Board/CombatDiceHolder").transform.localPosition = new Vector3(73, 0, 0);
            GameObject.Find("SceneHolder/Board/CheckDiceHolder").transform.localPosition = new Vector3(85, 0, 0);
        }
    }

}
