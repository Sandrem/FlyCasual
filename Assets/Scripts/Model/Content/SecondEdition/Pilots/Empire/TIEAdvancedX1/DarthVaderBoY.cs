using Abilities.SecondEdition;
using Content;
using System;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship
{
    namespace SecondEdition.TIEAdvancedX1
    {
        public class DarthVaderBoY : TIEAdvancedX1
        {
            public DarthVaderBoY() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Darth Vader",
                    "Battle of Yavin",
                    Faction.Imperial,
                    6,
                    6,
                    0,
                    isLimited: true,
                    abilityType: typeof(DarthVaderBoYAbility),
                    force: 3,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.ForcePower,
                        UpgradeType.Talent,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie,
                        Tags.DarkSide,
                        Tags.Sith
                    },
                    skinName: "Blue",
                    isStandardLayout: true
                );

                MustHaveUpgrades.Add(typeof(Marksmanship));
                MustHaveUpgrades.Add(typeof(Hate));
                MustHaveUpgrades.Add(typeof(AfterBurners));

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/a/a9/Darthvader-battleofyavin.png";

                PilotNameCanonical = "darthvader-battleofyavin";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DarthVaderBoYAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostShip.PilotInfo.PilotName,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                count: 1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Blank },
                sideCanBeChangedTo: DieSide.Success,
                payAbilityCost: SpendForce
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack &&
                Combat.DiceRollAttack.Blanks > 0 &&
                HostShip.State.Force > 0;
        }

        private int GetAiPriority()
        {
            return 45;
        }

        private void SpendForce(Action<bool> callback)
        {
            HostShip.State.SpendForce(1, delegate { callback(true); });
        }
    }
}