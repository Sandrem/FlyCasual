using Upgrade;
using System;
using Ship;
using Abilities;
using UnityEngine;
using Conditions;
using System.Linq;
using SubPhases;
using ActionsList;
using UpgradesList;
using RuleSets;

namespace UpgradesList
{
    public class DirectorKrennic : GenericUpgrade, ISecondEditionUpgrade
    {
        public DirectorKrennic() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Director Krennic";
            Cost = 5;

            isUnique = true;
                        
            UpgradeAbilities.Add(new DirectorKrennicAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            UpgradeAbilities.RemoveAll(a => a is DirectorKrennicAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.DirectorKrennicAbilitySE());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Imperial;
        }
    }
}

namespace Abilities
{
    public class DirectorKrennicAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnSetupStart += RegisterDirectorKrennicAbility;
            SubscribeToAttackFinish();
        }

        protected virtual void SubscribeToAttackFinish()
        {
            GenericShip.OnAttackFinishGlobal += OptimizedPrototypeKrennicTargetLockEffect;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupStart -= RegisterDirectorKrennicAbility;
            GenericShip.OnAttackFinishGlobal -= OptimizedPrototypeKrennicTargetLockEffect;
        }

        private void RegisterDirectorKrennicAbility()
        {            
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Director Krennic decision",
                TriggerType = TriggerTypes.OnSetupStart,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = SelectDirectorKrennicTarget,
            });
        }

        private void SelectDirectorKrennicTarget(object Sender, System.EventArgs e)
        {
            SelectTargetForAbility(
                  AssignOptimizedPrototype,
                  IsGalacticEmpireShipWith3OrLessShields,
                  GetAiOptimizedPrototypePriority,
                  HostShip.Owner.PlayerNo,
                  true,
                  null,
                  HostUpgrade.Name,
                  "Choose Galactic Empire ship with 3 or fewer Shields.\nIt gets Optimized Prototype condition.",
                  HostUpgrade.ImageUrl
              );
        }

        protected virtual void AssignOptimizedPrototype()
        {
            TargetShip.Tokens.AssignCondition(typeof(OptimizedPrototype));
            SelectShipSubPhase.FinishSelection();
        }

        private bool IsGalacticEmpireShipWith3OrLessShields(GenericShip ship)
        {
            var match = ship.Owner.PlayerNo == HostShip.Owner.PlayerNo && ship.MaxShields <= 3 && ship.SubFaction == SubFaction.GalacticEmpire;
            return match;
        }

        private int GetAiOptimizedPrototypePriority(GenericShip ship)
        {
            int result = 0;

            result += ship.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.Cost);

            return result;
        }

        private void OptimizedPrototypeKrennicTargetLockEffect(GenericShip ship)
        {
            if (ship.ShipId == Combat.Attacker.ShipId)
            {
                var onRangePrototypeAttacked = Combat.Attacker.Owner.PlayerNo == HostShip.Owner.PlayerNo
                    && Combat.Attacker.Tokens.HasToken<OptimizedPrototype>()
                    && Combat.ChosenWeapon is PrimaryWeaponClass
                    && BoardTools.Board.GetRangeOfShips(Combat.Attacker, HostShip) <= 2;
                if (onRangePrototypeAttacked)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskAcquireTargetLock);
                }
            }
        }

        private void AskAcquireTargetLock(object sender, System.EventArgs e)
        {
            AskToUseAbility(AlwaysUseByDefault, AcquireTargetLock, null, null, true, string.Format("Does {0} want to acquire target lock on {1}?", HostShip.PilotName, Combat.Defender.PilotName));
        }

        private void AcquireTargetLock(object sender, System.EventArgs e)
        {
            Messages.ShowInfo(string.Format("Optimized Prototype: {0} gets target lock on {1}", HostShip.PilotName, Combat.Defender.PilotName));
            Actions.AcquireTargetLock(HostShip, Combat.Defender, DecisionSubPhase.ConfirmDecision, DecisionSubPhase.ConfirmDecision);
        }
    }

    namespace SecondEdition
    {
        public class DirectorKrennicAbilitySE : DirectorKrennicAbility
        {
            protected override void AssignOptimizedPrototype()
            {
                TargetShip.Tokens.AssignCondition(typeof(OptimizedPrototypeSE));
                SelectShipSubPhase.FinishSelection();
            }

            protected override void SubscribeToAttackFinish() { }
        }
    }
}

namespace Conditions
{
    public class OptimizedPrototype : Tokens.GenericToken
    {
        public OptimizedPrototype(GenericShip host) : base(host)
        {
            Name = "Optimized Prototype Condition";
            Temporary = false;
            Tooltip = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/conditions/optimized-prototype.png";            
        }

        public override void WhenAssigned()
        {
            SubscribeToOptimizedPrototypeConditionEffects();
        }

        public override void WhenRemoved()
        {
            UnsubscribeFromOptimizedPrototypeConditionEffects();
        }

        private void SubscribeToOptimizedPrototypeConditionEffects()
        {
            Host.MaxShields++;
            Host.TryRegenShields();

            Roster.UpdateDamageIndicators(Host, Host.InfoPanel);

            Host.AfterGenerateAvailableActionEffectsList += AddOptimizedPrototypeCancelResultModification;
        }
                
        private void UnsubscribeFromOptimizedPrototypeConditionEffects()
        {
            Host.MaxShields--;
            Host.AfterGenerateAvailableActionEffectsList -= AddOptimizedPrototypeCancelResultModification;            
        }

