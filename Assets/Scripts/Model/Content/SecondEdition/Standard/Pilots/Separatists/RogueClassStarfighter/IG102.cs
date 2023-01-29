using Abilities.SecondEdition;
using Content;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RogueClassStarfighter
    {
        public class IG102 : RogueClassStarfighter
        {
            public IG102() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "IG-102",
                    "Dueling Droid",
                    Faction.Separatists,
                    4,
                    5,
                    19,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.IG102Ability),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Cannon,
                        UpgradeType.Cannon,
                        UpgradeType.Modification,
                        UpgradeType.Modification,
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Droid
                    }
                );

                ShipInfo.ActionIcons.SwitchToDroidActions();

                DeadToRights oldAbility = (DeadToRights)ShipAbilities.First(n => n.GetType() == typeof(DeadToRights));
                oldAbility.DeactivateAbility();
                ShipAbilities.Remove(oldAbility);
                ShipAbilities.Add(new NetworkedCalculationsAbility());

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/ig102.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class IG102Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsDiceModificationAvailable,
                GetDiceModificationPriority,
                DiceModificationType.Change,
                1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Blank },
                sideCanBeChangedTo: DieSide.Focus
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.CurrentDiceRoll.Blanks > 0) result = 100;

            return result;
        }

        private bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Defence)
            {
                if (Combat.Attacker.PilotInfo.Initiative >= HostShip.PilotInfo.Initiative)
                {
                    result = true;
                }
            }

            return result;
        }
    }
}