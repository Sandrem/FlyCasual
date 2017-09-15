using Ship;
using Upgrade;

namespace UpgradesList
{
    public class VeteranInstincts : GenericUpgrade, IModifyPilotSkill
    {
        public VeteranInstincts() : base()
        {

            Type = UpgradeType.Elite;
            Name = "Veteran Instincts";
            ShortName = "Vet. Instincts";
            Cost = 1;
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
