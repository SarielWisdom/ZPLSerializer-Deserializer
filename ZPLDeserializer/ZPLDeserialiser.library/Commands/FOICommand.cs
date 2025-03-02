using ZPLDeserialiser.library.Models;

namespace ZPLDeserialiser.library
{
    /// <summary>
    /// ZPL command ^FO: defines the field position (X, Y) and alignment.
    /// </summary>
    public class FOCommand : ZPLCommand
    {
        /// <summary>
        /// X coordinate, in points. Value between 0 and 32,000. Default = 0.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Y coordinate, in points. Value between 0 and 32,000. Default = 0.
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Field alignment.
        /// Valid values: 0 (left alignment), 1 (right alignment), 2 (auto).
        /// Default = depends on global configuration ^FW (not handled here).
        /// </summary>
        public int? Alignment { get; }

        public FOCommand(IEnumerable<string> parameters)
            : base("FO", parameters)
        {
            // Convert parameters to a list for easy access to each one.
            var paramList = parameters.ToList();

            // X
            if (paramList.Count > 0 && int.TryParse(paramList[0], out var xVal))
            {
                X = xVal;
            }
            else
            {
                X = 0;  // Default value
            }

            // Y
            if (paramList.Count > 1 && int.TryParse(paramList[1], out var yVal))
            {
                Y = yVal;
            }
            else
            {
                Y = 0;  // Default value
            }

            // Alignment (optional)
            if (paramList.Count > 2 && int.TryParse(paramList[2], out var alignmentVal))
            {
                Alignment = alignmentVal;
            }
            else
            {
                // The default is governed by ^FW, here we simply use null
                Alignment = null;
            }
        }
    }
}
