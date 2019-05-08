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
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.PoeDameronAbility),
                    extraUpgradeIcon: UpgradeType.Talent
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
            ship.AddAvailableDiceModification(new PoeDameronAction() { HostShip = HostShip });
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
                if (HostShip.Tokens.HasToken(typeof(Tokens.FocusToken)))
                {
                    Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Success);
                }
                else
                {
                    Messages.ShowErrorToHuman("This ability cannot be used: the ship has no focus tokens");
                }

                callBack();
            }

            public override bool IsDiceModificationAvailable()
            {
                bool result = false;
                if ((HostShip.Tokens.HasToken(typeof(Tokens.FocusToken))) && (Combat.CurrentDiceRoll.Focuses > 0)) result = true;
                return result;
            }

            public override int GetDiceModificationPriority()
            {
                int result = 0;

                if (HostShip.Tokens.HasToken(typeof(Tokens.FocusToken)))
                {
                    if (Combat.CurrentDiceRoll.Focuses > 0) result = 100;
                }

                return result;
            }
        }

    }
}
