using System.Collections;
using System.Collections.Generic;
using BoardTools;
using Ship;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.ARC170
    {
        public class ThaneKyrell : ARC170
        {
            public ThaneKyrell() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Thane Kyrell",
                    4,
                    26,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.ThaneKyrellPilotAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    // After an enemy ship inside your firing arc at Range 1-3 attacks another friendly ship, you may perform a free action.
    public class ThaneKyrellPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnAttackFinishGlobal += RegisterBraylenStrammPilotAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackFinishGlobal -= RegisterBraylenStrammPilotAbility;
        }

        private void RegisterBraylenStrammPilotAbility(GenericShip ship)
        {
            if (Combat.Attacker == ship && ship.Owner != HostShip.Owner && Combat.Defender != HostShip && Combat.Defender.Owner == HostShip.Owner)
            {
                ShotInfo arcInfo = new ShotInfo(HostShip, ship, HostShip.PrimaryWeapon);
                if (arcInfo.InArc && arcInfo.Range <= 3)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, PerformFreeAction);
                }
            }
        }

        private void PerformFreeAction(object sender, System.EventArgs e)
        {
            var previousSelectedShip = Selection.ThisShip;
            Selection.ThisShip = HostShip;

            HostShip.AskPerformFreeAction(HostShip.GetAvailableActions(), delegate
            {
                Selection.ThisShip = previousSelectedShip;
                Triggers.FinishTrigger();
            });
        }
    }
}