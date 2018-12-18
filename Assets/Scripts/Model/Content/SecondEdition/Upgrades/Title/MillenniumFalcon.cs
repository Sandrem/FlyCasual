using Ship;
using Upgrade;
using System.Collections.Generic;
using Actions;
using ActionsList;
using Tokens;
using System.Linq;

namespace UpgradesList.SecondEdition
{
    public class MillenniumFalcon : GenericUpgrade
    {
        public MillenniumFalcon() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Millennium Falcon",
                UpgradeType.Title,
                cost: 6,
                isLimited: true,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.YT1300.YT1300)),
                addAction: new ActionInfo(typeof(EvadeAction)),
                abilityType: typeof(Abilities.SecondEdition.MilleniumFalconAbility),
                seImageNumber: 103
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class MilleniumFalconAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsDiceModificationAvailable,
                GetDiceModificationPriority,
                DiceModificationType.Reroll,
                1
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsDiceModificationAvailable()
        {
            bool result = false;
            if (HostShip.Tokens.HasToken(typeof(EvadeToken)) && (Combat.AttackStep == CombatStep.Defence))
            {
                result = true;
            }
            return result;
        }

        private int GetDiceModificationPriority()
        {
            return 90;
        }

        private int HasTokenPriority(GenericShip ship)
        {
            if (ship.Tokens.HasToken(typeof(FocusAction))) return 100;
            if (ship.ActionBar.HasAction(typeof(EvadeAction)) || ship.Tokens.HasToken(typeof(EvadeAction))) return 50;
            if (ship.ActionBar.HasAction(typeof(TargetLockAction)) || ship.Tokens.HasToken(typeof(TargetLockAction), '*')) return 50;
            return 0;
        }
        private int GetAiPriority(GenericShip ship)
        {
            int result = 0;

            result += HasTokenPriority(ship);
            result += ship.PilotInfo.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.UpgradeInfo.Cost);

            return result;
        }
    }
}
