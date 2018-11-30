using Ship;
using System.Linq;

namespace Ship
{
    namespace FirstEdition.Firespray31
    {
        public class KrassisTrelix : Firespray31
        {
            public KrassisTrelix() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Krassis Trelix",
                    5,
                    36,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.KrassisTrelixAbility),
                    factionOverride: Faction.Imperial
                );

                ModelInfo.SkinName = "Krassis Trelix";
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class KrassisTrelixAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += KrassisTrelixPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= KrassisTrelixPilotAbility;
        }

        public void KrassisTrelixPilotAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new KrassisTrelixAction());
        }

        private class KrassisTrelixAction : ActionsList.GenericAction
        {
            public KrassisTrelixAction()
            {
                Name = DiceModificationName = "Krassis Trelix's ability";
                IsReroll = true;
            }

            public override void ActionEffect(System.Action callBack)
            {
                DiceRerollManager diceRerollManager = new DiceRerollManager
                {
                    NumberOfDiceCanBeRerolled = 1,
                    CallBack = callBack
                };
                diceRerollManager.Start();
            }

            public override bool IsDiceModificationAvailable()
            {
                bool result = false;
                if (Combat.AttackStep == CombatStep.Attack && (Combat.ChosenWeapon as Upgrade.GenericSpecialWeapon) != null)
                {
                    result = true;
                }
                return result;
            }

            public override int GetDiceModificationPriority()
            {
                int result = 0;

                if (Combat.AttackStep == CombatStep.Attack && (Combat.ChosenWeapon as Upgrade.GenericSpecialWeapon) != null)
                {
                    if (Combat.DiceRollAttack.Blanks > 0)
                    {
                        result = 90;
                    }
                    else if (Combat.DiceRollAttack.Focuses > 0 && Combat.Attacker.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) == 0)
                    {
                        result = 90;
                    }
                    else if (Combat.DiceRollAttack.Focuses > 0)
                    {
                        result = 30;
                    }
                }

                return result;
            }

        }
    }
}