using Ship;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEFoFighter
    {
        public class Midnight : TIEFoFighter
        {
            public Midnight() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Midnight\"",
                    6,
                    44,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.MidnightAbility),
                    extraUpgradeIcon: UpgradeType.Talent //,
                                                         //seImageNumber: 120
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/56/94/56940164-d919-4b04-8303-f39357555fad/swz18_a1_midnight.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MidnightAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnDefenceStartAsAttacker += AddOmegaLeaderPilotAbility;
            HostShip.OnAttackStartAsDefender += AddOmegaLeaderPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDefenceStartAsAttacker -= AddOmegaLeaderPilotAbility;
            HostShip.OnAttackStartAsDefender -= AddOmegaLeaderPilotAbility;
        }

        private void AddOmegaLeaderPilotAbility()
        {
            GenericShip enemyship;
            if (Combat.Defender.ShipId == HostShip.ShipId)
            {
                enemyship = Combat.Attacker;
            }
            else
            {
                enemyship = Combat.Defender;
            }

            if (ActionsHolder.HasTargetLockOn(HostShip, enemyship))
            {
                enemyship.OnTryAddAvailableDiceModification += UseOmegaLeaderRestriction;
                enemyship.OnTryAddDiceModificationOpposite += UseOmegaLeaderRestriction;
                enemyship.OnAttackFinish += RemoveOmegaLeaderPilotAbility;
            }
        }

        private void UseOmegaLeaderRestriction(GenericShip ship, ActionsList.GenericAction action, ref bool canBeUsed)
        {
            Messages.ShowErrorToHuman("Midnight: Unable to modify dice.");
            canBeUsed = false;
        }

        private void RemoveOmegaLeaderPilotAbility(GenericShip ship)
        {
            ship.OnTryAddAvailableDiceModification -= UseOmegaLeaderRestriction;
            ship.OnAttackFinish -= RemoveOmegaLeaderPilotAbility;
        }
    }
}