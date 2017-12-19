using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSSFeedIn
{
    /// <summary>
    /// A helper library to make it easier to deal with RSS feeds.
    /// </summary>
    public static class RSSFeedLibrary
    {
        /// <summary>
        /// Converts a string into it's corresponding WindDirection.
        /// 
        /// For example, the string "West North Westerly" will be translated to a WindDirection.WestNorthWest
        /// </summary>
        /// <param name="direction">The string describing the direction</param>
        /// <exception cref="System.NotSupportedException">Thrown when the given `direction` is invalid.</exception>
        /// <returns>The `direction` translated into a WindDirection</returns>
        public static WindDirection DirectionFromString(string direction)
        {
            var finalDirection = direction.Replace(" ", string.Empty);
            finalDirection = finalDirection.Replace("erly", string.Empty);

            WindDirection result;
            var isValidDirection = Enum.TryParse<WindDirection>(finalDirection, out result);

            if (!isValidDirection)
                throw new NotSupportedException($"Unknown direction '{finalDirection}'");

            return result;
        }
    }
}
