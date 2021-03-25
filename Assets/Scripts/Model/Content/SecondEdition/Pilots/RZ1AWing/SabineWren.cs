using Mods.ModsList;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ1AWing
    {
        public class SabineWren : RZ1AWing
        {
            public SabineWren() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Sabine Wren",
                    3,
                    37,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.SabineWrenAWingAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    abilityText: "While you defend or perform an attack, if the attack range is 1 and you are in the enemy ship's front arc, you may change 1 of your results to a hit or evade result."
                );

                PilotNameCanonical = "sabinewren-rz1awing";

                ModelInfo.SkinName = "Phoenix Squadron";

                ModelInfo.SkinName = "Green";

                ImageUrl = "https://i.imgur.com/zpIsycf.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SabineWrenAWingAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Sabine Wren",
                IsAvailable,
                AiPriority,
                DiceModificationType.Change,
                1,
                sideCanBeChangedTo: DieSide.Success
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        public bool IsAvailable()
        {
            return
            (
                Combat.ShotInfo.Range == 1 &&
                (
                    (Combat.Defender == HostShip && Combat.Attacker.SectorsInfo.IsShipInSector(HostShip, Arcs.ArcType.Front))
                    || (Combat.Attacker == HostShip && Combat.Defender.SectorsInfo.IsShipInSector(HostShip, Arcs.ArcType.Front))
                )
            );
        }

        public int AiPriority()
        {
            return 100;
        }
    }
}