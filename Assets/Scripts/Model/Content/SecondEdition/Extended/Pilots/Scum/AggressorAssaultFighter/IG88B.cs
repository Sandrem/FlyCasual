using Content;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.AggressorAssaultFighter
    {
        public class IG88B : AggressorAssaultFighter
        {
            public IG88B() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "IG-88B",
                    "Brutal Battledroid",
                    Faction.Scum,
                    4,
                    7,
                    16,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.IG88BAbility),
                    tags: new List<Tags>
                    {
                        Tags.BountyHunter,
                        Tags.Droid
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Cannon,
                        UpgradeType.Cannon,
                        UpgradeType.Device,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    seImageNumber: 198,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class IG88BAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker += CheckIG88Ability;
            Phases.Events.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker -= CheckIG88Ability;
            Phases.Events.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void CheckIG88Ability()
        {
            if (!IsAbilityUsed && !HostShip.IsCannotAttackSecondTime && HasCannonWeapon())
            {
                IsAbilityUsed = true;

                // Trigger must be registered just before it's resolution
                HostShip.OnCombatCheckExtraAttack += RegisterIG88BAbility;
            }
        }

        private bool HasCannonWeapon()
        {
            return HostShip.UpgradeBar.GetUpgradesOnlyFaceup().Count(n => n.HasType(UpgradeType.Cannon) && (n as IShipWeapon) != null) > 0;
        }

        private void RegisterIG88BAbility(GenericShip ship)
        {
            HostShip.OnCombatCheckExtraAttack -= RegisterIG88BAbility;

            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, UseIG88BAbility);
        }

        private void UseIG88BAbility(object sender, System.EventArgs e)
        {
            if (!HostShip.IsCannotAttackSecondTime)
            {
                HostShip.IsCannotAttackSecondTime = true;

                Combat.StartSelectAttackTarget(
                    HostShip,
                    FinishAdditionalAttack,
                    IsCannonShot,
                    HostShip.PilotInfo.PilotName,
                    "You may perform a cannon attack",
                    HostShip
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot attack another time", HostShip.PilotInfo.PilotName));
                Triggers.FinishTrigger();
            }
        }

        private void FinishAdditionalAttack()
        {
            // If attack is skipped, set this flag, otherwise regular attack can be performed second time
            Selection.ThisShip.IsAttackPerformed = true;

            //if bonus attack was skipped, allow bonus attacks again
            if (Selection.ThisShip.IsAttackSkipped) Selection.ThisShip.IsCannotAttackSecondTime = false;

            Triggers.FinishTrigger();
        }

        private bool IsCannonShot(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            bool result = false;

            GenericSpecialWeapon upgradeWeapon = weapon as GenericSpecialWeapon;
            if (upgradeWeapon != null && upgradeWeapon.HasType(UpgradeType.Cannon))
            {
                result = true;
            }
            else
            {
                if (!isSilent) Messages.ShowError("This attack must be performed using a Cannon");
            }

            return result;
        }
    }
}