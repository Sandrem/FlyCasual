using BoardTools;
using Content;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.Eta2Actis
{
    public class AaylaSecura : Eta2Actis
    {
        public AaylaSecura()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Aayla Secura",
                "Confident Warrior",
                Faction.Republic,
                5,
                5,
                15,
                isLimited: true,
                force: 2,
                abilityType: typeof(Abilities.SecondEdition.AaylaSecuraActisAbility),
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

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/c2/e2/c2e2ee1f-1bdd-4ff7-ad95-c442af9b510a/swz79_aayla-secura.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AaylaSecuraActisAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Aayla Secura",
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                count: 1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Blank },
                sideCanBeChangedTo: DieSide.Focus,
                isGlobal: true
            );
        }

        private bool IsAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Defence && Combat.Attacker.Owner.PlayerNo != HostShip.Owner.PlayerNo)
            {
                ShotInfoArc arcInfo = new ShotInfoArc(
                    HostShip,
                    Combat.Attacker,
                    HostShip.SectorsInfo.Arcs.First(n => n.Facing == Arcs.ArcFacing.Front)
                );

                if (arcInfo.InArc && arcInfo.Range <= 1) result = true;
            }

            return result;
        }

        private int GetAiPriority()
        {
            return 100;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}
