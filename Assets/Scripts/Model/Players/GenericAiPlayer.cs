using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Ship;
using ActionsList;

namespace Players
{

    public partial class GenericAiPlayer : GenericPlayer
    {

        public GenericAiPlayer() : base()
        {
            Type = PlayerType.Ai;
            Name = "AI";

            NickName = "A.I.";
            Title = "Protocol Droid";
            Avatar = "UpgradesList.C3PO";
        }

        public override void SetupShip()
        {
            base.SetupShip();

            foreach (var shipHolder in Ships)
            {
                if (!shipHolder.Value.IsSetupPerformed && shipHolder.Value.PilotSkill == Phases.CurrentSubPhase.RequiredPilotSkill)
                {
                    int direction = (Phases.CurrentSubPhase.RequiredPlayer == PlayerNo.Player1) ? -1 : 1;
                    Vector3 position  = shipHolder.Value.GetPosition() + new Vector3(0, 0, direction * 1.2f);

                    GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
                    Game.Wait(0.5f, delegate { Board.BoardManager.PlaceShip(shipHolder.Value, position, shipHolder.Value.GetAngles(), Phases.Next); });
                    return;
                }
            }

            Phases.Next();
        }

        public override void AssignManeuver()
        {
            base.AssignManeuver();

            foreach (var shipHolder in Ships)
            {
                Selection.ChangeActiveShip("ShipId:" + shipHolder.Value.ShipId);
                shipHolder.Value.SetAssignedManeuver(new Movement.StraightMovement(2, Movement.ManeuverDirection.Forward, Movement.ManeuverBearing.Straight, Movement.ManeuverColor.White));
            }
            Phases.Next();
        }

