using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QGo.Functions
{
    public static class MatchFunctions
    {
        public static int FindOverlapIndex(string match, string enteredText)
        {
            // Find where the entered text aligns with the match
            int index = match.IndexOf(enteredText, StringComparison.InvariantCultureIgnoreCase);
            if (index >= 0)
            {
                return index + enteredText.Length; // End of the overlap
            }

            // Fallback: If no clear overlap, place the cursor at the end of the match
            return match.Length;
        }
    }
}
