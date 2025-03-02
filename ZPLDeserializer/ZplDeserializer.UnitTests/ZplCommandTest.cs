namespace ZplDeserializer.UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using ZPLDeserializer.library;

    namespace ZPLDeserialiser.Tests
    {
        [TestClass]
        public class ZPLCommandTests
        {
            /// <summary>
            /// Test pour vérifier qu'avec zéro paramètre, on obtient
            /// simplement "^FO" (ou n'importe quel identifiant).
            /// </summary>
            [TestMethod]
            public void ToZPLSegment_NoParameters_ReturnsCaretAndIdentifier()
            {
                // Arrange
                var cmd = new ZPLCommand("FO", new List<string>());

                // Act
                var result = cmd.ToZPLSegment();

                // Assert
                Assert.AreEqual("^FO", result);
            }

            /// <summary>
            /// Test pour vérifier qu'avec un seul paramètre, 
            /// il s'ajoute juste après l'identifiant sans virgule supplémentaire.
            /// </summary>
            [TestMethod]
            public void ToZPLSegment_OneParameter_ReturnsCaretIdentifierAndParam()
            {
                // Arrange
                var cmd = new ZPLCommand("FD", new List<string> { "Hello" });

                // Act
                var result = cmd.ToZPLSegment();

                // Assert
                // On attend "^FDHello"
                Assert.AreEqual("^FDHello", result);
            }

            /// <summary>
            /// Test pour vérifier le cas de plusieurs paramètres, 
            /// censés être joints par des virgules.
            /// </summary>
            [TestMethod]
            public void ToZPLSegment_MultipleParameters_ReturnsJoinedByComma()
            {
                // Arrange
                var cmd = new ZPLCommand("FO", new List<string> { "100", "200", "2" });

                // Act
                var result = cmd.ToZPLSegment();

                // Assert
                // On attend "^FO100,200,2"
                Assert.AreEqual("^FO100,200,2", result);
            }

            /// <summary>
            /// (Optionnel) Test pour vérifier qu'un paramètre comportant déjà une virgule
            /// est restitué tel quel dans la chaîne. 
            /// </summary>
            [TestMethod]
            public void ToZPLSegment_ParameterWithComma_StaysIntact()
            {
                // Arrange
                var cmd = new ZPLCommand("FD", new List<string> { "Hello, world" });

                // Act
                var result = cmd.ToZPLSegment();

                // Assert
                // On attend "^FDHello, world"
                Assert.AreEqual("^FDHello, world", result);
            }
        }
    }


}
