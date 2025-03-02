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
            if (string.IsNullOrEmpty(zpl))
                return commands;

            int currentIndex = 0;
            int length = zpl.Length;

            while (currentIndex < length)
            {
                // Recherche le prochain '^'
                int caretIndex = zpl.IndexOf('^', currentIndex);
                if (caretIndex < 0)
                {
                    // Plus de '^' trouvé, on arrête
                    break;
                }

                // Vérifie qu'il y a au moins 2 caractères d'identifiant
                if (caretIndex + 2 >= length)
                {
                    // Pas assez de place pour un identifiant complet
                    break;
                }

                // Lecture de l'identifiant EXACT de 2 caractères
                string identifier = zpl.Substring(caretIndex + 1, 2);

                // La zone des paramètres commence juste après les 2 lettres
                int paramStart = caretIndex + 3;

                // Cherche le prochain '^' ou la fin du ZPL
                int nextCaret = zpl.IndexOf('^', paramStart);
                if (nextCaret < 0)
                {
                    nextCaret = length;
                }

                // On extrait le segment supposé être les paramètres
                string paramSegment = zpl.Substring(paramStart, nextCaret - paramStart).Trim();

                // -- Voici la modification majeure --
                List<string> paramList;

                // Cas des virgules qui ne sont pas des séparateurs
                if (identifier.Equals("FD", StringComparison.OrdinalIgnoreCase)
                    || identifier.Equals("FX", StringComparison.OrdinalIgnoreCase))
                {
                    // Pour ^FD ou ^FX, on considère tout le segment comme un seul paramètre
                    paramList = new List<string> { paramSegment };
                }
                else
                {
                    // Pour toutes les autres commandes, on découpe par virgule
                    paramList = paramSegment
                        .Split(',')
                        .Select(s => s.Trim())
                        .Where(s => s.Length > 0)
                        .ToList();
                }

                // Instanciation de la commande
                var command = new ZPLCommand(identifier, paramList);
                commands.Add(command);

                // On reprend la boucle après nextCaret
                currentIndex = nextCaret;
            }

            return commands;
        }
    }
}
