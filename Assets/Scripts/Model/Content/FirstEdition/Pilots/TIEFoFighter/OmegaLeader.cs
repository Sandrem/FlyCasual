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
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class OmegaLeaderAbility : GenericAbility
    {
        GenericShip LockedShip;

        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += AddOmegaLeaderPilotAbility;
            HostShip.OnAttackStartAsDefender += AddOmegaLeaderPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= AddOmegaLeaderPilotAbility;
            HostShip.OnAttackStartAsDefender -= AddOmegaLeaderPilotAbility;

            if (LockedShip != null) RemoveOmegaLeaderPilotAbility(LockedShip);
        }

        private void AddOmegaLeaderPilotAbility()
        {
            if (Combat.Defender.ShipId == HostShip.ShipId)
            {
                LockedShip = Combat.Attacker;
            }
            else
            {
                LockedShip = Combat.Defender;
            }

            if (ActionsHolder.HasTargetLockOn(HostShip, LockedShip))
            {
                LockedShip.OnTryAddAvailableDiceModification += UseOmegaLeaderRestriction;
                LockedShip.OnTryAddDiceModificationOpposite += UseOmegaLeaderRestriction;
                LockedShip.OnAttackFinish += RemoveOmegaLeaderPilotAbility;
            }
        }

        private void UseOmegaLeaderRestriction(GenericShip ship, ActionsList.GenericAction action, ref bool canBeUsed)
        {
            Messages.ShowErrorToHuman("\"Omega Leader\": The target is unable to modify dice");
            canBeUsed = false;
        }

        private void RemoveOmegaLeaderPilotAbility(GenericShip ship)
        {
            ship.OnTryAddAvailableDiceModification -= UseOmegaLeaderRestriction;
            ship.OnTryAddDiceModificationOpposite -= UseOmegaLeaderRestriction;
            ship.OnAttackFinish -= RemoveOmegaLeaderPilotAbility;
        }
    }
}
