using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using ActionsList;
using GameModes;
using SubPhases;
using GameCommands;

public enum Faction
{
    None,
    Rebel,
    Imperial,
    Scum
}

public enum SubFaction
{
    None,
    RebelAlliance,
    Resistance,
    GalacticEmpire,
    FirstOrder,
    ScumAndVillainy
}

namespace Players
{
    public enum PlayerNo
    {
        Player1,
        Player2
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
        public PlayerType Type;
        public string Name;
        public PlayerNo PlayerNo;
        public bool UsesHotacAiRules;
        public int SquadCost;

        public string NickName;
        public string Title;
        public string Avatar;

        public GameObject PlayerInfoPanel;

        public Dictionary<string, GenericShip> Ships = new Dictionary<string, GenericShip>();

        public Dictionary<string, GenericShip> EnemyShips
        {
            get
            {
                return AnotherPlayer.Ships;
            }
        }

        public GenericPlayer AnotherPlayer
        {
            get
            {
                return Roster.GetPlayer(Roster.AnotherPlayer(PlayerNo));
            }
        }

        public int Id { get { return (PlayerNo == PlayerNo.Player1) ? 1 : 2; } }

        public void SetPlayerNo(PlayerNo playerNo)
        {
            PlayerNo = playerNo;
        }

        public virtual void SetupShip()
        {
            Roster.HighlightPlayer(PlayerNo);
            GameController.CheckExistingCommands();
        }

        public virtual void AssignManeuver()
        {
            Roster.HighlightPlayer(PlayerNo);
            GameController.CheckExistingCommands();
        }

        public virtual void PerformManeuver()
        {
            Roster.HighlightPlayer(PlayerNo);
            GameController.CheckExistingCommands();
        }

        public virtual void PerformAttack()
        {
            Roster.HighlightPlayer(PlayerNo);
            GameController.CheckExistingCommands();
        }

        public virtual void UseDiceModifications(DiceModificationTimingType type)
        {
            Roster.HighlightPlayer(PlayerNo);
            Combat.ShowDiceModificationButtons(type);
            GameController.CheckExistingCommands();
        }

        public virtual void TakeDecision()
        {
            Roster.HighlightPlayer(PlayerNo);
            GameController.CheckExistingCommands();
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
            // TODO: Better explanations
            if (!Rules.TargetIsLegalForShot.IsLegal())
            {
                //automatic error messages
            }
            else if (!Combat.ShotInfo.IsShotAvailable)
            {
                Messages.ShowErrorToHuman("Target is outside your firing arc");
            }
            else if (Combat.ShotInfo.Range > Combat.ChosenWeapon.MaxRange || Combat.ShotInfo.Range < Combat.ChosenWeapon.MinRange)
            {
                Messages.ShowErrorToHuman("Target is outside your firing range");
            }

            //TODO: except non-legal targets, bupmed for example, biggs?
            Roster.HighlightShipsFiltered(FilterShipsToAttack);

            UI.ShowSkipButton();
            UI.HighlightNextButton();

            if (Phases.CurrentSubPhase is ExtraAttackSubPhase)
            {
                (Phases.CurrentSubPhase as ExtraAttackSubPhase).RevertSubphase();
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

        public virtual void ChangeManeuver(Action<string> callback, Func<string, bool> filter = null) { }

        public virtual void SelectManeuver(Action<string> callback, Func<string, bool> filter = null)
        {
            Phases.CurrentSubPhase.IsReadyForCommands = true;
        }

        public virtual void StartExtraAttack() { }

        public virtual void SelectShipForAbility()
        {
            Roster.HighlightPlayer(PlayerNo);
            GameController.CheckExistingCommands();
        }

        public virtual void SelectObstacleForAbility()
        {
            Roster.HighlightPlayer(PlayerNo);
            GameController.CheckExistingCommands();
        }

        public float AveragePilotSkillOfRemainingShips()
        {
            float pilotSkillValue = 0;
            foreach (GenericShip s in Ships.Values)
            {
                pilotSkillValue += s.PilotSkill;
            }
            return Math.Max(0, pilotSkillValue / Ships.Count);
        }

        public float AveragePilotSkillOfRemainingEnemyShips()
        {
            float pilotSkillValue = 0;
            foreach (GenericShip s in EnemyShips.Values)
            {
                pilotSkillValue += s.PilotSkill;
            }
            return Math.Max(0, pilotSkillValue / Ships.Count);
        }

        public virtual void RerollManagerIsPrepared() { }

        public virtual void PerformTractorBeamReposition(GenericShip ship) { }

        public virtual void PlaceObstacle()
        {
            Roster.HighlightPlayer(PlayerNo);
            GameController.CheckExistingCommands();
        }

        public virtual void PerformSystemsActivation()
        {
            Roster.HighlightPlayer(PlayerNo);
            GameController.CheckExistingCommands();
        }

        public virtual void SyncDiceResults()
        {
            GameController.CheckExistingCommands();
        }

        public virtual void SyncDiceRerollSelected()
        {
            JSONObject[] diceRerollSelectedArray = new JSONObject[DiceRoll.CurrentDiceRoll.DiceList.Count];
            for (int i = 0; i < DiceRoll.CurrentDiceRoll.DiceList.Count; i++)
            {
                bool isSelected = DiceRoll.CurrentDiceRoll.DiceList[i].IsSelected;
                string isSelectedText = isSelected.ToString();
                JSONObject isSelectedJson = new JSONObject();
                isSelectedJson.AddField("selected", isSelectedText);
                diceRerollSelectedArray[i] = isSelectedJson;
            }
            JSONObject diceRerollSelected = new JSONObject(diceRerollSelectedArray);
            JSONObject parameters = new JSONObject();
            parameters.AddField("dice", diceRerollSelected);

            GameController.SendCommand(
                GameCommandTypes.SyncDiceRerollSelected,
                Phases.CurrentSubPhase.GetType(),
                parameters.ToString()
            );
        }

        public virtual void InformAboutCrit()
        {
            InformCrit.ShowPanelVisible();
            InformCrit.DisableConfirmButton();

            GameController.CheckExistingCommands();
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
