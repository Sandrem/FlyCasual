using Ship;
using System;
using System.Collections.Generic;
using Upgrade;
using System.Linq;
using Tokens;
using Content;

namespace Ship
{
    namespace SecondEdition.Z95AF4Headhunter
    {
        public class Bossk : Z95AF4Headhunter
        {
            public Bossk() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Bossk",
                    "Fearsome Hunter",
                    Faction.Scum,
                    4,
                    3,
                    10,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.BosskPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Modification,
                        UpgradeType.Illicit
                    },
                    tags: new List<Tags>
                    {
                        Tags.BountyHunter
                    },
                    skinName: "Nashtah Pup"
                );

                PilotNameCanonical = "bossk-z95af4headhunter";

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/f5/a0/f5a0b23e-82c1-4092-98b2-7f01bd2577db/swz58_bossk.png";

                ShipAbilities.Add(new Abilities.SecondEdition.PursuitCraft());
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PursuitCraft : GenericAbility
    {
        public override string Name => "Pursuit Craft";

        private GenericShip HoundsTooth;
        private List<BlueTargetLockToken> BlueLocks;

        public override void ActivateAbility()
        {
            HostShip.OnUndockingFinish += RegisterOwnTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnUndockingFinish -= RegisterOwnTrigger;
        }

        private void RegisterOwnTrigger(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnUndockingFinish, AskToUseOwnAbility);
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            HoundsTooth = GetHoundsTooth();

            if (HoundsTooth == null) Triggers.FinishTrigger();

            BlueLocks = HoundsTooth.Tokens.GetAllTokens()
                .Where(n => n is BlueTargetLockToken)
                .Select(n => n as BlueTargetLockToken)
                .ToList();

            if (BlueLocks.Count == 0)
            {
                Messages.ShowInfo("Pursuit Craft: Hound's Tooth doesn't have a lock, ability is skipped");
                Triggers.FinishTrigger();
            }
            else
            {
                AskToUseAbility(
                    "Pursuit Craft",
                    AlwaysUseByDefault,
                    AcquireTargetLock,
                    descriptionLong: "Do you want to acquire a lock on a ship the friendly Hound's Tooth has locked",
                    showSkipButton: true,
                    requiredPlayer: HostShip.Owner.PlayerNo
                );
            }
        }

        private void AcquireTargetLock(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            if (BlueLocks.Count == 1)
            {
                ActionsHolder.AcquireTargetLock(
                    HostShip,
                    BlueLocks.First().OtherTargetLockTokenOwner,
                    Triggers.FinishTrigger,
                    Triggers.FinishTrigger
                );
            }
            else
            {
                SelectTargetForAbility(
                    AcquireTargetLockOnSelectedShip,
                    FilterTargets,
                    GetAiPriority,
                    HostShip.Owner.PlayerNo,
                    name: "Pursuit Craft",
                    description: "You may acquire a lock on a ship the friendly Hound's Tooth has locked?",
                    showSkipButton: true
                );
            }
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost;
        }

        private bool FilterTargets(GenericShip ship)
        {
            return BlueLocks.Any(n => n.OtherTargetLockTokenOwner == ship as ITargetLockable);
        }

        private void AcquireTargetLockOnSelectedShip()
        {
            SubPhases.SelectShipSubPhase.FinishSelectionNoCallback();

            ActionsHolder.AcquireTargetLock(
                HostShip,
                TargetShip,
                Triggers.FinishTrigger,
                Triggers.FinishTrigger
            );
        }

        private GenericShip GetHoundsTooth()
        {
            return HostShip.Owner.Ships.FirstOrDefault(n => n.Value.UpgradeBar.HasUpgradeInstalled(typeof(UpgradesList.SecondEdition.HoundsTooth))).Value;
        }
    }
}
