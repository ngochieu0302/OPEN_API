using System;
using System.Collections.Generic;
using System.Text;

namespace ESCS.COMMON.ExtensionMethods
{
    public static class MenuExtension
    {
        public static string ActiveParent(string id_menu_cha, string current_menu)
        {
            if (!string.IsNullOrEmpty(current_menu) && id_menu_cha.ToLower() == current_menu.ToLower())
            {
                return "active open";
            }
            return "";
        }
        public static string ActiveChildren(string id_menu, string current_menu)
        {
            if (!string.IsNullOrEmpty(current_menu) && id_menu.ToLower() == current_menu.ToLower())
            {
                return "active";
            }
            return "";
        }
    }
}
