using Ship;
using Abilities.FirstEdition;

namespace Ship
{
    namespace FirstEdition.XWing
    {
        public class LukeSkywalker : XWing
        {
            public LukeSkywalker() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Luke Skywalker",
                    8,
                    28,
                    limited: 1,
                    abilityText: "When defending, you may change 1 of your focus results to an evade result.",
                    abilityType: typeof(LukeSkywalkerAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}

namespace Abilities.FirstEdition
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