        public override void PerformManeuver()
        {
            base.PerformManeuver();

            bool foundToActivate = false;
            foreach (var shipHolder in Roster.GetPlayer(Phases.CurrentPhasePlayer).Ships)
            {
                if (shipHolder.Value.PilotSkill == Phases.CurrentSubPhase.RequiredPilotSkill)
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
            ShipMovementScript.PerformStoredManeuver(ship.ShipId);
        }

        //TODOL Don't skip attack of all PS ships if one cannot attack (Biggs interaction)

        public override void PerformAttack()
        {
            base.PerformAttack();

            Console.Write("AI is going to perform attack", LogTypes.AI);

            SelectShipThatCanAttack(SelectTargetForAttack);
        }

        private void SelectTargetForAttack()
        {
            GenericShip targetForAttack = null;

            // TODO: Fix bug with missing chosen weapon

            if (Selection.ThisShip != null)
            {
                if (!DebugManager.DebugNoCombat)
                {
                    Dictionary<GenericShip, float> enemyShips = GetEnemyShipsAndDistance(Selection.ThisShip, ignoreCollided: true, inArcAndRange: true);

                    targetForAttack = GetTargetWithAssignedTargetLock(enemyShips);

                    if (targetForAttack != null)
                    {
                        Console.Write("Ship has Target Lock on " + targetForAttack.PilotName + "(" + targetForAttack.ShipId + ")", LogTypes.AI);
                    }
                    else
                    {
                        Console.Write("Ship doesn't have Target Lock on enemy", LogTypes.AI);
                    }

                    if (targetForAttack == null)
                    {
                        targetForAttack = SelectNearestTarget(enemyShips);
                        if (targetForAttack != null)
                        {
                            Console.Write("Ship selected nearest target " + targetForAttack.PilotName + "(" + targetForAttack.ShipId + ")", LogTypes.AI);
                        }
                        else
                        {
                            Console.Write("Ship cannot find valid enemy for attack", LogTypes.AI);
                        }
                    }

                }

                Selection.ThisShip.CallAfterAttackWindow();
                Selection.ThisShip.IsAttackPerformed = true;
            }

            if (targetForAttack != null)
            {
                Console.Write("Ship attacks target\n", LogTypes.AI, true, "yellow");

                Selection.TryToChangeAnotherShip("ShipId:" + targetForAttack.ShipId);
                Combat.TryPerformAttack();
            }
            else
            {
                Console.Write("Attack is skipped\n", LogTypes.AI, true, "yellow");
                OnTargetNotLegalForAttack();
            }
        }

        private GenericShip SelectNearestTarget(Dictionary<GenericShip, float> enemyShips)
        {
            GenericShip targetForAttack = null;

            foreach (var shipHolder in enemyShips)
            {
                GenericShip newTarget = null;
                newTarget = TryToDeclareTarget(shipHolder.Key, shipHolder.Value);

                if (newTarget != null)
                {
                    if (DebugManager.DebugAI) Debug.Log("Previous target for attack: " + targetForAttack);
                    if (DebugManager.DebugAI) if (targetForAttack != null) Debug.Log("Previous target has higher distance: " + (enemyShips[targetForAttack] > enemyShips[newTarget]));
                    if ((targetForAttack == null) || (enemyShips[targetForAttack] > enemyShips[newTarget]))
                    {
                        targetForAttack = newTarget;
                        if (DebugManager.DebugAI) Debug.Log("AI has target for attack with primary weapon: " + targetForAttack);
                    }
                }
            }

            return targetForAttack;
        }

        private GenericShip GetTargetWithAssignedTargetLock(Dictionary<GenericShip, float> enemyShips)
        {
            GenericShip targetForAttack = null;

            foreach (var shipHolder in enemyShips)
            {
                if (Actions.GetTargetLocksLetterPair(Selection.ThisShip, shipHolder.Key) != ' ')
                {
                    return TryToDeclareTarget(shipHolder.Key, shipHolder.Value);
                }
            }

            return targetForAttack;
        }

        private static void SelectShipThatCanAttack(Action callback)
        {
            foreach (var shipHolder in Roster.GetPlayer(Phases.CurrentPhasePlayer).Ships)
            {
                if (shipHolder.Value.PilotSkill == Phases.CurrentSubPhase.RequiredPilotSkill)
                {
                    if (!shipHolder.Value.IsAttackPerformed)
                    {
                        Selection.ChangeActiveShip("ShipId:" + shipHolder.Value.ShipId);
                        Console.Write(Selection.ThisShip.PilotName + "(" + Selection.ThisShip.ShipId + ") is selected as attacker", LogTypes.AI);
                        break;
                    }
                }
            }

            if (Selection.ThisShip != null)
            {
                Selection.ThisShip.CallCombatActivation(callback);
            }
            else
            {
                callback();
            }
        }

        private GenericShip TryToDeclareTarget(GenericShip targetShip, float distance)
        {
            GenericShip selectedTargetShip = targetShip;

            if (DebugManager.DebugAI) Debug.Log("AI checks target for attack: " + targetShip);

            if (targetShip.IsReadyToBeDestroyed)
            {
                if (DebugManager.DebugAI) Debug.Log("But this target is already destroyed");
                return null;
            }

            if (DebugManager.DebugAI) Debug.Log("Ship is selected before validation: " + selectedTargetShip);
            Selection.TryToChangeAnotherShip("ShipId:" + selectedTargetShip.ShipId);

            IShipWeapon chosenWeapon = null;

            foreach (var upgrade in Selection.ThisShip.UpgradeBar.GetUpgradesOnlyFaceup())
            {
                IShipWeapon secondaryWeapon = (upgrade as IShipWeapon);
                if (secondaryWeapon != null)
                {
                    if (secondaryWeapon.IsShotAvailable(targetShip))
                    {
                        chosenWeapon = secondaryWeapon;
                        break;
                    }
                }
            }

            chosenWeapon = chosenWeapon ?? Selection.ThisShip.PrimaryWeapon;
            Combat.ChosenWeapon = chosenWeapon;
            Combat.ShotInfo = new Board.ShipShotDistanceInformation(Selection.ThisShip, Selection.AnotherShip, Combat.ChosenWeapon);

            if (Rules.TargetIsLegalForShot.IsLegal(true) && Combat.ChosenWeapon.IsShotAvailable(Selection.AnotherShip))
            {
                if (DebugManager.DebugAI) Debug.Log("AI target legal: " + Selection.AnotherShip);
            }
            else
            {
                if (DebugManager.DebugAI) Debug.Log("But validation is not passed: " + selectedTargetShip);
                selectedTargetShip = null;
            }

            if (DebugManager.DebugAI) Debug.Log("AI decision about " + targetShip + " : " + selectedTargetShip);

            return selectedTargetShip;
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
                        Board.ShipDistanceInformation distanceInfo = new Board.ShipDistanceInformation(thisShip, shipHolder.Value);
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

        public override void UseOwnDiceModifications()
        {
            base.UseOwnDiceModifications();

            Selection.ActiveShip = (Combat.AttackStep == CombatStep.Attack) ? Combat.Attacker : Combat.Defender;

            Selection.ActiveShip.GenerateAvailableActionEffectsList();
            List<GenericAction> availableActionEffectsList = Selection.ActiveShip.GetAvailableActionEffectsList();

            Dictionary<GenericAction, int> actionsPriority = new Dictionary<GenericAction, int>();

            foreach (var actionEffect in availableActionEffectsList)
            {
                int priority = actionEffect.GetActionEffectPriority();
                Selection.ActiveShip.CallOnAiGetDiceModificationPriority(actionEffect, ref priority);
                actionsPriority.Add(actionEffect, priority);
            }

            actionsPriority = actionsPriority.OrderByDescending(n => n.Value).ToDictionary(n => n.Key, n => n.Value);

            bool isActionEffectTaken = false;

            if (actionsPriority.Count > 0)
            {
                KeyValuePair<GenericAction, int> prioritizedActionEffect = actionsPriority.First();
                if (prioritizedActionEffect.Value > 0)
                {
                    isActionEffectTaken = true;
                    Messages.ShowInfo("AI uses \"" + prioritizedActionEffect.Key.Name + "\"");
                    GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
                    Game.Wait(1, delegate {
                        Selection.ActiveShip.AddAlreadyExecutedActionEffect(prioritizedActionEffect.Key);
                        prioritizedActionEffect.Key.ActionEffect(UseOwnDiceModifications);
                    });                    
                }
            }

            if (!isActionEffectTaken)
            {
                GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
                Game.Wait(2, delegate { Phases.CurrentSubPhase.CallBack(); });
            }
        }

        public override void UseOppositeDiceModifications()
        {
            base.UseOppositeDiceModifications();

            Selection.ActiveShip.GenerateAvailableOppositeActionEffectsList();
            List<GenericAction> availableOppositeActionEffectsList = Selection.ActiveShip.GetAvailableOppositeActionEffectsList();

            Dictionary<GenericAction, int> oppositeActionsPriority = new Dictionary<GenericAction, int>();

            foreach (var oppositeActionEffect in availableOppositeActionEffectsList)
            {
                int priority = oppositeActionEffect.GetActionEffectPriority();
                oppositeActionsPriority.Add(oppositeActionEffect, priority);
            }

            oppositeActionsPriority = oppositeActionsPriority.OrderByDescending(n => n.Value).ToDictionary(n => n.Key, n => n.Value);

            bool isActionEffectTaken = false;

            if (oppositeActionsPriority.Count > 0)
            {
                KeyValuePair<GenericAction, int> prioritizedOppositeActionEffect = oppositeActionsPriority.First();
                if (prioritizedOppositeActionEffect.Value > 0)
                {
                    isActionEffectTaken = true;
                    Messages.ShowInfo("AI uses \"" + prioritizedOppositeActionEffect.Key.Name + "\"");
                    GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
                    Game.Wait(1, delegate {
                        Selection.ActiveShip.AddAlreadyExecutedOppositeActionEffect(prioritizedOppositeActionEffect.Key);
                        prioritizedOppositeActionEffect.Key.ActionEffect(UseOppositeDiceModifications);
                    });
                }
            }

            if (!isActionEffectTaken)
            {
                Selection.ActiveShip = (Combat.AttackStep == CombatStep.Attack) ? Combat.Attacker : Combat.Defender;
                Selection.ActiveShip.Owner.UseOwnDiceModifications();
            }
        }

        public override void UseCompareResultsDiceModifications()
        {
            base.UseCompareResultsDiceModifications();

            Selection.ActiveShip = Combat.Attacker;

            Selection.ActiveShip.GenerateAvailableCompareResultsEffectsList();
            List<GenericAction> availableCompareResultsEffectsList = Selection.ActiveShip.GetAvailableCompareResultsEffectsList();

            Dictionary<GenericAction, int> actionsPriority = new Dictionary<GenericAction, int>();

            foreach (var actionEffect in availableCompareResultsEffectsList)
            {
                int priority = actionEffect.GetActionEffectPriority();
                actionsPriority.Add(actionEffect, priority);
            }

            actionsPriority = actionsPriority.OrderByDescending(n => n.Value).ToDictionary(n => n.Key, n => n.Value);

            bool isActionEffectTaken = false;

            if (actionsPriority.Count > 0)
            {
                KeyValuePair<GenericAction, int> prioritizedActionEffect = actionsPriority.First();
                if (prioritizedActionEffect.Value > 0)
                {
                    isActionEffectTaken = true;
                    Messages.ShowInfo("AI uses \"" + prioritizedActionEffect.Key.Name + "\"");
                    GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
                    Game.Wait(1, delegate {
                        Selection.ActiveShip.AddAlreadyExecutedCompareResultsEffect(prioritizedActionEffect.Key);
                        prioritizedActionEffect.Key.ActionEffect(UseCompareResultsDiceModifications);
                    });
                }
            }

            if (!isActionEffectTaken)
            {
                GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
                Game.Wait(2, Combat.CompareResultsAndDealDamage);
            }
        }

        public override void TakeDecision()
        {
            Phases.CurrentSubPhase.DoDefault();
        }

        public override void ConfirmDiceCheck()
        {
            (Phases.CurrentSubPhase as SubPhases.DiceRollCheckSubPhase).Confirm();
        }

        public override void OnTargetNotLegalForAttack()
        {
            Selection.ThisShip.CallAfterAttackWindow();
            Selection.ThisShip.IsAttackPerformed = true;

            Selection.ThisShip.CallCombatDeactivation(
                delegate { Phases.FinishSubPhase(typeof(SubPhases.CombatSubPhase)); }
            );
        }

        public override void ChangeManeuver(Action<string> callback, Func<string, bool> filter = null)
        {
            callback(Selection.ThisShip.AssignedManeuver.ToString());
        }

        public override void SelectManeuver(Action<string> callback, Func<string, bool> filter = null)
        {
            callback(Selection.ThisShip.AssignedManeuver.ToString());
        }

        public override void StartExtraAttack()
        {
            // TODO: Handle extra attack targets

            UI.SkipButtonEffect();
        }

        public override void SelectShipForAbility()
        {
            (Phases.CurrentSubPhase as SubPhases.SelectShipSubPhase).AiSelectPrioritizedTarget();
        }

        public override void RerollManagerIsPrepared()
        {
            DiceRerollManager.CurrentDiceRerollManager.ConfirmRerollButtonIsPressed();
        }
    }

}
