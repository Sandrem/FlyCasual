using Abilities;
using ActionsList;
using GameModes;
using Ship;
using SubPhases;
using System;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList
{
    public class Daredevil : GenericUpgrade
    {
        public Daredevil() : base()
        {
            Type = UpgradeType.Elite;
            Name = "Daredevil";
            Cost = 3;

            UpgradeAbilities.Add(new DaredevilAbility());
        }
    }
}

namespace Abilities
{
    public class DaredevilAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionsList += DaredevilAddAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionsList -= DaredevilAddAction;
        }

        private void DaredevilAddAction(GenericShip host)
        {
            GenericAction newAction = new DaredevilAction
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = HostShip
            };
            host.AddAvailableAction(newAction);
        }
    }
}

namespace ActionsList
{
    public class DaredevilAction : GenericAction
    {
        private bool _hasOneTurns = false;

        public DaredevilAction()
        {
            Name = EffectName = "Daredevil";
        }

        public override void ActionTake()
        {
            Phases.CurrentSubPhase.Pause();

            Triggers.RegisterTrigger(new Trigger
            {
                Name = "Daredevil turn",
                TriggerType = TriggerTypes.OnAbilityDirect,
                TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                EventHandler = SelectDaredevilManeuver
            });

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, RegisterDaredevilExecutionTrigger);
        }

        private void SelectDaredevilManeuver(object sender, EventArgs e)
        {
            TryAddManeuversForDaredevil("1.L.T");
            TryAddManeuversForDaredevil("1.R.T");

            Selection.ThisShip.Owner.SelectManeuver(GameMode.CurrentGameMode.AssignManeuver, IsOneTurn);
        }

        private void RegisterDaredevilExecutionTrigger()
        {
            Triggers.RegisterTrigger(new Trigger
            {
                Name = "Daredevil Execution",
                TriggerType = TriggerTypes.OnManeuver,
                TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
                EventHandler = PerformDaredevilManeuver
            });

            Triggers.ResolveTriggers(TriggerTypes.OnManeuver, ReceiveStress);
        }

        private void PerformDaredevilManeuver(object sender, EventArgs e)
        {
            Messages.ShowInfoToHuman(string.Format("Performing Daredevil: {0} receives one stress token", Selection.ThisShip.PilotName));
            Selection.ThisShip.AssignedManeuver.Perform();

            if (!_hasOneTurns)
            {
                RemoveManeuversForDaredevil("1.L.T");
                RemoveManeuversForDaredevil("1.R.T");
            }
        }

        private void ReceiveStress()
        {
            Selection.ThisShip.Tokens.AssignToken(new StressToken(Selection.ThisShip), DoesHaveBoost);
        }

        private void DoesHaveBoost()
        {
            if (Selection.ThisShip.GetAvailableActionsList().Any(action => action is BoostAction))
            {
                PhaseCleanup();
            }
            else
            {
                Messages.ShowInfoToHuman("This ship does not have the boost action, rolling for damage");
                DiceRollCheck(
                    delegate
                    {
                        Phases.FinishSubPhase(typeof(DaredevilSubPhase));
                        PhaseCleanup();
                    });
            }
        }

        private void PhaseCleanup()
        {
            Phases.FinishSubPhase(typeof(ActionDecisonSubPhase));
            Triggers.FinishTrigger();
        }

        private void DiceRollCheck(Action callback)
        {
            Selection.ActiveShip = Selection.ThisShip;
            DaredevilSubPhase dieSubPhase = (DaredevilSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(DaredevilSubPhase),
                callback);

            dieSubPhase.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;
            dieSubPhase.Start();
        }

        private void TryAddManeuversForDaredevil(string maneuver)
        {
            if (Selection.ThisShip.Maneuvers.ContainsKey(maneuver))
            {
                _hasOneTurns = true;
                return;
            }
            Selection.ThisShip.Maneuvers.Add(maneuver, Movement.ManeuverColor.White);
        }

        private void RemoveManeuversForDaredevil(string maneuver)
        {
            Selection.ThisShip.Maneuvers.Remove(maneuver);
        }

        private bool IsOneTurn(string maneuverString)
        {
            bool result = true;
            Movement.MovementStruct movement = new Movement.MovementStruct(maneuverString);
            if (movement.Speed != Movement.ManeuverSpeed.Speed1)
            {
                result = false;
            }

            if (movement.Bearing != Movement.ManeuverBearing.Turn)
            {
                result = false;
            }

            return result;
        }

        public override int GetActionPriority()
        {
            int result = 0;
            return result;
        }
    }
}

namespace SubPhases
{
    public class DaredevilSubPhase : DiceRollCheckSubPhase
    {
        public override void Prepare()
        {
            diceType = DiceKind.Attack;
            diceCount = 2;

            finishAction = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            CurrentDiceRoll.RemoveAllFailures();
            if (!CurrentDiceRoll.IsEmpty)
            {
                foreach (Die die in CurrentDiceRoll.DiceList)
                {
                    if (die.Side == DieSide.Crit || die.Side == DieSide.Success)
                    {
                        SufferDamage();
                        break;
                    }
                }
            }

            CallBack();
        }

        private void NoDamage()
        {
            CallBack();
        }

        private void SufferDamage()
        {
            for (int i = 0; i < CurrentDiceRoll.DiceList.Count; i++)
            {
                Selection.ActiveShip.AssignedDamageDiceroll.AddDice(CurrentDiceRoll.DiceList[i].Side);

                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Suffer Daredevil damage",
                    TriggerType = TriggerTypes.OnDamageIsDealt,
                    TriggerOwner = Selection.ActiveShip.Owner.PlayerNo,
                    EventHandler = Selection.ActiveShip.SufferDamage,
                    EventArgs = new DamageSourceEventArgs()
                    {
                        Source = "Daredevil",
                        DamageType = DamageTypes.CardAbility
                    },
                    Skippable = true
                });
            }

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, Phases.CurrentSubPhase.Resume);
        }
    }
}