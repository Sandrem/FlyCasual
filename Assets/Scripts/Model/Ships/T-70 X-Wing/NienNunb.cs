using System;
using Ship;
using Movement;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using BoardTools;

namespace Ship
{
    namespace T70XWing
    {
        public class NienNunb : T70XWing
        {
            public NienNunb() : base()
            {
                PilotName = "Nien Nunb";
                PilotSkill = 7;
                Cost = 29;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.NienNunbAbility());
            }
        }
    }
}

namespace Abilities
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

        private void RegisterNienNunbAbility(Ship.GenericShip ship, System.Type tokenType)
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

        private void RemoveStress(object sender, System.EventArgs e) {
            HostShip.Tokens.RemoveToken(typeof(StressToken), SubPhases.DecisionSubPhase.ConfirmDecision);
        }
    }
}