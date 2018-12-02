using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.T70XWing
    {
        public class NienNunb : T70XWing
        {
            public NienNunb() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Nien Nunb",
                    7,
                    29,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.NienNunbAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class NienNunbAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            HostShip.OnTokenIsAssigned += RegisterNienNunbAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsAssigned -= RegisterNienNunbAbility;
        }

        private void RegisterNienNunbAbility(GenericShip ship, System.Type tokenType)
        {
            if (tokenType == typeof(Tokens.StressToken))
            {
                var shipCount = Roster.AllShips.Values
                    .Where(s => s.Owner.Id != HostShip.Owner.Id)
                    .Where(s =>
                    {
                        ShotInfo arcInfo = new ShotInfo(HostShip, s, HostShip.PrimaryWeapon);
                        return arcInfo.InArc && arcInfo.Range <= 1;
                    })
                    .Count();
                if (shipCount >= 1)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, AskRemoveStress);
                }
            }
        }

        private void AskRemoveStress(object sender, System.EventArgs e)
        {
            if (!alwaysUseAbility)
            {
                AskToUseAbility(AlwaysUseByDefault, RemoveStress, null, null, true);
            }
            else
            {
                HostShip.Tokens.RemoveToken(typeof(StressToken), Triggers.FinishTrigger);
            }
        }

        private void RemoveStress(object sender, System.EventArgs e)
        {
            HostShip.Tokens.RemoveToken(typeof(StressToken), SubPhases.DecisionSubPhase.ConfirmDecision);
        }
    }
}