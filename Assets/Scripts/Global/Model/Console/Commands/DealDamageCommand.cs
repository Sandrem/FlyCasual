using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ship;

namespace CommandsList
{
    public class DealDamageCommand : GenericCommand
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
                for (int i = 0; i < regularDamage; i++)
                {
                    ship.AssignedDamageDiceroll.AddDice(DieSide.Success);

                    Triggers.RegisterTrigger(new Trigger()
                    {
                        Name = "Suffer damage",
                        TriggerType = TriggerTypes.OnDamageIsDealt,
                        TriggerOwner = ship.Owner.PlayerNo,
                        EventHandler = ship.SufferDamage,
                        Skippable = true,
                        EventArgs = new DamageSourceEventArgs()
                        {
                            Source = "Console",
                            DamageType = DamageTypes.Console
                        }
                    });
                }

                for (int i = 0; i < criticalDamage; i++)
                {
                    ship.AssignedDamageDiceroll.AddDice(DieSide.Crit);

                    Triggers.RegisterTrigger(new Trigger()
                    {
                        Name = "Suffer damage",
                        TriggerType = TriggerTypes.OnDamageIsDealt,
                        TriggerOwner = ship.Owner.PlayerNo,
                        EventHandler = ship.SufferDamage,
                        Skippable = true,
                        EventArgs = new DamageSourceEventArgs()
                        {
                            Source = "Console",
                            DamageType = DamageTypes.Console
                        }
                    });
                }

                Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, ShowMessage);
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
