using Upgrade;
using System.Collections.Generic;
using Ship;
using System.Linq;
using ActionsList;
using Actions;

namespace UpgradesList.SecondEdition
{
    public class SaturationSalvo : GenericUpgrade
    {
        public SaturationSalvo() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Saturation Salvo",
                UpgradeType.Talent,
                cost: 6,
                abilityType: typeof(Abilities.SecondEdition.SaturationSalvoAbility),
                restriction: new ActionBarRestriction(typeof(ReloadAction)),
                seImageNumber: 14
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class SaturationSalvoAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModificationsOpposite += RegisterSaturationSalvoAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModificationsOpposite -= RegisterSaturationSalvoAbility;
        }

        private void RegisterSaturationSalvoAbility(GenericShip host)
        {
            GenericSpecialWeapon weapon = Combat.ChosenWeapon as GenericSpecialWeapon;
            if (weapon != null)
            {
                if (weapon.HasType(UpgradeType.Torpedo) || weapon.HasType(UpgradeType.Missile))
                {
                    ActionsList.GenericAction newAction = new ActionsList.SaturationSalvoActionEffect()
                    {
                        ImageUrl = HostUpgrade.ImageUrl,
                        HostShip = host
                    };
                    host.AddAvailableDiceModification(newAction);
                }
            }
        }
    }
}

namespace ActionsList
{
    public class SaturationSalvoActionEffect : GenericAction
    {

        public SaturationSalvoActionEffect()
        {
            Name = DiceModificationName = "Saturation Salvo";
            DiceModificationTiming = DiceModificationTimingType.Opposite;
        }

        public override int GetDiceModificationPriority()
        {
            return 80;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;
            GenericSpecialWeapon weapon = Combat.ChosenWeapon as GenericSpecialWeapon;

            if (Combat.AttackStep == CombatStep.Defence && weapon.State.UsesCharges && weapon.State.Charges > 0)
            {
                result = true;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            GenericSpecialWeapon weapon = Combat.ChosenWeapon as GenericSpecialWeapon;
            weapon.State.SpendCharge();
            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                NumberOfDiceCanBeRerolled = 2,
                IsOpposite = true,
                CallBack = callBack
            };
            diceRerollManager.Start();
        }

    }
}