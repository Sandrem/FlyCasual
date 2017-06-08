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

    public partial class GenericShip
    {
        protected GameManagerScript Game;

        public int ShipId { get; set; }

        public Movement AssignedManeuver { get; set; }
        public bool IsSetupPerformed { get; set; }
        public bool IsManeuverPerformed { get; set; }
        public bool IsAttackPerformed { get; set; }
        public bool IsDestroyed { get; set; }
        public bool IsBumped { get; set; }
        public bool isUnique = false;
        public bool IsSkipsAction { get; set; }

        public Faction faction;
        public List<Faction> factions = new List<Faction>();

        public string Type { get; set; }
        public Vector3 StartingPosition { get; set; }
        public string PilotName { get;  set; }
        public string ImageUrl { get; set; }
        public string ManeuversImageUrl { get; set; }
        public Players.GenericPlayer Owner { get; set; }

        public int Firepower { get; set; }
        public int MaxHull { get; set; }
        public int Hull { get; set; }
        public int MaxShields { get; set; }
        public int Shields { get; set; }

        public List<Collider> ObstaclesLanded = new List<Collider>();
        public List<Collider> ObstaclesHit = new List<Collider>();

        public GenericAiTable HotacManeuverTable;

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

        public GameObject Model { get; set; }
        public GameObject InfoPanel { get; set;  }
        public GenericShip LastShipCollision { get; set; }

        protected   List<ActionsList.GenericAction> BuiltInActions              = new List<ActionsList.GenericAction>();
        private     List<ActionsList.GenericAction> AvailableActionsList        = new List<ActionsList.GenericAction>();
        private     List<ActionsList.GenericAction> AvailableFreeActionsList    = new List<ActionsList.GenericAction>();
        private     List<ActionsList.GenericAction> AlreadyExecutedActions      = new List<ActionsList.GenericAction>();
        public      List<ActionsList.GenericAction> AvailableActionEffects      = new List<ActionsList.GenericAction>();

        public List<Tokens.GenericToken> AssignedTokens = new List<Tokens.GenericToken>();
        public List<CriticalHitCard.GenericCriticalHit> AssignedCrits = new List<CriticalHitCard.GenericCriticalHit>();
        public Dictionary<Upgrade.UpgradeSlot, int> BuiltInSlots = new Dictionary<Upgrade.UpgradeSlot, int>();
        public Dictionary<string, ManeuverColor> Maneuvers = new Dictionary<string, ManeuverColor>();
        public List<KeyValuePair<Upgrade.UpgradeSlot, Upgrade.GenericUpgrade>> InstalledUpgrades = new List<KeyValuePair<Upgrade.UpgradeSlot, Upgrade.GenericUpgrade>>();

        //EVENTS

        public delegate void EventHandler();
        public delegate void EventHandlerInt(ref int data);
        public delegate void EventHandlerBool(ref bool data);
        public delegate void EventHandlerActionBool(ActionsList.GenericAction action, ref bool data);
        public delegate void EventHandlerShip(GenericShip ship);
        public delegate void EventHandlerActionEffectsList(ref List<ActionsList.GenericAction> list);
        public delegate void EventHandlerShipMovement(GenericShip ship, ref Movement movement);
        public delegate void EventHandlerShipCrit(GenericShip ship, ref CriticalHitCard.GenericCriticalHit crit);

        public event EventHandler OnDestroyed;
        public event EventHandler OnDefence;
        public event EventHandler OnAttack;
        public event EventHandlerShip OnMovementStart;
        public event EventHandlerShip OnMovementFinish;
        public event EventHandlerShip OnPositionFinish;
        public event EventHandlerShip OnMovementFinishWithColliding;
        public event EventHandlerShip OnMovementFinishWithoutColliding;
        public event EventHandlerShip AfterTokenIsAssigned;
        public event EventHandlerShip AfterTokenIsRemoved;
        public event EventHandlerShip AfterAssignedDamageIsChanged;
        public event EventHandlerShip AfterStatsAreChanged;
        public event EventHandlerShip AfterAvailableActionListIsBuilt;
        public event EventHandlerShip AfterAttackWindow;
        public event EventHandlerShip OnCombatPhaseStart;
        public event EventHandlerShip OnLandedOnObstacle;
        public event EventHandlerInt AfterGotNumberOfPrimaryWeaponAttackDices;
        public event EventHandlerInt AfterGotNumberOfPrimaryWeaponDefenceDices;
        public event EventHandlerInt AfterGetPilotSkill;
        public event EventHandlerInt AfterGetAgility;
        public event EventHandlerBool OnTrySpendFocus;
        public event EventHandlerBool OnTryReroll;
        public event EventHandlerBool OnTryPerformAttack;
        public event EventHandlerBool OnCheckFaceupCrit;
        public event EventHandlerActionBool OnTryPerformAction;
        public event EventHandlerActionEffectsList AfterGenerateDiceModifications;
        public event EventHandlerShipMovement AfterGetManeuverColor;
        public event EventHandlerShipMovement AfterGetManeuverAvailablity;
        public event EventHandlerShipCrit OnAssignCrit;

        public GenericShip()
        {
            AddUpgradeSlot(Upgrade.UpgradeSlot.Modification);
        }

        public void InitializeGenericShip(Players.PlayerNo playerNo, int shipId, Vector3 position)
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

            Owner = Roster.GetPlayer(playerNo);
            ShipId = shipId;
            StartingPosition = position;

            BuiltInActions.Add(new ActionsList.FocusAction());

            CreateModel(StartingPosition);
        }

        public virtual void InitializeShip()
        {
            Shields = MaxShields;
            Hull = MaxHull;
        }

        public virtual void InitializePilot()
        {
            SetShipInstertImage();
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
            int result = 0;

            if (Combat.SecondaryWeapon == null)
            {
                result = Firepower;
                AfterGotNumberOfPrimaryWeaponAttackDices(ref result);
            }
            else
            {
                result = Combat.SecondaryWeapon.GetAttackValue();
            }
            
            if (result < 0) result = 0;
            return result;
        }

        public int GetNumberOfDefenceDices(GenericShip attackerShip)
        {
            int result = Agility;

            if (Combat.SecondaryWeapon == null)
            {
                AfterGotNumberOfPrimaryWeaponDefenceDices(ref result);
            }
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
                foreach (Dice dice in damage.DiceList)
                {
                    if ((dice.Side == DiceSide.Success) || (dice.Side == DiceSide.Crit))
                    {
                        if (CheckFaceupCrit(dice))
                        {
                            CriticalHitsDeck.DrawCrit(this);
                        }
                        else
                        {
                            SufferHullDamage();
                        }
                    }
                }
            }

            AfterAssignedDamageIsChanged(this);
        }

        private bool CheckFaceupCrit(Dice dice)
        {
            bool result = false;

            if (dice.Side == DiceSide.Crit) result = true;

            if (OnCheckFaceupCrit != null) OnCheckFaceupCrit(ref result);

            return result;
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
                Roster.DestroyShip(this.GetTag());
                OnDestroyed();
                IsDestroyed = true;
            }
        }

        //TRIGGERS

        //todo: think about name
        public void AttackStart()
        {
            //TODO: move ot rule
            if (Combat.Attacker.ShipId == this.ShipId) IsAttackPerformed = true;
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

        public void FinishPosition()
        {
            OnPositionFinish(this);
        }

        public void FinishMovementWithColliding()
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
            Upgrade.GenericUpgrade newUpgrade = (Upgrade.GenericUpgrade)System.Activator.CreateInstance(System.Type.GetType(upgradeName));

            Upgrade.UpgradeSlot slot = newUpgrade.Type;
            if (HasFreeUpgradeSlot(slot))
            {
                newUpgrade.AttachToShip(this);
                InstalledUpgrades.Add(new KeyValuePair<Upgrade.UpgradeSlot, Upgrade.GenericUpgrade>(newUpgrade.Type, newUpgrade));
                Roster.UpdateUpgradesPanel(this, this.InfoPanel);
                Roster.OrganizeRosters();
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

        public void AskPerformFreeAction(ActionsList.GenericAction action)
        {
            AvailableFreeActionsList = new List<ActionsList.GenericAction>();
            AddAvailableFreeAction(action);
            Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).PerformFreeAction();
        }

        public void GenerateAvailableActionsList()
        {
            AvailableActionsList = new List<ActionsList.GenericAction>();

            foreach (var action in BuiltInActions)
            {
                AddAvailableAction(action);
            }

            if (AfterAvailableActionListIsBuilt != null) AfterAvailableActionListIsBuilt(this);
        }

        public bool CanPerformAction(ActionsList.GenericAction action)
        {
            bool result = true;

            if (OnTryPerformAction != null) OnTryPerformAction(action, ref result);

            return result;
        }

        public bool CallTryPerformAttack(bool result = true)
        {
            if (OnTryPerformAttack != null) OnTryPerformAttack(ref result);

            return result;
        }

        public void CallOnCombatPhaseStart()
        {
            if (OnCombatPhaseStart != null) OnCombatPhaseStart(this);
        }

        public void CheckLandedOnObstacle()
        {
            if (ObstaclesLanded.Count > 0)
            {
                foreach (var obstacle in ObstaclesLanded)
                {
                    if (!ObstaclesHit.Contains(obstacle)) ObstaclesHit.Add(obstacle);
                }

                Game.UI.ShowError("Landed on obstacle");
                if (OnLandedOnObstacle != null) OnLandedOnObstacle(this);
            }
        }

        public List<ActionsList.GenericAction> GetAvailableActionsList()
        {
            return AvailableActionsList;
        }

        public List<ActionsList.GenericAction> GetAvailableFreeActionsList()
        {
            return AvailableFreeActionsList;
        }

        public void AddAvailableAction(ActionsList.GenericAction action)
        {
            if (CanPerformAction(action))
            {
                AvailableActionsList.Add(action);
            }
        }

        public void AddAvailableFreeAction(ActionsList.GenericAction action)
        {
            if (CanPerformAction(action))
            {
                AvailableFreeActionsList.Add(action);
            }
        }

        public void AddAlreadyExecutedAction(ActionsList.GenericAction action)
        {
            AlreadyExecutedActions.Add(action);
        }

        public void ClearAlreadyExecutedActions()
        {
            AlreadyExecutedActions = new List<ActionsList.GenericAction>();
        }

        public void RemoveAlreadyExecutedAction(System.Type type)
        {
            List<ActionsList.GenericAction> keys = new List<ActionsList.GenericAction>(AlreadyExecutedActions);

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

            List<ActionsList.GenericAction> keys = new List<ActionsList.GenericAction>(AvailableActionEffects);

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
            AvailableActionEffects = new List<ActionsList.GenericAction>(); ;

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
                        Actions.ReleaseTargetLockLetter((assignedToken as Tokens.GenericTargetLockToken).Letter);
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

        public int GetAttackTypes(int distance, bool inArc)
        {
            int result = 0;

            if (CanShootWithPrimaryWeaponAt(distance, inArc)) result++;

            foreach (var upgrade in InstalledUpgrades)
            {
                if (upgrade.Value.Type == Upgrade.UpgradeSlot.Torpedoes)
                {
                    if ((upgrade.Value as Upgrade.GenericSecondaryWeapon).IsShotAvailable(Selection.AnotherShip)) result++;
                }
            }

            return result;
        }

        public bool CanShootWithPrimaryWeaponAt(GenericShip anotherShip)
        {
            bool result = true;
            int distance = Actions.GetFiringRange(this, anotherShip);
            bool inArc = Actions.InArcCheck(this, anotherShip);
            result = CanShootWithPrimaryWeaponAt(distance, inArc);
            return result;
        }

        public bool CanShootWithPrimaryWeaponAt(int distance, bool inArc)
        {
            bool result = true;
            if (distance > 3) return false;
            //if (distance < 1) return false;
            if (!inArc) return false;
            return result;
        }

        public void CallAfterAttackWindow()
        {
            if (AfterAttackWindow != null) AfterAttackWindow(this);
        }
    }

}
