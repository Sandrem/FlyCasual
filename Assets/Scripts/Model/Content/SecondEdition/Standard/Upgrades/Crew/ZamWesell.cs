using Ship;
using Upgrade;
using System.Linq;
using Tokens;
using System;
using Conditions;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class ZamWesell : GenericUpgrade
    {
        public ZamWesell() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Zam Wesell",
                UpgradeType.Crew,
                cost: 11,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Separatists, Faction.Scum),
                abilityType: typeof(Abilities.SecondEdition.ZamWesellCrewAbility),
                charges: 2
            );

            Avatar = new AvatarInfo(
                Faction.Scum,
                new Vector2(239, 1)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/77/bd/77bd5f12-05de-4c34-9e5a-e8dfa636de52/swz82_a1_upgrade_zam-wessel.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class ZamWesellCrewAbility : ZamWesellPilotAbility
    {
        protected override string AbilityHostName { get { return HostUpgrade.UpgradeInfo.Name; } }

        protected override void LoseChargesOnSetup(GenericShip ship)
        {
            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: 2 Charges on {HostUpgrade.UpgradeInfo.Name} are lost during Setup");
            HostUpgrade.State.SpendCharges(2);
        }

        protected override void AssignSecretCondition(Type conditionType)
        {
            AssignedCondition = Activator.CreateInstance(conditionType, HostShip) as ZamWesellSecretCondition;
            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: Secret condition of {HostUpgrade.UpgradeInfo.Name} is assigned");
            HostShip.Tokens.AssignToken(AssignedCondition, Triggers.FinishTrigger);
        }

        protected override void RestoreCharges(int count)
        {
            HostUpgrade.State.RestoreCharges(count);
        }

        protected override int GetCharges()
        {
            return HostUpgrade.State.Charges;
        }

        protected override void LoseCharges(int count)
        {
            for (int i = 0; i < count; i++)
            {
                HostUpgrade.State.LoseCharge();
            }
        }
    }
}