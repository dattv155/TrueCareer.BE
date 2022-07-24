using NUnit.Framework;
using System.Threading.Tasks;
using TrueCareer.Entities;
using TrueCareer.Enums;
using TrueCareer.Services.MInformation;
using System.Linq;
using Microsoft.EntityFrameworkCore;
namespace TrueCareer.Tests.MInformation
{
    class UserInformationTest:BaseTests
    {
        private InformationService InformationService ;
        [SetUp]
         public void Setup()
        {
            Init();
            IInformationValidator InformationValidator = new InformationValidator(UOW, CurrentContext);
            InformationService = new InformationService(UOW, CurrentContext, null, InformationValidator, Logging);
        }   
        [Test]
        public async Task UserInformation_Create_InsertToDB()
        {
            Information Input = new Information
            {
                Id = 1,
                Name = "Công ty FPT",
                Role = "Web Developer",
                Description = "Phát triển thành công hệ thống quản lý phân phối DMS",
                InformationTypeId = InformationTypeEnum.EXPERIENCE.Id,
                Image = "https://picsum.photos/200/300"
            };
            await InformationService.Create(Input);

            var Output = await DataContext.Information.Where(x => x.Id == Input.Id).FirstOrDefaultAsync();

            Assert.AreEqual(Input.Id, Output.Id);
            Assert.AreEqual(Input.Name, Output.Name);
            Assert.AreEqual(Input.Role, Output.Name);
            Assert.AreEqual(Input.Description, Output.Description);
            Assert.AreEqual(Input.InformationTypeId, Output.InformationTypeId);
            Assert.AreEqual(Input.Image, Output.Image);
        }
    }
}
