using ActionsList;
using BoardTools;
using Players;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class HondoOhnaka : GenericUpgrade
    {
        public HondoOhnaka() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Hondo Ohnaka",
                UpgradeType.Crew,
                cost: 5,
                abilityType: typeof(Abilities.SecondEdition.HondoOhnakaAbility)
            );

            Avatar = new AvatarInfo(
                Faction.Scum,
                new Vector2(237, 1)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/f2/30/f230e89a-4885-4721-b2a0-7c0e1ef1726f/swz-hondo-ohnaka.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class HondoOhnakaAbility : GenericAbility
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
            ship.AddAvailableAction(new HondoOhnakaAction()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                HostShip = HostShip,
                Source = HostUpgrade,
                Ability = this,
                Name = "Hondo Ohnaka"
            });
        }

        private class HondoOhnakaAction : GenericAction
        {
            public GenericAbility Ability { get; set; }

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

                subphase.DescriptionShort = Source.UpgradeInfo.Name;
                subphase.DescriptionLong = "Choose 2 ships at range 1-3 of you that are friendly to each other. Coordinate one of the chosen ships, then jam the other, ignoring range restrictions.";
                subphase.ImageSource = Source;

                subphase.Start();
            }

            private bool FilterMultiSelection(GenericShip ship)
            {
                DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
                return distInfo.Range >= 1 && distInfo.Range <= 3 && AllShipsInSelectionAreSameTeam(ship);
            }

            private bool AllShipsInSelectionAreSameTeam(GenericShip ship)
            {
                if (Selection.MultiSelectedShips.Count == 0)
                {
                    return true;
                }
                else
                {
                    return ship.Owner.PlayerNo == Selection.MultiSelectedShips.First().Owner.PlayerNo;
                }
            }

            private void AskToChooseFirstToCoordinate(Action callback)
            {
                Ability.SelectTargetForAbility(
                    delegate { CoordinateFirstShip(callback); },
                    FilterCoordinateTargets,
                    GetAiPriority,
                    HostShip.Owner.PlayerNo,
                    name: Source.UpgradeInfo.Name,
                    description: "Coordnite first ship",
                    imageSource: Source
                );
            }

            private void CoordinateFirstShip(Action callback)
            {
                SelectShipSubPhase.FinishSelectionNoCallback();

                Selection.ChangeActiveShip(Ability.TargetShip);

                Messages.ShowInfo($"{Source.UpgradeInfo.Name}: {Ability.TargetShip.PilotInfo.PilotName} is Coordinated");

                Ability.TargetShip.AskPerformFreeAction(
                    GetPossibleActions(),
                    delegate {
                        Selection.ChangeActiveShip(HostShip);
                        Selection.MultiSelectedShips.Remove(Ability.TargetShip);
                        AskToChooseSecondToJam(callback);
                    },
                    "Coordinate action",
                    "You are coordinated by " + HostShip.PilotInfo.PilotName
                );
            }

            private List<GenericAction> GetPossibleActions()
            {
                return Selection.ThisShip.GetAvailableActions();
            }

            private bool FilterCoordinateTargets(GenericShip ship)
            {
                return Selection.MultiSelectedShips.Contains(ship);
            }

            private int GetAiPriority(GenericShip ship)
            {
                return 0;
            }

            private void AskToChooseSecondToJam(Action callback)
            {
                Ability.SelectTargetForAbility(
                    delegate { JamSecondShip(callback); },
                    FilterJamTargets,
                    GetAiPriority,
                    HostShip.Owner.PlayerNo,
                    name: Source.UpgradeInfo.Name,
                    description: "Jam second ship",
                    imageSource: Source
                );
            }

            private void JamSecondShip(Action callback)
            {
                SelectShipSubPhase.FinishSelectionNoCallback();

                Messages.ShowInfo($"{Source.UpgradeInfo.Name}: {Ability.TargetShip.PilotInfo.PilotName} is Jammed");

                Ability.TargetShip.Tokens.AssignToken(
                    new JamToken(Ability.TargetShip, HostShip.Owner),
                    delegate {
                        Selection.MultiSelectedShips.Remove(Ability.TargetShip);
                        callback();
                    }
                );
            }

            private bool FilterJamTargets(GenericShip ship)
            {
                return Selection.MultiSelectedShips.Contains(ship);
            }
        }
    }
}