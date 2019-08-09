using Upgrade;
using Ship;
using ActionsList;
using System;
using BoardTools;
using System.Linq;

namespace UpgradesList.SecondEdition
{
    public class SeventhFleetGunner : GenericUpgrade
    {
        public SeventhFleetGunner() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Seventh Fleet Gunner",
                UpgradeType.Gunner,
                cost: 9,
                restriction: new FactionRestriction(Faction.Republic),
                charges: 1,
                abilityType: typeof(Abilities.SecondEdition.SeventhFleetGunnerAbility)
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/a532c1de311e8d0a288af8232495a007.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SeventhFleetGunnerAbility : GenericAbility
    {
        public override string Name { get { return "Seventh Fleet Gunner (ID: " + HostShip.ShipId + ")"; } }

        public override void ActivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal += CheckAbility;
            HostShip.OnCheckSystemsAbilityActivation += CheckRestoreAbility;
            HostShip.OnSystemsAbilityActivation += RegisterRestoreAbilityTrigger;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal -= CheckAbility;
            HostShip.OnCheckSystemsAbilityActivation -= CheckRestoreAbility;
            HostShip.OnSystemsAbilityActivation -= RegisterRestoreAbilityTrigger;
        }

        private void CheckRestoreAbility(GenericShip ship, ref bool flag)
        {
            if (HostUpgrade.State.Charges == 0) flag = true;
        }

        private void RegisterRestoreAbilityTrigger(GenericShip ship)
        {
            if (HostUpgrade.State.Charges == 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, AskToRestoreAbility);
            }
        }

        private void AskToRestoreAbility(object sender, EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            AskToUseAbility(
                "Seventh Fleet Gunner",
                NeverUseByDefault,
                RestoreAbility,
                descriptionLong: "Do you want to gain 1 Disarm Token to recover 1 Charge?",
                imageHolder: HostUpgrade
            );
        }

        private void RestoreAbility(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            HostUpgrade.State.RestoreCharge();

            HostShip.Tokens.AssignToken(
                typeof(Tokens.WeaponsDisabledToken),
                Triggers.FinishTrigger
            );
        }

        private void CheckAbility()
        {
            if (Combat.Attacker.ShipId != HostShip.ShipId
                && Combat.Attacker.Owner.PlayerNo == HostShip.Owner.PlayerNo
                && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon
                && HostUpgrade.State.Charges > 0
            )
            {
                ShotInfo shotInfo = new ShotInfo(HostShip, Combat.Defender, HostShip.PrimaryWeapons.First());
                if (shotInfo.InArc)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskToUseGunnerAbility);
                }
            }
        }

        private void AskToUseGunnerAbility(object sender, EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            AskToUseAbility(
                "Seventh Fleet Gunner (ID:" + HostShip.ShipId + ")",
                NeverUseByDefault,
                UseGunnerAbility,
                descriptionLong: "You may spend charge to allow the attaker to roll 1 additional die, to a maximum of 4",
                imageHolder: HostUpgrade
            );
        }

        private void UseGunnerAbility(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            HostUpgrade.State.LoseCharge();

            Combat.Attacker.AfterGotNumberOfPrimaryWeaponAttackDice += AddDie;
            Combat.Attacker.AfterGotNumberOfAttackDiceCap += Max4Dice;

            Selection.ChangeActiveShip(Combat.Attacker);
            Triggers.FinishTrigger();
        }

        private void AddDie(ref int count)
        {
            Combat.Attacker.AfterGotNumberOfPrimaryWeaponAttackDice -= AddDie;
            Messages.ShowInfo("Seventh Fleet Gunner: Attacker rolls an additional attack die");
            count++;
        }

        private void Max4Dice(ref int count)
        {
            Combat.Attacker.AfterGotNumberOfAttackDiceCap -= Max4Dice;
            if (count > 4)
            {
                Messages.ShowInfo("Seventh Fleet Gunner: Only 4 attack dice are allowed");
                count = 4;
            }
        }
    }
}