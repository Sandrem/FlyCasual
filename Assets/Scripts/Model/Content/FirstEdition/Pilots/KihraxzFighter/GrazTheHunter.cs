using BoardTools;
using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace FirstEdition.KihraxzFighter
    {
        public class GrazTheHunter : KihraxzFighter
        {
            public GrazTheHunter() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Graz The Hunter",
                    6,
                    25,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.GrazTheHunterAbility)
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class GrazTheHunterAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotStartAsDefender += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotStartAsDefender -= CheckConditions;
        }

        private void CheckConditions()
        {
            ShotInfo shotInformation = new ShotInfo(HostShip, Combat.Attacker, HostShip.PrimaryWeapon);
            if (shotInformation.InArc)
            {
                HostShip.AfterGotNumberOfDefenceDice += RollExtraDice;
            }
        }

        private void RollExtraDice(ref int count)
        {
            count++;
            Messages.ShowInfo("Attacker is within firing arc. Roll 1 additional defense die.");
            HostShip.AfterGotNumberOfDefenceDice -= RollExtraDice;
        }
    }
}