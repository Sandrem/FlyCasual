using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModes;
using BoardTools;
using Bombs;
using Obstacles;

public delegate void CallBackFunction();

public class GameManagerScript : MonoBehaviour {

    public PrefabsList PrefabsList;

    public UI UI;
    public ShipMovementScript Movement;

    void Start()
    {
        SetApplicationParameters();
        InitializeScripts();

        //Global.Initialize();

        Phases.Initialize();
        Rules.Initialize();
        Board.Initialize();
        Roster.Initialize();
        Selection.Initialize();
        BombsManager.Initialize();
        Actions.Initialize();
        Combat.Initialize();
        Triggers.Initialize();
        DamageDecks.Initialize();
        new ObstaclesManager();

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
            if (!Console.IsActive) UI.GoNextShortcut();
        }

        if (Phases.CurrentSubPhase != null) Phases.CurrentSubPhase.Update();

        //TestShotDistance();
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

    private void TestShotDistance()
    {
        Ship.GenericShip ship1 = Roster.GetShipById("ShipId:1");
        Ship.GenericShip ship2 = Roster.GetShipById("ShipId:2");
        ShotInfo shotInfo = new ShotInfo(ship1, ship2, ship1.PrimaryWeapon);
        if (shotInfo.IsShotAvailable) MovementTemplates.ShowRangeRuler(shotInfo.MinDistance); else MovementTemplates.ReturnRangeRuler();
    }

}
