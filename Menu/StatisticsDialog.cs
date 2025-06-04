using System;
using AllKills.Menu.StatisticsData;
using Menu;
using UnityEngine;

namespace AllKills.Menu
{
    /// <summary>
    ///     The dialog that will show the players statistics for a particular play-through.
    /// </summary>
    public class StatisticsDialog : Dialog
    {
        #region Properties

        /// <summary> The statistics menu where everything will be displayed. </summary>
        public readonly StatisticsMenu Menu;

        /// <summary> The button to close the dialog. </summary>
        public readonly SimpleButton CloseButton;

        #endregion

        #region Private Properties

        /// <summary> Action that will be called when this dialog is closed. </summary>
        private readonly Action _onClose;

        #endregion

        #region Auto-Properties

        public Options Options => manager.rainWorld.options;

        #endregion

        public StatisticsDialog(ProcessManager manager, Campaign campaign, Action onClose) : base(manager)
        {
            // Setup
            this.manager = manager;
            _onClose = onClose;

            // Background
            darkSprite.alpha = 0.9f;

            // Close Button
            CloseButton = new SimpleButton(
                this,
                pages[0],
                Translate("CLOSE"),
                "CLOSE",
                new Vector2(
                    manager.rainWorld.options.ScreenSize.x - 180f - manager.rainWorld.options.SafeScreenOffset.x,
                    Mathf.Max(manager.rainWorld.options.SafeScreenOffset.y, 15f)),
                new Vector2(110f, 30f));
            pages[0].subObjects.Add(CloseButton);

            // Add statistics menu
            if (campaign is null)
            {
                MenuLabel noDataLabel = new MenuLabel(
                    this,
                    pages[0],
                    "No Data",
                    new Vector2(manager.rainWorld.options.ScreenSize.x / 2, manager.rainWorld.options.ScreenSize.y / 2),
                    new Vector2(30f, 60f),
                    true);
                pages[0].subObjects.Add(noDataLabel);
            }
            else
            {
                Vector2 menuPos = new Vector2(0f, 0f);
                Menu = new StatisticsMenu(this, pages[0], menuPos, manager.rainWorld.options.ScreenSize, campaign);
                pages[0].subObjects.Add(Menu);
            }
        }

        /// <summary>
        ///     Handle emitted signals.
        /// </summary>
        /// <param name="sender">
        ///     The sender of the message.
        /// </param>
        /// <param name="message">
        ///     The signal message.
        /// </param>
        public override void Singal(MenuObject sender, string message)
        {
            // ReSharper disable once InvertIf
            if (message == "CLOSE")
            {
                PlaySound(SoundID.MENU_Switch_Page_Out);
                _onClose();
                manager.StopSideProcess(this);
            }
        }
    }
}