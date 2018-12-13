using BoardTools;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.ProtectorateStarfighter
    {
        public class OldTeroch : ProtectorateStarfighter
        {
            public OldTeroch() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Old Teroch",
                    7,
                    26,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.OldTerochAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    // At the start of the combat phase, you may choose 1 enemy ship
    //  at range 1. If you're inside its firing arc, it discards all
    //  focus and evade tokens.
    public class OldTerochAbility : GenericAbility
    {
        protected string AbilityDescription = "Choose a ship. If you are inside its firing arc, it discards all focus and evade tokens.";
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
                // Available selection are only within Range 1.
                // TODO : build the list if the enemy can fire to the ship
                SelectTargetForAbility(
                    ActivateOldTerochAbility,
                    FilterTargetsOfAbility,
                    GetAiPriorityOfTarget,
                    HostShip.Owner.PlayerNo,
                    HostShip.PilotName,
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
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.Enemy }) && FilterTargetsByRange(ship, 1, 1) && FilterTargetInEnemyArcWithTokens(ship);
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

        private bool FilterTargetInEnemyArcWithTokens(GenericShip ship)
        {
            ShotInfo shotInfo = new ShotInfo(ship, HostShip, ship.PrimaryWeapons);
            return shotInfo.InArc && (ship.Tokens.HasToken(typeof(FocusToken)) || ship.Tokens.HasToken(typeof(EvadeToken)));
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
                Messages.ShowError(HostShip.PilotName + " is not within " + TargetShip.PilotName + " firing arc.");
            }
        }

        protected virtual void DiscardTokens()
        {
            Messages.ShowInfo(string.Format("{0} discarded all Focus and Evade tokens from {1}", HostShip.PilotName, TargetShip.PilotName));
            DiscardAllFocusTokens();
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