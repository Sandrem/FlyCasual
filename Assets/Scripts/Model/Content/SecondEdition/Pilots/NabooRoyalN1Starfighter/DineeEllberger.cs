﻿
using Abilities.SecondEdition;
using Ship;
// using System.Collections;
// using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.NabooRoyalN1Starfighter
    {
        public class DineeEllberger : NabooRoyalN1Starfighter
        {
            public DineeEllberger() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Dineé Ellberger",
                    3,
                    38,
                    isLimited: true,
                    abilityText: "While you defend or perform an attack, if the speed of your revealed maneuver is the same as the enemy ship's, that ship's dice cannot be modified.",
                    abilityType: typeof(DineeEllbergerAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/48/69/4869e3a3-e56f-4abb-8af3-19bf76c80764/swz40_dinee-ellberger.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DineeEllbergerAbility : GenericAbility
    {
        GenericShip EnemyShip;
        public override void ActivateAbility()
        {
            HostShip.OnDefenceStartAsAttacker += CheckDineeEllbergerAbility;
            HostShip.OnAttackStartAsDefender += CheckDineeEllbergerAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDefenceStartAsAttacker -= CheckDineeEllbergerAbility;
            HostShip.OnAttackStartAsDefender -= CheckDineeEllbergerAbility;
        }

        private void CheckDineeEllbergerAbility()
        {
            if (Combat.Defender.ShipId == HostShip.ShipId)
            {
                EnemyShip = Combat.Attacker;
            }
            else
            {
                EnemyShip = Combat.Defender;
            }
            if (HostShip.RevealedManeuver == null || EnemyShip.RevealedManeuver == null) return;

            if (HostShip.RevealedManeuver.Speed == EnemyShip.RevealedManeuver.Speed)
            {
                Messages.ShowInfo("Dineé Ellberger: Enemy's dice cannot be modified.");
                EnemyShip.OnTryAddAvailableDiceModification += UseOmegaLeaderRestriction;
                HostShip.OnTryAddDiceModificationOpposite += UseOmegaLeaderRestriction;
                EnemyShip.OnAttackFinish += RemoveOmegaLeaderPilotAbility;
            }
        }

        private void UseOmegaLeaderRestriction(GenericShip ship, ActionsList.GenericAction action, ref bool canBeUsed)
        {
            Messages.ShowErrorToHuman("\"Dineé Ellberger\": Enemy's dice cannot be modified.");
            canBeUsed = false;
        }

        private void RemoveOmegaLeaderPilotAbility(GenericShip ship)
        {
            ship.OnTryAddAvailableDiceModification -= UseOmegaLeaderRestriction;
            HostShip.OnTryAddDiceModificationOpposite -= UseOmegaLeaderRestriction;
            ship.OnAttackFinish -= RemoveOmegaLeaderPilotAbility;
        }
    }
}
