using Arcs;
using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.VultureClassDroidFighter
{
    public class PreciseHunter : VultureClassDroidFighter
    {
        public PreciseHunter()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Precise Hunter",
                "Pinpoint Protocols",
                Faction.Separatists,
                3,
                3,
                8,
                limited: 3,
                abilityType: typeof(Abilities.SecondEdition.PreciseHunterAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Cannon
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                }
            );
            
            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/37/0c/370c5cb2-0f0d-4d6f-9358-eb3cad9088dc/swz29_precise-hunter.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you perform an attack, if the defender is in your bullseye arc, you may reroll one blank result.
    public class PreciseHunterAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Reroll,
                1,
                new List<DieSide> { DieSide.Blank }
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        public bool IsDiceModificationAvailable()
        {
            return (Combat.AttackStep == CombatStep.Attack
                && Combat.Attacker == HostShip
                && Combat.Attacker.SectorsInfo.IsShipInSector(Combat.Defender, ArcType.Bullseye));
        }

        public int GetDiceModificationAiPriority()
        {
            return 95;
        }
    }
}
