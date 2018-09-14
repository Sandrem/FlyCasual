using RuleSets;
using Ship;
using SubPhases;
using System;
using Tokens;

namespace Ship
{
    namespace TIEAdvanced
    {
        public class ZertikStrom : TIEAdvanced, ISecondEditionPilot
        {
            public ZertikStrom() : base()
            {
                PilotName = "Zertik Strom";
                PilotSkill = 3;
                Cost = 45;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
                PilotRuleType = typeof(SecondEdition);
                PilotAbilities.Add(new Abilities.SecondEdition.ZertikStromAbility());

                SEImageNumber = 96;
            }

            public void AdaptPilotToSecondEdition()
            {
                // Nope
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
            Phases.Events.OnEndPhaseStart_Triggers += UseZertikStromAbility;
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