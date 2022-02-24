using Abilities.SecondEdition;
using BoardTools;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.ResistanceTransportPod
{
    public class RoseTico : ResistanceTransportPod
    {
        public RoseTico()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Rose Tico",
                "Earnest Engineer",
                Faction.Resistance,
                3,
                3,
                8,
                isLimited: true,
                abilityType: typeof(RoseTicoResistanceTransportPodAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Modification
                }
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/e7/36/e736f7ba-c5dd-4222-9bdb-b889eea03293/swz45_rose-tico.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class RoseTicoResistanceTransportPodAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostShip.PilotInfo.PilotName,
                () => { return true; },
                () => { return 90; },
                DiceModificationType.Reroll,
                GetRerollNumber
            );
        }

        private int GetRerollNumber()
        {
            int friendlyShipsInArc = 0;

            foreach (GenericShip friendlyShip in HostShip.Owner.Ships.Values)
            {
                if (friendlyShip.ShipId == HostShip.ShipId) continue;

                ShotInfo shotInfo = new ShotInfo(Combat.Attacker, friendlyShip, Combat.Attacker.PrimaryWeapons.First());
                if (shotInfo.InArc) friendlyShipsInArc++;
            }

            return friendlyShipsInArc;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}