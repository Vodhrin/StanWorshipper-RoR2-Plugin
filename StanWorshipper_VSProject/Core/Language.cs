using System;
using R2API;

namespace StanWorshipper.Core
{
    internal static class Language
    {
        public static void Initialize()
        {
            LanguageAPI.Add("STANWORSHIPPER_DESCRIPTION", "An apostle of Stan." + Environment.NewLine);
            LanguageAPI.Add("STANWORSHIPPER_NAME", "Stan Worshipper" + Environment.NewLine);
            LanguageAPI.Add("STANWORSHIPPER_SUBTITLE", "Crazed, Fat, and Horny" + Environment.NewLine);
            LanguageAPI.Add("STANWORSHIPPER_OUTRO_FLAVOR", "..and so he left, horny as fuck.");

            LanguageAPI.Add("STANWORSHIPPER_PASSIVE_NAME", "Bound by Blood");
            LanguageAPI.Add("STANWORSHIPPER_PASSIVE_DESCRIPTION", "Damage inflicted by summoned Stans is treated as being dealt by you.");
            LanguageAPI.Add("STANWORSHIPPER_SLOT1_PRIMARY_NAME", "Poopslinger");
            LanguageAPI.Add("STANWORSHIPPER_SLOT1_PRIMARY_DESCRIPTION", "Launches a smelly fart glob which deals <style=cIsDamage>650% damage</style>.");
            LanguageAPI.Add("STANWORSHIPPER_SLOT2_SECONDARY_NAME", "Summon Minor Stan Fragments");
            LanguageAPI.Add("STANWORSHIPPER_SLOT2_SECONDARY_DESCRIPTION", "Summons <style=cIsUtility>3 weak Stan fragments</style>. Each fragment burns enemies in a 5m radius around itself for <style=cIsDamage>140% damage</style>.");
            LanguageAPI.Add("STANWORSHIPPER_SLOT3_UTILITY_NAME", "Sacrifice");
            LanguageAPI.Add("STANWORSHIPPER_SLOT3_UTILITY_DESCRIPTION", "Deals <style=cIsDamage>175% damage</style> every second for 7 seconds in a 30m radius around you. Also heals you for <style=cIsHealth>8% of your maximum health</style>."); 
            LanguageAPI.Add("STANWORSHIPPER_SLOT4_ULTIMATE_NAME", "Summon Major Stan Fragment");
            LanguageAPI.Add("STANWORSHIPPER_SLOT4_ULTIMATE_DESCRIPTION", "Summons <style=cIsUtility>a powerful Stan fragment</style>. Uses a more powerful version of Poopslinger, dealing <style=cIsDamage>5x190% damage</style>.");

            LanguageAPI.Add("WEAKSTANFRAGMENT_NAME", "Lesser Stan Fragment");
            LanguageAPI.Add("STRONGSTANFRAGMENT_NAME", "Greater Stan Fragment");
        }
    }
}
