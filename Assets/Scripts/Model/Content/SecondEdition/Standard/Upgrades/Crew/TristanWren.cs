using Ship;
using Upgrade;
using System.Collections.Generic;
using System;
using BoardTools;
using SquadBuilderNS;

namespace UpgradesList.SecondEdition
{
    public class TristanWren : GenericUpgrade
    {
        public TristanWren() : base()
        {
            UpgradeInfo = new UpgradeCardInfo
            (
                "Tristan Wren",
                UpgradeType.Crew,
                cost: 2,
                isLimited: true,
                charges: 1,
                regensCharges: true,
                restriction: new FactionRestriction(Faction.Rebel, Faction.Scum, Faction.Imperial),
                abilityType: typeof(Abilities.SecondEdition.TristanWrenCrewAbility)
            );

            ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/f/fb/Tristanwren.png";
        }

        public override bool IsAllowedForSquadBuilderPostCheck(SquadList squadList)
        {
            bool result = false;

            if (squadList.SquadFaction == Faction.Rebel)
            {
                result = true;
            }
            else
            {
                foreach (var shipHolder in squadList.Ships)
                {
                    if (shipHolder.Instance.PilotInfo.PilotName == "Gar Saxon")
                    {
                        return true;
                    }

                    foreach (var upgrade in shipHolder.Instance.UpgradeBar.GetUpgradesAll())
                    {
                        if (upgrade.UpgradeInfo.Name == "Gar Saxon")
                        {
                            return true;
                        }
                    }
                }

                if (result != true)
                {
                    Messages.ShowError("Tristan Wren cannot be in a non-Rebel squad that does not contain Gar Saxon");
                }

            }

            return result;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TristanWrenCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification
            (
                HostUpgrade.UpgradeInfo.Name,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Success },
                sideCanBeChangedTo: DieSide.Crit,
                isGlobal: true,
                payAbilityCost: PayAbilityCost
            );
        }

        private bool IsAvailable()
        {
            DistanceInfo distInfo = new DistanceInfo(Combat.Attacker, HostShip);

            return Tools.IsSameTeam(Combat.Attacker, HostShip)
                && distInfo.Range <= 3
                && IsSpecialMissileAttack()
                && HostUpgrade.State.Charges > 0
                && Combat.DiceRollAttack.RegularSuccesses > 0;
        }

        private bool IsSpecialMissileAttack()
        {
            return Combat.ChosenWeapon.WeaponType == WeaponTypes.Missile
                || Combat.ChosenWeapon.WeaponType == WeaponTypes.Torpedo
                || Combat.ChosenWeapon.WeaponType == WeaponTypes.Talent
                || Combat.ChosenWeapon.WeaponType == WeaponTypes.Force;
        }

        private int GetAiPriority()
        {
            return 41;
        }

        private void PayAbilityCost(Action<bool> callback)
        {
            HostUpgrade.State.LoseCharge();
            callback(true);
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}