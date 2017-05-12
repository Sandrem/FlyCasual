using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{

    public enum ManeuverColor
    {
        None,
        Green,
        White,
        Red
    }

    public enum DefaultAction
    {
        Focus,
        Evade,
        TargetLock,
        BarrelRoll
    }

    public enum Token
    {
        Stress,
        Focus,
        Evade
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

        public Dictionary<Token, int> AssignedTokens = new Dictionary<Token, int>();

        public bool isUnique = false;
        //public bool FactionRestriction

        public Dictionary <string, DefaultAction> BuiltInActions = new Dictionary<string, DefaultAction>();
        public Dictionary<string, DefaultAction> FreeActions = new Dictionary<string, DefaultAction>();

        public List<string> AlreadyExecutedActions = new List<string>();
        public List<CriticalHitCard.GenericCriticalHit> AssignedCrits = new List<CriticalHitCard.GenericCriticalHit>();

        public Dictionary<Upgrade.UpgradeSlot, int> BuiltInSlots = new Dictionary<Upgrade.UpgradeSlot, int>();

        public Dictionary<string, ManeuverColor> Maneuvers = new Dictionary<string, ManeuverColor>();
        public int ShipId { get; set; }

        public Dictionary<string, ShipActionExecution> AvailableActionsList = new Dictionary<string, ShipActionExecution>();
        public Dictionary<string, ShipActionExecution> AvailableFreeActionsList = new Dictionary<string, ShipActionExecution>();

        public Dictionary<string, DiceModification> AvailableDiceModifications = new Dictionary<string, DiceModification>();
        private Dictionary<string, DiceModification> DefaultDiceMidifications = new Dictionary<string, DiceModification>();

        public List<KeyValuePair<Upgrade.UpgradeSlot, Upgrade.GenericUpgrade>> InstalledUpgrades = new List<KeyValuePair<Upgrade.UpgradeSlot, Upgrade.GenericUpgrade>>();

        //EVENTS

        public delegate void EventHandler();
        public delegate void EventHandlerInt(ref int data);
        public delegate void EventHandlerBool(ref bool data);
        public delegate void EventHandlerBoolBool(ref bool data, bool afterMovement);
        public delegate void EventHandlerShip(GenericShip ship);
        public delegate void EventHandlerShipBool(GenericShip ship, bool afterMovement);
        public delegate void EventHandlerDiceModificationDict(ref Dictionary<string, DiceModification> dict);
        public delegate void EventHandlerShipMovement(GenericShip ship, ref Movement movement);
        public delegate void EventHandlerShipCrit(GenericShip ship, ref CriticalHitCard.GenericCriticalHit crit);

        public event EventHandler OnDestroyed;
        public event EventHandler OnDefence;
        public event EventHandler OnAttack;
        public event EventHandlerShip OnMovementStart;
        public event EventHandlerShip OnMovementFinish;
        public event EventHandlerShip OnMovementFinishWithColliding;
        public event EventHandlerShip OnMovementFinishWithoutColliding;
        public event EventHandlerShip AfterStressTokenIsAssigned;
        public event EventHandlerShip AfterStressTokenIsRemoved;
        public event EventHandlerShip AfterFocusTokenIsAssigned;
        public event EventHandlerShip AfterFocusTokenIsRemoved;
        public event EventHandlerShip AfterEvadeTokenIsAssigned;
        public event EventHandlerShip AfterEvadeTokenIsRemoved;
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
        public event EventHandlerDiceModificationDict AfterGenerateDiceModifications;
        public event EventHandlerShipMovement AfterGetManeuverColor;
        public event EventHandlerShipMovement AfterGetManeuverAvailablity;
        public event EventHandlerShipCrit OnAssignCrit;

        public GenericShip(Player playerNo, int shipId, Vector3 position)
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

            PlayerNo = playerNo;
            ShipId = shipId;

            GenerateDefaultDiceModifications();

            AddUpgradeSlot(Upgrade.UpgradeSlot.Modification);
        }

        protected void InitializeValues()
        {
            Shields = MaxShields;
            Hull = MaxHull;
        }

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

        public void ClearAlreadyExecutedActions()
        {
            AlreadyExecutedActions = new List<string>();
        }

        //Todo: Avoid this, move to 1 static place
        private void GenerateDefaultDiceModifications()
        {
            //fix this
            //if (Game.Combat == null) Game.Combat = Game.GetComponent<CombatManagerScript>();

            DefaultDiceMidifications.Add("Focus", Game.Combat.ApplyFocus);
            DefaultDiceMidifications.Add("Evade", Game.Combat.ApplyEvade);
            //DefaultDiceModofications.Add("Reroll", Game.Combat.RerollDices);
        }

        protected void SetModel(Vector3 position)
        {
            Model = new ShipModelScript(this, position);
        }

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

        private void AddToken(Token token)
        {
            if (AssignedTokens.ContainsKey(token))
            {
                AssignedTokens[token]++;
            }
            else
            {
                AssignedTokens.Add(token, 1);
            }
        }

        private void RemoveToken(Token token)
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
            }
            else
            {
                AssignedTokens.Add(token, 1);
            }
        }

        public void PerformFocusAction()
        {
            AddToken(Token.Focus);
            AfterFocusTokenIsAssigned(this);
        }

        public void PerformEvadeAction()
        {
            AddToken(Token.Evade);
            AfterEvadeTokenIsAssigned(this);
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

        public void ClearTokens()
        {
            while (HasToken(Token.Focus))
            {
                SpendFocusToken();
                AfterFocusTokenIsRemoved(this);
            }
            while (HasToken(Token.Evade))
            {
                SpendEvadeToken();
                AfterEvadeTokenIsRemoved(this);
            }
        }

        public void SpendFocusToken()
        {
            RemoveToken(Token.Focus);
            AfterFocusTokenIsRemoved(this);
        }

        public void SpendEvadeToken()
        {
            RemoveToken(Token.Evade);
            AfterEvadeTokenIsRemoved(this);
        }

        public bool HasToken(Token token)
        {
            return AssignedTokens.ContainsKey(token);
        }

        public bool CanSpendFocus()
        {
            bool result = false;
            if (HasToken(Token.Focus)) result = true;
            if (OnTrySpendFocus != null) OnTrySpendFocus(ref result);
            return result;
        }

        public bool CanReroll()
        {
            bool result = false;
            if (HasToken(Token.Focus)) result = true;
            if (OnTryReroll != null) OnTryReroll(ref result);
            return result;
        }

        public bool CanSpendEvade()
        {
            bool result = false;
            if (HasToken(Token.Evade)) result = true;
            return result;
        }

        public bool CanSpendTargetLock()
        {
            bool result = false;
            //NOT IMPLEMENTED
            return result;
        }

        public bool CanPerformAction(DefaultAction action, bool afterMovement)
        {
            bool result = false;
            if (BuiltInActions.ContainsValue(action)) result = true;
            if (AlreadyExecutedActions.Contains(ActionToString(action)))
            {
                result = false;
            }
            OnTryPerformAction(ref result, afterMovement);
            return result;
        }

        public bool CanPerformFreeAction(DefaultAction action, bool afterMovement)
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
        }

        public void AssignStressToken()
        {
            AddToken(Token.Stress);
            AfterStressTokenIsAssigned(this);
        }

        public void TryRemoveStressToken()
        {
            if (HasToken(Token.Stress)) RemoveStressToken();
        }

        public void RemoveStressToken()
        {
            RemoveToken(Token.Stress);
            AfterStressTokenIsRemoved(this);
        }

        public void GenerateDiceModificationButtons()
        {
            AvailableDiceModifications = new Dictionary<string, DiceModification>();

            //Rewrite?
            if (Game.Selection.ActiveShip.CanSpendFocus())
            {
                AvailableDiceModifications.Add("Spend Focus", DefaultDiceMidifications["Focus"]);
            }
            if (Game.Selection.ActiveShip.CanSpendEvade() && (Game.Combat.AttackStep == CombatStep.Defence))
            {
                AvailableDiceModifications.Add("Spend Evade", DefaultDiceMidifications["Evade"]);
            }

            if (AfterGenerateDiceModifications != null) AfterGenerateDiceModifications(ref AvailableDiceModifications);

        }

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

        public void GenerateAvailableActionsList(bool afterMovement)
        {
            AvailableActionsList = new Dictionary<string, ShipActionExecution>();

            foreach (var action in BuiltInActions)
            {
                switch (action.Value)
                {
                    case DefaultAction.Focus:
                        if (CanPerformAction(DefaultAction.Focus, afterMovement)) AvailableActionsList.Add(action.Key, PerformFocusAction);
                        break;
                    case DefaultAction.Evade:
                        if (CanPerformAction(DefaultAction.Evade, afterMovement)) AvailableActionsList.Add(action.Key, PerformEvadeAction);
                        break;
                    case DefaultAction.TargetLock:
                        if (CanPerformAction(DefaultAction.TargetLock, afterMovement)) AvailableActionsList.Add(action.Key, delegate { });
                        break;
                    case DefaultAction.BarrelRoll:
                        if (CanPerformAction(DefaultAction.BarrelRoll, afterMovement)) AvailableActionsList.Add(action.Key, delegate { });
                        break;
                    default:
                        break;
                }
            }
            if (AfterAvailableActionListIsBuilt != null) AfterAvailableActionListIsBuilt(this, afterMovement);
        }

        public void GenerateAvailableFreeActionsList(bool afterMovement)
        {
            AvailableFreeActionsList = new Dictionary<string, ShipActionExecution>();

            foreach (var action in FreeActions)
            {
                switch (action.Value)
                {
                    case DefaultAction.Focus:
                        if (CanPerformFreeAction(DefaultAction.Focus, afterMovement)) AvailableFreeActionsList.Add(action.Key, PerformFocusAction);
                        break;
                    case DefaultAction.Evade:
                        if (CanPerformFreeAction(DefaultAction.Evade, afterMovement)) AvailableFreeActionsList.Add(action.Key, PerformEvadeAction);
                        break;
                    case DefaultAction.TargetLock:
                        if (CanPerformFreeAction(DefaultAction.TargetLock, afterMovement)) AvailableFreeActionsList.Add(action.Key, delegate { });
                        break;
                    case DefaultAction.BarrelRoll:
                        if (CanPerformFreeAction(DefaultAction.BarrelRoll, afterMovement)) AvailableFreeActionsList.Add(action.Key, delegate { });
                        break;
                    default:
                        break;
                }
            }

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

        public void AskPerformFreeAction(string name, DefaultAction action, bool afterMovement)
        {
            Game.Selection.isUIlocked = true;
            Game.Selection.isInTemporaryState = true;
            Game.UI.Helper.UpdateTemporaryState("Perform free action");
            FreeActions = new Dictionary<string, DefaultAction>();
            FreeActions.Add(name, action);
            Game.UI.ActionsPanel.ShowFreeActionsPanel(afterMovement);
        }

        public string ActionToString(DefaultAction action)
        {
            string result = "";
            switch (action)
            {
                case DefaultAction.Focus:
                    result = "Focus";
                    break;
                case DefaultAction.Evade:
                    result = "Evade";
                    break;
                case DefaultAction.TargetLock:
                    result = "Target Lock";
                    break;
                case DefaultAction.BarrelRoll:
                    result = "Barrel Roll";
                    break;
                default:
                    break;
            }
            return result;
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

        public void ChangeAgility(int value)
        {
            Agility += value;
            AfterStatsAreChanged(this);
        }

        public Dictionary<string, ManeuverColor> GetManeuvers()
        {
            Dictionary<string, ManeuverColor> result = new Dictionary<string, ManeuverColor>();

            foreach (var maneuverHolder in Maneuvers)
            {
                Movement movement =  Game.Movement.ManeuverFromString(maneuverHolder.Key);
                if (AfterGetManeuverColor!=null) AfterGetManeuverColor(this, ref movement);
                if (AfterGetManeuverAvailablity!=null) AfterGetManeuverAvailablity(this, ref movement);
                result.Add(maneuverHolder.Key, movement.ColorComplexity);
            }

            return result;
        }

        public void SufferCrit(CriticalHitCard.GenericCriticalHit crit)
        {
            if (OnAssignCrit!=null) OnAssignCrit(this, ref crit);

            if (crit != null)
            {
                SufferHullDamage();
                AssignedCrits.Add(crit);
                crit.AssignCrit(this);
            }
        }

    }

}
