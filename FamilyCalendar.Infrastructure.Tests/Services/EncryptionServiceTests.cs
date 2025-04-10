using NUnit.Framework;
using System.Text;
using FamilyCalender.Infrastructure.Services;

namespace FamilyCalendar.Infrastructure.Tests.Services
{
    [TestFixture]
    public class EncryptionServiceTests
    {
        [Test]
        public void HardCodedKeyManagerTests()
        {
            var stringData = "Micke rocks in a world of code!";
            var byteData = Encoding.UTF8.GetBytes(stringData);
            var vector1 = "1"; // calendar id
            var vector2 = "2"; // different calendar id

            var encryptionManager = new EncryptionService(EncryptionService.Magic);
            
            byte[] encryption1 = encryptionManager.Encrypt(byteData, vector1);
            Assert.That(byteData, Is.EqualTo(encryptionManager.Decrypt(encryption1, vector1)));

            byte[] encryption2 = encryptionManager.Encrypt(byteData, vector2);
            Assert.That(byteData, Is.EqualTo(encryptionManager.Decrypt(encryption2, vector2)));

            Assert.That(encryption1, Is.Not.EqualTo(encryption2));

            byte[] encryption3 = encryptionManager.EncryptString(stringData, vector1);
            Assert.That(stringData, Is.EqualTo(encryptionManager.DecryptString(encryption3, vector1)));

            string encryption4 = encryptionManager.EncryptStringToString(stringData, vector2);
            Assert.That(stringData, Is.EqualTo(encryptionManager.DecryptStringToString(encryption4, vector2)));
            
        }
       

        [Test]
        public void AutoDetectTests()
        {
            var stringData = "Micke rocks in a world of code!";
            var vector1 = "1";

            var encryptionManager = new EncryptionService(EncryptionService.Magic);
            
            var encryption1 = encryptionManager.AutoDetectEncryptStringToString(stringData, vector1);
            Console.WriteLine(encryption1);

            Assert.That(stringData, Is.EqualTo(encryptionManager.AutoDetectDecryptStringToString(encryption1, vector1)));
            Assert.That(stringData, Is.EqualTo(encryptionManager.AutoDetectDecryptStringToString(stringData, vector1)));
        }


        [Test]
        public void AutoDetectEncryptedStringTests()
        {
            string stringData = "ɛͷʗ\u25cfA63alaeoiGK3byArgxP6u-IOitoE9OxJuijg9jtaNTs1";
            string vector1 = "1";

            var encryptionManager = new EncryptionService(EncryptionService.Magic);
        
            string encryption1 = encryptionManager.AutoDetectDecryptStringToString(stringData, vector1);
            Console.WriteLine(encryption1);
            
            Assert.That(encryption1, Is.EqualTo("Micke rocks in a world of code!"));
        }
    }
}
