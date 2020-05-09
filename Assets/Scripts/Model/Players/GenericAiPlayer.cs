using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Ship;
using ActionsList;
using BoardTools;
using SubPhases;
using GameModes;
using GameCommands;

namespace Players
{

    public partial class GenericAiPlayer : GenericPlayer
    {

        public GenericAiPlayer() : base()
        {
            PlayerType = PlayerType.Ai;
            Name = "AI";
        }

        public override void SetupShip()
        {
            base.SetupShip();

            foreach (var shipHolder in Ships)
            {
                if (!shipHolder.Value.IsSetupPerformed && shipHolder.Value.State.Initiative == Phases.CurrentSubPhase.RequiredInitiative)
                {
                    Selection.ChangeActiveShip(shipHolder.Value);

                    int direction = (Phases.CurrentSubPhase.RequiredPlayer == PlayerNo.Player1) ? -1 : 1;
                    Vector3 position = shipHolder.Value.GetPosition() - direction * new Vector3(0, 0, Board.BoardIntoWorld(Board.DISTANCE_1 + Board.RANGE_1));

                    GameManagerScript.Wait(
                        0.5f,
                        delegate {
                            GameCommand command = SetupSubPhase.GeneratePlaceShipCommand(shipHolder.Value.ShipId, position, shipHolder.Value.GetAngles());
                            GameMode.CurrentGameMode.ExecuteCommand(command);
                        }
                    );
                    return;
                }
            }

            Phases.Next();
        }

        public override void PerformManeuver()
        {
            base.PerformManeuver();

            bool foundToActivate = false;
            foreach (var shipHolder in Roster.GetPlayer(Phases.CurrentPhasePlayer).Ships)
            {
                if (shipHolder.Value.State.Initiative == Phases.CurrentSubPhase.RequiredInitiative)
                {
                    if (!shipHolder.Value.IsManeuverPerformed)
                    {
                        foundToActivate = true;
                        Selection.ChangeActiveShip("ShipId:" + shipHolder.Value.ShipId);
                        ActivateShip(shipHolder.Value);
                        break;
                    }
                }
            }

            if (!foundToActivate)
            {
                Phases.Next();
            }
        }

        public virtual void ActivateShip(GenericShip ship)
        {
            Selection.ChangeActiveShip("ShipId:" + ship.ShipId);
            PerformManeuverOfShip(ship);
        }

        protected void PerformManeuverOfShip(GenericShip ship)
        {
            ship.IsManeuverPerformed = true;
            GameCommand command = ShipMovementScript.GenerateActivateAndMoveCommand(Selection.ThisShip.ShipId);
            GameMode.CurrentGameMode.ExecuteCommand(command);
        }

        //TODO: Don't skip attack of all PS ships if one cannot attack (Biggs interaction)

        public override void PerformAttack()
        {
            base.PerformAttack();

            Console.Write("AI is going to perform attack", LogTypes.AI);

            GenericShip attacker = GetShipThatCanAttack();

            if (attacker != null)
            {
                GameMode.CurrentGameMode.ExecuteCommand(
                    CombatSubPhase.GenerateCombatActivationCommand(attacker.ShipId)
                );
            }
            else
            {
                Debug.Log("AI cannot find ship to activate");
            }
        }

        private void PerformAttackContinue()
        {
            if (Selection.ThisShip != null)
            {
                GenericShip targetForAttack = SelectTargetForAttack();

                Selection.ThisShip.IsAttackPerformed = true;

                if (targetForAttack != null)
                {
                    Selection.ThisShip.IsAttackPerformed = true;

                    Console.Write(Selection.ThisShip.PilotName + " attacks target " + targetForAttack.PilotName + ".\n", LogTypes.AI, true, "yellow");

                    Messages.ShowInfo("Attacking with " + Combat.ChosenWeapon.Name);

                    GameCommand command = Combat.GenerateIntentToAttackCommand(Selection.ThisShip.ShipId, targetForAttack.ShipId, true, Combat.ChosenWeapon);
                    if (command != null) GameMode.CurrentGameMode.ExecuteServerCommand(command);
                }
                else
                {
                    Console.Write("The attack has been skipped.\n", LogTypes.AI, true, "yellow");
                    OnTargetNotLegalForAttack();
                }
            }
        }

        protected virtual GenericShip SelectTargetForAttack()
        {
            if (DebugManager.DebugNoCombat) return null;

            return AI.HotAC.TargetForAttackSelector.SelectTargetAndWeapon(Selection.ThisShip);
        }

