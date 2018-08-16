using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ship;

namespace CommandsList
{
    public class DealDamageCommand : GenericCommand, IDamageSource
    {
        public DealDamageCommand()
        {
            Keyword = "dealdamage";
            Description =   "Deals damage to ship:\n" +
                            "dealdamage id:<shipId> hits:<number> crits:<number>";

            Console.AddAvailableCommand(this);
        }

        public override void Execute(Dictionary<string, string> parameters)
        {
            int shipId = -1;
            if (parameters.ContainsKey("id")) int.TryParse(parameters["id"], out shipId);

            int regularDamage = 0;
            if (parameters.ContainsKey("hits")) int.TryParse(parameters["hits"], out regularDamage);

            int criticalDamage = 0;
            if (parameters.ContainsKey("crits")) int.TryParse(parameters["crits"], out criticalDamage);

            if (shipId != -1 && regularDamage >= 0 && criticalDamage >= 0)
            {
                DealDamage(shipId, regularDamage, criticalDamage);
            }
            else
            {
                ShowHelp();
            }
        }

        private void DealDamage(int shipId, int regularDamage, int criticalDamage)
        {
            GenericShip ship = Roster.AllShips.FirstOrDefault(n => n.Key == "ShipId:" + shipId).Value;

            if (ship != null)
            {
                DamageSourceEventArgs consoleDamage = new DamageSourceEventArgs()
                {
                    Source = this,
                    SourceDescription = "Console",
                    DamageType = DamageTypes.Console
                };

                ship.Damage.TryResolveDamage(regularDamage, criticalDamage, consoleDamage, ShowMessage);
            }
            else
            {
                ShowHelp();
            }
        }

        private void ShowMessage()
        {
            Console.Write("DealDamage command is resolved", LogTypes.Everything, true);
        }
    }
}
