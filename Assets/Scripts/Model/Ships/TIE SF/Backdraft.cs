using Ship;
using Abilities;
using ActionsList;

namespace Ship
{
    namespace TIESF
    {
        public class Backdraft : TIESF
        {
            public Backdraft() : base()
            {
                PilotName = "\"Backdraft\"";
                PilotSkill = 7;
                Cost = 27;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new BackdraftAbility());
            }
        }
    }
}

namespace Abilities
{
    public class BackdraftAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddBackdraftAbility;            
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddBackdraftAbility;
        }

        private void AddBackdraftAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new BackdraftAbilityAction());
        }        
    }
}

namespace ActionsList
{
    public class BackdraftAbilityAction : GenericAction
    {        
        public BackdraftAbilityAction()
        {
            Name = DiceModificationName = "\"Backdraft\" ability";
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;
            result = 110;
            return result;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack && !Combat.ShotInfo.InPrimaryArc)
            {
                result = true;
            }
            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.AddDice(DieSide.Crit).ShowWithoutRoll();
            Combat.CurrentDiceRoll.OrganizeDicePositions();            
            callBack();
        }        
    }

}


