using RuleSets;

namespace Ship
{
    namespace ProtectorateStarfighter
    {
        public class FennRau : ProtectorateStarfighter, ISecondEditionPilot
        {
            public FennRau() : base()
            {
                PilotName = "Fenn Rau";
                PilotSkill = 9;
                Cost = 28;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.FennRauScumAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 6;
                Cost = 68;
            }
        }
    }
}

namespace Abilities
{
    public class FennRauScumAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckFennRauAbility;
            HostShip.AfterGotNumberOfDefenceDice += CheckFennRauAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckFennRauAbility;
            HostShip.AfterGotNumberOfDefenceDice -= CheckFennRauAbility;
        }

        private void CheckFennRauAbility(ref int value)
        {
            if (Combat.ShotInfo.Range == 1)
            {
                Messages.ShowInfo("Fenn Rau: +1 die");
                value++;
            }
        }
    }
}
