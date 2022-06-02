using Abilities.SecondEdition;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.NabooRoyalN1Starfighter
    {
        public class DineeEllberger : NabooRoyalN1Starfighter
        {
            public DineeEllberger() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Dineé Ellberger",
                    "Bravo Five",
                    Faction.Republic,
                    3,
                    4,
                    14,
                    isLimited: true,
                    abilityText: "While you defend or perform an attack, if the speed of your revealed maneuver is the same as the enemy ship's, that ship's dice cannot be modified.",
                    abilityType: typeof(DineeEllbergerAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent
                    }
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
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": Enemy's dice cannot be modified.");
                EnemyShip.OnTryAddAvailableDiceModification += UseDiceRestriction;
                HostShip.OnTryAddDiceModificationOpposite += UseDiceRestriction;
                EnemyShip.OnAttackFinish += RemoveOmegaLeaderPilotAbility;
            }
        }

        private void UseDiceRestriction(GenericShip ship, ActionsList.GenericAction diceModification, ref bool canBeUsed)
        {
            if (!diceModification.IsNotRealDiceModification)
            {
                Messages.ShowErrorToHuman(HostShip.PilotInfo.PilotName + ": Enemy's dice cannot be modified.");
                canBeUsed = false;
            }
        }

        private void RemoveOmegaLeaderPilotAbility(GenericShip ship)
        {
            ship.OnTryAddAvailableDiceModification -= UseDiceRestriction;
            HostShip.OnTryAddDiceModificationOpposite -= UseDiceRestriction;
            ship.OnAttackFinish -= RemoveOmegaLeaderPilotAbility;
        }
    }
}
