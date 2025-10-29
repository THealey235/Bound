

namespace Bound.Models
{
    //make this an enum and make names Names instead of strings
    public struct Names
    {
        #region StateNames
        public readonly string Level0 = "levelzero";
        public readonly string Settings = "settings";
        public readonly string MainMenu = "mainmenu";
        public readonly string CharacterInit = "newgame";
        public readonly string Inventory = "inventory";
        public readonly string GameOptions = "gameoptions";
        public readonly string StatsWindow = "statswindow";
        public readonly string InventoryWindow = "invWindow";
        public readonly string ItemFinder = "itemFinder";
        #endregion

        #region Misc IDs
        public readonly string ID_PlayerName = "PlayerName";
        public readonly string ID_CurrentLevel = "Level";
        #endregion

        public Names()
        {
        }
    }
}
