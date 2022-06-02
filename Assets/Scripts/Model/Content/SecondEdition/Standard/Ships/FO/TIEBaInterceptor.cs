using System;
using System.Collections.Generic;
using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship;
using Ship.CardInfo;
using SubPhases;
using Tokens;
using UnityEngine;
using Upgrade;

namespace Ship.SecondEdition.TIEBaInterceptor
{
    public class TIEBaInterceptor : GenericShip
    {
        public TIEBaInterceptor() : base()
        {
            ShipInfo = new ShipCardInfo25
            (
                "TIE/ba Interceptor",
                BaseSize.Small,
                new FactionData
                (
                    new Dictionary<Faction, Type>
                    {
                        { Faction.FirstOrder, typeof(MajorVonreg) }
                    }
                ),
                new ShipArcsInfo(ArcType.Front, 3), 3, 2, 2,
                new ShipActionsInfo
                (
                    new ActionInfo(typeof(FocusAction)),
                    new ActionInfo(typeof(EvadeAction)),
                    new ActionInfo(typeof(TargetLockAction)),
                    new ActionInfo(typeof(BarrelRollAction)),
                    new ActionInfo(typeof(BoostAction))
                ),
                new ShipUpgradesInfo
                (
                    UpgradeType.Talent,
                    UpgradeType.Tech,
                    UpgradeType.Modification                    
                )
            );

            ShipAbilities.Add(new Abilities.SecondEdition.FineTunedThrusters());

            ModelInfo = new ShipModelInfo
            (
                "TIE Ba Interceptor",
                "Crimson",
                new Vector3(-3.4f, 7.5f, 5.55f),
                1.5f
            );

            DialInfo = new ShipDialInfo
            (
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Easy),

                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.SegnorsLoop, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.SegnorsLoop, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),

                new ManeuverInfo(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn, MovementComplexity.Complex)
            );

            SoundInfo = new ShipSoundInfo
            (
                new List<string>()
                {
                    "TIE-Fly1",
                    "TIE-Fly2",
                    "TIE-Fly3",
                    "TIE-Fly4",
                    "TIE-Fly5",
                    "TIE-Fly6",
                    "TIE-Fly7"
                },
                "TIE-Fire", 3
            );

            ShipIconLetter = 'j';
        }
    }
}

namespace Abilities.SecondEdition
{
    public class FineTunedThrusters : GenericAbility
    {
        public override string Name { get { return "Fine-Tuned Thrusters"; } }

        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += TryRegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= TryRegisterTrigger;
        }

        private void TryRegisterTrigger(GenericShip ship)
        {
            if (!HostShip.IsStrained && !HostShip.IsDepleted)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToUseOwnAbility);
            }
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            SelectDebuffDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<SelectDebuffDecisionSubphase>(
                "Select debuff subphase",
                Triggers.FinishTrigger
            );

            subphase.DescriptionShort = "Fine-Tuned Thrusters";
            subphase.DescriptionLong = "You may gain Strain or Deplete token to perform Lock or Barrel Roll action";

            subphase.DecisionOwner = HostShip.Owner;
            subphase.ShowSkipButton = true;

            subphase.AddDecision(
                "Gain Strain token",
                SelectStrainToken
            );

            subphase.AddDecision(
                "Gain Deplete token",
                SelectDepleteToken
            );

            subphase.DefaultDecisionName = "";

            subphase.Start();
        }

        private void SelectDepleteToken(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.Tokens.AssignToken(typeof(DepleteToken), AskToPerformAbilityAction);
        }

        private void SelectStrainToken(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.Tokens.AssignToken(typeof(StrainToken), AskToPerformAbilityAction);
        }

        private void AskToPerformAbilityAction()
        {
            HostShip.AskPerformFreeAction(
                new List<GenericAction>()
                {
                    new TargetLockAction(),
                    new BarrelRollAction()
                },
                Triggers.FinishTrigger,
                "Fine-Tuned Thrusters"
            );
        }

        private class SelectDebuffDecisionSubphase : DecisionSubPhase { };
    }
}