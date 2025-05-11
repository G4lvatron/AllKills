using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL.Menu.Remix.MixedUI;
using Menu;
using UnityEngine;
using OpScrollBox = Menu.Remix.MixedUI.OpScrollBox;
using UiElement = Menu.Remix.MixedUI.UIelement;

namespace AllKills.Menu
{
    public class StatisticsMenu : PositionedMenuObject
    {
        public StatisticsMenu(StatisticsDialog menu, MenuObject owner, Vector2 pos) : base(menu, owner, pos)
        {
            // Setup
            this.menu = menu;

            AddScrollBox();
        }

        private void AddScrollBox()
        {
        }
    }
}