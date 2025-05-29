using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Menu;
using UnityEngine;

namespace AllKills.Menu
{
    /// <summary>
    ///     The handler for the statistics menu on the slugcat sleep screen.
    /// </summary>
    public class SlugcatSelectMenuStatistics
    {
        #region Properties

        /// <summary>
        ///     The button to open the statistics.
        /// </summary>
        public SymbolButton StatisticsButton;

        #endregion

        /// <summary>
        ///     Attach the summary menu functionality to the slugcat select screen.
        /// </summary>
        public void Attach()
        {
            On.Menu.SlugcatSelectMenu.ctor += Hook_SlugcatSelectMenu;
            On.Menu.SlugcatSelectMenu.Singal += Hook_Signal;
        }

        /// <summary>
        ///     Hook: Add the button to open the statistics to the slugcat select screen.
        /// </summary>
        /// <param name="orig">
        ///     The original method for adding sub-objects, this is called before anything else.
        /// </param>
        /// <param name="self">
        ///     The slugcat select screen.
        /// </param>
        /// <param name="manager">
        ///     The process manager, this is simply passed to the original constructor.
        /// </param>
        public void Hook_SlugcatSelectMenu(
            On.Menu.SlugcatSelectMenu.orig_ctor orig,
            SlugcatSelectMenu self,
            ProcessManager manager)
        {
            orig(self, manager);

            float restartTextOffset = SlugcatSelectMenu.GetRestartTextOffset(self.CurrLang);
            StatisticsButton = new SymbolButton(
                self,
                self.pages[0],
                "Glyph_Info",
                "STATISTICS",
                new Vector2(self.startButton.pos.x + 200f + restartTextOffset, 90f));
            self.pages[0].subObjects.Add(StatisticsButton);
        }

        /// <summary>
        ///     Hook: Handle selecting the statistics button.
        /// </summary>
        /// <param name="orig">
        ///     The original signal method that is used for all message signals.
        /// </param>
        /// <param name="self">
        ///     The slugcat select screen.
        /// </param>
        /// <param name="sender">
        ///     The menu object that sent the message.
        /// </param>
        /// <param name="message">
        ///     The message. Here we are looking for the string '<c>STATISTICS</c>'.
        /// </param>
        public void Hook_Signal(
            On.Menu.SlugcatSelectMenu.orig_Singal orig,
            SlugcatSelectMenu self,
            MenuObject sender,
            string message)
        {
            orig(self, sender, message);

            // ReSharper disable once InvertIf
            if (message == "STATISTICS")
            {
                self.PlaySound(SoundID.MENU_Switch_Page_In);
                StatisticsDialog dialog = new StatisticsDialog(self.manager, () => { });
                self.manager.ShowDialog(dialog);
            }
        }
    }
}