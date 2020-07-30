﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using ActionsList;
using GameModes;
using SubPhases;
using GameCommands;
using Obstacles;
using System.Linq;
using Remote;

public enum Faction
{
    None,
    Rebel,
    Imperial,
    Scum,
    Resistance,
    FirstOrder,
    Republic,
    Separatists
}

public enum ForceAlignment
{
    None,
    Light,
    Dark
}

namespace Players
{
    public enum PlayerNo
    {
        Player1,
        Player2,
        PlayerNone
    }

    public enum PlayerType
    {
        Human,
        Ai,
        Network,
        Replay
    }

    public partial class GenericPlayer
    {
        public PlayerType PlayerType;
        public string Name;
        public PlayerNo PlayerNo;
        public bool UsesHotacAiRules;
        public int SquadCost;

        public string NickName;
        public string Title;
        public string Avatar;

        public GameObject PlayerInfoPanel;

        public Dictionary<string, GenericShip> Units = new Dictionary<string, GenericShip>();
        public Dictionary<string, GenericShip> Ships { get { return Units.Where(n => !(n.Value is GenericRemote)).ToDictionary(n => n.Key, m => m.Value); } }
        public Dictionary<string, GenericShip> Remotes { get { return Units.Where(n => n.Value is GenericRemote).ToDictionary(n => n.Key, m => m.Value); } }
        public List<GenericObstacle> ChosenObstacles = new List<GenericObstacle>();
        public Dictionary<string, GenericShip> EnemyShips { get { return AnotherPlayer.Ships; } }
        public GenericPlayer AnotherPlayer { get { return Roster.GetPlayer(Roster.AnotherPlayer(PlayerNo)); } }
        public int Id { get { return (PlayerNo == PlayerNo.Player1) ? 1 : 2; } }

        public virtual void AskAssignManeuver()
        {
            Roster.HighlightPlayer(PlayerNo);
        }

        public void SetPlayerNo(PlayerNo playerNo)
        {
            PlayerNo = playerNo;
        }

        public virtual void SetupShip()
        {
            Roster.HighlightPlayer(PlayerNo);
        }

        public virtual void SetupBomb()
        {
            Roster.HighlightPlayer(PlayerNo);
        }

        public virtual void AssignManeuversStart()
        {
            Roster.HighlightPlayer(PlayerNo);
        }

        public virtual void PerformManeuver()
        {
            Roster.HighlightPlayer(PlayerNo);
        }

        public virtual void PerformAttack()
        {
            Roster.HighlightPlayer(PlayerNo);
        }

        public virtual void UseDiceModifications(DiceModificationTimingType type)
        {
            Roster.HighlightPlayer(PlayerNo);
        }

        public virtual void TakeDecision()
        {
            Roster.HighlightPlayer(PlayerNo);
        }

        public virtual void AfterShipMovementPrediction()
        {
            Selection.ThisShip.AssignedManeuver.LaunchShipMovement();
        }

        public virtual void ConfirmDiceCheck() { }

        public virtual void ToggleCombatDiceResults(bool isActive) { }

        public virtual bool IsNeedToShowManeuver(GenericShip ship) { return false; }

        public virtual void OnTargetNotLegalForAttack()
        {
            // TODOREVERT
            Messages.ShowErrorToHuman("The target is not a legal target for the attacker");

            /*// TODO: Better explanations
            if (!Rules.TargetIsLegalForShot.IsLegal())
            {
                //automatic error messages
            }
            else if (!Combat.ShotInfo.IsShotAvailable)
            {
                Messages.ShowErrorToHuman("Target is outside your firing arc");
            }*/
            // TODOREVERT
            /*else if (Combat.ShotInfo.Range > Combat.ChosenWeapon.MaxRange || Combat.ShotInfo.Range < Combat.ChosenWeapon.MinRange)
            {
                Messages.ShowErrorToHuman("Target is outside your firing range");
            }*/

            //TODO: except non-legal targets, bupmed for example, biggs?
            Roster.HighlightShipsFiltered(FilterShipsToAttack);

            UI.ShowSkipButton();
            UI.HighlightSkipButton();

            if (Phases.CurrentSubPhase is AttackExecutionSubphase)
            {
                (Phases.CurrentSubPhase as AttackExecutionSubphase).RevertSubphase();
            }
            else
            {
                Phases.CurrentSubPhase.IsReadyForCommands = true;
            }
        }

        private bool FilterShipsToAttack(GenericShip ship)
        {
            return ship.Owner.PlayerNo != Phases.CurrentSubPhase.RequiredPlayer;
        }

        public virtual void ChangeManeuver(Action<string> doWithManeuverString, Action callback, Func<string, bool> filter = null) { }

        public virtual void SelectManeuver(Action<string> doWithManeuverString, Action callback, Func<string, bool> filter = null)
        {
            Phases.CurrentSubPhase.IsReadyForCommands = true;
        }

        public virtual void StartExtraAttack() { }

        public virtual void SelectShipForAbility()
        {
            Roster.HighlightPlayer(PlayerNo);
        }

        public virtual void SelectShipsForAbility()
        {
            Roster.HighlightPlayer(PlayerNo);
        }

        public virtual void SelectObstacleForAbility()
        {
            Roster.HighlightPlayer(PlayerNo);
        }

        public virtual void SetupShipMidgame()
        {
            Roster.HighlightPlayer(PlayerNo);
        }

        public virtual void MoveObstacleMidgame()
        {
            Roster.HighlightPlayer(PlayerNo);
        }

        public float AveragePilotSkillOfRemainingShips()
        {
            float pilotSkillValue = 0;
            foreach (GenericShip s in Ships.Values)
            {
                pilotSkillValue += s.State.Initiative;
            }
            return Math.Max(0, pilotSkillValue / Ships.Count);
        }

        public float AveragePilotSkillOfRemainingEnemyShips()
        {
            float pilotSkillValue = 0;
            foreach (GenericShip s in EnemyShips.Values)
            {
                pilotSkillValue += s.State.Initiative;
            }
            return Math.Max(0, pilotSkillValue / Ships.Count);
        }

        public virtual void RerollManagerIsPrepared()
        {
            Phases.CurrentSubPhase.IsReadyForCommands = true;
        }

        public virtual void PerformTractorBeamReposition(GenericShip ship) { }

        public virtual void PlaceObstacle()
        {
            Roster.HighlightPlayer(PlayerNo);
        }

        public virtual void PerformSystemsActivation()
        {
            Roster.HighlightPlayer(PlayerNo);
        }

        public virtual void SyncDiceResults()
        {
            Phases.CurrentSubPhase.IsReadyForCommands = true;
        }

        public virtual void SyncDiceRerollSelected() { }

        public virtual void InformAboutCrit()
        {
            InformCrit.ShowPanelVisible();
            InformCrit.DisableConfirmButton();
        }

        public virtual void DiceCheckConfirm()
        {
            GameController.SendCommand(
                GameCommandTypes.ConfirmDiceCheck,
                Phases.CurrentSubPhase.GetType()
            );
        }
    }

}
