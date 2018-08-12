using BoardTools;
using RuleSets;
using Ship;

namespace Ship
{
    namespace TIEFighter
    {
        public class ValenRudor : TIEFighter, ISecondEditionPilot
        {
            public ValenRudor() : base()
            {
                PilotName = "Valen Rudor";
                PilotSkill = 3;
                Cost = 28;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotRuleType = typeof(SecondEdition);
                PilotAbilities.Add(new Abilities.SecondEdition.ValenRudorAbilitySE());
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not required
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ValenRudorAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnAttackFinishGlobal += ValenRudorAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackFinishGlobal -= ValenRudorAbility;
        }

        private void ValenRudorAbility(GenericShip ship)
        {
            ShotInfo shotInformation = new ShotInfo(Combat.Defender, HostShip, Combat.ChosenWeapon);
            if (shotInformation.Range <= 1 && ship == Combat.Defender && ship.Owner.PlayerNo == HostShip.Owner.PlayerNo)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, PerformFreeAction);
            }
        }

        private void PerformFreeAction(object sender, System.EventArgs e)
        {
            var previousSelectedShip = Selection.ThisShip;
            Selection.ThisShip = HostShip;

            HostShip.AskPerformFreeAction(HostShip.GetAvailableActions(), delegate
            {
                Selection.ThisShip = previousSelectedShip;
                Triggers.FinishTrigger();
            });
        }
    }
}
