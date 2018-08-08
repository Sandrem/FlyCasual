using RuleSets;
using Ship;

namespace Ship
{
    namespace Kihraxz
    {
        public class TalonbaneCobra : Kihraxz, ISecondEditionPilot
        {
            public TalonbaneCobra() : base()
            {
                PilotName = "Talonbane Cobra";
                PilotSkill = 9;
                Cost = 28;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                IsUnique = true;

                PilotAbilities.Add(new Abilities.TalonbaneCobraAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 5;
                Cost = 50;
            }
        }
    }
}

namespace Abilities
{
    public class TalonbaneCobraAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += TalonbaneCobraDiceCheck;
            HostShip.AfterGotNumberOfDefenceDice += TalonbaneCobraDiceCheck;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= TalonbaneCobraDiceCheck;
            HostShip.AfterGotNumberOfDefenceDice -= TalonbaneCobraDiceCheck;
        }

        private void TalonbaneCobraDiceCheck(ref int diceCount)
        {
            if (Combat.ChosenWeapon.GetType() == typeof(PrimaryWeaponClass))
            {
                if (Combat.AttackStep == CombatStep.Attack && Combat.ShotInfo.Range == 1)
                {
                    Messages.ShowInfo("Talonbane Cobra: +1 attack die");
                    diceCount++;
                }
                if (Combat.AttackStep == CombatStep.Defence && Combat.ShotInfo.Range == 3)
                {
                    Messages.ShowInfo("Talonbane Cobra: +1 defence die");
                    diceCount++;
                }
            }
        }
    }
}
