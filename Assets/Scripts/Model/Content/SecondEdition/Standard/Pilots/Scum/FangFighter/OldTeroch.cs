using Arcs;
using BoardTools;
using Content;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class OldTeroch : FangFighter
        {
            public OldTeroch() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Old Teroch",
                    "Mandalorian Mentor",
                    Faction.Scum,
                    5,
                    6,
                    16,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.OldTerochAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Torpedo
                    },
                    tags: new List<Tags>
                    {
                        Tags.Mandalorian
                    },
                    seImageNumber: 156,
                    skinName: "Zealous Recruit"
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //At the start of the Engagement Phase, you may choose 1 enemy ship at range 1.
    //If you do and you are in its (front arc), it removes all of its green tokens.

    public class OldTerochAbility : GenericAbility
    {
        private string AbilityDescription = "Choose a ship. If you are inside its front firing arc, it removes all of its green tokens.";

        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += CheckOldTerochAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= CheckOldTerochAbility;
        }

        private void CheckOldTerochAbility()
        {
            // give user the option to use ability
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskSelectShip);
        }

        private void AskSelectShip(object sender, System.EventArgs e)
        {
            // first check if there is at least one enemy at range 1
            if (TargetsForAbilityExist(FilterTargetsOfAbility))
            {
                Selection.ChangeActiveShip(HostShip);

                // Available selection are only within Range 1.
                // TODO : build the list if the enemy can fire to the ship
                SelectTargetForAbility(
                    ActivateOldTerochAbility,
                    FilterTargetsOfAbility,
                    GetAiPriorityOfTarget,
                    HostShip.Owner.PlayerNo,
                    HostShip.PilotInfo.PilotName,
                    AbilityDescription,
                    HostShip
                );
            }
            else
            {
                // no enemy in range
                Triggers.FinishTrigger();
            }
        }

        private bool FilterTargetsOfAbility(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.Enemy }) && FilterTargetsByRange(ship, 1, 1) && FilterTargetByArcAndTokens(ship);
        }

        private int GetAiPriorityOfTarget(GenericShip ship)
        {
            int priority = 50;

            priority += ship.Tokens.CountTokensByType(typeof(FocusToken)) * 10;
            priority += ship.Tokens.CountTokensByType(typeof(EvadeToken)) * 10;
            if (ActionsHolder.HasTargetLockOn(ship, HostShip)) priority += 10;
            priority += ship.State.Firepower * 10;

            return priority;
        }

        private bool FilterTargetByArcAndTokens(GenericShip ship)
        {
            bool inFrontArcOfEnemy = ship.SectorsInfo.IsShipInSector(HostShip, ArcType.Front);
            return inFrontArcOfEnemy && ship.Tokens.HasGreenTokens;
        }

        private void ActivateOldTerochAbility()
        {
            ShotInfo shotInfo = new ShotInfo(TargetShip, HostShip, HostShip.PrimaryWeapons);
            // Range is already checked in "SelectTargetForAbility", only check if the Host is in the Target firing arc.
            // Do not use InPrimaryWeaponFireZone, reason :
            //		VCX-100 without docked Phantom cannot shoot using special rear arc,so InPrimaryWeaponFireZone
            //		will be false but this is still arc, so ability should be active - so just InArc is checked,
            //		even ship cannot shoot from it.
            if (shotInfo.InArc == true)
            {
                DiscardTokens();
            }
            else
            {
                Messages.ShowError(HostShip.PilotInfo.PilotName + " is not within " + TargetShip.PilotInfo.PilotName + "'s firing arc");
            }
        }

        private void DiscardTokens()
        {
            Messages.ShowInfo(string.Format("{0} removed all green tokens from {1}", HostShip.PilotInfo.PilotName, TargetShip.PilotInfo.PilotName));
            TargetShip.Tokens.RemoveAllTokensByColor(TokenColors.Green, SelectShipSubPhase.FinishSelection);
        }

        private void DiscardAllFocusTokens()
        {
            TargetShip.Tokens.RemoveAllTokensByType(
                typeof(FocusToken),
                DiscardAllEvadeTokens
            );
        }

        private void DiscardAllEvadeTokens()
        {
            TargetShip.Tokens.RemoveAllTokensByType(
                typeof(EvadeToken),
                SelectShipSubPhase.FinishSelection
            );
        }
    }
}
