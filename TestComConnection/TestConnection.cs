using ComProject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WpfApp1.Json;

namespace TestComConnection
{
    [TestClass]
    public class TestConnection
    {
        [TestMethod]
        public void Bytelength()
        {
            // ---- Arrange
            var com = new ClassComConnection();
            com.SetSerialPort("5458");

            // ---- Act
            com.SendData(opCode.P10, 101, 1);
            var Result =  com.GetInffo;

            // --- Assert
            Assert.AreEqual(20, Result.Count);

        }

        [TestMethod]
        public void CheckData()
        {
            // ---- Arrange
            var com2 = new ClassComConnection();
            com2.SetSerialPort("5458");

            // ---- Act
            com2.SendData(opCode.P10, 36, 1);
            SerializeJson SendData = new SerializeJson(com2, 36, opCode.P10.ToString(),false);
            var Result = SendData.Data;

            // --- Assert
            Assert.IsNotNull(Result);

        }

        private string Hash;
        [TestMethod]
        public void Fixedvalues()
        {
            // ---- Arrange
            var com3 = new ClassComConnection();
            com3.SetSerialPort("5458");

            // ---- Act
            com3.SendData(opCode.P10, 36, 1);
            SerializeJson SendData = new SerializeJson(com3, 50, opCode.P10.ToString(), false);
            foreach (var item in com3.GetInffo)
            {
                Hash += " " + item.ToString();
            }
            
            // --- Assert
            Assert.AreEqual(SendData.Data.HashCode, Hash);

        }

    }
}
