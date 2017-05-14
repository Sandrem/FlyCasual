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
        public bool IsBumped { get; set; }

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

        public List<Tokens.GenericToken> AssignedTokens = new List<Tokens.GenericToken>();

        public bool isUnique = false;
        //public bool FactionRestriction

        protected List<Actions.GenericAction> BuiltInActions = new List<Actions.GenericAction>();
        private List<Actions.GenericAction> AlreadyExecutedActions = new List<Actions.GenericAction>();
        public List<Actions.GenericAction> AvailableActionEffects = new List<Actions.GenericAction>();
        private List<Actions.GenericAction> AvailableActionsList = new List<Actions.GenericAction>();
        private List<Actions.GenericAction> AvailableFreeActionsList = new List<Actions.GenericAction>();

        public List<CriticalHitCard.GenericCriticalHit> AssignedCrits = new List<CriticalHitCard.GenericCriticalHit>();

        public Dictionary<Upgrade.UpgradeSlot, int> BuiltInSlots = new Dictionary<Upgrade.UpgradeSlot, int>();

        public Dictionary<string, ManeuverColor> Maneuvers = new Dictionary<string, ManeuverColor>();
        public int ShipId { get; set; }

        public List<KeyValuePair<Upgrade.UpgradeSlot, Upgrade.GenericUpgrade>> InstalledUpgrades = new List<KeyValuePair<Upgrade.UpgradeSlot, Upgrade.GenericUpgrade>>();

        //EVENTS

        public delegate void EventHandler();
        public delegate void EventHandlerInt(ref int data);
        public delegate void EventHandlerBool(ref bool data);
        public delegate void EventHandlerActionBool(Actions.GenericAction action, ref bool data);
        public delegate void EventHandlerShip(GenericShip ship);
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
        public event EventHandlerShip AfterAvailableActionListIsBuilt;
        public event EventHandlerInt AfterGotNumberOfAttackDices;
        public event EventHandlerInt AfterGotNumberOfDefenceDices;
        public event EventHandlerInt AfterGetPilotSkill;
        public event EventHandlerInt AfterGetAgility;
        public event EventHandlerBool OnTrySpendFocus;
        public event EventHandlerBool OnTryReroll;
        public event EventHandlerActionBool OnTryPerformAction;
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

        public void AskPerformFreeAction(Actions.GenericAction action)
        {
            AvailableFreeActionsList = new List<Actions.GenericAction>();
            AddAvailableFreeAction(action);
            Game.UI.ActionsPanel.ShowFreeActionsPanel();
        }

        public void GenerateAvailableActionsList()
        {
            AvailableActionsList = new List<Actions.GenericAction>();

            foreach (var action in BuiltInActions)
            {
                AddAvailableAction(action);
            }

            if (AfterAvailableActionListIsBuilt != null) AfterAvailableActionListIsBuilt(this);
        }

        public bool CanPerformAction(Actions.GenericAction action)
        {
            bool result = true;

            OnTryPerformAction(action, ref result);

            return result;
        }

        public List<Actions.GenericAction> GetAvailableActionsList()
        {
            return AvailableActionsList;
        }

        public List<Actions.GenericAction> GetAvailableFreeActionsList()
        {
            return AvailableFreeActionsList;
        }

        public void AddAvailableAction(Actions.GenericAction action)
        {
            if (CanPerformAction(action))
            {
                AvailableActionsList.Add(action);
            }
        }

        public void AddAvailableFreeAction(Actions.GenericAction action)
        {
            if (CanPerformAction(action))
            {
                AvailableFreeActionsList.Add(action);
            }
        }

        public void AddAlreadyExecutedAction(Actions.GenericAction action)
        {
            AlreadyExecutedActions.Add(action);
        }

        public void ClearAlreadyExecutedActions()
        {
            AlreadyExecutedActions = new List<Actions.GenericAction>();
        }

        public void RemoveAlreadyExecutedAction(System.Type type)
        {
            List<Actions.GenericAction> keys = new List<Actions.GenericAction>(AlreadyExecutedActions);

            foreach (var executedAction in keys)
            {
                if (executedAction.GetType() == type)
                {
                    AlreadyExecutedActions.Remove(executedAction);
                    return;
                }
            }
        }

        public bool AlreadyExecutedAction(System.Type type)
        {
            bool result = false;
            foreach (var executedAction in AlreadyExecutedActions)
            {
                if (executedAction.GetType() == type)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        // ACTION EFFECTS

        public void GenerateDiceModificationButtons()
        {
            GenerateDiceModificationList();

            List<Actions.GenericAction> keys = new List<Actions.GenericAction>(AvailableActionEffects);

            foreach (var action in keys)
            {
                if (!(action.IsActionEffectAvailable()))
                {
                    AvailableActionEffects.Remove(action);
                }
            }

        }

        public void GenerateDiceModificationList()
        {
            AvailableActionEffects = new List<Actions.GenericAction>(); ;

            foreach (var token in AssignedTokens)
            {
                token.GetAvailableEffects(ref AvailableActionEffects);
            }

            if (AfterGenerateDiceModifications != null) AfterGenerateDiceModifications(ref AvailableActionEffects);
        }

        // TOKENS

        public Tokens.GenericToken GetToken(System.Type type, char letter = ' ')
        {
            Tokens.GenericToken result = null;

            foreach (var assignedToken in AssignedTokens)
            {
                if (assignedToken.GetType() == type)
                {
                    if (assignedToken.GetType().BaseType == typeof(Tokens.GenericTargetLockToken))
                    {
                        if (((assignedToken as Tokens.GenericTargetLockToken).Letter == letter) || (letter == '*'))
                        {
                            return assignedToken;
                        }
                    }
                    else
                    {
                        return assignedToken;
                    }
                }
            }
            return result;
        }

        public char GetTargetLockLetterPair(GenericShip targetShip)
        {
            char result = ' ';

            Tokens.GenericToken blueToken = GetToken(typeof(Tokens.BlueTargetLockToken), '*');
            if (blueToken != null)
            {
                char foundLetter = (blueToken as Tokens.BlueTargetLockToken).Letter;

                Tokens.GenericToken redToken = targetShip.GetToken(typeof(Tokens.RedTargetLockToken), foundLetter);
                if (redToken != null)
                {
                    return foundLetter;
                }
            }
            return result;
        }

        public void AssignToken(Tokens.GenericToken token, char letter = ' ')
        {
            Tokens.GenericToken assignedToken = GetToken(token.GetType(), letter);

            if (assignedToken != null)
            {
                assignedToken.Count++;
            }
            else                
            {
                AssignedTokens.Add(token);
            }

            if (AfterTokenIsAssigned != null) AfterTokenIsAssigned(this);
        }

        public void RemoveToken(System.Type type, char letter = ' ', bool recursive = false)
        {
            Tokens.GenericToken assignedToken = GetToken(type, letter);

            if (assignedToken != null)
            {

                if (assignedToken.Count > 1)
                {
                    assignedToken.Count--;
                    if (AfterTokenIsRemoved != null) AfterTokenIsRemoved(this);

                    if (recursive)
                    {
                        RemoveToken(type, letter, true);
                    }
                }
                else
                {
                    if (assignedToken.GetType().BaseType == typeof(Tokens.GenericTargetLockToken))
                    {
                        Game.Actions.ReleaseTargetLockLetter((assignedToken as Tokens.GenericTargetLockToken).Letter);
                    }
                    AssignedTokens.Remove(assignedToken);
                    if (AfterTokenIsRemoved != null) AfterTokenIsRemoved(this);
                }
            }
        }

        public void SpendToken(System.Type type, char letter = ' ')
        {
            RemoveToken(type, letter);
        }

        public void ClearAllTokens()
        {
            List<Tokens.GenericToken> keys = new List<Tokens.GenericToken>(AssignedTokens);

            foreach (var token in keys)
            {
                if (token.Temporary)
                {
                    RemoveToken(token.GetType(), '*', true);
                }
            }
        }

    }

}
