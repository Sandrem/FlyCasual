using Actions;
using ActionsList;
using BoardTools;
using RulesList;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class EnhancedJammingSuite : GenericUpgrade
    {
        public EnhancedJammingSuite() : base()
        {
            UpgradeInfo = new UpgradeCardInfo
            (
                "Enhanced Jamming Suite",
                types: new List<UpgradeType> { UpgradeType.Configuration, UpgradeType.Tech },
                cost: 0,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.TIEWiWhisperModifiedInterceptor.TIEWiWhisperModifiedInterceptor)),
                abilityType: typeof(Abilities.SecondEdition.EnhancedJammingSuiteAbility),
                addAction: new ActionInfo(typeof(JamAction)),
                addActionLinks: new List<LinkedActionInfo>()
                {
                    new LinkedActionInfo(typeof(FocusAction),       typeof(JamAction), ActionColor.White),
                    new LinkedActionInfo(typeof(BarrelRollAction),  typeof(JamAction), ActionColor.White),
                    new LinkedActionInfo(typeof(BoostAction),       typeof(JamAction), ActionColor.White)
                }
            );
            
            ImageUrl = "https://i.imgur.com/PZiB0nf.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class EnhancedJammingSuiteAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            JamRule.OnCheckJamIsAllowed += CheckToAllow;
            HostShip.AfterGotNumberOfDefenceDice += CheckDefenseBonus;
        }

        public override void DeactivateAbility()
        {
            JamRule.OnCheckJamIsAllowed -= CheckToAllow;
            HostShip.AfterGotNumberOfDefenceDice -= CheckDefenseBonus;
        }

        public void CheckToAllow(ref List<JamIsNotAllowedReasons> blockers, GenericShip jamSource, GenericShip jamTarget)
        {
            if (jamSource == HostShip)
            {
                // Allow friendly by team type
                if (blockers.Contains(JamIsNotAllowedReasons.FriendlyShip))
                {
                    if (Tools.IsSameTeam(jamSource, jamTarget)) blockers.Remove(JamIsNotAllowedReasons.FriendlyShip);
                }

                // Allow this ship by team type and range
                if (Tools.IsSameShip(jamTarget, HostShip))
                {
                    blockers.Remove(JamIsNotAllowedReasons.FriendlyShip);
                    blockers.Remove(JamIsNotAllowedReasons.NotInRange);
                }
            }
        }

        private void CheckDefenseBonus(ref int count)
        {
            if (AttackerHasNoGreenTokens() || ThereIsJammedShipInAttacjArc())
            {
                Messages.ShowInfo($"{HostUpgrade.UpgradeInfo.Name}: {HostShip.PilotInfo.PilotName} rolls 1 additional defense die");
                count++;
            }
        }

        private bool AttackerHasNoGreenTokens()
        {
            return Combat.Attacker.Tokens.CountTokensByColor(Tokens.TokenColors.Green) == 0;
        }

        private bool ThereIsJammedShipInAttacjArc()
        {
            foreach (GenericShip ship in Roster.AllShips.Values)
            {
                ShotInfoArc shotInfoArc = new ShotInfoArc(Combat.Attacker, ship, Combat.ArcForShot);
                if (shotInfoArc.InArc && ship.IsJammed) return true;
            }

            return false;
        }
    }
}