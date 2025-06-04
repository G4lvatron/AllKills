using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Menu;
using UnityEngine;

namespace AllKills.Util
{
    public static class ResourceHandling
    {
        /// <summary>
        ///     Attaches this feature to Rain World.
        /// </summary>
        public static void Attach()
        {
            On.RainWorld.LoadModResources += Hook_LoadModResources;
            On.RainWorld.UnloadResources += Hook_UnloadResources;
            On.CreatureSymbol.SpriteNameOfCreature += Hook_SpriteNameOfCreature;
            On.ItemSymbol.SpriteNameForItem += Hook_SpriteNameForItem;
            On.ItemSymbol.ColorForItem += Hook_ColourForItem;
        }

        /// <summary>
        ///     Hook: Load the texture atlas for the custom textures from this mod.
        /// </summary>
        /// <param name="orig">
        ///     The original method for loading resources.
        /// </param>
        /// <param name="self">
        ///     The rain world instance.
        /// </param>
        private static void Hook_LoadModResources(On.RainWorld.orig_LoadModResources orig, RainWorld self)
        {
            // ReSharper disable once StringLiteralTypo
            Futile.atlasManager.LoadAtlas("Atlases/iconsak");
            orig(self);
        }

        /// <summary>
        ///     Hook: Unload the texture atlas for the custom textures from this mod.
        /// </summary>
        /// <param name="orig">
        ///     The original method for unloading resources.
        /// </param>
        /// <param name="self">
        ///     The rain world instance.
        /// </param>
        private static void Hook_UnloadResources(On.RainWorld.orig_UnloadResources orig, RainWorld self)
        {
            // ReSharper disable once StringLiteralTypo
            Futile.atlasManager.UnloadAtlas("Atlases/iconsak");
            orig(self);
        }

        /// <summary>
        ///     Hook: Lets the game find the sprites for the newly added creature textures.
        /// </summary>
        /// <param name="orig">
        ///     The original method that finds the sprite name for a creature.
        /// </param>
        /// <param name="iconData">
        ///     The data of the creature whose sprite name should be returned.
        /// </param>
        /// <returns>
        ///     The sprite name of any newly added sprite when applicable, otherwise the original return value.
        /// </returns>
        private static string Hook_SpriteNameOfCreature(On.CreatureSymbol.orig_SpriteNameOfCreature orig,
            IconSymbol.IconSymbolData iconData)
        {
            if (iconData.critType == CreatureTemplate.Type.TempleGuard)
                return "Kill_Guard";

            if (iconData.critType == CreatureTemplate.Type.SmallCentipede)
                return "Kill_SmallCentipede";

            string result = orig(iconData);

            return result == "Futile_White" ? "Futile_White" : result;
        }

        /// <summary>
        ///     Hook: Lets the game find the sprites for the newly added item textures
        /// </summary>
        /// <param name="orig">
        ///     The original method that finds the sprite name for an item.
        /// </param>
        /// <param name="itemType">
        ///     The type of item to get the sprite for.
        /// </param>
        /// <param name="intData">
        ///     The additional optional data for the item.
        /// </param>
        /// <returns>
        ///     The sprite name of any newly added sprite when applicable, otherwise the original return value.
        /// </returns>
        private static string Hook_SpriteNameForItem(On.ItemSymbol.orig_SpriteNameForItem orig,
            AbstractPhysicalObject.AbstractObjectType itemType, int intData)
        {
            if (itemType == AbstractPhysicalObject.AbstractObjectType.KarmaFlower)
                return "Karma_Flower";

            string result = orig(itemType, intData);

            return result == "Futile_White" ? "Futile_White" : result;
        }

        /// <summary>
        ///     Hook: Lets the game find the colours for the newly added sprites
        /// </summary>
        /// <param name="orig">
        ///     The original method that finds the colour of an item.
        /// </param>
        /// <param name="itemType">
        ///     The type of item to get the colour of.
        /// </param>
        /// <param name="intData">
        ///     The additional optional data for the item.
        /// </param>
        /// <returns>
        ///     The sprite name of any newly added sprite when applicable, otherwise the original return value.
        /// </returns>
        private static Color Hook_ColourForItem(On.ItemSymbol.orig_ColorForItem orig,
            AbstractPhysicalObject.AbstractObjectType itemType, int intData)
        {
            if (itemType == AbstractPhysicalObject.AbstractObjectType.KarmaFlower)
                return new Color(252f / 255f, 177f / 255f, 3f / 255f);

            Color result = orig(itemType, intData);

            return result == global::Menu.Menu.MenuRGB(global::Menu.Menu.MenuColors.MediumGrey)
                ? global::Menu.Menu.MenuRGB(global::Menu.Menu.MenuColors.MediumGrey)
                : result;
        }
    }
}