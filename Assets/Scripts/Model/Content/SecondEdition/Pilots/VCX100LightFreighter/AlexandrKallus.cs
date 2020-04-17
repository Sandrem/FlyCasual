using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.VCX100LightFreighter
    {
        public class AlexandrKallus : VCX100LightFreighter
        {
            public AlexandrKallus() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Alexandr Kallus",
                    4,
                    69,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.AlexandrKallusAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/d0/bf/d0bf7c63-2c2c-4372-8ace-7299d180c774/swz66_alexsandr-kallus.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you defend, if the attacker modified any attack dice, you may roll an additional defense die.
    public class AlexandrKallusAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfDefenceDice += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfDefenceDice -= CheckAbility;
        }

        private void CheckAbility(ref int value)
        {
            if (Combat.AttackStep == CombatStep.Defence && Combat.DiceRollAttack.ModifiedBy.Contains(Combat.Attacker.Owner.PlayerNo))
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + " rolls an additional defense die");
                value++;
            }
        }
    }
}