        private static GenericShip GetShipThatCanAttack()
        {
            foreach (var shipHolder in Roster.GetPlayer(Phases.CurrentPhasePlayer).Ships)
            {
                if (shipHolder.Value.State.Initiative == Phases.CurrentSubPhase.RequiredInitiative)
                {
                    if (!shipHolder.Value.IsAttackPerformed)
                    {
                        Console.Write(shipHolder.Value.PilotInfo.PilotName + "(" + shipHolder.Value.ShipId + ") is selected as attacker", LogTypes.AI);
                        return shipHolder.Value;
                    }
                }
            }

            return null;
        }

        public GenericShip FindNearestEnemyShip(GenericShip thisShip, bool ignoreCollided = false, bool inArcAndRange = false)
        {
            Dictionary<GenericShip, float> results = GetEnemyShipsAndDistance(thisShip, ignoreCollided, inArcAndRange);
            GenericShip result = null;
            if (results.Count != 0)
            {
                result = results.OrderBy(n => n.Value).First().Key;
            }
            return result;
        }


        // TODO: Remove, used in AI/HotAC/TargetForAttackSelector
        public Dictionary<GenericShip, float> GetEnemyShipsAndDistance(GenericShip thisShip, bool ignoreCollided = false, bool inArcAndRange = false)
        {
            Dictionary<GenericShip, float> results = new Dictionary<GenericShip, float>();

            foreach (var shipHolder in Roster.GetPlayer(Roster.AnotherPlayer(thisShip.Owner.PlayerNo)).Ships)
            {
                if (!shipHolder.Value.IsDestroyed)
                {

                    if (ignoreCollided)
                    {
                        if (thisShip.LastShipCollision != null)
                        {
                            if (thisShip.LastShipCollision.ShipId == shipHolder.Value.ShipId)
                            {
                                continue;
                            }
                        }
                        if (shipHolder.Value.LastShipCollision != null)
                        {
                            if (shipHolder.Value.LastShipCollision.ShipId == thisShip.ShipId)
                            {
                                continue;
                            }
                        }
                    }

                    if (inArcAndRange)
                    {
                        BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(thisShip, shipHolder.Value);
                        if ((distanceInfo.Range > 3))
                        {
                            continue;
                        }
                    }

                    float distance = Vector3.Distance(thisShip.GetCenter(), shipHolder.Value.GetCenter());
                    results.Add(shipHolder.Value, distance);
                }
            }
            results = results.OrderBy(n => n.Value).ToDictionary(n => n.Key, n => n.Value);

            return results;
        }

        public override void UseDiceModifications(DiceModificationTimingType type)
        {
            base.UseDiceModifications(type);

            switch (type)
            {
                case DiceModificationTimingType.Normal:
                    Selection.ActiveShip = (Combat.AttackStep == CombatStep.Attack) ? Combat.Attacker : Combat.Defender;
                    break;
                case DiceModificationTimingType.AfterRolled:
                    Selection.ActiveShip = (Combat.AttackStep == CombatStep.Attack) ? Combat.Attacker : Combat.Defender;
                    break;
                case DiceModificationTimingType.Opposite:
                    Selection.ActiveShip = (Combat.AttackStep == CombatStep.Attack) ? Combat.Defender : Combat.Attacker;
                    break;
                case DiceModificationTimingType.CompareResults:
                    Selection.ActiveShip = Combat.Attacker;
                    break;
                default:
                    break;
            }

            Dictionary<GenericAction, int> actionsPriority = new Dictionary<GenericAction, int>();

            foreach (var diceModification in Combat.DiceModifications.AvailableDiceModifications.Values)
            {
                int priority = diceModification.GetDiceModificationPriority();
                Selection.ActiveShip.CallOnAiGetDiceModificationPriority(diceModification, ref priority);
                actionsPriority.Add(diceModification, priority);
            }

            actionsPriority = actionsPriority.OrderByDescending(n => n.Value).ToDictionary(n => n.Key, n => n.Value);

            bool isActionEffectTaken = false;

            if (actionsPriority.Count > 0)
            {
                KeyValuePair<GenericAction, int> prioritizedActionEffect = actionsPriority.First();
                if (prioritizedActionEffect.Value > 0)
                {
                    isActionEffectTaken = true;

                    GameManagerScript.Wait(1, delegate {
                        GameCommand command = DiceModificationsManager.GenerateDiceModificationCommand(prioritizedActionEffect.Key.Name);
                        GameMode.CurrentGameMode.ExecuteCommand(command);
                    });
                }
            }

            if (!isActionEffectTaken)
            {
                GameManagerScript.Wait(1, delegate {
                    GameCommand command = DiceModificationsManager.GenerateDiceModificationCommand("OK");
                    GameMode.CurrentGameMode.ExecuteCommand(command);
                });
            }
        }

        public override void ConfirmDiceCheck()
        {
            DiceCheckConfirm();
        }

