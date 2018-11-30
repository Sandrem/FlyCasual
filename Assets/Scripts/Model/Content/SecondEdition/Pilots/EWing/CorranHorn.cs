using BoardTools;
using Ship;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.EWing
    {
        public class CorranHorn : EWing
        {
            public CorranHorn() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Corran Horn",
                    5,
                    74,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CorranHornAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 50
                );

                ModelInfo.SkinName = "Green";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //At initiative 0, you may perform a bonus primary attack against an enemy ship in your bullseye firing arc. 
    //If you do, at the start of the next Planning Phase, gain 1 disarm token.
    public class CorranHornAbility : CorranHornBaseAbility
    {
        public CorranHornAbility()
        {
            TriggerType = TriggerTypes.OnCombatPhaseEnd;
            Description = "You may perform a bonus bullseye primary attack\nGain 1 disarm token next round";
            ExtraAttackFilter = IsBullsEyePrimary;
        }

        public override void ActivateAbility()
        {
            //This is technically not the correct timing, but works for now. The combat phase should be rewritten to allow 
            //for abilities to add extra activations 
            Phases.Events.OnCombatPhaseEnd_Triggers += RegisterCorranHornAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseEnd_Triggers -= RegisterCorranHornAbility;
        }

        private bool IsBullsEyePrimary(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            bool result = false;
            if (weapon is PrimaryWeaponClass && new ShotInfo(HostShip, defender, weapon).InArcByType(Arcs.ArcType.Bullseye))
            {
                result = true;
            }
            else
            {
                if (!isSilent) Messages.ShowError("Attack must be performed with primary weapon in bullseye arc");
            }
            return result;
        }
    }
}