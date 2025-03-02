namespace ZPLDeserializer.library
{
    /// <summary>
    /// Generic class representing any zpl command, 
    /// for example : ^FO, ^FD, etc.
    /// </summary>
    public class ZPLCommand
    {
        /// <summary>
        /// ex: "FO", "FD", "FS", "GB", "BC", "XA", "XZ", ...
        /// </summary>
        public string CommandIdentifier { get; }

        /// <summary>
        /// Raw parameters, separated by comma (e.g. [ "50", "75", "0" ]).
        /// </summary>
        public List<string> Parameters { get; }

        /// <summary>
        /// Standard constructor.
        /// </summary>
        public ZPLCommand(string commandIdentifier, IEnumerable<string> parameters)
        {
            CommandIdentifier = commandIdentifier;
            Parameters = parameters.ToList();
        }

        /// <summary>
        /// Convert the command to a ZPL segment.
        /// </summary>
        public string ToZPLSegment()
        {
            // If no parameters, we just return the command identifier
            if (!Parameters.Any())
            {
                return $"^{CommandIdentifier}";
            }

            // Else, we return the command identifier followed by the parameters
            return $"^{CommandIdentifier}{string.Join(",", Parameters)}";
        }
    }
}
