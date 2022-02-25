using Content;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEAdvancedV1
    {
        public class GrandInquisitor : TIEAdvancedV1
        {
            public GrandInquisitor() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Grand Inquisitor",
                    "Master of the Inquisitorious",
                    Faction.Imperial,
                    5,
                    6,
                    17,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.GrandInquisitorAbility),
                    force: 2,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.ForcePower,
                        UpgradeType.ForcePower,
                        UpgradeType.Talent,
                        UpgradeType.Missile
                    },
                    tags: new List<Tags>
                    {
                        Tags.DarkSide,
                        Tags.Tie
                    },
                    seImageNumber: 99
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GrandInquisitorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += RegisterInquisitorAttackAbility;
            HostShip.OnAttackStartAsDefender += RegisterInquisitorDefenseAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= RegisterInquisitorAttackAbility;
            HostShip.OnAttackStartAsDefender -= RegisterInquisitorDefenseAbility;

            Rules.DistanceBonus.OnCheckAllowRangeOneBonus -= ApplyRangeOneBonus;
            Rules.DistanceBonus.OnCheckPreventRangeOneBonus -= PreventRangeOneBonus;
        }

        private void RegisterInquisitorAttackAbility()
        {
            if (HostShip.State.Force < 1)
                return;

            if (Combat.ShotInfo.Range == 1)
                return;

            RegisterAbilityTrigger(TriggerTypes.OnAttackStart, delegate
            {
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    AlwaysUseByDefault,
                    UseInquisitorAttackAbility,
                    descriptionLong: "Do you want to spend 1 Force to apply the range 1 bonus?",
                    imageHolder: HostShip
                );
            });
        }

        private void RegisterInquisitorDefenseAbility()
        {
            if (HostShip.State.Force < 1)
                return;

            if (Combat.ShotInfo.Range > 1)
                return;

            RegisterAbilityTrigger(TriggerTypes.OnAttackStart, delegate
            {
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    AlwaysUseByDefault,
                    UseInquisitorDefenseAbility,
                    descriptionLong: "Do you want to spend 1 Force to prevent the range 1 bonus?",
                    imageHolder: HostShip
                );
            });
        }

        private void UseInquisitorAttackAbility(object sender, EventArgs e)
        {
            Rules.DistanceBonus.OnCheckAllowRangeOneBonus += ApplyRangeOneBonus;
            HostShip.State.SpendForce(1, DecisionSubPhase.ConfirmDecision);
        }

        private void UseInquisitorDefenseAbility(object sender, EventArgs e)
        {
            Rules.DistanceBonus.OnCheckPreventRangeOneBonus += PreventRangeOneBonus;
            HostShip.State.SpendForce(1, DecisionSubPhase.ConfirmDecision);
        }

        private void ApplyRangeOneBonus(ref bool isActive)
        {
            Rules.DistanceBonus.OnCheckAllowRangeOneBonus -= ApplyRangeOneBonus;

            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: Range 1 bonus is applied");
            isActive = true;
        }

        private void PreventRangeOneBonus(ref bool isActive)
        {
            Rules.DistanceBonus.OnCheckPreventRangeOneBonus -= PreventRangeOneBonus;

            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: Range 1 bonus is prevented");
            isActive = false;
        }
    }
}