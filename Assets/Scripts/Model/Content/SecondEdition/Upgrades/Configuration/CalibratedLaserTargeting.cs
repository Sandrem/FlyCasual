using Arcs;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class CalibratedLaserTargeting : GenericUpgrade
    {
        public CalibratedLaserTargeting() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Calibrated Laser Targeting",
                types: new List<UpgradeType> { UpgradeType.Configuration, UpgradeType.Modification },
                cost: 0, //TODO
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.Delta7Aethersprite.Delta7Aethersprite)),
                abilityType: typeof(Abilities.SecondEdition.CalibratedLaserTargetingAbility)
                //seImageNumber: ??
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/4a/32/4a32d934-9d57-433c-8fb6-ce6c1cb52224/swz34_calibrated-laser-targeting.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    //While you perform a primary attack, if the defender is in your bullseye arc, add 1 focus result.
    public class CalibratedLaserTargetingAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostUpgrade.UpgradeInfo.Name,
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Add,
                1,
                sideCanBeChangedTo: DieSide.Focus
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
        public bool IsDiceModificationAvailable()
        {
            return (Combat.AttackStep == CombatStep.Attack
                && Combat.Attacker == HostShip
                && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon
                && Combat.Attacker.SectorsInfo.IsShipInSector(Combat.Defender, ArcType.Bullseye));
        }

        public int GetDiceModificationAiPriority()
        {
            return 110;
        }
    }
}