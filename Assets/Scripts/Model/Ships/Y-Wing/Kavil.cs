using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ship
{
    namespace YWing
    {
        public class Kavil : YWing
        {
            public Kavil() : base()
            {
                PilotName = "Kavil";
                PilotSkill = 7;
                Cost = 24;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.SalvagedAstromech);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                faction = Faction.Scum;

                SkinName = "Brown";

                PilotAbilities.Add(new Abilities.KavilAbility());
            }
        }
    }
}

namespace Abilities
{
    public class KavilAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += KavilPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= KavilPilotAbility;
        }

        private void KavilPilotAbility(ref int diceCount)
        {
            if (!Combat.ShotInfo.InArc)
            {
                diceCount++;
            }
        }

    }
}