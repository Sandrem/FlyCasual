using Ship;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.T70XWing
    {
        public class PoeDameron : T70XWing
        {
            public PoeDameron() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Poe Dameron",
                    8,
                    31,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.PoeDameronAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );

                ModelInfo.SkinName = "Black One";
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class PoeDameronAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddPoeDameronPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddPoeDameronPilotAbility;
        }

        private void AddPoeDameronPilotAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new PoeDameronAction() { Host = HostShip });
        }

        private class PoeDameronAction : ActionsList.GenericAction
        {
            public PoeDameronAction()
            {
                Name = DiceModificationName = "Poe Dameron's ability";

                IsTurnsOneFocusIntoSuccess = true;
            }

            public override void ActionEffect(System.Action callBack)
            {
                if (Host.Tokens.HasToken(typeof(Tokens.FocusToken)))
                {
                    Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Success);
                }
                else
                {
                    Messages.ShowErrorToHuman("Cannot use ability - no Focus token");
                }

                callBack();
            }

            public override bool IsDiceModificationAvailable()
            {
                bool result = false;
                if ((Host.Tokens.HasToken(typeof(Tokens.FocusToken))) && (Combat.CurrentDiceRoll.Focuses > 0)) result = true;
                return result;
            }

            public override int GetDiceModificationPriority()
            {
                int result = 0;

                if (Host.Tokens.HasToken(typeof(Tokens.FocusToken)))
                {
                    if (Combat.CurrentDiceRoll.Focuses > 0) result = 100;
                }

                return result;
            }
        }

    }
}
