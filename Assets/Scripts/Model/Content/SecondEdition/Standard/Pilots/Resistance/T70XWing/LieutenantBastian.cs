using BoardTools;
using Content;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Tokens;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class LieutenantBastian : T70XWing
        {
            public LieutenantBastian() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Lieutenant Bastian",
                    "Optimistic Analyst",
                    Faction.Resistance,
                    2,
                    5,
                    10,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LieutenantBastianAbility),
                    tags: new List<Tags>
                    {
                        Tags.XWing
                    }
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LieutenantBastianAbility : GenericAbility
    {
        private GenericShip ShipToAssignLock;

        public override void ActivateAbility()
        {
            GenericShip.OnDamageCardIsDealtGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnDamageCardIsDealtGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (ship.Owner.PlayerNo == HostShip.Owner.PlayerNo) return;

            DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
            if (distInfo.Range == 1 || distInfo.Range == 2)
            {
                ShipToAssignLock = ship;
                RegisterAbilityTrigger(TriggerTypes.OnDamageCardIsDealt, AskAcquireTargetLock);
            }
        }

        private void AskAcquireTargetLock(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                ShouldAbilityBeUsed,
                AcquireTargetLock,
                descriptionLong: "Do you want to acquire a Lock on " + ShipToAssignLock.PilotInfo.PilotName + "?",
                imageHolder: HostShip
            );
        }

        private bool ShouldAbilityBeUsed()
        {
            return (!HostShip.Tokens.HasToken<BlueTargetLockToken>(letter: '*'));
        }

        private void AcquireTargetLock(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            Messages.ShowInfo(HostShip.PilotInfo.PilotName + " acquired a Lock on " + ShipToAssignLock.PilotInfo.PilotName);
            ActionsHolder.AcquireTargetLock(HostShip, ShipToAssignLock, Triggers.FinishTrigger, Triggers.FinishTrigger);
        }
    }
}