using Abilities.Parameters;
using ActionsList;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.LaatIGunship
    {
        public class Hawk : LaatIGunship
        {
            public Hawk() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Hawk\"",
                    "Valkyrie 2929",
                    Faction.Republic,
                    4,
                    5,
                    20,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.HawkAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone
                    }
                );
                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/a0/d0/a0d0bfa8-544c-4872-8f6e-ef5fc3329c3a/swz70_a1_hawk_ship.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class HawkAbility : TriggeredAbility
    {
        public override TriggerForAbility Trigger => new AtTheStartOfPhase(typeof(SubPhases.EndStartSubPhase));

        public override AbilityPart Action => new EachShipCanDoAction
        (
            eachShipAction: PerformReposition,
            conditions: new ConditionsBlock
            (
                new TeamCondition(ShipTypes.Friendly),
                new RangeToHostCondition(minRange: 0, maxRange: 1),
                new RevealedManeuverCondition(minSpeed: 3, maxSpeed: 5)
            ),
            description: new AbilityDescription
            (
                "\"Hawk\"",
                "Friendly ships may gain 1 strain token to perfrom a Barrel Roll or Boost action",
                imageSource: HostShip
            )
        );

        private void PerformReposition(GenericShip ship, Action callback)
        {
            Selection.ChangeActiveShip(ship);

            ship.Tokens.AssignToken
            (
                typeof(Tokens.StrainToken),
                delegate
                {
                    ship.AskPerformFreeAction
                    (
                        new List<GenericAction>()
                        { 
                            new BarrelRollAction(){ HostShip = ship },
                            new BoostAction(){ HostShip = ship },
                        },
                        callback,
                        descriptionShort: "\"Hawk\"",
                        descriptionLong: "You can perform barrel roll or boost action",
                        imageHolder: ship
                    );
                }
            );
        }
    }
}