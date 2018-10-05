using BoardTools;
using RuleSets;

namespace Ship
{
    namespace Kihraxz
    {
        public class GrazTheHunter : Kihraxz, ISecondEditionPilot
        {
            public GrazTheHunter()
            {
                PilotName = "Graz The Hunter";
                PilotSkill = 6;
                Cost = 25;

                IsUnique = true;

                PilotAbilities.Add(new Abilities.GrazTheHunterAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotName = "Graz";
                PilotSkill = 4;
                Cost = 47;

                PilotAbilities.RemoveAll(ability => ability is Abilities.GrazTheHunterAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.GrazAbilitySE());

                SEImageNumber = 192;
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

namespace Abilities.SecondEdition
{
    public class GrazAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotStartAsAttacker += CheckConditionsOffense;
            HostShip.OnShotStartAsDefender += CheckConditionsDefense;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotStartAsAttacker -= CheckConditionsOffense;
            HostShip.OnShotStartAsDefender -= CheckConditionsDefense;
        }

        private void CheckConditionsDefense()
        {
            if (Board.IsShipInFacingOnly(Combat.Attacker, HostShip, Arcs.ArcFacing.Rear180))
            {
                HostShip.AfterGotNumberOfDefenceDice += RollExtraDefenseDice;
            }
        }

        private void CheckConditionsOffense()
        {
            if (Board.IsShipInFacingOnly(Combat.Defender, HostShip, Arcs.ArcFacing.Rear180))
            {
                HostShip.AfterGotNumberOfAttackDice += RollExtraAttackDice;
            }
        }

        private void RollExtraDefenseDice(ref int count)
        {
            count++;
            Messages.ShowInfo("Graz is behind the combatant. Roll an additional die.");
            HostShip.AfterGotNumberOfDefenceDice -= RollExtraDefenseDice;
        }

        private void RollExtraAttackDice(ref int count)
        {
            count++;
            Messages.ShowInfo("Graz is behind the combatant. Roll an additional die.");
            HostShip.AfterGotNumberOfAttackDice -= RollExtraAttackDice;
        }
    }
}
