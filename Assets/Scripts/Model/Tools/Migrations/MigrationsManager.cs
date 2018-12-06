using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Migrations
{
    public class CardInfo
    {
        public Type CardType { get; private set; }
        public Type RuleType { get; private set; }

        public CardInfo(Type cardType, Type ruleType)
        {
            CardType = cardType;
            RuleType = ruleType;
        }
    }

    public static class MigrationsManager
    {
        public static void PerformMigrations()
        {
            Console.Write("Migration Manager is called");

            int activeVersion = GetActiveVersion();
            if (activeVersion == Global.CurrentVersionInt)
            {
                Console.Write("Migrations are not needed: version is actual");
                return;
            }

            List<GenericMigration> plannedMigrations = new List<GenericMigration>();

            foreach (Type migrationType in GetAvailableMigrations())
            {
                GenericMigration migration = (GenericMigration)Activator.CreateInstance(migrationType);
                if (activeVersion < migration.Version)
                {
                    plannedMigrations.Add(migration);
                }
            }

            plannedMigrations.OrderBy(n => n.Version);

            foreach (GenericMigration plannedMigration in plannedMigrations)
            {
                plannedMigration.DoMigration();
            }

            Console.Write("All migrations are finished");
        }

        private static int GetActiveVersion()
        {
            return PlayerPrefs.GetInt("LastMigrationVersion", 0);
        }

        private static List<Type> GetAvailableMigrations()
        {
            IEnumerable<Type> typesIEnum =
                from types in Assembly.GetExecutingAssembly().GetTypes()
                where types.Namespace != null
                where types.Namespace == "Migrations.MigrationsList"
                select types;
            return typesIEnum.ToList();
        }
    }
}

