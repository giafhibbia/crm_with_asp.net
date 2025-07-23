using System.Text.RegularExpressions;

namespace MyAuthDemo.Helpers
{
    public static class AssetCodeHelper
    {
        /// <summary>
        /// Generate the next unique asset code, given a category code/prefix and all existing codes for that category.
        /// </summary>
        /// <param name="categoryCode">E.g. "GS500AF"</param>
        /// <param name="existingCodes">All asset codes for that category, e.g. ["GS500AF000001", "GS500AF000002"]</param>
        /// <returns>Next asset code, e.g. "GS500AF000003"</returns>
        public static string GenerateNextAssetCode(string categoryCode, IEnumerable<string> existingCodes)
        {
            // Pattern: GS500AF000001 (prefix + 7 digit number)
            var pattern = $@"^{Regex.Escape(categoryCode)}(\d{{7}})$";
            var maxNumber = 0;

            foreach (var code in existingCodes)
            {
                var match = Regex.Match(code, pattern);
                if (match.Success)
                {
                    var numberPart = match.Groups[1].Value;
                    if (int.TryParse(numberPart, out var num) && num > maxNumber)
                        maxNumber = num;
                }
            }
            var nextNumber = maxNumber + 1;
            return $"{categoryCode}{nextNumber.ToString("D7")}";
        }
    }
}
