using Ship;

namespace Ship
{
    namespace Kihraxz
    {
        public class TalonbaneCobra : Kihraxz
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
                if (Combat.AttackStep == CombatStep.Attack &&
                    Combat.ShotInfo.Range == 1)
                {
                    diceCount++;
                }
                if (Combat.AttackStep == CombatStep.Defence && 
                    Combat.ShotInfo.Range == 3)
                {
                    diceCount++;
                }
            }
        }
    }
}
