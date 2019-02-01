using Actions;
using ActionsList;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ1AWing
    {
        public class ArvelCrynyd : RZ1AWing
        {
            public ArvelCrynyd() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Arvel Crynyd",
                    3,
                    34,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ArvelCrynydAbility),
                    extraUpgradeIcons: new List<UpgradeType> { UpgradeType.Talent, UpgradeType.Talent },
                    seImageNumber: 20
                );

                ModelInfo.SkinName = "Blue";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ArvelCrynydAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.PrimaryWeapons.ForEach(n => n.WeaponInfo.MinRange = 0);
            HostShip.OnCanAttackBumpedTarget += CanAttack;

            HostShip.OnActionIsFailed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.PrimaryWeapons.ForEach(n => n.WeaponInfo.MinRange = 1);
            HostShip.OnCanAttackBumpedTarget -= CanAttack;

            HostShip.OnActionIsFailed += CheckAbility;
        }

        private void CanAttack(ref bool canAttack, GenericShip attacker, GenericShip defender)
        {
            canAttack = true;
        }

        private void CheckAbility(GenericAction action, List<ActionFailReason> failReasons, ref bool isDefaultFailOverwritten)
        {
            if (action is BoostAction
                && failReasons.Count == 1
                && failReasons.First() == ActionFailReason.Bumped
            )
            {
                Messages.ShowInfo("!!!");
                isDefaultFailOverwritten = true;
            }
        }
    }
}