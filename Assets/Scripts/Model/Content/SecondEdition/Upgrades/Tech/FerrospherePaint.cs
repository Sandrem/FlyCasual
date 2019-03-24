using ActionsList;
using BoardTools;
using Ship;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class FerrospherePaint : GenericUpgrade
    {
        public FerrospherePaint() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ferrosphere Paint",
                UpgradeType.Tech,
                cost: 6,
                restriction: new FactionRestriction(Faction.Resistance),
                abilityType: typeof(Abilities.SecondEdition.FerrospherePaintAbility)
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/55d8ff7d35b714d9c9a6ef1fd7732a60.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class FerrospherePaintAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnTargetLockIsAcquiredGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTargetLockIsAcquiredGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip activeShip, GenericShip lockedShip)
        {
            if (lockedShip == HostShip)
            {
                if (!activeShip.SectorsInfo.IsShipInSector(lockedShip, Arcs.ArcType.Bullseye))
                {
                    RegisterAbilityTrigger(TriggerTypes.OnTargetLockIsAcquired, delegate { AssignStress(activeShip); });
                }
            }
        }

        private void AssignStress(GenericShip activeShip)
        {
            Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + " on " + HostShip.PilotInfo.PilotName + " causes " + activeShip.PilotInfo.PilotName + " to gain 1 stress token from their target lock.");
            activeShip.Tokens.AssignToken(typeof(StressToken), Triggers.FinishTrigger);
        }
    }
}