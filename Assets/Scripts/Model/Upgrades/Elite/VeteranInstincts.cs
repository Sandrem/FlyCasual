using Ship;
using UnityEngine;
using Upgrade;
using Abilities;

namespace UpgradesList
{
    public class VeteranInstincts : GenericUpgrade, IModifyPilotSkill
    {
        public VeteranInstincts() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Veteran Instincts";
            Cost = 1;

            Avatar = new AvatarInfo(Faction.Imperial, new Vector2(56, 0));

            UpgradeAbilities.Add(new VeteranInstinctsAbility());
        }

        public override void PreAttachToShip(GenericShip host)
        {
            base.PreAttachToShip(host);

            host.AddPilotSkillModifier(this);
        }

        public override void PreDettachFromShip()
        {
            base.PreDettachFromShip();

            Host.RemovePilotSkillModifier(this);
        }

        public void ModifyPilotSkill(ref int pilotSkill)
        {
            pilotSkill += 2;
        }
    }
}

namespace Abilities
{
    public class VeteranInstinctsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Roster.UpdateShipStats(HostShip);
        }

        public override void DeactivateAbility()
        {
            Roster.UpdateShipStats(HostShip);
        }
    }
}
