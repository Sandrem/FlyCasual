using Upgrade;
using Ship;
using ActionsList;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class AgileGunner : GenericUpgrade
    {
        public AgileGunner() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Agile Gunner",
                UpgradeType.Gunner,
                cost: 3,
                abilityType: typeof(Abilities.SecondEdition.AgileGunnerAbility),
                seImageNumber: 162
            );

            Avatar = new AvatarInfo(
                Faction.Imperial,
                new Vector2(367, 5),
                new Vector2(100, 100)
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AgileGunnerAbility : GenericAbility
    {
        // During the End Phase, you may rotate your turret indicator.

        public override void ActivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers -= RegisterTrigger;
        }

        private void RegisterTrigger()
        {
            if (HostShip.ShipInfo.ArcInfo.IsMobileTurretShip())
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, UseAbility);
            }
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            AskToUseAbility(
                "Agile Gunner",
                NeverUseByDefault,
                UseAgileGunnerAbility,
                descriptionLong: "Do you want to rotate your turret arc indicator?",
                imageHolder: HostUpgrade,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void UseAgileGunnerAbility(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            new RotateArcAction().DoOnlyEffect(Triggers.FinishTrigger);
        }
    }
}