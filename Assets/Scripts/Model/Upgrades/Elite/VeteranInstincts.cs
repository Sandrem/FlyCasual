using Ship;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class VeteranInstincts : GenericUpgrade, IModifyPilotSkill
    {
        public VeteranInstincts() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Veteran Instincts";
            Cost = 1;

            AvatarOffset = new Vector2(56, 0);
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);
            host.AddPilotSkillModifier(this);
            Roster.UpdateShipStats(host);
        }

        public void ModifyPilotSkill(ref int pilotSkill)
        {
            pilotSkill += 2;
        }
    }
}
