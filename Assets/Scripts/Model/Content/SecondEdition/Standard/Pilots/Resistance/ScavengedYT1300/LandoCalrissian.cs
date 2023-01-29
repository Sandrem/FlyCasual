using ActionsList;
using BoardTools;
using Content;
using Movement;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ScavengedYT1300
    {
        public class LandoCalrissian : ScavengedYT1300
        {
            public LandoCalrissian() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Lando Calrissian",
                    "Old General",
                    Faction.Resistance,
                    5,
                    8,
                    20,
                    isLimited: true,
                    charges: 3,
                    regensCharges: 1,
                    abilityType: typeof(Abilities.SecondEdition.LandoCalrissianScavengedYT1300Ability),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Illicit,
                        UpgradeType.Illicit,
                        UpgradeType.Title,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Freighter,
                        Tags.YT1300
                    }
                );

                PilotNameCanonical = "landocalrissian-scavengedyt1300";

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/01ba9a50-1f9e-4ba8-be73-171a3ae59511/SWZ97_LandoCalrissianlegal+%281%29.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LandoCalrissianScavengedYT1300Ability : GenericAbility
    {
        public List<GenericShip> SelectedShips = new List<GenericShip>();
        public int ShipIndex = 0;

        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckConditions;
            HostShip.OnMovementFinishSuccessfully += RegisterMovementTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckConditions;
            HostShip.OnMovementFinishSuccessfully -= RegisterMovementTrigger;
        }

        protected void CheckConditions(GenericAction action)
        {
            if (action.IsRed && HasTargetsForAbility() && HostShip.State.Charges > 0)
            {
                //TODO figure out why RotateArcAction is different
                if (action is RotateArcAction)
                {
                    DecisionSubPhase.ConfirmDecisionNoCallback();
                }
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, StartMultiSelectionSubphase);
            }
        }

        protected void RegisterMovementTrigger(GenericShip ship)
        {
            if (HostShip.GetLastManeuverColor() == MovementComplexity.Complex && HasTargetsForAbility())
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, StartMultiSelectionSubphase);
            }
        }

        private void StartMultiSelectionSubphase(object sender, EventArgs e)
        {
            MultiSelectionSubphase subphase = Phases.StartTemporarySubPhaseNew<MultiSelectionSubphase>("Lando Calrissian", Triggers.FinishTrigger);

            subphase.RequiredPlayer = HostShip.Owner.PlayerNo;

            subphase.Filter = FilterSelection;
            subphase.GetAiPriority = GetAiPriority;
            subphase.MaxToSelect = HostShip.State.Charges;
            subphase.WhenDone = GrantActionRecursive;

            subphase.DescriptionShort = HostShip.PilotInfo.PilotName;
            subphase.DescriptionLong = "Spend any number of charges to choose that many friendly ships Range 0-2. The chosen ships may perform an action, even while stressed.";
            subphase.ImageSource = HostShip;

            subphase.Start();
        }

        private void GrantActionRecursive(Action callback)
        {
            GenericShip currentShip = Selection.MultiSelectedShips.FirstOrDefault();

            if (currentShip == null)
            {
                Selection.ChangeActiveShip(HostShip);
                callback();
            }
            else
            {
                Selection.MultiSelectedShips.Remove(currentShip);

                Selection.ChangeActiveShip(currentShip);

                currentShip.BeforeActionIsPerformed += PayCost;
                currentShip.OnCheckCanPerformActionsWhileStressed += ConfirmThatIsPossible;
                currentShip.OnCanPerformActionWhileStressed += AlwaysAllow;

                var actions = currentShip.GetAvailableActions();

                currentShip.AskPerformFreeAction(
                    actions,
                    delegate
                    {
                        currentShip.OnCheckCanPerformActionsWhileStressed -= ConfirmThatIsPossible;
                        currentShip.OnCanPerformActionWhileStressed -= AlwaysAllow;
                        currentShip.BeforeActionIsPerformed -= PayCost;
                        GrantActionRecursive(callback);
                    },
                    HostShip.PilotInfo.PilotName,
                    "You may perform an action, even if you is stressed.",
                    HostShip
                );
            }
        }

        private void SetupGrantAction(Action callback)
        {
            if (Selection.MultiSelectedShips.Count < 1)
            {
                callback();
            }
            else
            {
                SelectedShips.AddRange(Selection.MultiSelectedShips);
                GrantAction(ShipIndex, callback);
            }
        }

        private void GrantAction(int shipIndex, Action callback)
        {
            GenericShip ship = SelectedShips[shipIndex];

            Selection.ThisShip = ship;
            Selection.DeselectAllShips();
            Selection.ChangeActiveShip(ship);

            ship.BeforeActionIsPerformed += PayCost;
            ship.OnCheckCanPerformActionsWhileStressed += ConfirmThatIsPossible;
            ship.OnCanPerformActionWhileStressed += AlwaysAllow;

            var actions = ship.GetAvailableActions();

            ship.AskPerformFreeAction(
                actions,
                delegate {
                    ship.OnCheckCanPerformActionsWhileStressed -= ConfirmThatIsPossible;
                    ship.OnCanPerformActionWhileStressed -= AlwaysAllow;
                    Selection.DeselectAllShips();
                    Selection.ThisShip = HostShip;
                    ship.BeforeActionIsPerformed -= PayCost;
                    if (ShipIndex < SelectedShips.Count - 1)
                    {
                        ShipIndex++;
                        GrantAction(ShipIndex, callback);
                    }
                    else
                    {
                        callback();
                    }
                },
                HostShip.PilotInfo.PilotName,
                "You may perform an action, even if you is stressed.",
                HostShip
            );

        }

        private void PayCost(GenericAction action, ref bool isFreeAction)
        {
            action.HostShip.BeforeActionIsPerformed -= PayCost;

            RegisterAbilityTrigger(TriggerTypes.BeforeActionIsPerformed, SpendCharge);
        }

        private void SpendCharge(object sender, EventArgs e)
        {
            HostShip.SpendCharge();
            Triggers.FinishTrigger();
        }

        private bool HasTargetsForAbility()
        {
            foreach (GenericShip ship in HostShip.Owner.Ships.Values)
            {
                if (FilterSelection(ship)) return true;
            }

            return false;
        }

        private bool FilterSelection(GenericShip ship)
        {
            return FilterByTargetType(ship, TargetTypes.OtherFriendly) && FilterTargetsByRange(ship, 0, 2);
        }

        private int GetAiPriority(GenericShip ship)
        {
            int priority = 0;

            if (!ship.Tokens.HasToken(typeof(Tokens.FocusToken))) priority += 100;

            priority += ship.PilotInfo.Cost;

            return priority;
        }

        private void ConfirmThatIsPossible(ref bool isAllowed)
        {
            AlwaysAllow(null, ref isAllowed);
        }

        private void AlwaysAllow(GenericAction action, ref bool isAllowed)
        {
            isAllowed = true;
        }

    }
}