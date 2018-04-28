using Ship;
using Upgrade;
using Abilities;
using ActionsList;

namespace UpgradesList
{
    public class GuidanceChips : GenericUpgrade
    {
        public GuidanceChips() : base()
        {
            Types.Add(UpgradeType.Modification);
            Name = "Guidance Chips";
            Cost = 0;

            UpgradeAbilities.Add(new GuidanceChipsAbility());
        }
    }
}

namespace Abilities
{
    public class GuidanceChipsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList += GuidanceChipsActionEffect;
            Phases.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= GuidanceChipsActionEffect;
            Phases.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void GuidanceChipsActionEffect(GenericShip host)
        {
            GenericAction newAction = new GuidanceChipsEffect()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = host,
                Source = HostUpgrade
            };
            host.AddAvailableActionEffect(newAction);
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
            Name = EffectName = "Guidance Chips";
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Attack && !(Source.UpgradeAbilities[0] as GuidanceChipsAbility).IsGuidanceChipsAbilityUsed())
            {
                GenericSecondaryWeapon secondaryWeapon = (Combat.ChosenWeapon as GenericSecondaryWeapon);
                if (secondaryWeapon != null)
                {
                    if (secondaryWeapon.hasType(UpgradeType.Torpedo) || secondaryWeapon.hasType(UpgradeType.Missile))
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        public override int GetActionEffectPriority()
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
            DieSide newResult = (Host.Firepower >= 3) ? DieSide.Crit : DieSide.Success;

            DieSide oldResult = DieSide.Crit;
            if (Combat.CurrentDiceRoll.Blanks > 0) oldResult = DieSide.Blank;
            else if (Combat.CurrentDiceRoll.Focuses > 0) oldResult = DieSide.Focus;
            else if (Combat.CurrentDiceRoll.RegularSuccesses > 0) oldResult = DieSide.Success;

            Combat.CurrentDiceRoll.ChangeOne(oldResult, newResult);

            (Source.UpgradeAbilities[0] as GuidanceChipsAbility).SetGuidancEShipsAbilityAsUsed();

            callBack();
        }

    }

}
