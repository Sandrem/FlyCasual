using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.NimbusClassVWing
    {
        public class Contrail : NimbusClassVWing
        {
            public Contrail() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Contrail\"",
                    "CT-4981",
                    Faction.Republic,
                    5,
                    3,
                    11,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ContrailAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Astromech,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone,
                        Tags.Tie
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/b0/bc/b0bcccdb-fd02-4ab1-847b-66bae01e7ddc/swz80_ship_contrail.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ContrailAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification
            (
                "\"Contrail\"",
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                count: 1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Focus },
                sideCanBeChangedTo: DieSide.Blank,
                timing: DiceModificationTimingType.Opposite
            );
        }

        private bool IsAvailable()
        {
            return ManeuversHaveTheSameBearing()
                && Combat.CurrentDiceRoll.HasResult(DieSide.Focus);
        }

        private bool ManeuversHaveTheSameBearing()
        {
            bool result = false;

            GenericShip anotherShip = GetAnotherShip();
            if (HostShip.RevealedManeuver != null && anotherShip != null && anotherShip.RevealedManeuver != null)
            {
                if (HostShip.RevealedManeuver.Bearing == anotherShip.RevealedManeuver.Bearing) result = true;
            }

            return result;
        }

        private GenericShip GetAnotherShip()
        {
            if (Combat.Attacker.ShipId == HostShip.ShipId)
            {
                return Combat.Defender;
            }
            else if (Combat.Defender.ShipId == HostShip.ShipId)
            {
                return Combat.Attacker;
            }
            else
            {
                return null;
            }
        }

        private int GetAiPriority()
        {
            return 55;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}