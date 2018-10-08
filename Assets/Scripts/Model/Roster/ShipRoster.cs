﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Players;
using Ship;
using SquadBuilderNS;
using System;
using GameCommands;

public static partial class Roster
{
    //Players

    public static List<GenericPlayer> Players;

    public static GenericPlayer Player1 { get { return Players.Find(n => n.PlayerNo == PlayerNo.Player1); } }
    public static GenericPlayer Player2 { get { return Players.Find(n => n.PlayerNo == PlayerNo.Player2); } }

    //Ships

    public static Dictionary<string, GenericShip> AllShips;

    public static Dictionary<string, GenericShip> ShipsPlayer1 { get { return Player1.Ships; } }
    public static Dictionary<string, GenericShip> ShipsPlayer2 {get { return Player2.Ships; } }

    public static List<GenericShip> Reserve;

    // SQUADRONS

    private static void PrepareSquadrons()
    {
        if (ReplaysManager.Mode == ReplaysMode.Write)
        {
            foreach (var squad in SquadBuilder.SquadLists)
            {
                JSONObject parameters = new JSONObject();
                parameters.AddField("player", squad.PlayerNo.ToString());
                parameters.AddField("type", squad.PlayerType.ToString());

                squad.SavedConfiguration["description"].str = squad.SavedConfiguration["description"].str.Replace("\n", "");
                parameters.AddField("list", squad.SavedConfiguration);

                GameController.SendCommand(
                    GameCommandTypes.SquadsSync,
                    null,
                    parameters.ToString()
                );

                Console.Write("Command is executed: " + GameCommandTypes.SquadsSync, LogTypes.GameCommands, true, "aqua");
                GameController.GetCommand().Execute();
            };
        }
        else if (ReplaysManager.Mode == ReplaysMode.Read)
        {
            for (int i = 0; i < 2; i++)
            {
                GameCommand command = GameController.GetCommand();
                if (command.Type == GameCommandTypes.SquadsSync)
                {
                    Console.Write("Command is executed: " + command.Type, LogTypes.GameCommands, true, "aqua");
                    command.Execute();
                }
            }
        }

    }

    //PLAYERS CREATION

    private static void CreatePlayers()
    {
        foreach (var squadList in SquadBuilder.SquadLists)
        {
            Type playerType = squadList.PlayerType;

            bool isHotacAi = (playerType == typeof(HotacAiPlayer));

            if (ReplaysManager.Mode == ReplaysMode.Read)
            {
                playerType = typeof(ReplayPlayer);
            }

            GenericPlayer player = CreatePlayer(playerType, squadList.PlayerNo);
            player.UsesHotacAiRules = isHotacAi;

            Players.Add(player);
        }
    }

    private static GenericPlayer CreatePlayer(System.Type type, PlayerNo playerNo)
    {
        GenericPlayer player = (GenericPlayer) System.Activator.CreateInstance(type);
        player.SetPlayerNo(playerNo);
        return player;
    }

    //SHIP CREATION

    private static void SpawnAllShips()
    {
        foreach (var squadList in SquadBuilder.SquadLists)
        {
            SquadBuilder.SetPlayerSquadFromImportedJson(squadList.Name, squadList.SavedConfiguration, squadList.PlayerNo, delegate { });

            if (Roster.GetPlayer(squadList.PlayerNo).GetType() != typeof(HotacAiPlayer))
            {
                JSONObject playerInfo = squadList.SavedConfiguration.GetField("PlayerInfo");
                Roster.GetPlayer(squadList.PlayerNo).NickName = playerInfo.GetField("NickName").str;
                Roster.GetPlayer(squadList.PlayerNo).Title = playerInfo.GetField("Title").str;
                Roster.GetPlayer(squadList.PlayerNo).Avatar = playerInfo.GetField("Avatar").str;
            }

            Roster.GetPlayer(squadList.PlayerNo).SquadCost = squadList.Points;
        }

        // Keep order, ships must have same ID on both clients
        ShipFactory.Initialize();
        foreach (SquadBuilderShip shipConfig in SquadBuilder.GetSquadList(PlayerNo.Player1).GetShips())
        {
            GenericShip newShip = ShipFactory.SpawnShip(shipConfig);
            AddShipToLists(newShip);
        }
        foreach (SquadBuilderShip shipConfig in SquadBuilder.GetSquadList(PlayerNo.Player2).GetShips())
        {
            GenericShip newShip = ShipFactory.SpawnShip(shipConfig);
            AddShipToLists(newShip);
        }

        BoardTools.Board.SetShips();
    }

    private static void AddShipToLists(GenericShip newShip)
    {
        AllShips.Add(newShip.GetTag(), newShip);
        newShip.Owner.Ships.Add(newShip.GetTag(), newShip);
    }

    //SHIP DESTRUCTION

    public static void DestroyShip(string id)
    {
        HideShip(id);

        OrganizeRosters();
    }

    public static void HideShip(string id)
    {
        var ship = GetShipById(id);

        if (ship != null)
        {
            ship.SetActive(false);
            ship.SetPosition(new Vector3(0, -100, 0));
            ship.InfoPanel.SetActive(false);
            ship.Owner.Ships.Remove(id);
            AllShips.Remove(id);
        }
    }

    public static void DockShip(string id)
    {
        var ship = GetShipById(id);

        if (ship != null)
        {
            ship.SetActive(false);
            TogglePanelActive(ship, false);
            ship.SetDockedName(true);
            ship.Owner.Ships.Remove(id);
            AllShips.Remove(id);
        }
    }

