﻿using Ship;
using Upgrade;
using SubPhases;
using Conditions;
using System.Linq;
using Tokens;
using ActionsList;
using System;
using Actions;

namespace UpgradesList.SecondEdition
{
    public class DirectorKrennic : GenericUpgrade
    {
        public DirectorKrennic() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Director Krennic",
                UpgradeType.Crew,
                cost: 4,
                isLimited: true,
                addAction: new ActionInfo(typeof(TargetLockAction)),
                restriction: new FactionRestriction(Faction.Imperial),
                abilityType: typeof(Abilities.SecondEdition.DirectorKrennicAbility),
                seImageNumber: 114
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class DirectorKrennicAbility : Abilities.FirstEdition.DirectorKrennicAbility
    {
        protected override string Prompt
        {
            get
            {
                return "Choose another friendly ship.\nIt gets the Optimized Prototype condition.";
            }
        }

        protected override void AssignOptimizedPrototype()
        {
            TargetShip.Tokens.AssignCondition(new OptimizedPrototypeSE(TargetShip) { SourceUpgrade = HostUpgrade });
            SelectShipSubPhase.FinishSelection();
        }

        protected override void SecondAbility() { }

        protected override bool CheckRequirements(GenericShip ship)
        {
            var match = ship.Owner.PlayerNo == HostShip.Owner.PlayerNo
                && ship.ShipId != HostShip.ShipId;
            return match;
        }
    }
}

namespace Conditions
{
    public class OptimizedPrototypeSE : GenericToken
    {
        public GenericUpgrade SourceUpgrade;

        public OptimizedPrototypeSE(GenericShip host) : base(host)
        {
            Name = ImageName = "Optimized Prototype Condition";
            Temporary = false;

            Tooltip = "https://raw.githubusercontent.com/Sandrem/xwing-data2-test/master/images/conditions/optimized-prototype.png";
        }

        public override void WhenAssigned()
        {
            Host.OnGenerateDiceModifications += AddOptimizedPrototypeCancelResultModification;
        }

        public override void WhenRemoved()
        {
            Host.OnGenerateDiceModifications -= AddOptimizedPrototypeCancelResultModification;
        }

        private void AddOptimizedPrototypeCancelResultModification(GenericShip ship)
        {
            GenericAction action = new ActionsList.SecondEdition.OptimizedPrototypeDiceModificationSE()
            {
                HostShip = Host,
                ImageUrl = Tooltip,
                Source = SourceUpgrade
            };

            Host.AddAvailableDiceModification(action);
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
                Name = DiceModificationName = "Optimized Prototype";
            }

            public override bool IsDiceModificationAvailable()
            {
                if (Combat.AttackStep != CombatStep.Attack) return false;
                if (Combat.ChosenWeapon.WeaponType != WeaponTypes.PrimaryWeapon) return false;
                if (!Combat.Attacker.SectorsInfo.IsShipInSector(Combat.Defender, Arcs.ArcType.Front)) return false;
                if (Combat.DiceRollAttack.Focuses == 0 && Combat.DiceRollAttack.Successes == 0) return false;
                if (!IsLockedByFriendlyKrennicShip()) return false;
                if (!IsWayToHarmPreset()) return false;

                return true;
            }

            private bool IsLockedByFriendlyKrennicShip()
            {
                GenericShip friendlyKrennicShip = HostShip.Owner.Ships.Values.FirstOrDefault(n => n.UpgradeBar.GetUpgradesOnlyFaceup().Any(u => u is UpgradesList.SecondEdition.DirectorKrennic));
                if (friendlyKrennicShip == null)
                {
                    return false;
                }
                else
                {
                    foreach (BlueTargetLockToken token in friendlyKrennicShip.Tokens.GetTokens<BlueTargetLockToken>('*'))
                    {
                        if (token.OtherTargetLockTokenOwner == Combat.Defender) return true;
                    }
                }
                return false;
            }

            private bool IsWayToHarmPreset()
            {
                return Combat.Defender.State.ShieldsCurrent != 0
                    || Combat.Defender.Damage.GetFacedownCards().Count != 0;
            }

