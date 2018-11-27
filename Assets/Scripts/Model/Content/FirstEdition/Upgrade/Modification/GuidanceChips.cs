using Upgrade;
using Ship;
using System.Collections.Generic;
using System.Linq;
using ActionsList;

namespace UpgradesList.FirstEdition
{
    public class GuidanceChips : GenericUpgrade
    {
        public GuidanceChips() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Guidance Chips",
                UpgradeType.Modification,
                cost: 0,
                abilityType: typeof(Abilities.FirstEdition.GuidanceChipsAbility)
            );
        }
    }
}

namespace Abilities.FirstEdition
{
    public class GuidanceChipsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += GuidanceChipsActionEffect;
            Phases.Events.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= GuidanceChipsActionEffect;
            Phases.Events.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void GuidanceChipsActionEffect(GenericShip host)
        {
            GenericAction newAction = new GuidanceChipsEffect()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = host,
                Source = HostUpgrade
            };
            host.AddAvailableDiceModification(newAction);
        }

        public bool IsGuidanceChipsAbilityUsed()
        {
            return IsAbilityUsed;
        }

        public void SetGuidancEShipsAbilityAsUsed()
        {
            IsAbilityUsed = true;
        }
    }
}

namespace ActionsList
{

    public class GuidanceChipsEffect : GenericAction
    {

        public GuidanceChipsEffect()
        {
            Name = DiceModificationName = "Guidance Chips";
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Attack && !(Source.UpgradeAbilities[0] as Abilities.FirstEdition.GuidanceChipsAbility).IsGuidanceChipsAbilityUsed())
            {
                GenericSecondaryWeapon secondaryWeapon = (Combat.ChosenWeapon as GenericSecondaryWeapon);
                if (secondaryWeapon != null)
                {
                    if (secondaryWeapon.HasType(UpgradeType.Torpedo) || secondaryWeapon.HasType(UpgradeType.Missile))
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.CurrentDiceRoll.Blanks == 1) result = 100;
            else if (Combat.CurrentDiceRoll.Blanks > 1) result = 55;
            else if (Combat.CurrentDiceRoll.Focuses == 1) result = 55;
            else result = 30;

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            DieSide newResult = (Host.State.Firepower >= 3) ? DieSide.Crit : DieSide.Success;

            DieSide oldResult = DieSide.Crit;
            if (Combat.CurrentDiceRoll.Blanks > 0) oldResult = DieSide.Blank;
            else if (Combat.CurrentDiceRoll.Focuses > 0) oldResult = DieSide.Focus;
            else if (Combat.CurrentDiceRoll.RegularSuccesses > 0) oldResult = DieSide.Success;

            Combat.CurrentDiceRoll.ChangeOne(oldResult, newResult);

            (Source.UpgradeAbilities[0] as Abilities.FirstEdition.GuidanceChipsAbility).SetGuidancEShipsAbilityAsUsed();

            callBack();
        }

    }

}