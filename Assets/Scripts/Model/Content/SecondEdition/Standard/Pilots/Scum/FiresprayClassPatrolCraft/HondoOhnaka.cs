using ActionsList;
using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FiresprayClassPatrolCraft
    {
        public class HondoOhnaka : FiresprayClassPatrolCraft
        {
            public HondoOhnaka() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Hondo Ohnaka",
                    "I Smell Profit!",
                    Faction.Scum,
                    1,
                    7,
                    18,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.HondoOhnakaPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Cannon,
                        UpgradeType.Crew,
                        UpgradeType.Illicit,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    skinName: "Jango Fett"
                );

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/f0da444b-7695-43ad-b637-e7918f33a83c/SWZ97_HondoOhnakalegal.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class HondoOhnakaPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateActions += AddAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateActions -= AddAction;
        }

        private void AddAction(GenericShip ship)
        {
            ship.AddAvailableAction(new HondoOhnakaPilotAction()
            {
                ImageUrl = HostShip.ImageUrl,
                HostShip = HostShip,
                Ability = this,
                Name = "Hondo Ohnaka"
            });
        }

        private class HondoOhnakaPilotAction : GenericAction
        {
            public GenericAbility Ability { get; set; }

            private GenericShip FirstShip;
            private GenericShip SecondShip;

            public override void ActionTake()
            {
                StartMultiselect();
            }

            private void StartMultiselect()
            {
                MultiSelectionSubphase subphase = Phases.StartTemporarySubPhaseNew<MultiSelectionSubphase>("Hondo Ohnaka", Phases.CurrentSubPhase.CallBack);

                subphase.RequiredPlayer = HostShip.Owner.PlayerNo;

                subphase.Filter = FilterMultiSelection;
                subphase.GetAiPriority = GetAiPriority;
                subphase.MaxToSelect = 2;
                subphase.WhenDone = AskToChooseFirstToCoordinate;

                subphase.DescriptionShort = HostShip.PilotInfo.PilotName; ;
                subphase.DescriptionLong = "Choose 2 ships at range 0-3 of you that are friendly to each other";
                subphase.ImageSource = HostShip;

                subphase.Start();
            }

            private bool FilterMultiSelection(GenericShip ship)
            {
                DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
                return distInfo.Range >= 0 && distInfo.Range <= 3 && AllShipsInSelectionAreDifferentTeam(ship);
            }

            private bool AllShipsInSelectionAreDifferentTeam(GenericShip ship)
            {
                if (Selection.MultiSelectedShips.Count == 0)
                {
                    return true;
                }
                else
                {
                    return ship.Owner.PlayerNo != Selection.MultiSelectedShips.First().Owner.PlayerNo;
                }
            }

            private void AskToChooseFirstToCoordinate(Action callback)
            {
                Ability.SelectTargetForAbility(
                    delegate { CoordinateFirstShip(callback); },
                    FilterCoordinateTargets,
                    GetAiPriority,
                    HostShip.Owner.PlayerNo,
                    name: HostShip.PilotInfo.PilotName,
                    description: "Choose a first ship that will perform an action",
                    imageSource: HostShip
                );
            }

            private void CoordinateFirstShip(Action callback)
            {
                SelectShipSubPhase.FinishSelectionNoCallback();

                Selection.ChangeActiveShip(Ability.TargetShip);

                Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: {Ability.TargetShip.PilotInfo.PilotName} is chosen to perform an action");

                FirstShip = Ability.TargetShip;
                AllowRedActions(FirstShip);

                Ability.TargetShip.AskPerformFreeAction(
                    GetPossibleActions(),
                    delegate {
                        Selection.ChangeActiveShip(HostShip);
                        Selection.MultiSelectedShips.Remove(Ability.TargetShip);
                        AskToCoordinateSecondShip(callback);
                    },
                    HostShip.PilotInfo.PilotName,
                    "You may perform an action from action bar of " + HostShip.PilotInfo.PilotName,
                    HostShip
                );
            }

            private void AllowRedActions(GenericShip ship)
            {
                ship.OnCanPerformActionWhileStressed += TemporaryAllowAnyActionsWhileStressed;
                ship.OnCheckCanPerformActionsWhileStressed += TemporaryAllowActionsWhileStressed;
                ship.OnActionIsPerformed += DisallowActionsWhileStressed;
                ship.OnActionIsSkipped += DisallowActionsWhileStressedAlt;
            }

            private void DisallowActionsWhileStressed(GenericAction action)
            {
                RemoveActionsWhileStressedUniversal();
            }

            private void DisallowActionsWhileStressedAlt(GenericShip ship)
            {
                RemoveActionsWhileStressedUniversal();
            }

            private void RemoveActionsWhileStressedUniversal()
            {
                if (FirstShip != null)
                {
                    FirstShip.OnCanPerformActionWhileStressed -= TemporaryAllowAnyActionsWhileStressed;
                    FirstShip.OnCheckCanPerformActionsWhileStressed -= TemporaryAllowActionsWhileStressed;
                    FirstShip.OnActionIsPerformed -= DisallowActionsWhileStressed;
                    FirstShip.OnActionIsSkipped -= DisallowActionsWhileStressedAlt;
                }

                if (SecondShip != null)
                {
                    SecondShip.OnCanPerformActionWhileStressed -= TemporaryAllowAnyActionsWhileStressed;
                    SecondShip.OnCheckCanPerformActionsWhileStressed -= TemporaryAllowActionsWhileStressed;
                    SecondShip.OnActionIsPerformed -= DisallowActionsWhileStressed;
                    SecondShip.OnActionIsSkipped -= DisallowActionsWhileStressedAlt;
                }
            }

            private void TemporaryAllowAnyActionsWhileStressed(GenericAction action, ref bool isAllowed)
            {
                isAllowed = true;
            }

            private void TemporaryAllowActionsWhileStressed(ref bool isAllowed)
            {
                isAllowed = true;
            }

            private List<GenericAction> GetPossibleActions()
            {
                return Selection.ThisShip.GetAvailableActions().Where(n => IsInActionBarOfHondo(n)).ToList();
            }

            private bool IsInActionBarOfHondo(GenericAction action)
            {
                return HostShip.ActionBar.AllActions.FirstOrDefault(n => n.GetType() == action.GetType()) != null;
            }

            private bool FilterCoordinateTargets(GenericShip ship)
            {
                return Selection.MultiSelectedShips.Contains(ship);
            }

            private int GetAiPriority(GenericShip ship)
            {
                return 0;
            }

            private void AskToCoordinateSecondShip(Action callback)
            {
                Ability.SelectTargetForAbility(
                    delegate { CoordinateSecondShip(callback); },
                    FilterJamTargets,
                    GetAiPriority,
                    HostShip.Owner.PlayerNo,
                    name: HostShip.PilotInfo.PilotName,
                    description: "Choose a second ship that will perform an action",
                    imageSource: HostShip
                );
            }

            private void CoordinateSecondShip(Action callback)
            {
                SelectShipSubPhase.FinishSelectionNoCallback();

                Selection.ChangeActiveShip(Ability.TargetShip);

                Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: {Ability.TargetShip.PilotInfo.PilotName} is chosen to perform an action");

                SecondShip = Ability.TargetShip;
                AllowRedActions(SecondShip);

                Ability.TargetShip.AskPerformFreeAction(
                    GetPossibleActions(),
                    delegate {
                        Selection.ChangeActiveShip(HostShip);
                        Selection.MultiSelectedShips.Remove(Ability.TargetShip);
                        FinishAbility(callback);
                    },
                    HostShip.PilotInfo.PilotName,
                    "You may perform an action from action bar of " + HostShip.PilotInfo.PilotName,
                    HostShip
                );
            }

            private bool FilterJamTargets(GenericShip ship)
            {
                return Selection.MultiSelectedShips.Contains(ship);
            }

            private void FinishAbility(Action callback)
            {
                Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: Calculate token is gained");
                HostShip.Tokens.AssignToken(typeof(CalculateToken), callback);
            }
        }
    }
}