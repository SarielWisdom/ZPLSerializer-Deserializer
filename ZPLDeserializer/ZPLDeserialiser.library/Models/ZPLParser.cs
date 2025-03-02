namespace ZPLDeserialiser.library.Models
{
    public static class ZPLParser
    {
        /// <summary>
        /// Parse a ZPL assuming that ALL commands have an EXACT 2-letter identifier.
        /// Examples: ^FO, ^FD, ^FS, ^GB, ^CF, ^XA, ^XZ, ...
        /// </summary>
        public static List<ZPLCommand> ParseAllCommands(string zpl)
        {
            var commands = new List<ZPLCommand>();
            int i = 0;
            int length = zpl.Length;

            while (i < length)
            {
                if (zpl[i] == '^')
                {
                    // S'assurer qu'on peut lire 2 lettres
                    if (i + 2 < length)
                    {
                        // Lire identifiant
                        string identifier = zpl.Substring(i + 1, 2);
                        i += 3; // Skip '^XX'

                        // Collecter paramSegment jusqu'au prochain '^'
                        int paramStart = i;
                        while (i < length && zpl[i] != '^')
                            i++;

                        // i pointe sur '^' ou length
                        int segLength = i - paramStart;
                        string paramSegment = zpl.Substring(paramStart, segLength).Trim();

                        List<string> paramList;
                        if (identifier.Equals("FD", StringComparison.OrdinalIgnoreCase)
                            || identifier.Equals("FX", StringComparison.OrdinalIgnoreCase))
                        {
                            // Conserver tout le paramSegment en un seul bloc
                            paramList = new List<string> { paramSegment };
                        }
                        else
                        {
                            // Découper manuellement par virgule
                            paramList = new List<string>();
                            int start = 0;
                            for (int k = 0; k < paramSegment.Length; k++)
                            {
                                if (paramSegment[k] == ',')
                                {
                                    if (k > start)
                                    {
                                        paramList.Add(paramSegment.Substring(start, k - start).Trim());
                                    }
                                    start = k + 1;
                                }
                            }
                            // Dernier param
                            if (start < paramSegment.Length)
                            {
                                paramList.Add(paramSegment.Substring(start).Trim());
                            }
                        }

                        commands.Add(new ZPLCommand(identifier, paramList));
                    }
                    else
                    {
                        // Fin de chaîne, pas de place pour identifiant
                        break;
                    }
                }
                else
                {
                    i++;
                }
            }

            return commands;
        }
    }
}