            private class OptimizedPrototypeDecisionSubPhase : DecisionSubPhase { }

            public override void ActionEffect(Action callBack)
            {
                StartSubphaseTrigger(callBack);
            }

            private void StartSubphaseTrigger(Action callback)
            {
                Triggers.RegisterTrigger(
                    new Trigger()
                    {
                        Name = "Optimized Prototype Decision",
                        TriggerOwner = HostShip.Owner.PlayerNo,
                        TriggerType = TriggerTypes.OnAbilityDirect,
                        EventHandler = StartSubphase
                    }
                );

                Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, callback);
            }

            private void StartSubphase(object sender, System.EventArgs e)
            {
                var newSubPhase = Phases.StartTemporarySubPhaseNew<OptimizedPrototypeDecisionSubPhase>(Name, Triggers.FinishTrigger);

                newSubPhase.DescriptionShort = "Director Krennic's Optimized Prototype";
                newSubPhase.DescriptionLong = "Choose what effect to apply to the defender:";
                newSubPhase.ImageSource = Source;

                newSubPhase.RequiredPlayer = HostShip.Owner.PlayerNo;
                newSubPhase.ShowSkipButton = true;
                newSubPhase.OnSkipButtonIsPressed = DontUseOptimizedPrototype;

                if (Combat.Defender.State.ShieldsCurrent > 0)
                {
                    if (Combat.DiceRollAttack.Focuses > 0)
                    {
                        newSubPhase.AddDecision("Spend Focus result to remove shield", (s, o) => SpendDieForRemoveShieldEffect(DieSide.Focus));
                    }
                    if (Combat.DiceRollAttack.RegularSuccesses > 0)
                    {
                        newSubPhase.AddDecision("Spend Hit result to remove shield", (s, o) => SpendDieForRemoveShieldEffect(DieSide.Success));
                    }
                    if (Combat.DiceRollAttack.CriticalSuccesses > 0)
                    {
                        newSubPhase.AddDecision("Spend Crit result to remove shield", (s, o) => SpendDieForRemoveShieldEffect(DieSide.Crit));
                    }
                }

                if (Combat.Defender.Damage.HasFacedownCards)
                {
                    if (Combat.DiceRollAttack.Focuses > 0)
                    {
                        newSubPhase.AddDecision("Spend Focus result to flip facedown damage card", (s, o) => SpendDieForExposeDamageCardEffect(DieSide.Focus));
                    }
                    if (Combat.DiceRollAttack.RegularSuccesses > 0)
                    {
                        newSubPhase.AddDecision("Spend Hit result to flip facedown damage card", (s, o) => SpendDieForExposeDamageCardEffect(DieSide.Success));
                    }
                    if (Combat.DiceRollAttack.CriticalSuccesses > 0)
                    {
                        newSubPhase.AddDecision("Spend Crit result to flip facedown damage card", (s, o) => SpendDieForExposeDamageCardEffect(DieSide.Crit));
                    }
                }

                newSubPhase.DefaultDecisionName = newSubPhase.GetDecisions().Select(d => d.Name).FirstOrDefault();
                newSubPhase.Start();
            }

            private void SpendDieForRemoveShieldEffect(DieSide side)
            {
                DecisionSubPhase.ConfirmDecisionNoCallback();
                Combat.DiceRollAttack.RemoveType(side);
                DefenderLosesShield();
            }

            private void DefenderLosesShield()
            {
                Combat.Defender.LoseShield();
                Triggers.FinishTrigger();
            }

            private void DontUseOptimizedPrototype()
            {
                DecisionSubPhase.ConfirmDecision();
            }

            public override int GetDiceModificationPriority()
            {
                int result = 0;

                if (IsWayToHarmPreset()) result = 53;

                return result;
            }

            private void SpendDieForExposeDamageCardEffect(DieSide side)
            {
                DecisionSubPhase.ConfirmDecisionNoCallback();
                Combat.DiceRollAttack.RemoveType(side);
                DefenderExposesDamageCard();
            }

            private void DefenderExposesDamageCard()
            {
                Combat.Defender.Damage.ExposeRandomFacedownCard(Triggers.FinishTrigger);
            }
        }
    }
}