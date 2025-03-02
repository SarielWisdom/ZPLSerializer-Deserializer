namespace ZPLDeserializer.library.Models
{
    /// <summary>
    /// Fournit une méthode pour parser un ZPL en supposant que toutes 
    /// les commandes ont un identifiant EXACT de deux lettres (ex: ^FO, ^FD, ^CF...).
    /// </summary>
    public static class ZPLParser
    {
        /// <summary>
        /// Analyse la chaîne ZPL et renvoie une liste ordonnée de commandes (ZPLCommand).
        /// Les identifiants ^FD et ^FX ne seront pas subdivisés par virgule.
        /// </summary>
        /// <param name="zpl">
        /// Chaîne ZPL contenant des commandes à deux lettres (^FO, ^FD, ^FS, ^GB, ^CF, ^XA, ^XZ...).
        /// </param>
        /// <returns>Une liste de ZPLCommand dans l'ordre d'apparition.</returns>
        /// <remarks>
        /// Exemple : "^FO50,50^FDHello^FS^XZ" 
        /// renverra 4 commandes : FO, FD, FS, XZ.
        /// </remarks>
        public static List<ZPLCommand> ParseAllCommands(string zpl)
        {
            // Sécurité : on gère le cas d'une chaîne nulle ou vide
            if (string.IsNullOrEmpty(zpl))
            {
                return new List<ZPLCommand>();
            }

            var commands = new List<ZPLCommand>();

            int currentIndex = 0;
            int totalLength = zpl.Length;

            while (currentIndex < totalLength)
            {
                // Repère un caret '^'
                if (zpl[currentIndex] == '^')
                {
                    // Vérifie qu'il reste de la place pour lire 2 lettres
                    if (currentIndex + 2 < totalLength)
                    {
                        // Lecture de l'identifiant sur 2 caractères
                        string identifier = zpl.Substring(currentIndex + 1, 2);
                        currentIndex += 3; // On avance après '^XX'

                        // Recherche la portion de texte jusqu'au prochain '^' ou la fin
                        int segmentStart = currentIndex;
                        while (currentIndex < totalLength && zpl[currentIndex] != '^')
                        {
                            currentIndex++;
                        }

                        // On récupère la portion des "paramètres" bruts
                        int segmentLength = currentIndex - segmentStart;
                        string paramSegment = zpl
                            .Substring(segmentStart, segmentLength)
                            .Trim();

                        // Selon la commande, on parse différemment
                        List<string> paramList;
                        if (IsFullTextCommand(identifier))
                        {
                            // ^FD ou ^FX => on ne découpe pas par virgule
                            paramList = new List<string> { paramSegment };
                        }
                        else
                        {
                            // Toutes les autres commandes => découpe par virgule
                            paramList = SplitParamSegmentByComma(paramSegment);
                        }

                        // On crée la commande et on l'ajoute à la liste
                        commands.Add(new ZPLCommand(identifier, paramList));
                    }
                    else
                    {
                        // Il ne reste pas assez de caractères pour un identifiant 2 lettres
                        break;
                    }
                }
                else
                {
                    // Si le caractère n'est pas '^', on continue d'avancer
                    currentIndex++;
                }
            }

            return commands;
        }

        /// <summary>
        /// Indique si l'identifiant correspond à une commande qui doit conserver 
        /// l'intégralité de la chaîne (notamment ^FD, ^FX) sans découpage.
        /// </summary>
        private static bool IsFullTextCommand(string identifier)
        {
            // On autorise l'utilisation insensible à la casse
            return identifier.Equals("FD", StringComparison.OrdinalIgnoreCase)
                || identifier.Equals("FX", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Découpe une chaîne en segments, en séparant par virgule, 
        /// puis en supprimant les espaces autour de chaque segment.
        /// </summary>
        private static List<string> SplitParamSegmentByComma(string paramSegment)
        {
            var result = new List<string>();
            int start = 0;

            for (int i = 0; i < paramSegment.Length; i++)
            {
                if (paramSegment[i] == ',')
                {
                    if (i > start)
                    {
                        string trimmedPart = paramSegment.Substring(start, i - start).Trim();
                        result.Add(trimmedPart);
                    }
                    start = i + 1;
                }
            }

            // Ajout du dernier paramètre s'il existe
            if (start < paramSegment.Length)
            {
                string lastPart = paramSegment.Substring(start).Trim();
                result.Add(lastPart);
            }

            return result;
        }
    }
}