        private void AddOptimizedPrototypeCancelResultModification(GenericShip ship)
        {
            OptimizedPrototypeAction action = new OptimizedPrototypeAction()
            {
                Host = Host                
            };

            Host.AddAvailableActionEffect(action);
        }
    }

    public class OptimizedPrototypeSE : Tokens.GenericToken
    {
        public OptimizedPrototypeSE(GenericShip host) : base(host)
        {
            Name = "Optimized Prototype Condition";
            Temporary = false;

            //TODO: URL
            Tooltip = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/conditions/optimized-prototype.png";
        }

        public override void WhenAssigned()
        {
            Host.AfterGenerateAvailableActionEffectsList += AddOptimizedPrototypeCancelResultModification;
        }

        public override void WhenRemoved()
        {
            Host.AfterGenerateAvailableActionEffectsList -= AddOptimizedPrototypeCancelResultModification;
        }

        private void AddOptimizedPrototypeCancelResultModification(GenericShip ship)
        {
            GenericAction action = new ActionsList.SecondEdition.OptimizedPrototypeDiceModificationSE()
            {
                Host = Host
            };

            Host.AddAvailableActionEffect(action);
        }
    }
}

namespace ActionsList
{
    public class OptimizedPrototypeAction : GenericAction
    {
        public OptimizedPrototypeAction()
        {
            Name = EffectName = "Optimized Prototype";            
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = true;

            if (Combat.AttackStep != CombatStep.Attack) result = false;
            if (!(Combat.ChosenWeapon is PrimaryWeaponClass)) result = false;
            if (!Combat.DiceRollAttack.ResultsArray.Any()) result = false;
            if (Combat.Defender.Shields == 0) result = false;

            return result;
        }

        private class OptimizedPrototypeDecisionSubPhase : DecisionSubPhase { }

        public override void ActionEffect(System.Action callBack)
        {
            var newSubPhase = Phases.StartTemporarySubPhaseNew<OptimizedPrototypeDecisionSubPhase>(Name, callBack);

            newSubPhase.RequiredPlayer = Host.Owner.PlayerNo;
            newSubPhase.InfoText = "Spend die result to make defender lose a shield?";
            newSubPhase.ShowSkipButton = true;
            newSubPhase.OnSkipButtonIsPressed = DontUseOptimizedPrototype;

            if (Combat.DiceRollAttack.Blanks > 0)
            {
                newSubPhase.AddDecision("Spend Blank", (s, o) => SpendBlankForEffect(DieSide.Blank));
            }
            if (Combat.DiceRollAttack.Focuses > 0)
            {
                newSubPhase.AddDecision("Spend Eye", (s, o) => SpendBlankForEffect(DieSide.Focus));
            }
            if (Combat.DiceRollAttack.RegularSuccesses > 0)
            {
                newSubPhase.AddDecision("Spend Hit", (s, o) => SpendBlankForEffect(DieSide.Success));
            }
            if (Combat.DiceRollAttack.CriticalSuccesses > 0)
            {
                newSubPhase.AddDecision("Spend Critical Hit", (s, o) => SpendBlankForEffect(DieSide.Crit));
            }

            newSubPhase.DefaultDecisionName = newSubPhase.GetDecisions().Select(d => d.Name).FirstOrDefault();
            newSubPhase.Start();
        }

        private void SpendBlankForEffect(DieSide side)
        {
            Combat.DiceRollAttack.RemoveType(side);
            DefenderSuffersDamage();            
        }

        private void DefenderSuffersDamage()
        {            
            Combat.Defender.AssignedDamageDiceroll.AddDice(DieSide.Success);

            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = "Damage from Optimized Prototype",
                    TriggerType = TriggerTypes.OnDamageIsDealt,
                    TriggerOwner = Combat.Defender.Owner.PlayerNo,
                    EventHandler = Combat.Defender.SufferDamage,
                    EventArgs = new DamageSourceEventArgs()
                    {
                        Source = Combat.Attacker,
                        DamageType = DamageTypes.CardAbility
                    },
                    Skippable = true
                }
            );

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, DecisionSubPhase.ConfirmDecision);
        }

        private void DontUseOptimizedPrototype()
        {
            DecisionSubPhase.ConfirmDecision();
        }
         
        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (IsActionEffectAvailable())
            {
                if (Combat.DiceRollAttack.Blanks > 0)
                {
                    result = 90;
                }
                else if (Combat.DiceRollAttack.Focuses > 0 && Combat.Attacker.GetAvailableActionEffectsList().Count(n => n.IsTurnsAllFocusIntoSuccess) == 0)
                {
                    result = 90;
                }
                else if (Combat.DiceRollAttack.Focuses > 0)
                {
                    result = 30;
                }
            }

            return result;
        }


    }
}

namespace ActionsList
{
    namespace SecondEdition
    {
        public class OptimizedPrototypeDiceModificationSE : GenericAction
        {
            public OptimizedPrototypeDiceModificationSE()
            {
                Name = EffectName = "Optimized Prototype";
            }

            public override void ActionEffect(Action callBack)
            {
                //TODO: If you do, choose one: the defender loses 1 shield or the defender flips 1 of its facedown damage cards.
            }

            public override bool IsActionAvailable()
            {
                //TODO: forward primary attack
                //TODO: Check TL
                //TODO: At least one non-blank die must be in pool

                return true;
            }

            public override int GetActionEffectPriority()
            {
                return 0;
            }
        }
    }
}