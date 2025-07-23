// Helpers/EnumHelper.cs
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyAuthDemo.Helpers
{
    public static class EnumHelper
    {
        /// <summary>
        /// Convert any enum to a list of SelectListItem for dropdowns, with friendly names for MachineStatusType.
        /// </summary>
        public static List<SelectListItem> ToSelectList<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = GetFriendlyName(e)
                })
                .ToList();
        }

        public static string GetFriendlyName<T>(T value) where T : Enum
        {
            // Customize for MachineStatusType and MachineCondition, extend as needed
            string name = value.ToString();
            return name switch
            {
                "StockReady" => "Ready/Stock",
                "InUseByCustomer" => "Used by Customer",
                "InUseForOfficeTest" => "Used for Office Test",
                "UnderRepairAtCustomer" => "Under Repair at Customer",
                "UnderRepairAtOffice" => "Under Repair at Office",
                "DamagedUnusable" => "Damaged/Unusable",
                "New" => "New",
                "Used" => "Used",
                _ => SplitCamelCase(name)
            };
        }

        private static string SplitCamelCase(string input)
        {
            // Optional: turns "MyEnumValue" into "My Enum Value"
            return System.Text.RegularExpressions.Regex.Replace(
                input,
                "(\\B[A-Z])", " $1"
            );
        }
    }
}
