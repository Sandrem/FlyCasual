using ActionsList;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEVnSilencer
    {
        public class Rush : TIEVnSilencer
        {
            public Rush() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Rush\"",
                    2,
                    57,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.RushAbility)
                );

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/886d715885da65bdf10ad7c68e4d0a93.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class RushAbility : GenericAbility, IModifyPilotSkill
    {
        public override void ActivateAbility()
        {
            HostShip.OnDamageCardIsDealt += ApplyInitiative;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDamageCardIsDealt -= ApplyInitiative;
            HostShip.State.RemovePilotSkillModifier(this);
        }

        private void ApplyInitiative(GenericShip ship)
        {
            HostShip.OnDamageCardIsDealt -= ApplyInitiative;
            HostShip.State.AddPilotSkillModifier(this);
        }

        public void ModifyPilotSkill(ref int pilotSkill)
        {
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": Iniative is set to 6");
            pilotSkill = 6;
        }
    }
}