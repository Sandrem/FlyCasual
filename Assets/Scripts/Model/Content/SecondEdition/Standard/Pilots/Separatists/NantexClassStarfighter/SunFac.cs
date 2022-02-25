using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.NantexClassStarfighter
    {
        public class SunFac : NantexClassStarfighter
        {
            public SunFac() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Sun Fac",
                    "Archduke’s Enforcer",
                    Faction.Separatists,
                    6,
                    6,
                    18,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.SunFacAbility),
                    abilityText: "while you perform a primary attack, if the defender is tractored, roll 1 additional attack die.",
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Modification
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/57/9f/579f4d25-2f04-4463-8a15-465fcd7ee83e/swz47_cards-sun-fac.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SunFacAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfPrimaryWeaponAttackDice -= CheckAbility;
        }

        private void CheckAbility(ref int count)
        {
            if (Combat.Defender.IsTractored)
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": The defender is tractored, you roll an additional attack die");
                count++;
            }
        }
    }
}