    public static void UndockShip(GenericShip ship)
    {
        if (ship != null)
        {
            ship.SetActive(true);
            TogglePanelActive(ship, true);
            ship.SetDockedName(false);
            ship.Owner.Ships.Add("ShipId:" + ship.ShipId, ship);
            AllShips.Add("ShipId:" + ship.ShipId, ship);
        }
    }

    public static void ShowShip(GenericShip ship)
    {
        if (ship != null)
        {
            ship.SetActive(true);
            ship.InfoPanel.SetActive(true);
            ship.Owner.Ships.Add("ShipId:" + ship.ShipId, ship);
            AllShips.Add("ShipId:" + ship.ShipId, ship);
        }
    }

    //TOOLS

    public static GenericShip GetShipById(string id)
    {
		if (AllShips.Any (x => x.Key == id)) {
			return AllShips[id];
		}

		return null;
    }

    public static GenericPlayer GetPlayer(PlayerNo playerNo)
    {
        return (playerNo == PlayerNo.Player1) ? Roster.Player1 : Roster.Player2;
    }

    public static GenericPlayer GetPlayer(int playerNo)
    {
        return (playerNo == 1) ? Roster.Player1 : Roster.Player2;
    }

    public static int AnotherPlayer(int player)
    {
        return (player == 1) ? 2 : 1;
    }

    public static PlayerNo AnotherPlayer(PlayerNo playerNo)
    {
        return (playerNo == PlayerNo.Player1) ? PlayerNo.Player2 : PlayerNo.Player1;
    }

    //FIND SHIPS BY REQUEST

    public static Dictionary<string, GenericShip> ListSamePlayerAndPilotSkill(PlayerNo playerNo, int pilotSkill)
    {
        var results =
            from n in AllShips
            where n.Value.Owner.PlayerNo == playerNo
            where n.Value.PilotSkill == pilotSkill
            select n;

        return results.ToDictionary(t => t.Key, t => t.Value);
    }

    public static Dictionary<int, int> ListAnotherPlayerButSamePilotSkill(int previousPilotSkill, int PilotSkillSubPhasePlayer)
    {
        var results =
            from n in AllShips
            where n.Value.PilotSkill == previousPilotSkill
            where n.Value.Owner.Id != PilotSkillSubPhasePlayer
            select n;

        return results.ToDictionary(t => previousPilotSkill, t => t.Value.Owner.Id);
    }

    // CHECK ALL SHIPS IN ROSTER

    public static bool AllManuversAreAssigned(PlayerNo playerNo)
    {
        var results =
            from n in AllShips
            where n.Value.Owner.PlayerNo == playerNo
            where n.Value.AssignedManeuver == null && !RulesList.IonizationRule.IsIonized(n.Value)
            select n;

        //if (results.Count() > 0) Game.UI.ShowError("Not all ship are assigned their maneuvers");
        return (results.Count() == 0);
    }

    public static bool AllManueversArePerformed()
    {
        var results =
            from n in AllShips
            where n.Value.IsManeuverPerformed == false
            select n;

        if (results.Count() > 0) Messages.ShowErrorToHuman("Not all ship executed their maneuvers");
        return (results.Count() == 0);
    }

    public static bool NoSamePlayerAndPilotSkillNotAttacked()
    {
        Dictionary<string, GenericShip> samePlayerAndPilotSkill = ListSamePlayerAndPilotSkill(Phases.CurrentSubPhase.RequiredPlayer, Phases.CurrentSubPhase.RequiredPilotSkill);
        foreach (var item in samePlayerAndPilotSkill)
        {
            if (item.Value.IsAttackPerformed == false)
            {
                return false;
            }
        }
        return true;
    }

    //TODO: Rework

    public static int CheckIsAnyTeamIsEliminated()
    {
        int result = 0;
        if (ShipsPlayer1.Count == 0)
        {
            result += 1;
        }
        if (ShipsPlayer2.Count == 0)
        {
            result += 2;
        }
        return result;
    }

    // TODO: ??? Move to selection

    public static void SetRaycastTargets(bool value)
    {
        foreach (var shipHolder in AllShips)
        {
            shipHolder.Value.SetRaycastTarget(value);
        }
    }

    // NEW

    public static void HighlightShipsFiltered(Func<GenericShip, bool> filter)
    {
        AllShipsHighlightOff();

        foreach (GenericShip ship in Roster.AllShips.Values)
        {
            if (filter(ship))
            {
                RosterPanelHighlightOn(ship);
            }
        }
    }

    public static void AllShipsHighlightOff()
    {
        RosterAllPanelsHighlightOff();
        foreach (var ship in AllShips)
        {
            ship.Value.HighlightCanBeSelectedOff();
        }
    }

    public static void HighlightShipOff(GenericShip ship)
    {
        ship.HighlightCanBeSelectedOff();
        RosterPanelHighlightOff(ship);
    }

    // RESERVE

    public static void MoveToReserve(GenericShip ship)
    {
        ship.SetActive(false);
        TogglePanelActive(ship, false);
        ship.SetInReserveName(true);

        AllShips.Remove("ShipId:" + ship.ShipId);
        ship.Owner.Ships.Remove("ShipId:" + ship.ShipId);

        Reserve.Add(ship);
    }

    public static void ReturnFromReserve(GenericShip ship)
    {
        ship.SetActive(true);
        TogglePanelActive(ship, true);
        ship.SetInReserveName(false);

        AllShips.Add("ShipId:" + ship.ShipId, ship);
        ship.Owner.Ships.Add("ShipId:" + ship.ShipId, ship);

        Reserve.Remove(ship);
    }

}
