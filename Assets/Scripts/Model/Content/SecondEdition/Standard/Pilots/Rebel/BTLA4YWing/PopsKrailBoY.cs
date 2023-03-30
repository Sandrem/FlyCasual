using Abilities.SecondEdition;
using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class PopsKrailBoY : BTLA4YWing
        {
            public PopsKrailBoY() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Pops\" Krail",
                    "Battle of Yavin",
                    Faction.Rebel,
                    4,
                    4,
                    0,
                    isLimited: true,
                    abilityType: typeof(PopsKrailBoYAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Turret,
                        UpgradeType.Torpedo,
                        UpgradeType.Astromech,
                        UpgradeType.Device,
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.YWing
                    },
                    isStandardLayout: true
                );

                ShipAbilities.Add(new HopeAbility());

                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.IonCannonTurret));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.AdvProtonTorpedoes));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.R4Astromech));

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/9/9b/Popskrail-battleofyavin.png";

                PilotNameCanonical = "popskrail-battleofyavin";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PopsKrailBoYAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification
            (
                HostShip.PilotInfo.PilotName,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                2
            );
        }

        private int GetAiPriority()
        {
            return 90;
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && Combat.ArcForShot.ArcType == Arcs.ArcType.SingleTurret;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}