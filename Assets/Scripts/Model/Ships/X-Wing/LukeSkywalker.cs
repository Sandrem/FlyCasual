using Ship;
using RuleSets;

namespace Ship
{
    namespace XWing
    {
        public class LukeSkywalker : XWing, ISecondEditionPilot
        {
            public LukeSkywalker() : base()
            {
                PilotName = "Luke Skywalker";
                PilotSkill = 8;
                Cost = 28;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.LukeSkywalkerAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 5;
                MaxForce = 2;
                Cost = 60;

                PilotAbilities.RemoveAll(a => a is Abilities.LukeSkywalkerAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.LukeSkywalkerAbility());

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Force);
            }
        }
    }
}

namespace Abilities
{
    public class LukeSkywalkerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddLukeSkywalkerPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddLukeSkywalkerPilotAbility;
        }

        private void AddLukeSkywalkerPilotAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new LukeSkywalkerAction());
        }

        private class LukeSkywalkerAction : ActionsList.GenericAction
        {
            public LukeSkywalkerAction()
            {
                Name = DiceModificationName = "Luke Skywalker's ability";

                IsTurnsOneFocusIntoSuccess = true;
            }

            public override void ActionEffect(System.Action callBack)
            {
                Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Success);
                callBack();
            }

            public override bool IsDiceModificationAvailable()
            {
                bool result = false;
                if (Combat.AttackStep == CombatStep.Defence) result = true;
                return result;
            }

            public override int GetDiceModificationPriority()
            {
                int result = 0;

                if (Combat.AttackStep == CombatStep.Defence)
                {
                    if (Combat.DiceRollAttack.Successes > Combat.DiceRollDefence.Successes)
                    {
                        if (Combat.DiceRollDefence.Focuses > 0) result = 80;
                    }
                }

                return result;
            }
        }

    }
}

namespace Abilities.SecondEdition
{
    //After you become the defender (before dice are rolled), you may recover 1 Force.
    public class LukeSkywalkerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnDefenceStartAsDefender += RecoverForce;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDefenceStartAsDefender -= RecoverForce;
        }

        private void RecoverForce()
        {
            if (HostShip.Force < HostShip.MaxForce)
            {
                HostShip.Force++;
                Messages.ShowInfo("Luke Skywalker recovered 1 Force");
            }
        }
    }
}