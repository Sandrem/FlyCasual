using Upgrade;
using Abilities;
using RuleSets;

namespace UpgradesList
{

    public class R5D8 : GenericUpgrade, ISecondEditionUpgrade
    {
        public R5D8() : base()
        {
            Types.Add(UpgradeType.Astromech);
            Name = "R5-D8";
            isUnique = true;
            Cost = 3;

            UpgradeAbilities.Add(new R5D8Ability());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            MaxCharges = 3;

            UpgradeAbilities.RemoveAll(a => a is R5D8Ability);
            UpgradeAbilities.Add(new Abilities.SecondEdition.R5AstromechAbility());
        }
    }

}

namespace Abilities
{
    public class R5D8Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateActions += R5D8AddAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateActions -= R5D8AddAction;
        }

        private void R5D8AddAction(Ship.GenericShip host)
        {
            ActionsList.GenericAction action = new ActionsList.R5D8Action()
            {
                ImageUrl = HostUpgrade.ImageUrl
            };
            host.AddAvailableAction(action);
        }
    }
}

namespace ActionsList
{

    public class R5D8Action : GenericAction
    {
        public R5D8Action()
        {
            Name = DiceModificationName = "R5-D8: Try to repair";
        }

        public override void ActionTake()
        {
            Selection.ActiveShip = Selection.ThisShip;
            Phases.StartTemporarySubPhaseOld(
                "R5-D8: Try to repair",
                typeof(SubPhases.R5D8CheckSubPhase),
                delegate {
                    Phases.FinishSubPhase(typeof(SubPhases.R5D8CheckSubPhase));
                    Phases.CurrentSubPhase.CallBack();
                }
            );
        }
    }

}

namespace SubPhases
{
    public class R5D8CheckSubPhase : DiceRollCheckSubPhase
    {

        public override void Prepare()
        {
            DiceKind = DiceKind.Defence;
            DiceCount = 1;

            AfterRoll = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            if (CurrentDiceRoll.DiceList[0].Side == DieSide.Success || CurrentDiceRoll.DiceList[0].Side == DieSide.Focus)
            {
                if (Selection.ThisShip.Damage.DiscardRandomFacedownCard())
                {
                    Sounds.PlayShipSound("R2D2-Proud");
                    Messages.ShowInfoToHuman("Facedown Damage card is discarded");
                }
            }

            CallBack();
        }

    }

}
