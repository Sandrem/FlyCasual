using Arcs;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.YV666
    {
        public class MoraloEval : YV666
        {
            public MoraloEval() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "MoraloEval",
                    6,
                    34,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.MoraloEvalAbility)
                );

                ModelInfo.SkinName = "Crimson";
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class MoraloEvalAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGameStart += ChangeSpecialWeaponsRestrictions;
        }

        public override void DeactivateAbility()
        {
            RestoreSpecialWeaponsRestrictions();
        }

        private void ChangeSpecialWeaponsRestrictions()
        {
            HostShip.OnGameStart -= ChangeSpecialWeaponsRestrictions;

            foreach (GenericUpgrade upgrade in HostShip.UpgradeBar.GetSpecialWeaponsAll())
            {
                GenericSpecialWeapon weapon = upgrade as GenericSpecialWeapon;
                if (weapon.HasType(UpgradeType.Cannon)) weapon.WeaponInfo.ArcRestrictions.Add(ArcType.FullFront);
            }
        }

        private void RestoreSpecialWeaponsRestrictions()
        {
            foreach (GenericUpgrade upgrade in HostShip.UpgradeBar.GetSpecialWeaponsAll())
            {
                GenericSpecialWeapon weapon = upgrade as GenericSpecialWeapon;
                if (weapon.HasType(UpgradeType.Cannon)) weapon.WeaponInfo.ArcRestrictions.Remove(ArcType.FullFront);
            }
        }

    }
}