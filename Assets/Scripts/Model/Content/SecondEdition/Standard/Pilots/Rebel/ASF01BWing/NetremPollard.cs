using Abilities.SecondEdition;
using ActionsList;
using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.ASF01BWing
{
    public class NetremPollard : ASF01BWing
    {
        public NetremPollard() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Netrem Pollard",
                "Dagger Leader",
                Faction.Rebel,
                3,
                5,
                18,
                isLimited: true,
                abilityType: typeof(NetremPollardAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Missile,
                    UpgradeType.Device,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.BWing
                },
                skinName: "Red"
            );

            ImageUrl = "https://i.imgur.com/tDMmS4S.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class NetremPollardAbility : GenericAbility
    {
        // After you barrel roll, you may choose 1 friendly ship that is not stressed at range 0-1.
        // That ship gains 1 stress token, then you rotate 180o
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckAbility;
        }

        private void CheckAbility(GenericAction action)
        {
            if (action is BarrelRollAction && HasTargetsForAbility())
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, SelectTarget);
            }
        }

        private bool HasTargetsForAbility()
        {
            foreach (GenericShip ship in HostShip.Owner.Ships.Values)
            {
                if (FilterTargets(ship)) return true;
            }

            return false;
        }

        private void SelectTarget(object sender, EventArgs e)
        {
            SelectTargetForAbility
            (
                ExecuteAbility,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                name: HostShip.PilotInfo.PilotName,
                description: "You may choose 1 friendly ship - that ship gains 1 stress token, then you can rotate 180 degrees",
                imageSource: HostShip
            );
        }

        private void ExecuteAbility()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();
            TargetShip.Tokens.AssignToken(typeof(StressToken), Rotate180);
        }

        private void Rotate180()
        {
            HostShip.Rotate180(Triggers.FinishTrigger);
        }

        private bool FilterTargets(GenericShip ship)
        {
            DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
            return (distInfo.Range <= 1 && !ship.IsStressed && Tools.IsSameTeam(HostShip, ship));
        }

        private int GetAiPriority(GenericShip ship)
        {
            return 0;
        }
    }
}