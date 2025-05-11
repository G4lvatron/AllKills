using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllKills.Menu;
using Expedition;
using HUD;
using Menu;
using RWCustom;
using UnityEngine;
using Watcher;

namespace AllKills.Menu
{
    /// <summary>
    ///     The handler for the statistics menu on the sleep and death screen.
    /// </summary>
    public class SleepAndDeathScreenStatistics
    {
        #region Properties

        /// <summary>
        ///     The button to open the statistics.
        /// </summary>
        public SymbolButton StatisticsButton;

        /// <summary>
        ///     <c>true</c> if the dialog is open, <c>false</c> otherwise.
        /// </summary>
        public bool StatsDialogOpen;

        #endregion

        /// <summary>
        ///     Attach the summary menu functionality to the sleep and death screen.
        /// </summary>
        public void Attach()
        {
            On.Menu.SleepAndDeathScreen.AddSubObjects += Hook_AddSubObjects;
            On.Menu.SleepAndDeathScreen.Singal += Hook_Signal;
            On.Menu.SleepAndDeathScreen.UpdateInfoText += Hook_UpdateInfoText;
            On.Menu.SleepAndDeathScreen.Update += Hook_Update;
            On.RWInput.PlayerInput_int += Hook_PlayerInput;
        }

        #region Sleep Screen

        /// <summary>
        ///     Hook: Add the button to open the statistics to the sleep and death screen.
        /// </summary>
        /// <param name="orig">
        ///     The original method for adding sub-objects, this is called before anything else.
        /// </param>
        /// <param name="self">
        ///     The sleep and death screen.
        /// </param>
        private void Hook_AddSubObjects(On.Menu.SleepAndDeathScreen.orig_AddSubObjects orig,
            SleepAndDeathScreen self)
        {
            orig(self);

            StatisticsButton = new SymbolButton(
                self,
                self.pages[0],
                "Glyph_Info",
                "STATISTICS",
                new Vector2(
                    self.ContinueAndExitButtonsXPos - 94f - self.manager.rainWorld.options.SafeScreenOffset.x,
                    Mathf.Max(self.manager.rainWorld.options.SafeScreenOffset.y, 15f) + 50f));
            self.pages[0].subObjects.Add(StatisticsButton);
        }

        /// <summary>
        ///     Hook: Handle selecting the statistics button.
        /// </summary>
        /// <param name="orig">
        ///     The original signal method that is used for all message signals.
        /// </param>
        /// <param name="self">
        ///     The sleep and death screen.
        /// </param>
        /// <param name="sender">
        ///     The menu object that sent the message.
        /// </param>
        /// <param name="message">
        ///     The message. Here we are looking for the string '<c>STATISTICS</c>'.
        /// </param>
        private void Hook_Signal(On.Menu.SleepAndDeathScreen.orig_Singal orig,
            SleepAndDeathScreen self, MenuObject sender, string message)
        {
            orig(self, sender, message);

            // ReSharper disable once InvertIf
            if (message == "STATISTICS")
            {
                self.PlaySound(SoundID.MENU_Switch_Page_In);
                StatisticsDialog dialog = new StatisticsDialog(self.manager, () => StatsDialogOpen = false);
                self.manager.ShowDialog(dialog);
                StatsDialogOpen = true;
            }
        }

        /// <summary>
        ///     Hook: Adds general functionality:
        ///     Sets the statistics button to greyed out when the other buttons are.
        ///     Disables the map from the watcher campaign being open at the same time as the statistics.
        /// </summary>
        /// <param name="orig">
        ///     The original update method, used for most of the logic within the component.
        /// </param>
        /// <param name="self">
        ///     The sleep and death screen.
        /// </param>
        private void Hook_Update(On.Menu.SleepAndDeathScreen.orig_Update orig,
            SleepAndDeathScreen self)
        {
            orig(self);

            if (StatisticsButton != null)
                StatisticsButton.buttonBehav.greyedOut = self.ButtonsGreyedOut;

            if (StatsDialogOpen && self.mapToggle)
                self.mapToggle = false;
        }

        /// <summary>
        ///     Hook: Set the info text for when the statistics button is highlighted.
        /// </summary>
        /// <param name="orig">
        ///     The original method for getting info text.
        /// </param>
        /// <param name="self">
        ///     The sleep and death screen.
        /// </param>
        /// <returns>
        ///     The info text for statistics screen, if applicable. Otherwise, the result of the original method.s
        /// </returns>
        private static string Hook_UpdateInfoText(On.Menu.SleepAndDeathScreen.orig_UpdateInfoText orig,
            SleepAndDeathScreen self)
        {
            return self.selectedObject is SimpleButton selectedButton && selectedButton.signalText == "STATISTICS"
                ? "View the statistics for your play-through"
                : orig(self);
        }

        /// <summary>
        ///     Hook: If the stats dialog is open this will ensure that the map button cannot be pressed to open the map.
        ///     TODO: There will be a nicer way to fix this issue.
        /// </summary>
        /// <param name="orig">
        ///     The original method for getting player inputs.
        /// </param>
        /// <param name="player">
        ///     The player.
        /// </param>
        /// <returns>
        ///     The player's inputs. However, if the stats dialog is open, the map button state will always be returned as false.
        /// </returns>
        private Player.InputPackage Hook_PlayerInput(On.RWInput.orig_PlayerInput_int orig, int player)
        {
            Player.InputPackage inputs = orig(player);

            if (StatsDialogOpen)
            {
                inputs = new Player.InputPackage(
                    inputs.gamePad,
                    inputs.controllerType,
                    inputs.x,
                    inputs.y,
                    inputs.jmp,
                    inputs.thrw,
                    inputs.pckp,
                    false,
                    inputs.crouchToggle,
                    inputs.spec);
            }

            return inputs;
        }

        #endregion
    }
}