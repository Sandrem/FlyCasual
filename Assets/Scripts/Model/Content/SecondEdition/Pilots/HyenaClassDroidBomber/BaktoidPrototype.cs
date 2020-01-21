using System;
using System.Collections.Generic;
using System.Linq;
using Ship;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.HyenaClassDroidBomber
{
    public class BaktoidPrototype : HyenaClassDroidBomber
    {
        public BaktoidPrototype()
        {
            PilotInfo = new PilotCardInfo(
                "Baktoid Prototype",
                1,
                28,
                limited: 2,
                abilityType: typeof(Abilities.SecondEdition.BaktoidPrototypeAbility),
                extraUpgradeIcons: new List<UpgradeType> { UpgradeType.Sensor, UpgradeType.Missile, UpgradeType.Missile },
                pilotTitle: "Function over Form"
            );
            
            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/254bc7be63cc9f4e629198f8d45f2f9b.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BaktoidPrototypeAbility : GenericAbility
    {
        private List<Type> SupportedTokenTypes = new List<Type>() {
            typeof(BlueTargetLockToken),
            typeof(CalculateToken),
            typeof(FocusToken)
        };

        public override void ActivateAbility()
        {
            HostShip.OnModifyWeaponAttackRequirement += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnModifyWeaponAttackRequirement -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, GenericSpecialWeapon weapon, ref Type tokenType, bool isSilent)
        {
            if (ConditionsAreMet(tokenType))
            {
                if (!isSilent) Messages.ShowInfo(string.Format("{0} ignores attack requirement", ship.PilotInfo.PilotName));
                tokenType = null;
            }
        }

        private bool ConditionsAreMet(Type tokenType)
        {
            return SupportedTokenTypes.Contains(tokenType)
                && HostShip.Owner.Ships.Values.Any(
                    n => ActionsHolder.HasTargetLockOn(n, Selection.AnotherShip)
                    && n.ShipAbilities.Any(a => a is NetworkedCalculationsAbility)
                );
        }
    }
}