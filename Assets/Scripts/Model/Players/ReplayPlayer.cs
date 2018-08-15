using ActionsList;
using GameCommands;
using GameModes;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Players
{

    public partial class ReplayPlayer : GenericPlayer
    {
        public ReplayPlayer() : base()
        {
            Type = PlayerType.Replay;
            Name = "Replay";
        }

        public override void TakeDecision()
        {
            GameCommand command = GameController.GetCommand();
            if (command == null) return;

            if (command.Type == GameCommandTypes.Decision && Phases.CurrentSubPhase.GetType() == command.SubPhase && Phases.CurrentSubPhase.IsReadyForCommands)
            {
                Console.Write("Replay: Saved Decision is executed", color: "aqua");
                GameController.ConfirmCommand();

                DecisionSubPhase.ExecuteDecision(command.GetString("name"));
            }
            else
            {
                Console.Write("Replay: No saved Decision", color: "aqua");
            }
        }

        public override void PlaceObstacle()
        {
            base.PlaceObstacle();

            GameCommand command = GameController.GetCommand();
            if (command == null) return;

            if (command.Type == GameCommandTypes.ObstaclePlacement && Phases.CurrentSubPhase.GetType() == command.SubPhase && Phases.CurrentSubPhase.IsReadyForCommands)
            {
                Console.Write("Replay: Obstacle Placement is executed", color: "aqua");
                GameController.ConfirmCommand();

                ObstaclesPlacementSubPhase.PlaceObstacle(
                    command.GetString("name"),
                    new Vector3(command.GetFloat("positionX"), command.GetFloat("positionY"), command.GetFloat("positionZ")),
                    new Vector3(command.GetFloat("rotationX"), command.GetFloat("rotationY"), command.GetFloat("rotationZ"))
                );
            }
            else
            {
                Console.Write("Replay: No saved Obstacle Placement", color: "aqua");
            }
        }

        public override void SetupShip()
        {
            base.SetupShip();

            GameCommand command = GameController.GetCommand();
            if (command == null) return;

            if (command.Type == GameCommandTypes.ShipPlacement && Phases.CurrentSubPhase.GetType() == command.SubPhase && Phases.CurrentSubPhase.IsReadyForCommands)
            {
                Console.Write("Replay: Ship Setup is executed", color: "aqua");
                GameController.ConfirmCommand();

                SetupSubPhase.PlaceShip(
                    int.Parse(command.GetString("id")),
                    new Vector3(command.GetFloat("positionX"), command.GetFloat("positionY"), command.GetFloat("positionZ")),
                    new Vector3(command.GetFloat("rotationX"), command.GetFloat("rotationY"), command.GetFloat("rotationZ"))
                );
            }
            else
            {
                Console.Write("Replay: No saved Ship Setup", color: "aqua");
            }
            
        }

        public override void AssignManeuver()
        {
            base.AssignManeuver();

            GameCommand command = GameController.GetCommand();
            if (command == null) return;

            if (command.Type == GameCommandTypes.AssignManeuver && Phases.CurrentSubPhase.GetType() == command.SubPhase && Phases.CurrentSubPhase.IsReadyForCommands)
            {
                Console.Write("Replay: Assign Maneuver is executed", color: "aqua");
                GameController.ConfirmCommand();

                ShipMovementScript.AssignManeuver(int.Parse(command.GetString("id")), command.GetString("maneuver"));

                //Check next commands
                GameCommand nextCommand = GameController.GetCommand();
                if (nextCommand != null && nextCommand.SubPhase == command.SubPhase) GameController.Next();
            }
            else
            {
                Console.Write("Replay: No saved Assign Maneuver", color: "aqua");
            }
        }

        public override void PressNext()
        {
            GameCommand command = GameController.GetCommand();
            if (command == null) return;

            if (command.Type == GameCommandTypes.PressNext && Phases.CurrentSubPhase.GetType() == command.SubPhase && Phases.CurrentSubPhase.IsReadyForCommands)
            {
                Console.Write("Replay: Press Next is executed", color: "aqua");
                GameController.ConfirmCommand();

                UI.SendNextButtonCommand();

                //Check next commands
                GameCommand nextCommand = GameController.GetCommand();
                if (nextCommand != null && nextCommand.SubPhase == command.SubPhase && Phases.CurrentSubPhase.IsReadyForCommands) GameController.Next();
            }
            else
            {
                Console.Write("Replay: No saved Press Next", color: "aqua");
            }
        }

        public override void PressSkip()
        {
            GameCommand command = GameController.GetCommand();
            if (command == null) return;

            if (command.Type == GameCommandTypes.PressSkip && Phases.CurrentSubPhase.GetType() == command.SubPhase && Phases.CurrentSubPhase.IsReadyForCommands)
            {
                Console.Write("Replay: Press Skip is executed", color: "aqua");
                GameController.ConfirmCommand();

                UI.SendSkipButtonCommand();
            }
            else
            {
                Console.Write("Replay: No saved Press Skip", color: "aqua");
            }
        }

        public override void PerformManeuver()
        {
            base.PerformManeuver();

            GameCommand command = GameController.GetCommand();
            if (command == null) return;

            if (command.Type == GameCommandTypes.ActivateAndMove && Phases.CurrentSubPhase.GetType() == command.SubPhase && Phases.CurrentSubPhase.IsReadyForCommands)
            {
                Console.Write("Replay: Activate Ship Movement is executed", color: "aqua");
                GameController.ConfirmCommand();

                Selection.ChangeActiveShip("ShipId:" + int.Parse(command.GetString("id")));
                ShipMovementScript.ActivateAndMove(int.Parse(command.GetString("id")));
            }
            else
            {
                Console.Write("Replay: No saved Activate Ship Movement", color: "aqua");
            }

            if (command.Type == GameCommandTypes.AssignManeuver && Phases.CurrentSubPhase.GetType() == command.SubPhase && Phases.CurrentSubPhase.IsReadyForCommands)
            {
                Console.Write("Replay: Assign Maneuver is executed", color: "aqua");
                GameController.ConfirmCommand();

                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Assign HotAC AI maneuver",
                    TriggerType = TriggerTypes.OnAbilityDirect,
                    TriggerOwner = this.PlayerNo,
                    EventHandler = delegate
                    {
                        ShipMovementScript.AssignManeuver(int.Parse(command.GetString("id")), command.GetString("maneuver"));
                    }
                });

                Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, delegate {
                    //Check next commands
                    GameCommand nextCommand = GameController.GetCommand();
                    if (nextCommand != null && nextCommand.SubPhase == command.SubPhase) GameController.Next();
                });
            }
            else
            {
                Console.Write("Replay: No saved Assign Maneuver", color: "aqua");
            }
        }

        public override void PerformAttack()
        {
            base.PerformManeuver();

            GameCommand command = GameController.GetCommand();
            if (command == null) return;

            if (command.Type == GameCommandTypes.DeclareAttack && Phases.CurrentSubPhase.GetType() == command.SubPhase && Phases.CurrentSubPhase.IsReadyForCommands)
            {
                Console.Write("Replay: Declare Attack is executed", color: "aqua");
                GameController.ConfirmCommand();

                Combat.DeclareIntentToAttack(
                    int.Parse(command.GetString("id")),
                    int.Parse(command.GetString("target")),
                    bool.Parse(command.GetString("weaponIsAlreadySelected"))
                );
            }
            else
            {
                Console.Write("Replay: No saved Declare Attack", color: "aqua");
            }

            if (command.Type == GameCommandTypes.PressSkip && Phases.CurrentSubPhase.GetType() == command.SubPhase && Phases.CurrentSubPhase.IsReadyForCommands)
            {
                Console.Write("Replay: Press Skip is executed", color: "aqua");
                GameController.ConfirmCommand();

                UI.SendSkipButtonCommand();
            }
            else
            {
                Console.Write("Replay: No saved Press Skip", color: "aqua");
            }
        }

        public override void UseDiceModifications(DiceModificationTimingType type)
        {
            base.UseDiceModifications(type);
            Combat.ShowDiceModificationButtons(type);

            GameCommand command = GameController.GetCommand();
            if (command == null) return;

            if (command.Type == GameCommandTypes.DiceModification && Phases.CurrentSubPhase.GetType() == command.SubPhase && Phases.CurrentSubPhase.IsReadyForCommands)
            {
                Console.Write("Replay: Dice Modification is executed", color: "aqua");
                GameController.ConfirmCommand();

                string diceModificationName = command.GetString("name");
                if (diceModificationName == "OK")
                {
                    Combat.ConfirmDiceResultsClient();
                }
                else
                {
                    Combat.UseDiceModification(diceModificationName);
                }
            }
            else
            {
                Console.Write("Replay: No saved Dice Modification", color: "aqua");
            }
        }

        public override void SelectShipForAbility()
        {
            GameCommand command = GameController.GetCommand();
            if (command == null) return;

            if (command.Type == GameCommandTypes.SelectShip && Phases.CurrentSubPhase.GetType() == command.SubPhase && Phases.CurrentSubPhase.IsReadyForCommands)
            {
                Console.Write("Replay: Select ship is executed", color: "aqua");
                GameController.ConfirmCommand();

                SelectShipSubPhase.SelectShip(int.Parse(command.GetString("id")));

            }
            else
            {
                Console.Write("Replay: No saved Select ship", color: "aqua");
            }
        }

        public override void SyncDiceResults()
        {
            GameCommand command = GameController.GetCommand();
            if (command == null) return;

            if (command.Type == GameCommandTypes.SyncDiceResults && Phases.CurrentSubPhase.GetType() == command.SubPhase && Phases.CurrentSubPhase.IsReadyForCommands)
            {
                Console.Write("Replay: Sync Dice Results is executed", color: "aqua");
                GameController.ConfirmCommand();

                List<DieSide> correctSides = new List<DieSide>();
                JSONObject jsonHolder = (JSONObject)command.GetParameter("sides");
                foreach (var dieInfo in jsonHolder.list)
                {
                    DieSide side = (DieSide)Enum.Parse(typeof(DieSide), dieInfo["side"].str);
                    correctSides.Add(side);
                }

                DiceRoll.SyncDiceResults(correctSides);
            }
            else
            {
                Console.Write("Replay: No saved Sync Dice Results", color: "aqua");
            }
        }

        public override void SyncDiceRerollSelected()
        {
            GameCommand command = GameController.GetCommand();
            if (command == null) return;

            if (command.Type == GameCommandTypes.SyncDiceRerollSelected && Phases.CurrentSubPhase.GetType() == command.SubPhase && Phases.CurrentSubPhase.IsReadyForCommands)
            {
                Console.Write("Replay: Sync Dice Reroll Selected is executed", color: "aqua");
                GameController.ConfirmCommand();

                List<bool> selectedDice = new List<bool>();
                JSONObject jsonHolder = (JSONObject)command.GetParameter("dice");
                foreach (var dieInfo in jsonHolder.list)
                {
                    bool isSelected = bool.Parse(dieInfo["selected"].str);
                    selectedDice.Add(isSelected);
                }

                DiceRerollManager.SyncDiceRerollSelected(selectedDice);
            }
            else
            {
                Console.Write("Replay: No saved Sync Dice Reroll Selected", color: "aqua");
            }
        }

        public override void RerollManagerIsPrepared()
        {
            DiceRerollManager.CurrentDiceRerollManager.ConfirmRerollButtonIsPressed();
        }

        public override void InformAboutCrit()
        {
            GameCommand command = GameController.GetCommand();
            if (command == null) return;

            if (command.Type == GameCommandTypes.ConfirmCrit && Phases.CurrentSubPhase.GetType() == command.SubPhase && Phases.CurrentSubPhase.IsReadyForCommands)
            {
                Console.Write("Replay: Confirm Crit is executed", color: "aqua");
                GameController.ConfirmCommand();

                InformCrit.ConfirmCrit();
            }
            else
            {
                Console.Write("Replay: No saved Confirm Crit", color: "aqua");
            }
        }

        public override void ConfirmDiceCheck()
        {
            GameCommand command = GameController.GetCommand();
            if (command == null) return;

            if (command.Type == GameCommandTypes.ConfirmDiceCheck && Phases.CurrentSubPhase.GetType() == command.SubPhase && Phases.CurrentSubPhase.IsReadyForCommands)
            {
                Console.Write("Replay: Confirm Dice Check is executed", color: "aqua");
                GameController.ConfirmCommand();

                (Phases.CurrentSubPhase as DiceRollCheckSubPhase).Confirm();
            }
            else
            {
                Console.Write("Replay: No saved Confirm Dice Check", color: "aqua");
            }
        }
    }

}
