namespace SubPhases
{
    public class SelectTargetForSecondAttackSubPhase : SelectShipSubPhase
    {

        public override void Prepare()
        {
            targetsAllowed.Add(TargetTypes.Enemy);
            finishAction = FinishActon;

            UI.ShowSkipButton();

            Selection.ThisShip.Owner.StartExtraAttack();
        }

        private void FinishActon()
        {
            Selection.AnotherShip = TargetShip;
            Combat.ChosenWeapon = Selection.ThisShip.PrimaryWeapon;
            Combat.ShotInfo = new Board.ShipShotDistanceInformation(Selection.ThisShip, TargetShip, Combat.ChosenWeapon);
            MovementTemplates.ShowFiringArcRange(Combat.ShotInfo);
            CallBack();
        }

        public override void RevertSubPhase() { }

        public override void SkipButton()
        {
            Phases.FinishSubPhase(typeof(SelectTargetForSecondAttackSubPhase));
            Triggers.FinishTrigger();
        }

        public override void Next()
        {
            UI.HideSkipButton();
        }

        public override void Resume()
        {
            UpdateHelpInfo();
            UI.ShowSkipButton();
        }

    }
}