        public override void OnTargetNotLegalForAttack()
        {
            /*Selection.ThisShip.CallAfterAttackWindow();
            Selection.ThisShip.IsAttackPerformed = true;
            Selection.ThisShip.CallCombatDeactivation(
                delegate { Phases.FinishSubPhase(typeof(CombatSubPhase)); }
            );*/

            Phases.CurrentSubPhase.IsReadyForCommands = true;
            GameMode.CurrentGameMode.ExecuteCommand(UI.GenerateSkipButtonCommand());
        }

        public override void ChangeManeuver(Action<string> doWithManeuverString, Action callback, Func<string, bool> filter = null)
        {
            DirectionsMenu.Show(doWithManeuverString, callback, filter);

            doWithManeuverString(Selection.ThisShip.AssignedManeuver.ToString());
        }

        public override void SelectManeuver(Action<string> doWithManeuverString, Action callback, Func<string, bool> filter = null)
        {
            doWithManeuverString(Selection.ThisShip.AssignedManeuver.ToString());

            callback();
        }

        public override void StartExtraAttack()
        {
            GenericShip targetForAttack = SelectTargetForAttack();

            if (targetForAttack != null)
            {
                Selection.ThisShip.IsAttackPerformed = true;

                Console.Write("Ship attacks target\n", LogTypes.AI, true, "yellow");

                Selection.AnotherShip = targetForAttack;
                Combat.TryPerformAttack(isSilent: true);
            }
            else
            {
                UI.SkipButtonEffect();
            }
        }

        public override void SelectShipForAbility()
        {
            base.SelectShipForAbility();

            if (Phases.CurrentSubPhase is SelectTargetForAttackSubPhase)
            {
                PerformAttackContinue();
            }
            else
            {
                (Phases.CurrentSubPhase as SelectShipSubPhase).AiSelectPrioritizedTarget();
            }
        }

        public override void SelectShipsForAbility()
        {
            base.SelectShipsForAbility();

            (Phases.CurrentSubPhase as MultiSelectionSubphase).AiSelectPrioritizedTarget();
        }

        public override void RerollManagerIsPrepared()
        {
            base.RerollManagerIsPrepared();
            DiceRerollManager.CurrentDiceRerollManager.ConfirmRerollButtonIsPressed();
        }

        public override void PlaceObstacle()
        {
            base.PlaceObstacle();

            ObstaclesPlacementSubPhase subphase = Phases.CurrentSubPhase as ObstaclesPlacementSubPhase;
            if (subphase.IsRandomSetupSelected[Roster.AnotherPlayer(this.PlayerNo)])
            {
                (Phases.CurrentSubPhase as ObstaclesPlacementSubPhase).PlaceRandom();
            }
            else
            {
                GameManagerScript.Wait(1, delegate
                {
                    (Phases.CurrentSubPhase as ObstaclesPlacementSubPhase).PlaceRandom();
                    Messages.ShowInfo("The AI has placed an obstacle");
                });
            }
        }

        public override void PerformSystemsActivation()
        {
            base.PerformSystemsActivation();
            UI.CallClickSkipPhase();
        }

        public override void InformAboutCrit()
        {
            base.InformAboutCrit();

            if (!Roster.Players.Any(p => p is HumanPlayer))
            {
                GameManagerScript.Wait(3, InformCrit.ButtonConfirm);
            }
            else
            {
                InformCrit.ShowConfirmButton();
            }
        }

        public override void SyncDiceResults()
        {
            base.SyncDiceResults();

            GameMode.CurrentGameMode.ExecuteCommand(DiceRoll.GenerateSyncDiceCommand());
        }

        public override void TakeDecision()
        {
            DecisionSubPhase subphase = (Phases.CurrentSubPhase as DecisionSubPhase);

            if (subphase.IsForced)
            {
                JSONObject parameters = new JSONObject();
                parameters.AddField("name", subphase.GetDecisions().First().Name);
                GameController.SendCommand(
                    GameCommandTypes.Decision,
                    Phases.CurrentSubPhase.GetType(),
                    parameters.ToString()
                );
            }
            else if (Phases.CurrentSubPhase is ActionDecisonSubPhase)
            {
                PerformActionFromList(Selection.ThisShip.GetAvailableActions());
            }
            else if (Phases.CurrentSubPhase is FreeActionDecisonSubPhase)
            {
                PerformActionFromList(Selection.ThisShip.GetAvailableFreeActions());
            }
            else (Phases.CurrentSubPhase as DecisionSubPhase).DoDefault();
        }

        protected virtual void PerformActionFromList(List<GenericAction> actionsList) { }

        public override void SyncDiceRerollSelected()
        {
            GameMode.CurrentGameMode.ExecuteCommand(DiceRerollManager.GenerateConfirmRerollCommand());
        }
    }
}
