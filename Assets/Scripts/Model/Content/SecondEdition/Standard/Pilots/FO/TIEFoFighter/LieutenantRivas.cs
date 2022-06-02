using BoardTools;
using Content;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEFoFighter
    {
        public class LieutenantRivas : TIEFoFighter
        {
            public LieutenantRivas() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Lieutenant Rivas",
                    "Inconvenient Witness",
                    Faction.FirstOrder,
                    1,
                    3,
                    4,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LieutenantRivasAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Tech,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/7188ec2eb699261dbd47a15df6164f4c.png";
            }
        }
    }
}


namespace Abilities.SecondEdition
{
    public class LieutenantRivasAbility : GenericAbility
    {
        private GenericShip ShipWithAssignedToken;

        public override void ActivateAbility()
        {
            GenericShip.OnTokenIsAssignedGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTokenIsAssignedGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, GenericToken token)
        {
            // To avoid infinite loop
            if (IsAbilityUsed) return;

            if (ship.Owner.PlayerNo == HostShip.Owner.PlayerNo) return;

            if (ActionsHolder.HasTargetLockOn(HostShip, ship)) return;

            if (token.TokenColor != TokenColors.Red && token.TokenColor != TokenColors.Orange) return;

            DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
            if (distInfo.Range == 1 || distInfo.Range == 2)
            {
                ShipWithAssignedToken = ship;
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, AskAcquireTargetLock);
            }
        }

        private void AskAcquireTargetLock(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                ShouldAbilityBeUsed,
                AcquireTargetLock,
                descriptionLong: "Do you want to acquire a Lock on " + ShipWithAssignedToken.PilotInfo.PilotName + "?",
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

            IsAbilityUsed = true;
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + " acquired a Lock on " + ShipWithAssignedToken.PilotInfo.PilotName);
            ActionsHolder.AcquireTargetLock(HostShip, ShipWithAssignedToken, FinishAbility, FinishAbility);
        }

        private void FinishAbility()
        {
            IsAbilityUsed = false;
            Triggers.FinishTrigger();
        }
    }
}