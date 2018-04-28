using Ship;
using UnityEngine;
using Upgrade;
using Abilities;

namespace UpgradesList
{
    public class VeteranInstincts : GenericUpgrade
    {
        public VeteranInstincts() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Veteran Instincts";
            Cost = 1;

            AvatarOffset = new Vector2(56, 0);

            UpgradeAbilities.Add(new VeteranInstinctsAbility());
        }
    }
}

namespace Abilities
{
    public class VeteranInstinctsAbility : GenericAbility, IModifyPilotSkill
    {
        public override void ActivateAbility()
        {
            HostShip.AddPilotSkillModifier(this);
            Roster.UpdateShipStats(HostShip);
        }

        public override void DeactivateAbility()
        {
            HostShip.RemovePilotSkillModifier(this);
            Roster.UpdateShipStats(HostShip);
        }

        public void ModifyPilotSkill(ref int pilotSkill)
        {
            pilotSkill += 2;
        }
    }
}
