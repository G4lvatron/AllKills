namespace AllKills
{
    /// <summary>
    ///     Utility class for sorting creatures on the end screen.
    /// </summary>
    internal class CreatureSort
    {
        /// <summary>
        ///     Get the ordering for the creatures.
        /// </summary>
        /// 
        /// <param name="creature"> The creature type. </param>
        /// 
        /// <returns>
        ///     A number representing the order.
        /// </returns>
        public static int GetCreatureValue(IconSymbol.IconSymbolData creature)
        {
            CreatureTemplate.Type type = creature.critType;

            // It's not a pattern matching expression, but it'll have to do...
            if (type == CreatureTemplate.Type.Slugcat) return 1;
            if (type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC) return 2;
            if (type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.HunterDaddy) return 3;
            if (type == CreatureTemplate.Type.GreenLizard) return 4;
            if (type == CreatureTemplate.Type.PinkLizard) return 5;
            if (type == CreatureTemplate.Type.BlueLizard) return 6;
            if (type == CreatureTemplate.Type.WhiteLizard) return 7;
            if (type == CreatureTemplate.Type.BlackLizard) return 8;
            if (type == CreatureTemplate.Type.YellowLizard) return 9;
            if (type == DLCSharedEnums.CreatureTemplateType.SpitLizard) return 10;
            if (type == DLCSharedEnums.CreatureTemplateType.ZoopLizard) return 11;
            if (type == CreatureTemplate.Type.CyanLizard) return 12;
            if (type == CreatureTemplate.Type.RedLizard) return 13;
            if (type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.TrainLizard) return 14;
            if (type == CreatureTemplate.Type.Salamander) return 15;
            if (type == DLCSharedEnums.CreatureTemplateType.EelLizard) return 16;
            if (type == CreatureTemplate.Type.Fly) return 17;
            if (type == CreatureTemplate.Type.CicadaA) return 18;
            if (type == CreatureTemplate.Type.CicadaB) return 19;
            if (type == CreatureTemplate.Type.Snail) return 20;
            if (type == CreatureTemplate.Type.Leech) return 21;
            if (type == CreatureTemplate.Type.SeaLeech) return 22;
            if (type == DLCSharedEnums.CreatureTemplateType.JungleLeech) return 23;
            if (type == CreatureTemplate.Type.PoleMimic) return 24;
            if (type == CreatureTemplate.Type.TentaclePlant) return 25;
            if (type == CreatureTemplate.Type.Scavenger) return 26;
            if (type == DLCSharedEnums.CreatureTemplateType.ScavengerElite) return 27;
            if (type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing) return 28;
            if (type == CreatureTemplate.Type.VultureGrub) return 29;
            if (type == CreatureTemplate.Type.Vulture) return 30;
            if (type == CreatureTemplate.Type.KingVulture) return 31;
            if (type == CreatureTemplate.Type.SmallCentipede) return 32;
            if (type == CreatureTemplate.Type.Centipede)
                return 33 + RWCustom.Custom.IntClamp(creature.intData, 1, 3) - 1;
            if (type == CreatureTemplate.Type.RedCentipede) return 36;
            if (type == CreatureTemplate.Type.Centiwing) return 37;
            if (type == DLCSharedEnums.CreatureTemplateType.AquaCenti) return 38;
            if (type == CreatureTemplate.Type.TubeWorm) return 39;
            if (type == CreatureTemplate.Type.Hazer) return 40;
            if (type == CreatureTemplate.Type.LanternMouse) return 41;
            if (type == CreatureTemplate.Type.Spider) return 42;
            if (type == CreatureTemplate.Type.BigSpider) return 43;
            if (type == DLCSharedEnums.CreatureTemplateType.MotherSpider) return 44;
            if (type == CreatureTemplate.Type.SpitterSpider) return 45;
            if (type == CreatureTemplate.Type.MirosBird) return 46;
            if (type == DLCSharedEnums.CreatureTemplateType.MirosVulture) return 47;
            if (type == CreatureTemplate.Type.BrotherLongLegs) return 48;
            if (type == CreatureTemplate.Type.DaddyLongLegs) return 49;
            if (type == DLCSharedEnums.CreatureTemplateType.TerrorLongLegs) return 50;
            if (type == DLCSharedEnums.CreatureTemplateType.Inspector) return 51;
            if (type == CreatureTemplate.Type.Deer) return 52;
            if (type == CreatureTemplate.Type.EggBug) return 53;
            if (type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.FireBug) return 54;
            if (type == CreatureTemplate.Type.DropBug) return 55;
            if (type == DLCSharedEnums.CreatureTemplateType.StowawayBug) return 56;
            if (type == CreatureTemplate.Type.SmallNeedleWorm) return 57;
            if (type == CreatureTemplate.Type.BigNeedleWorm) return 58;
            if (type == CreatureTemplate.Type.JetFish) return 59;
            if (type == DLCSharedEnums.CreatureTemplateType.Yeek) return 60;
            if (type == CreatureTemplate.Type.BigEel) return 61;
            if (type == DLCSharedEnums.CreatureTemplateType.BigJelly) return 62;
            if (type == CreatureTemplate.Type.GarbageWorm) return 63;
            if (type == CreatureTemplate.Type.Overseer) return 64;
            return type == CreatureTemplate.Type.TempleGuard ? 65 : int.MaxValue;
        }
    }
}