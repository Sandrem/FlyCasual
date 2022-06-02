using ActionsList;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEWiWhisperModifiedInterceptor
    {
        public class Nightfall : TIEWiWhisperModifiedInterceptor
        {
            public Nightfall() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Nightfall\"",
                    "709th Legion Veteran",
                    Faction.FirstOrder,
                    4,
                    4,
                    10,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.NightfallPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Missile
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://i.imgur.com/XEhy6Ej.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class NightfallPilotAbility : GenericAbility
    {
        private GenericShip SufferedShip;

        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += RegisterAssignJamToShipsInFlightPath;
            HostShip.OnActionIsPerformed += RegisterAssignJamToShipsInBoostPath;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= RegisterAssignJamToShipsInFlightPath;
            HostShip.OnActionIsPerformed -= RegisterAssignJamToShipsInBoostPath;
        }

        private void RegisterAssignJamToShipsInFlightPath(GenericShip ship)
        {
            if (ship.ShipsMovedThrough.Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AssignJamToShipsInFlightPath);
            }
        }

        private void AssignJamToShipsInFlightPath(object sender, EventArgs e)
        {
            List<GenericShip> sufferedShips = new List<GenericShip>(HostShip.ShipsMovedThrough);
            AssignTwoJamTokensRecursive(sufferedShips);
        }

        private void RegisterAssignJamToShipsInBoostPath(GenericAction action)
        {
            if (action is BoostAction && HostShip.ShipsBoostedThrough.Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AssignJamToShipsInBoostPath);
            }
        }

        private void AssignJamToShipsInBoostPath(object sender, EventArgs e)
        {
            List<GenericShip> sufferedShips = new List<GenericShip>(HostShip.ShipsBoostedThrough);
            AssignTwoJamTokensRecursive(sufferedShips);
        }

        private void AssignTwoJamTokensRecursive(List<GenericShip> sufferedShips)
        {
            if (sufferedShips.Count == 0)
            {
                Triggers.FinishTrigger();
            }
            else
            {

                SufferedShip = sufferedShips.First();
                sufferedShips.Remove(SufferedShip);

                Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: {SufferedShip.PilotInfo.PilotName} gains 2 jam tokens");

                SufferedShip.Tokens.AssignTokens(CreateJamToken, 2, delegate { AssignTwoJamTokensRecursive(sufferedShips); });
            }
        }

        private GenericToken CreateJamToken()
        {
            return new JamToken(SufferedShip, HostShip.Owner);
        }
    }
}
