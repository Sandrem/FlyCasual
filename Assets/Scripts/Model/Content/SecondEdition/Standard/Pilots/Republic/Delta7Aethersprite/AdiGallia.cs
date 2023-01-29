using Ship;
using SubPhases;
using System.Collections.Generic;
using Upgrade;
using Tokens;
using BoardTools;
using UnityEngine;
using Content;
using System;

namespace Ship.SecondEdition.Delta7Aethersprite
{
    public class AdiGallia : Delta7Aethersprite
    {
        public AdiGallia()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Adi Gallia",
                "Shooting Star",
                Faction.Republic,
                5,
                5,
                12,
                isLimited: true,
                force: 2,
                abilityType: typeof(Abilities.SecondEdition.AdiGalliaAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent
                },
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                },
                skinName: "Plo Koon"
            );

            ImageUrl = "https://infinitearenas.com/xw2/images/pilots/adigallia.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AdiGalliaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += RegisterAdiGalliaAttackAbility;
            HostShip.OnAttackStartAsDefender += RegisterAdiGalliaDefenseAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= RegisterAdiGalliaAttackAbility;
            HostShip.OnAttackStartAsDefender -= RegisterAdiGalliaDefenseAbility;

            Rules.DistanceBonus.OnCheckAllowRangeOneBonus -= PreventRangeThreeBonus;
            Rules.DistanceBonus.OnCheckPreventRangeOneBonus -= PreventRangeOneBonus;
        }

        private void RegisterAdiGalliaAttackAbility()
        {
            if (HostShip.State.Force < 1)
                return;

            if (Combat.ShotInfo.Range != 3)
                return;

            RegisterAbilityTrigger(TriggerTypes.OnAttackStart, delegate
            {
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    AlwaysUseByDefault,
                    UseAdiGalliaAttackAbility,
                    descriptionLong: "Do you want to spend 1 Force to prevent the range 3 bonus?",
                    imageHolder: HostShip
                );
            });
        }

        private void RegisterAdiGalliaDefenseAbility()
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
                    UseAdiGalliaDefenseAbility,
                    descriptionLong: "Do you want to spend 1 Force to prevent the range 1 bonus?",
                    imageHolder: HostShip
                );
            });
        }

        private void UseAdiGalliaAttackAbility(object sender, EventArgs e)
        {
            Rules.DistanceBonus.OnCheckAllowRangeThreeBonus += PreventRangeThreeBonus;
            HostShip.State.SpendForce(1, DecisionSubPhase.ConfirmDecision);
        }

        private void UseAdiGalliaDefenseAbility(object sender, EventArgs e)
        {
            Rules.DistanceBonus.OnCheckPreventRangeOneBonus += PreventRangeOneBonus;
            HostShip.State.SpendForce(1, DecisionSubPhase.ConfirmDecision);
        }

        private void PreventRangeThreeBonus(ref bool isActive)
        {
            Rules.DistanceBonus.OnCheckAllowRangeThreeBonus -= PreventRangeThreeBonus;

            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: Range 3 bonus is prevented");
            isActive = false;
        }

        private void PreventRangeOneBonus(ref bool isActive)
        {
            Rules.DistanceBonus.OnCheckPreventRangeOneBonus -= PreventRangeOneBonus;

            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: Range 1 bonus is prevented");
            isActive = false;
        }
    }
}
