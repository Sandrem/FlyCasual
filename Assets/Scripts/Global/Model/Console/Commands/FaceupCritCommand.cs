using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DamageDeckCard;
using Ship;
using Players;

namespace CommandsList
{
    public class CritCardCommand : GenericCommand
    {
        private Dictionary<string, Type> stringToType = new Dictionary<string, Type>()
        {
            { "blindedpilot",       typeof(BlindedPilot)        },
            { "consolefire",        typeof(ConsoleFire)         },
            { "damagedcockpit",     typeof(DamagedCockpit)      },
            { "damagedengine",      typeof(DamagedEngine)       },
            { "damagedsensorarray", typeof(DamagedSensorArray)  },
            { "directhit",          typeof(DirectHit)           },
            { "loosestabilizer",    typeof(LooseStabilizer)     },
            { "majorexplosion",     typeof(MajorExplosion)      },
            { "majorhullbreach",    typeof(MajorHullBreach)     },
            { "shakenpilot",        typeof(ShakenPilot)         },
            { "structuraldamage",   typeof(StructuralDamage)    },
            { "stunnedpilot",       typeof(StunnedPilot)        },
            { "thrustcontrolfire",  typeof(ThrustControlFire)   },
            { "weaponsfailure",     typeof(WeaponsFailure)      }
        };

        public CritCardCommand()
        {
            Keyword = "faceupcrit";
            Description =   "Deals faceup damage card to ship\n" +
                            "faceupcrit id:<shipId> type:<critCardName>\n" +
                            "where critcardname: blindedpilot, consolefire, damagedcockpit, damagedengine, damagedsensorarray, directhit, loosestabilizer, majorexplosion, majorhullbreach, shakenpilot, structuraldamage, stunnedpilot, thrustcontrolfire, weaponsfailure";

            Console.AddAvailableCommand(this);
        }

        public override void Execute(Dictionary<string, string> parameters)
        {
            int shipId = -1;
            if (parameters.ContainsKey("id")) int.TryParse(parameters["id"], out shipId);

            Type critType = null;
            string typeString = (parameters.ContainsKey("type")) ? parameters["type"] : null;
            if (typeString != null)
            {
                if (stringToType.ContainsKey(typeString)) critType = stringToType[typeString];
            }

            if (shipId != -1 && typeString != null)
            {
                RegisterDealFaceupCrit(shipId, critType);
            }
            else
            {
                ShowHelp();
            }
        }

        private void RegisterDealFaceupCrit(int shipId, Type critType)
        {
            GenericShip ship = Roster.AllShips.FirstOrDefault(n => n.Key == "ShipId:" + shipId).Value;

            if (ship != null)
            {
                GenericDamageCard critCard = (GenericDamageCard)System.Activator.CreateInstance(critType);
                DamageDecks.GetDamageDeck(ship.Owner.PlayerNo).PutOnTop(critCard);

                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Console: dead faceup crit card",
                    TriggerType = TriggerTypes.OnAbilityDirect,
                    TriggerOwner = ship.Owner.PlayerNo,
                    EventHandler = delegate
                    {
                        ship.SufferHullDamage(
                            true,
                            new DamageSourceEventArgs
                            {
                                Source = "Console",
                                DamageType = DamageTypes.Console
                            }
                        );
                    },
                });

                Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, ShowMessage);
            }
            else
            {
                ShowHelp();
            }
        }

        private void ShowMessage()
        {
            Console.Write("FaceupCrit command is resolved", LogTypes.Everything, true);
        }

    }
}
