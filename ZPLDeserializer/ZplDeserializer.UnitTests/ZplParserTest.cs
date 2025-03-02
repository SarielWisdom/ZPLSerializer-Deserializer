using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ZPLDeserializer.library.Models;

namespace ZPLDeserializer.Tests
{
    [TestClass]
    public class ZPLParserTests
    {
        /// <summary>
        /// V�rifie que ParseAllCommands retourne une liste vide
        /// lorsque l'entr�e est null ou vide.
        /// </summary>
        [TestMethod]
        public void ParseAllCommands_EmptyOrNull_ReturnsEmptyList()
        {
            // Null
            var resultNull = ZPLParser.ParseAllCommands(null);
            Assert.IsNotNull(resultNull);
            Assert.AreEqual(0, resultNull.Count, "Expected empty list for null input.");

            // Empty
            var resultEmpty = ZPLParser.ParseAllCommands(string.Empty);
            Assert.IsNotNull(resultEmpty);
            Assert.AreEqual(0, resultEmpty.Count, "Expected empty list for empty string.");
        }

        /// <summary>
        /// V�rifie la d�tection d'une suite simple de commandes classiques,
        /// sans param�tres complexes.
        /// </summary>
        [TestMethod]
        public void ParseAllCommands_BasicCommands_ReturnsExpectedList()
        {
            // Arrange
            // On place 4 commandes: XA, FO, FS, XZ (toutes sans param�tre)
            string zpl = "^XA^FO^FS^XZ";

            // Act
            var commands = ZPLParser.ParseAllCommands(zpl);

            // Assert
            Assert.AreEqual(4, commands.Count, "Should find 4 commands.");
            Assert.AreEqual("XA", commands[0].CommandIdentifier);
            Assert.AreEqual("FO", commands[1].CommandIdentifier);
            Assert.AreEqual("FS", commands[2].CommandIdentifier);
            Assert.AreEqual("XZ", commands[3].CommandIdentifier);

            // V�rifie que les param�tres sont vides
            foreach (var cmd in commands)
            {
                Assert.IsNotNull(cmd.Parameters);
                Assert.AreEqual(0, cmd.Parameters.Count);
            }
        }

        /// <summary>
        /// V�rifie le comportement lorsqu'une commande "classique" poss�de plusieurs param�tres
        /// s�par�s par des virgules, p. ex. ^FO50,100,2.
        /// </summary>
        [TestMethod]
        public void ParseAllCommands_MultipleParams_SplitByComma()
        {
            // Arrange
            // On a ^FO, puis 3 param�tres
            string zpl = "^FO50,100,2^FS";

            // Act
            var commands = ZPLParser.ParseAllCommands(zpl);

            // Assert
            Assert.AreEqual(2, commands.Count, "Should have 2 commands: FO, FS");
            var foCommand = commands[0];
            Assert.AreEqual("FO", foCommand.CommandIdentifier);
            Assert.AreEqual(3, foCommand.Parameters.Count, "Expected 3 parameters for ^FO");
            Assert.AreEqual("50", foCommand.Parameters[0]);
            Assert.AreEqual("100", foCommand.Parameters[1]);
            Assert.AreEqual("2", foCommand.Parameters[2]);
        }

        /// <summary>
        /// V�rifie le cas d'une commande ^FD qui doit conserver son contenu entier,
        /// y compris s'il contient des virgules.
        /// </summary>
        [TestMethod]
        public void ParseAllCommands_FDCommandWithComma_NotSplit()
        {
            // Arrange
            // ^FD contient "Hello, world", qu'on ne veut pas scinder
            string zpl = "^XA^FDHello, world^FS^XZ";

            // Act
            var commands = ZPLParser.ParseAllCommands(zpl);

            // Assert
            // On attend 4 commandes : XA, FD, FS, XZ
            Assert.AreEqual(4, commands.Count);
            Assert.AreEqual("FD", commands[1].CommandIdentifier);
            Assert.AreEqual(1, commands[1].Parameters.Count);
            Assert.AreEqual("Hello, world", commands[1].Parameters[0]);
        }

        /// <summary>
        /// V�rifie le m�me comportement pour ^FX (commentaire)
        /// lorsqu'il contient des virgules.
        /// </summary>
        [TestMethod]
        public void ParseAllCommands_FXCommandWithComma_NotSplit()
        {
            // Arrange
            // ^FX contient un commentaire "Some, comment, text"
            string zpl = "^XA^FXSome, comment, text^FS^XZ";

            // Act
            var commands = ZPLParser.ParseAllCommands(zpl);

            // Assert
            // On attend 4 commandes : XA, FX, FS, XZ
            Assert.AreEqual(4, commands.Count);
            Assert.AreEqual("FX", commands[1].CommandIdentifier);
            Assert.AreEqual(1, commands[1].Parameters.Count);
            Assert.AreEqual("Some, comment, text", commands[1].Parameters[0]);
        }

        /// <summary>
        /// Test interne pour v�rifier la robustesse en cas de fin de cha�ne abrupte.
        /// On se retrouve avec un ^ qui n'a pas 2 lettres derri�re.
        /// </summary>
        [TestMethod]
        public void ParseAllCommands_IncompleteIdentifier_ShouldStopGracefully()
        {
            // Arrange
            // Ici, on a un '^' final qui ne poss�de pas les 2 lettres requises.
            string zpl = "^XA^FO50,50^F";

            // Act
            var commands = ZPLParser.ParseAllCommands(zpl);

            // Assert
            // ^XA et ^FO sont bien reconnus, ^F est ignor� car incomplet
            Assert.AreEqual(2, commands.Count);
            Assert.AreEqual("XA", commands[0].CommandIdentifier);
            Assert.AreEqual("FO", commands[1].CommandIdentifier);
        }

        /// <summary>
        /// (Optionnel) Test de la commande SplitParamSegmentByComma en isolation.
        /// Comme elle est priv�e, on pourrait la rendre "internal" 
        /// et d�clarer [InternalsVisibleTo("ZPLDeserialiser.Tests")] 
        /// si l'on souhaite r�ellement la tester. 
        /// 
        /// Ici, on montre un exemple si on la rendait publique. 
        /// </summary>
        // [TestMethod]
        // public void SplitParamSegmentByComma_ShouldSplitAndTrim()
        // {
        //     // Arrange
        //     string segment = " 50 ,100,  2";
        //
        //     // Act
        //     var result = ZPLParser.SplitParamSegmentByComma(segment);
        //
        //     // Assert
        //     Assert.AreEqual(3, result.Count);
        //     Assert.AreEqual("50", result[0]);
        //     Assert.AreEqual("100", result[1]);
        //     Assert.AreEqual("2", result[2]);
        // }
    }
}
