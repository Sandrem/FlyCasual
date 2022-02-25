using Ship;
using Upgrade;
using ActionsList;
using Actions;
using SquadBuilderNS;
using UnityEngine;
using Content;

namespace UpgradesList.SecondEdition
{
    public class Maul : GenericUpgrade
    {
        public Maul() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Maul",
                UpgradeType.Crew,
                cost: 10,
                isLimited: true,
                addForce: 1,
                restriction: new FactionRestriction(Faction.Scum, Faction.Rebel),
                abilityType: typeof(Abilities.SecondEdition.MaulCrewAbility),
                seImageNumber: 136
            );

            Avatar = new AvatarInfo(
                Faction.Scum,
                new Vector2(418, 15)
            );
        }

        public override bool IsAllowedForSquadBuilderPostCheck(SquadList squadList)
        {
            bool result = false;

            if (squadList.SquadFaction == Faction.Scum)
            {
                result = true;
            }

            if (squadList.SquadFaction == Faction.Rebel)
            {
                foreach (var shipHolder in squadList.Ships)
                {
                    if (shipHolder.Instance.PilotInfo.PilotName == "Ezra Bridger")
                    {
                        return true;
                    }

                    foreach (var upgrade in shipHolder.Instance.UpgradeBar.GetUpgradesAll())
                    {
                        if (upgrade.UpgradeInfo.Name == "Ezra Bridger")
                        {
                            return true;
                        }
                    }
                }

                if (result != true)
                {
                    Messages.ShowError("Maul cannot be in a Rebel squad that does not contain Ezra Bridger");
                }

            }

            return result;
        }
    }
}


namespace Abilities.SecondEdition
{
    //After you suffer damage, you may gain 1 stress token to recover 1 force.
    //You can equip 'Dark Side' upgrades.
    public class MaulCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShieldLost += RegisterAbilityShield;
            HostShip.OnDamageCardIsDealt += RegisterAbilityHull;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShieldLost -= RegisterAbilityShield;
            HostShip.OnDamageCardIsDealt -= RegisterAbilityHull;
        }

        public override void ActivateAbilityForSquadBuilder()
        {
            HostShip.OnUpgradeEquipTagCheck += AllowDarkSideUpgrades;
        }

        public override void DeactivateAbilityForSquadBuilder()
        {
            HostShip.OnUpgradeEquipTagCheck -= AllowDarkSideUpgrades;
        }

        private void AllowDarkSideUpgrades(Tags tag, ref bool result)
        {
            if (tag == Tags.DarkSide) result = true;
        }

        private void RegisterAbilityShield()
        {
            if (HostShip.State.Force < HostShip.State.MaxForce)
                RegisterAbilityTrigger(TriggerTypes.OnShieldIsLost, RecoverForceToken);
        }

        private void RegisterAbilityHull(GenericShip ship)
        {
            if (HostShip.State.Force < HostShip.State.MaxForce)
                RegisterAbilityTrigger(TriggerTypes.OnDamageCardIsDealt, RecoverForceToken);
        }

        private void RecoverForceToken(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                ShouldUseAbility, 
                RecoverForce, 
                descriptionLong: "Do you want to gain 1 Stress Token to recover 1 Force?",
                imageHolder: HostUpgrade
            );
        }

        private bool ShouldUseAbility()
        {
            return HostShip.Tokens.CountTokensByType<Tokens.StressToken>() == 0;
        }

        private void RecoverForce(object sender, System.EventArgs e)
        {
            if (HostShip.State.Force < HostShip.State.MaxForce)
            {
                HostShip.State.RestoreForce();
                HostShip.Tokens.AssignToken(new Tokens.StressToken(HostShip), SubPhases.DecisionSubPhase.ConfirmDecision);
            }
            else
            {
                SubPhases.DecisionSubPhase.ConfirmDecision();
            }
        }
    }
}