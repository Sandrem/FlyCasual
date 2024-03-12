using System.Collections.Generic;
using Movement;
using Upgrade;
using Ship;
using SubPhases;
using System;
using Tokens;
using Arcs;
using Actions;
using ActionsList;
using UnityEngine;
using Ship.CardInfo;
using System.Linq;

namespace Ship
{
    namespace SecondEdition.TIESkStriker
    {
        public class TIESkStriker : GenericShip
        {
            public TIESkStriker() : base()
            {
                ShipInfo = new ShipCardInfo25
                (
                    "TIE/sk Striker",
                    BaseSize.Small,
                    new FactionData
                    (
                        new Dictionary<Faction, Type>
                        {
                            { Faction.Imperial, typeof(Duchess) }
                        }
                    ),
                    new ShipArcsInfo(ArcType.Front, 3), 2, 4, 0,
                    new ShipActionsInfo
                    (
                        new ActionInfo(typeof(FocusAction)),
                        new ActionInfo(typeof(EvadeAction)),
                        new ActionInfo(typeof(BarrelRollAction))
                    ),
                    new ShipUpgradesInfo()
                );

                ShipAbilities.Add(new Abilities.SecondEdition.AdaptiveAileronsAbility());

                ModelInfo = new ShipModelInfo
                (
                    "TIE Striker",
                    "Gray",
                    new Vector3(-3.45f, 7.15f, 5.55f),
                    2f
                );

                DialInfo = new ShipDialInfo
                (
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.SegnorsLoop, MovementComplexity.Complex),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.SegnorsLoop, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal)
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

                ShipIconLetter = 'T';
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AdaptiveAileronsAbility : GenericAbility
    {
        private GenericMovement SavedManeuver;

        private static readonly List<string> ChangedManeuversCodes = new List<string>() { "1.L.B", "1.F.S", "1.R.B" };
        private Dictionary<string, MovementComplexity> SavedManeuverColors;

        bool doAilerons = false;

        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsReadyToBeRevealed += RegisterAdaptiveAileronsAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsReadyToBeRevealed -= RegisterAdaptiveAileronsAbility;
        }

        private void RegisterAdaptiveAileronsAbility(GenericShip ship)
        {
            // AI doesn't know how to boost
            if (HostShip.Owner.GetType().IsSubclassOf(typeof(Players.GenericAiPlayer))) return;

            RegisterAbilityTrigger(TriggerTypes.OnManeuverIsReadyToBeRevealed, CheckCanUseAbility);
        }

        private void CheckCanUseAbility(object sender, EventArgs e)
        {
            if (HostShip.Tokens.HasToken(typeof(StressToken)) && (HostShip.PilotInfo.PilotName != "\"Duchess\""))
            {
                Triggers.FinishTrigger();
            }
            else
            {
                DoAdaptiveAileronsAbility();
            }
        }

        private void DoAdaptiveAileronsAbility()
        {
            SavedManeuver = HostShip.AssignedManeuver;

            SavedManeuverColors = new Dictionary<string, MovementComplexity>();
            foreach (var changedManeuver in ChangedManeuversCodes)
            {
                SavedManeuverColors.Add(changedManeuver, HostShip.DialInfo.PrintedDial.First(n => n.Key.ToString() == changedManeuver).Value);
                HostShip.Maneuvers[changedManeuver] = MovementComplexity.Normal;
            }

            Triggers.RegisterTrigger(
                new Trigger()
                {
                    Name = "Ailerons Planning",
                    TriggerType = TriggerTypes.OnAbilityDirect,
                    TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                    EventHandler = CheckForDuchess
                }
            );

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, ExecuteSelectedManeuver);
        }

        private void CheckForDuchess(object sender, EventArgs e)
        {
            if (HostShip.PilotInfo.PilotName == "\"Duchess\"")
            {
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    NeverUseByDefault,
                    UseAilerons,
                    DontUseAilerons,
                    descriptionLong: "Do you want to activate your Adaptive Ailerons?",
                    imageHolder: HostShip
                );
            }
            else
            {
                SelectAdaptiveAileronsManeuver(sender, e);
            }
        }

        private void DontUseAilerons(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecision();
        }

        private void UseAilerons(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            SelectAdaptiveAileronsManeuver(sender, e);
        }

        private void SelectAdaptiveAileronsManeuver(object sender, EventArgs e)
        {
            doAilerons = true;
            HostShip.Owner.ChangeManeuver(
                ShipMovementScript.SendAssignManeuverCommand,
                Triggers.FinishTrigger,
                AdaptiveAileronsFilter
            );
        }

        private void RestoreManuverColors(GenericShip ship)
        {
            foreach (var changedManeuver in ChangedManeuversCodes)
            {
                HostShip.Maneuvers[changedManeuver] = SavedManeuverColors[changedManeuver];
            }
        }

        private void ExecuteSelectedManeuver()
        {
            if (doAilerons)
            {
                HostShip.AssignedManeuver.IsRevealDial = false;
                HostShip.AssignedManeuver.GrantedBy = "Ailerons";
                HostShip.CanPerformActionsWhenBumped = true;
                HostShip.CanPerformActionsWhenOverlapping = true;
                ShipMovementScript.LaunchMovement(FinishAdaptiveAileronsAbility);
            }
            else
            {
                FinishAdaptiveAileronsAbility();
            }
        }

        private void FinishAdaptiveAileronsAbility()
        {
            doAilerons = false;
            HostShip.CanPerformActionsWhenBumped = false;
            HostShip.CanPerformActionsWhenOverlapping = false;
            RestoreManuverColors(HostShip);
            Phases.CurrentSubPhase.IsReadyForCommands = true;
            //ship may have flown off the board; only assign saved maneuver if ship is exists
            if (Roster.GetShipById("ShipId:" + Selection.ThisShip.ShipId) != null)
            {
                ManeuverSelectionSubphase subphase = Phases.StartTemporarySubPhaseNew<ManeuverSelectionSubphase>(
                    "Select a maneuver",
                    Triggers.FinishTrigger
                );
                subphase.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;
                subphase.Start();
                subphase.IsReadyForCommands = true;

                ShipMovementScript.AssignManeuver(Selection.ThisShip.ShipId, SavedManeuver.ToString());
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool AdaptiveAileronsFilter(string maneuverString)
        {
            GenericMovement movement = ShipMovementScript.MovementFromString(maneuverString, HostShip);
            if (movement.ManeuverSpeed != ManeuverSpeed.Speed1) return false;
            if (movement.Bearing == ManeuverBearing.Straight || movement.Bearing == ManeuverBearing.Bank) return true;

            return false;
        }
    }
}