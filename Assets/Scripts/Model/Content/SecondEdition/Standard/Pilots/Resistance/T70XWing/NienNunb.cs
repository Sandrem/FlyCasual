using BoardTools;
using Content;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class NienNunb : T70XWing
        {
            public NienNunb() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Nien Nunb",
                    "Sarcastic Survivor",
                    Faction.Resistance,
                    5,
                    5,
                    10,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.NienNunbAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile
                    },
                    tags: new List<Tags>
                    {
                        Tags.XWing
                    }
                );

                ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/00a3c393a33b33168bc61e47749e1474.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
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

        private void RegisterNienNunbAbility(GenericShip ship, GenericToken token)
        {
            if (token is StressToken)
            {
                var shipCount = Roster.AllShips.Values
                    .Where(s => s.Owner.Id != HostShip.Owner.Id)
                    .Where(s =>
                    {
                        ShotInfo arcInfo = new ShotInfo(HostShip, s, HostShip.PrimaryWeapons);
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
                AskToUseAbility(
                    HostShip.PilotInfo.PilotName,
                    AlwaysUseByDefault,
                    RemoveStress,
                    descriptionLong: "Do you want to discard Stress Token?",
                    imageHolder: HostShip,
                    showAlwaysUseOption: true
                );
            }
            else
            {
                HostShip.Tokens.RemoveToken(typeof(StressToken), Triggers.FinishTrigger);
            }
        }

        private void RemoveStress(object sender, System.EventArgs e)
        {
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + " discarded Stress token");
            HostShip.Tokens.RemoveToken(typeof(StressToken), SubPhases.DecisionSubPhase.ConfirmDecision);
        }
    }
}
