using Abilities.SecondEdition;
using BoardTools;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.TIEInterceptor
{
    public class CienaRee : TIEInterceptor
    {
        public CienaRee() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Ciena Ree",
                "Look Through My Eyes",
                Faction.Imperial,
                6,
                5,
                10,
                isLimited: true,
                abilityType: typeof(CienaReeAbility),
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Tie
                }
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/19/2b/192b0e2b-9b56-4480-bced-13933545bae3/swz84_pilot_cienaree.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CienaReeAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker += CheckKillAbility;
            GenericShip.OnShipIsDestroyedGlobal += CheckDestroyedAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker -= CheckKillAbility;
            GenericShip.OnShipIsDestroyedGlobal -= CheckDestroyedAbility;
        }

        private void CheckKillAbility(GenericShip ship)
        {
            if (Combat.Defender.IsDestroyed) RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, GetStress);
        }

        private void GetStress(object sender, EventArgs e)
        {
            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName} gains 1 stress token");
            HostShip.Tokens.AssignToken(typeof(StressToken), Triggers.FinishTrigger);
        }

        private void CheckDestroyedAbility(GenericShip ship, bool flag)
        {
            if (Tools.IsSameTeam(HostShip, ship))
            {
                DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
                if (distInfo.Range <= 3)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnShipIsDestroyed, RemoveStressToken);
                }
            }
        }

        private void RemoveStressToken(object sender, EventArgs e)
        {
            if (HostShip.Tokens.HasToken<StressToken>())
            {
                Messages.ShowInfo($"{HostShip.PilotInfo.PilotName} removes 1 stress token");
                HostShip.Tokens.RemoveToken(typeof(StressToken), Triggers.FinishTrigger);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}