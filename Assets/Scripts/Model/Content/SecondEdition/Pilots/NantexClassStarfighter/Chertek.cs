using Ship;
using System;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.NantexClassStarfighter
    {
        public class Chertek : NantexClassStarfighter
        {
            public Chertek() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Chertek",
                    4,
                    39,
                    isLimited: true,
                    extraUpgradeIcon: UpgradeType.Talent,
                    abilityType: typeof(Abilities.SecondEdition.ChertekAbility),
                    abilityText: "While you perform a primary attack, if the defender is tractored, you may reroll up to 2 attack dice."
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/cb/32/cb321604-f7ca-4429-9d8f-0da43b7b5d5f/swz47_cards-chertek.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ChertekAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostShip.PilotInfo.PilotName,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                2
            );
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon
                && Combat.Defender.IsTractored;
        }

        private int GetAiPriority()
        {
            return 90;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

    }
}