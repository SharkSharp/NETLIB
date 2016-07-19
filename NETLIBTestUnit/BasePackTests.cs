using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NETLIB.Tests
{
    [TestClass]
    public class BasePackTests
    {
        [TestMethod]
        public void WriteTestAreBuffersEqual()
        {
            // Arrange
            BasePack basePack = new BasePack();
            byte[] buff = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            // Act
            basePack.Write(buff, 0, buff.Length);

            // Assert - note that BasePack's buffer[0] is Pack's header, so written buffer is shifted by 1 to the right
            byte[] buffWritten = basePack.Buffer;
            for (int i = 0; i < buff.Length; i++)
                Assert.AreEqual(buff[i], buffWritten[i + 1]);
        }

        [TestMethod]
        public void ReadTestAreBuffersEqual()
        {
            // Arrange
            BasePack basePack = new BasePack();
            byte[] buff = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            basePack.Write(buff, 0, buff.Length);

            // Act
            byte[] buffRead = new byte[buff.Length];
            basePack.Read(buffRead, 0, buffRead.Length);

            // Assert
            CollectionAssert.AreEqual(buff, buffRead);
        }

        [TestMethod]
        public void PutStringTestAndGetStringTest()
        {
            // Arrange
            string stringPut = "Test string.";
            BasePack basePack = new BasePack();

            // Act
            basePack.PutString(stringPut);
            string stringGet = basePack.GetString();

            // Assert
            Assert.AreEqual(stringPut, stringGet);
        }

        [TestMethod]
        public void PutIntAtTheBeginningAndGetIntFromBeginningTest()
        {
            // Arrange
            int intPut = 123;
            BasePack basePack = new BasePack();

            // Act
            basePack.PutInt(intPut);
            int intGet = basePack.GetInt();

            // Assert
            Assert.AreEqual(intPut, intGet);
        }

        [TestMethod]
        public void PutDoubleAtTheBeginningAndGetDoubleFromTheBeginningTest()
        {
            // Arrange
            double doublePut = 123.4;
            BasePack basePack = new BasePack();

            // Act
            basePack.PutDouble(doublePut);
            double doubleGet = basePack.GetDouble();

            // Assert
            Assert.AreEqual(doublePut, doubleGet);
        }

        [TestMethod]
        public void PutFloatAtTheBeginningAndGetFloatFromTheBeginningTest()
        {
            // Arrange
            float floatPut = 123.4f;
            BasePack basePack = new BasePack();

            // Act
            basePack.PutFloat(floatPut);
            float floatGet = basePack.GetFloat();

            // Assert
            Assert.AreEqual(floatPut, floatGet);
        }

        [TestMethod]
        public void PutCharAtTheBeginningAndGetCharFromTheBeginningTest()
        {
            // Arrange
            char charPut = 'a';
            BasePack basePack = new BasePack();

            // Act
            basePack.PutChar(charPut);
            char charGet = basePack.GetChar();

            // Assert
            Assert.AreEqual(charPut, charGet);
        }

        [TestMethod]
        public void PutByteAtTheBeginningAndGetByteFromTheBeginningTest()
        {
            // Arrange
            byte bytePut = 7;
            BasePack basePack = new BasePack();

            // Act
            basePack.PutByte(bytePut);
            byte byteGet = basePack.GetByte();

            // Assert
            Assert.AreEqual(bytePut, byteGet);
        }

        [TestMethod]
        public void PutBoolAtTheBeginningAndGetBoolFromTheBeginningTest()
        {
            // Arrange
            bool boolPut = true;
            BasePack basePack = new BasePack();

            // Act
            basePack.PutBool(boolPut);
            bool boolGet = basePack.GetBool();

            // Assert
            Assert.AreEqual(boolPut, boolGet);
        }

        [TestMethod]
        public void ConversionFromByteArrayToBasePackTest()
        {
            // Arrange
            BasePack basePack = new BasePack();
            byte[] byteArray = new byte[BasePack.packSize];
            for (int i = 0; i < byteArray.Length; i++)
                byteArray[i] = (byte)(i % 256);

            // Act
            basePack = byteArray;

            // Assert
            for (int i = 0; i < byteArray.Length; i++)
                Assert.AreEqual(byteArray[i], basePack.Buffer[i]);
        }
    }
}