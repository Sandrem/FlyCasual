using Content;
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
                PilotInfo = new PilotCardInfo25
                (
                    "Zertik Strom",
                    "Pitiless Administrator",
                    Faction.Imperial,
                    3,
                    4,
                    6,
                    isLimited: true,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Missile,
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
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
                ITargetLockable tlocked = tlock.OtherTargetLockTokenOwner;

                if (tlocked is GenericShip)
                {
                    if (!(tlocked as GenericShip).Damage.HasFacedownCards)
                        return;

                    Triggers.RegisterTrigger(
                        new Trigger()
                        {
                            Name = HostShip.PilotInfo.PilotName + "'s Ability",
                            TriggerOwner = HostShip.Owner.PlayerNo,
                            TriggerType = TriggerTypes.OnEndPhaseStart,
                            EventHandler = delegate
                            {
                                AskToUseAbility(
                                    HostShip.PilotInfo.PilotName,
                                    AlwaysUseByDefault,
                                    RemoveTargetLock,
                                    descriptionLong: "Do you want to spend a lock you have on an enemy ship to expose 1 of that ship's damage cards?",
                                    imageHolder: HostShip
                                );
                            }
                        }
                    );
                }
            }
        }

        private void RemoveTargetLock(System.Object sender, EventArgs e)
        {
            BlueTargetLockToken tlock = HostShip.Tokens.GetToken(typeof(BlueTargetLockToken), '*') as BlueTargetLockToken;
            ITargetLockable tlocked = tlock.OtherTargetLockTokenOwner;

            HostShip.Tokens.RemoveToken(tlock, delegate { (tlocked as GenericShip).Damage.ExposeRandomFacedownCard(DecisionSubPhase.ConfirmDecision); });
        }
    }
}
