using Ship;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEFoFighter
    {
        public class OmegaLeader : TIEFoFighter
        {
            public OmegaLeader() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Omega Leader\"",
                    8,
                    21,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.OmegaLeaderAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class OmegaLeaderAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += AddOmegaLeaderPilotAbility;
            HostShip.OnAttackStartAsDefender += AddOmegaLeaderPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= AddOmegaLeaderPilotAbility;
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
            Messages.ShowErrorToHuman("Omega Leader: Unable to modify dice.");
            canBeUsed = false;
        }

        private void RemoveOmegaLeaderPilotAbility(GenericShip ship)
        {
            ship.OnTryAddAvailableDiceModification -= UseOmegaLeaderRestriction;
            ship.OnAttackFinish -= RemoveOmegaLeaderPilotAbility;
        }
    }
}
