using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEAdvancedX1
    {
        public class ZertikStrom : TIEAdvancedX1
        {
            public ZertikStrom() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Zertik Strom",
                    3,
                    42,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ZertikStromAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 96
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ZertikStromAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers += UseZertikStromAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers -= UseZertikStromAbility;
        }

        private void UseZertikStromAbility()
        {
            BlueTargetLockToken tlock = HostShip.Tokens.GetToken(typeof(BlueTargetLockToken), '*') as BlueTargetLockToken;

            // Do we have a target lock?
            if (tlock != null)
            {
                GenericShip tlocked = tlock.OtherTokenOwner;

                if (!tlocked.Damage.HasFacedownCards)
                    return;

                Triggers.RegisterTrigger(
                    new Trigger()
                    {
                        Name = "Zertik Strom's Ability",
                        TriggerOwner = HostShip.Owner.PlayerNo,
                        TriggerType = TriggerTypes.OnEndPhaseStart,
                        EventHandler = delegate
                        {
                            AskToUseAbility(AlwaysUseByDefault, RemoveTargetLock);
                        }
                    }
                );
            }
        }

        private void RemoveTargetLock(System.Object sender, EventArgs e)
        {
            BlueTargetLockToken tlock = HostShip.Tokens.GetToken(typeof(BlueTargetLockToken), '*') as BlueTargetLockToken;
            GenericShip tlocked = tlock.OtherTokenOwner;

            HostShip.Tokens.RemoveToken(tlock, delegate { tlocked.Damage.ExposeRandomFacedownCard(DecisionSubPhase.ConfirmDecision); });
        }
    }
}
