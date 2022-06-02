using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using UnityEngine;
using Upgrade;

namespace Ship.SecondEdition.Eta2Actis
{
    public class ObiWanKenobi : Eta2Actis
    {
        public ObiWanKenobi()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Obi-Wan Kenobi",
                "Guardian of Democracy",
                Faction.Republic,
                5,
                5,
                15,
                isLimited: true,
                force: 3,
                abilityType: typeof(Abilities.SecondEdition.ObiWanKenobiActisAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.ForcePower,
                    UpgradeType.Talent
                },
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                },
                skinName: "Blue"
            );

            PilotNameCanonical = "obiwankenobi-eta2actis";

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/2d/36/2d3610e5-ebc0-4448-8fb3-4b6dcc5f391a/swz79_obi-wan_kenobi.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ObiWanKenobiActisAbility : GenericAbility
    {
        private GenericShip TriggedShip;

        public override void ActivateAbility()
        {
            GenericShip.OnMovementFinishGlobal += CheckFirstPartOfConditions;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnMovementFinishGlobal -= CheckFirstPartOfConditions;
        }

        private void CheckFirstPartOfConditions(GenericShip ship)
        {
            if (((IsMe(ship)) || (IsAnakinSkywalkerInRange(ship)))
                && HasMoreEnemyShipsInRange(ship)
            )
            {
                TriggedShip = ship;
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, CheckSecondPartOfConditions);
            }
        }

        private bool IsMe(GenericShip ship)
        {
            return ship.ShipId == HostShip.ShipId;
        }

        private bool IsAnakinSkywalkerInRange(GenericShip ship)
        {
            bool result = false;

            if (ship.PilotInfo.PilotName == "Anakin Skywalker" && ship.Owner.PlayerNo == HostShip.Owner.PlayerNo)
            {
                DistanceInfo distanceInfo = new DistanceInfo(HostShip, ship);
                if (distanceInfo.Range <= 3)
                {
                    result = true;
                }
            }

            return result;
        }

        private bool HasMoreEnemyShipsInRange(GenericShip ship)
        {
            int anotherFriendlyShipsInRange = 0;
            int enemyShipsInRange = 0;

            foreach (GenericShip anotherShip in Roster.AllShips.Values)
            {
                if (anotherShip.ShipId == ship.ShipId) continue;

                DistanceInfo distInfo = new DistanceInfo(ship, anotherShip);
                if (distInfo.Range <= 1)
                {
                    if (ship.Owner.PlayerNo == anotherShip.Owner.PlayerNo)
                    {
                        anotherFriendlyShipsInRange++;
                    }
                    else
                    {
                        enemyShipsInRange++;
                    }
                }
            }

            return enemyShipsInRange > anotherFriendlyShipsInRange;
        }

        private void CheckSecondPartOfConditions(object sender, EventArgs e)
        {
            if (HostShip.State.Force > 0)
            {
                AskToUseObiWanKenobisAbility();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void AskToUseObiWanKenobisAbility()
        {
            AskToUseAbility(
                "Obi-Wan Kenobi",
                AlwaysUseByDefault,
                AssignFocusToken,
                descriptionLong: "Do you want to spend 1 force to assign 1 focus token?",
                imageHolder: HostShip,
                showSkipButton: true,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void AssignFocusToken(object sender, EventArgs e)
        {
            HostShip.State.SpendForce(
                1,
                delegate { TriggedShip.Tokens.AssignToken(typeof(FocusToken), DecisionSubPhase.ConfirmDecision); }
            );
        }
    }
}
