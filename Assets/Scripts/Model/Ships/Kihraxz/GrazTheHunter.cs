using BoardTools;

namespace Ship
{
    namespace Kihraxz
    {
        public class GrazTheHunter : Kihraxz
        {
            public GrazTheHunter()
            {
                PilotName = "Graz The Hunter";
                PilotSkill = 6;
                Cost = 25;

                IsUnique = true;

                PilotAbilities.Add(new Abilities.GrazTheHunterAbility());
            }
        }
    }
}

namespace Abilities
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
