using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{

    //todo: move to movement
    public enum ManeuverColor
    {
        None,
        Green,
        White,
        Red
    }

    public class GenericShip
    {

        protected GameManagerScript Game;

        public Movement AssignedManeuver { get; set; }
        public bool IsManeurPerformed { get; set; }
        public bool IsAttackPerformed { get; set; }
        public bool IsDestroyed { get; set; }

        public string Type
        {
            get;
            set;
        }
        public string PilotName
        {
            get;
            set;
        }
        public int Firepower
        {
            get;
            set;
        }
        private int _PilotSkill;
        public int PilotSkill
        {
            get
            {
                int result = _PilotSkill;
                if (AfterGetPilotSkill!=null) AfterGetPilotSkill(ref result);
                result = Mathf.Clamp(result, 0, 12);
                return result;
            }
            set
            {
                value = Mathf.Clamp(value, 0, 12);
                _PilotSkill = value;
            }
        }
        private int _Agility;
        public int Agility
        {
            get
            {
                int result = _Agility;
                if (AfterGetAgility != null) AfterGetAgility(ref result);
                result = Mathf.Max(result, 0);
                return result;
            }
            set
            {
                value = Mathf.Max(value, 0);
                _Agility = value;
            }
        }
        public int MaxHull
        {
            get;
            set;
        }
        public int Hull
        {
            get;
            set;
        }
        public int MaxShields
        {
            get;
            set;
        }
        public int Shields
        {
            get;
            set;
        }
        public ShipModelScript Model
        {
            get;
            set;
        }
        public GameObject InfoPanel
        {
            get;
            set;
        }
        public GenericShip LastShipCollision
        {
            get;
            set;
        }
        public Player PlayerNo
        {
            get;
            set;
        }

        public Dictionary<Tokens.GenericToken, int> AssignedTokens = new Dictionary<Tokens.GenericToken, int>();

        public bool isUnique = false;
        //public bool FactionRestriction

        public List<Actions.GenericAction> BuiltInActions = new List<Actions.GenericAction>();
        public List<Actions.GenericAction> FreeActions = new List<Actions.GenericAction>();
        public List<Actions.GenericAction> AlreadyExecutedActions = new List<Actions.GenericAction>();
        public List<Actions.GenericAction> AvailableActionEffects = new List<Actions.GenericAction>();

        public List<CriticalHitCard.GenericCriticalHit> AssignedCrits = new List<CriticalHitCard.GenericCriticalHit>();

        public Dictionary<Upgrade.UpgradeSlot, int> BuiltInSlots = new Dictionary<Upgrade.UpgradeSlot, int>();

        public Dictionary<string, ManeuverColor> Maneuvers = new Dictionary<string, ManeuverColor>();
        public int ShipId { get; set; }

        public List<Actions.GenericAction> AvailableActionsList = new List<Actions.GenericAction>();
        public List<Actions.GenericAction> AvailableFreeActionsList = new List<Actions.GenericAction>();

        public List<KeyValuePair<Upgrade.UpgradeSlot, Upgrade.GenericUpgrade>> InstalledUpgrades = new List<KeyValuePair<Upgrade.UpgradeSlot, Upgrade.GenericUpgrade>>();

        //EVENTS

        public delegate void EventHandler();
        public delegate void EventHandlerInt(ref int data);
        public delegate void EventHandlerBool(ref bool data);
        public delegate void EventHandlerBoolBool(ref bool data, bool afterMovement);
        public delegate void EventHandlerShip(GenericShip ship);
        public delegate void EventHandlerShipBool(GenericShip ship, bool afterMovement);
        public delegate void EventHandlerActionEffectsList(ref List<Actions.GenericAction> list);
        public delegate void EventHandlerShipMovement(GenericShip ship, ref Movement movement);
        public delegate void EventHandlerShipCrit(GenericShip ship, ref CriticalHitCard.GenericCriticalHit crit);

        public event EventHandler OnDestroyed;
        public event EventHandler OnDefence;
        public event EventHandler OnAttack;
        public event EventHandlerShip OnMovementStart;
        public event EventHandlerShip OnMovementFinish;
        public event EventHandlerShip OnMovementFinishWithColliding;
        public event EventHandlerShip OnMovementFinishWithoutColliding;
        public event EventHandlerShip AfterTokenIsAssigned;
        public event EventHandlerShip AfterTokenIsRemoved;
        public event EventHandlerShip AfterAssignedDamageIsChanged;
        public event EventHandlerShip AfterStatsAreChanged;
        public event EventHandlerShipBool AfterAvailableActionListIsBuilt;
        public event EventHandlerInt AfterGotNumberOfAttackDices;
        public event EventHandlerInt AfterGotNumberOfDefenceDices;
        public event EventHandlerInt AfterGetPilotSkill;
        public event EventHandlerInt AfterGetAgility;
        public event EventHandlerBool OnTrySpendFocus;
        public event EventHandlerBool OnTryReroll;
        public event EventHandlerBoolBool OnTryPerformAction;
        public event EventHandlerActionEffectsList AfterGenerateDiceModifications;
        public event EventHandlerShipMovement AfterGetManeuverColor;
        public event EventHandlerShipMovement AfterGetManeuverAvailablity;
        public event EventHandlerShipCrit OnAssignCrit;

        public GenericShip(Player playerNo, int shipId, Vector3 position)
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

            PlayerNo = playerNo;
            ShipId = shipId;

            AddUpgradeSlot(Upgrade.UpgradeSlot.Modification);
        }

        protected void InitializeValues()
        {
            Shields = MaxShields;
            Hull = MaxHull;
        }

        protected void SetModel(Vector3 position)
        {
            Model = new ShipModelScript(this, position);
        }

        // MANEUVERS

        public ManeuverColor GetColorComplexityOfManeuver(string maneuverString)
        {
            return Maneuvers[maneuverString];
        }

        public ManeuverColor GetLastManeuverColor()
        {
            ManeuverColor result = ManeuverColor.None;
            result = AssignedManeuver.ColorComplexity;
            return result;
        }

        public Dictionary<string, ManeuverColor> GetManeuvers()
        {
            Dictionary<string, ManeuverColor> result = new Dictionary<string, ManeuverColor>();

            foreach (var maneuverHolder in Maneuvers)
            {
                Movement movement = Game.Movement.ManeuverFromString(maneuverHolder.Key);
                if (AfterGetManeuverColor != null) AfterGetManeuverColor(this, ref movement);
                if (AfterGetManeuverAvailablity != null) AfterGetManeuverAvailablity(this, ref movement);
                result.Add(maneuverHolder.Key, movement.ColorComplexity);
            }

            return result;
        }

        // STAT MODIFICATIONS

        public void ChangeAgility(int value)
        {
            Agility += value;
            AfterStatsAreChanged(this);
        }

        public int GetNumberOfAttackDices(GenericShip targetShip)
        {
            int result = Firepower;
            AfterGotNumberOfAttackDices(ref result);
            if (result < 0) result = 0;
            return result;
        }

        public int GetNumberOfDefenceDices(GenericShip attackerShip)
        {
            int result = Agility;
            AfterGotNumberOfDefenceDices(ref result);
            return result;
        }

        // REGEN

        public bool TryRegenShields()
        {
            bool result = false;
            if (Shields < MaxShields)
            {
                result = true;
                Shields++;
                AfterAssignedDamageIsChanged(this);
            };
            return result;
        }

        public bool TryRegenHull()
        {
            bool result = false;
            if (Hull < MaxHull)
            {
                result = true;
                Hull++;
                AfterAssignedDamageIsChanged(this);
            };
            return result;
        }

        // DAMAGE

        public void SufferDamage(DiceRoll damage)
        {

            int shieldsBefore = Shields;

            Shields = Mathf.Max(Shields - damage.Successes, 0);

            damage.CancelHits(shieldsBefore - Shields);

            if (damage.Successes != 0)
            {
                foreach (Dice dice in damage.Dices)
                {
                    if (dice.Side == DiceSide.Success)
                    {
                        SufferHullDamage();
                    }
                    if (dice.Side == DiceSide.Crit)
                    {
                        Game.CritsDeck.DrawCrit(this);
                    }
                }
            }

            AfterAssignedDamageIsChanged(this);
        }

        public void SufferHullDamage()
        {
            Hull--;
            Hull = Mathf.Max(Hull, 0);

            IsHullDestroyedCheck();
        }

        public void SufferCrit(CriticalHitCard.GenericCriticalHit crit)
        {
            if (OnAssignCrit != null) OnAssignCrit(this, ref crit);

            if (crit != null)
            {
                SufferHullDamage();
                AssignedCrits.Add(crit);
                crit.AssignCrit(this);
            }
        }

        public void IsHullDestroyedCheck()
        {
            if (Hull == 0)
            {
                DestroyShip();
            }
        }

        public void DestroyShip()
        {
            if (!IsDestroyed)
            {
                Game.UI.AddTestLogEntry(PilotName + "\'s ship is destroyed");
                Game.Roster.DestroyShip(this.Model.GetTag());
                OnDestroyed();
                IsDestroyed = true;
            }
        }

        //TODO: Move from here
        public bool IsTargetInArc(GenericShip anotherShip)
        {
            //TODO: Show Shortest Distance
            //TODO: Adapt DistancRules to show how close to outOfArc;

            Vector3 vectorFacing = Model.GetFrontFacing();

            bool inArc = false;

            foreach (var objThis in Model.GetStandFrontEdgePoins())
            {
                foreach (var objAnother in anotherShip.Model.GetStandEdgePoints())
                {

                    Vector3 vectorToTarget = objAnother.Value - objThis.Value;
                    float angle = Vector3.Angle(vectorToTarget, vectorFacing);
                    //Debug.Log ("Angle between " + objThis.Key + " and " + objAnother.Key + " is: " + angle.ToString ());
                    if (angle < 45)
                    {
                        inArc = true;
                        //TODO: Comment shortcut to check all variants
                        //return inArc;
                    }
                }
            }

            return inArc;
        }

        //TRIGGERS

        //todo: think about name
        public void AttackStart()
        {
            if (OnAttack != null) OnAttack();
        }

        //todo: think about name
        public void DefenceStart()
        {
            if (OnDefence != null) OnDefence();
        }

        public void StartMoving()
        {
            OnMovementStart(this);
        }

        public void FinishMoving()
        {
            OnMovementFinish(this);
        }

        public void FinishMovingWithColliding()
        {
            OnMovementFinishWithColliding(this);
        }

        public void FinishMovingWithoutColliding()
        {
            OnMovementFinishWithoutColliding(this);
        }

        //UPGRADES

        protected void AddUpgradeSlot(Upgrade.UpgradeSlot slot)
        {
            if (!BuiltInSlots.ContainsKey(slot))
            {
                BuiltInSlots.Add(slot, 1);
            }
            else
            {
                BuiltInSlots[slot]++;
            }

        }

        public void InstallUpgrade(string upgradeName)
        {
            Upgrade.GenericUpgrade newUpgrade = (Upgrade.GenericUpgrade)System.Activator.CreateInstance(System.Type.GetType(upgradeName), this);

            Upgrade.UpgradeSlot slot = newUpgrade.Type;
            if (HasFreeUpgradeSlot(slot))
            {
                InstalledUpgrades.Add(new KeyValuePair<Upgrade.UpgradeSlot, Upgrade.GenericUpgrade>(newUpgrade.Type, newUpgrade));
                Game.UI.Roster.UpdateUpgradesPanel(this, this.InfoPanel);
            }

        }

        private bool HasFreeUpgradeSlot(Upgrade.UpgradeSlot slot)
        {
            bool result = false;
            if (BuiltInSlots.ContainsKey(slot))
            {
                int slotsAvailabe = BuiltInSlots[slot];
                foreach (var upgrade in InstalledUpgrades)
                {
                    if (upgrade.Key == slot) slotsAvailabe--;
                }

                if (slotsAvailabe > 0) result = true;
            }
            return result;
        }

        //ACTIONS

        public void AskPerformFreeAction(string name, Actions.GenericAction action, bool afterMovement)
        {
            Game.Selection.isUIlocked = true;
            Game.Selection.isInTemporaryState = true;
            Game.UI.Helper.UpdateTemporaryState("Perform free action");
            /*FreeActions = new Dictionary<string, DefaultAction>();
            FreeActions.Add(name, action);*/
            Game.UI.ActionsPanel.ShowFreeActionsPanel(afterMovement);
        }

        public void GenerateAvailableActionsList(bool afterMovement)
        {
            AvailableActionsList = new List<Actions.GenericAction>();

            foreach (var action in BuiltInActions)
            {
                AvailableActionsList.Add(action);
            }

            if (AfterAvailableActionListIsBuilt != null) AfterAvailableActionListIsBuilt(this, afterMovement);
        }

        public void GenerateAvailableFreeActionsList(bool afterMovement)
        {
            /*AvailableFreeActionsList = new Dictionary<string, ShipActionExecution>();

            foreach (var action in FreeActions)
            {
                AvailableFreeActionsList.Add(action.Key, action.Value);
            }*/

        }

        public void ClearAlreadyExecutedActions()
        {
            AlreadyExecutedActions = new List<Actions.GenericAction>();
        }

        /*public bool CanPerformAction(Actions.GenericAction action, bool afterMovement)
        {
            bool result = false;
            if (BuiltInActions.ContainsValue(action)) result = true;
            if (AlreadyExecutedActions.Contains(ActionToString(action)))
            {
                result = false;
            }
            OnTryPerformAction(ref result, afterMovement);
            return result;
        }*/

        /*public bool CanPerformFreeAction(Actions.GenericAction action, bool afterMovement)
        {
            bool result = true;
            if (AlreadyExecutedActions.Contains(ActionToString(action))){
                result = false;
            }
            OnTryPerformAction(ref result, afterMovement);
            return result;
        }

        public bool CanPerformFreeAction(string action, bool afterMovement)
        {
            bool result = true;
            if (AlreadyExecutedActions.Contains(action))
            {
                result = false;
            }
            OnTryPerformAction(ref result, afterMovement);
            return result;
        }*/

        // ACTION EFFECTS

        public void GenerateDiceModificationButtons()
        {
            //TODO: REWRITE FOR SECOD USING
            foreach (var action in AvailableActionEffects)
            {
                // if (Game.Selection.ActiveShip.CanSpendEvade() && (Game.Combat.AttackStep == CombatStep.Defence))
                //if (AlreadyExecutedActions.Contains(ActionToString(action)))
                if (!CanUseActionEffect(action))
                {
                    AvailableActionEffects.Remove(action);
                }
            }

            if (AfterGenerateDiceModifications != null) AfterGenerateDiceModifications(ref AvailableActionEffects);

        }

        private bool CanUseActionEffect(Actions.GenericAction action)
        {
            bool result = true;
            //TODO: ALL MAGIC HERE
            return result;
        }

        // TOKENS

        public void AddToken(Tokens.GenericToken token)
        {
            if (AssignedTokens.ContainsKey(token))
            {
                //TODO: AddEvent
                AssignedTokens[token]++;
            }
            else
            {
                AssignedTokens.Add(token, 1);
            }

            if (AfterTokenIsAssigned != null) AfterTokenIsAssigned(this);
        }

        public void RemoveToken(Tokens.GenericToken token)
        {
            if (AssignedTokens.ContainsKey(token))
            {
                if (AssignedTokens[token] > 1)
                {
                    AssignedTokens[token]--;
                }
                else
                {
                    AssignedTokens.Remove(token);
                }
                if (AfterTokenIsRemoved != null) AfterTokenIsRemoved(this);
            }
        }

        public void ClearTokens()
        {
            foreach (var tokenHolder in AssignedTokens)
            {
                if (tokenHolder.Key.Temporary)
                {
                    while (tokenHolder.Value > 0)
                    {
                        RemoveToken(tokenHolder.Key);
                    }
                }
            }
        }

        public void SpendToken(object type)
        {
            foreach (var token in AssignedTokens)
            {
                if (token.Key.GetType() == type)
                {
                    RemoveToken(token.Key);
                    break;
                }
            }
        }

        public bool HasToken(Tokens.GenericToken token)
        {
            return AssignedTokens.ContainsKey(token);
        }

    }

}
