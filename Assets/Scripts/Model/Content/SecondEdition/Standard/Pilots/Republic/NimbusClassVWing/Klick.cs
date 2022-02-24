using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.NimbusClassVWing
    {
        public class Klick : NimbusClassVWing
        {
            public Klick() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Klick\"",
                    "GC-1000",
                    Faction.Republic,
                    4,
                    3,
                    8,
                    isLimited: true,
                    charges: 1,
                    regensCharges: 1,
                    abilityType: typeof(Abilities.SecondEdition.KlickAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Astromech,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone,
                        Tags.Tie
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/d6/ba/d6baed95-5960-4615-9949-faf5a5c0d96f/swz80_ship_klick.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KlickAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal += RegisterKlickAttackBonusPreventionAbility;
            GenericShip.OnAttackStartAsDefenderGlobal += RegisterKlickDefenseBonusPreventionAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= RegisterKlickAttackBonusPreventionAbility;
            HostShip.OnAttackStartAsDefender -= RegisterKlickDefenseBonusPreventionAbility;

            Rules.DistanceBonus.OnCheckPreventRangeOneBonus -= PreventRangeOneBonus;
            Rules.DistanceBonus.OnCheckPreventRangeThreeBonus -= PreventRangeThreeBonus;
        }

        private void RegisterKlickAttackBonusPreventionAbility()
        {
            // No check for range - Grant Inquisitor's ability can be used

            if (HostShip.State.Charges == 0) return;

            if (!ActionsHolder.HasTargetLockOn(HostShip, Combat.Attacker)) return;

            DistanceInfo distInfo = new DistanceInfo(HostShip, Combat.Attacker);
            if (distInfo.Range < 1 || distInfo.Range > 3) return;

            RegisterAbilityTrigger(TriggerTypes.OnAttackStart, delegate
            {
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    UseIfRangeIsOne,
                    UseRangeOneBonusPreventionAbility,
                    descriptionLong: $"Do you want to spend 1 Charge to prevent the range 0-1 bonus?\n(Attack range is {Combat.ShotInfo.Range})",
                    imageHolder: HostShip
                );
            });
        }

        private bool UseIfRangeIsOne()
        {
            return Combat.ShotInfo.Range < 2;
        }

        private void RegisterKlickDefenseBonusPreventionAbility()
        {
            if (HostShip.State.Charges == 0) return;

            if (!ActionsHolder.HasTargetLockOn(HostShip, Combat.Defender)) return;

            DistanceInfo distInfo = new DistanceInfo(HostShip, Combat.Defender);
            if (distInfo.Range < 1 || distInfo.Range > 3) return;

            ShotInfo shotInfo = new ShotInfo(Combat.Attacker, Combat.Defender, Combat.Attacker.PrimaryWeapons);
            if (shotInfo.Range != 3) return;

            RegisterAbilityTrigger(TriggerTypes.OnAttackStart, delegate
            {
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    AlwaysUseByDefault,
                    UseRangeThreeBonusPreventionAbility,
                    descriptionLong: "Do you want to spend 1 Charge to prevent the range 3 bonus?",
                    imageHolder: HostShip
                );
            });
        }

        private void UseRangeOneBonusPreventionAbility(object sender, EventArgs e)
        {
            Rules.DistanceBonus.OnCheckPreventRangeOneBonus += PreventRangeOneBonus;
            HostShip.State.Charges--;
            DecisionSubPhase.ConfirmDecision();
        }

        private void UseRangeThreeBonusPreventionAbility(object sender, EventArgs e)
        {
            Rules.DistanceBonus.OnCheckPreventRangeThreeBonus += PreventRangeThreeBonus;
            HostShip.State.Charges--;
            DecisionSubPhase.ConfirmDecision();
        }

        private void PreventRangeOneBonus(ref bool isActive)
        {
            Rules.DistanceBonus.OnCheckPreventRangeOneBonus -= PreventRangeOneBonus;

            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: Range 0-1 bonus is prevented");
            isActive = false;
        }

        private void PreventRangeThreeBonus(ref bool isActive)
        {
            Rules.DistanceBonus.OnCheckPreventRangeThreeBonus -= PreventRangeThreeBonus;

            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: Range 3 bonus is prevented");
            isActive = false;
        }
    }
}