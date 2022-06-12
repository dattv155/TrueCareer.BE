using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using TrueCareer.BE.Models;
using TrueCareer.Enums;

namespace TrueCareer.Rpc
{
    public class SetupRoot
    {
        public const string InitEnum = "rpc/truecareer/setup/init-enum";
        public const string Init = "rpc/truecareer/setup/init";
    }
    public class SetupController : ControllerBase
    {
        private DataContext DataContext;
        public SetupController(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }
        [HttpGet, Route(SetupRoot.Init)]
        public ActionResult Init()
        {
            InitEnum();
            return Ok();
        }
        [HttpGet, Route(SetupRoot.InitEnum)]
        public ActionResult InitEnum()
        {
            InitRoleEnum();
            InitConnectionStatusEnum();
            InitConnectionTypeEnum();
            InitInformationTypeEnum();
            InitMbtiPersonalTypeEnum();
            InitMbtiSingleTypeEnum();
            InitNewsStatusEnum();
            InitSexEnum();
            InitMentorApprovalStatusEnum();
            InitGlobalUserTypeEnum();
            InitConversationAttachmentTypeEnum();
            InitUnitOfTimeEnum();
            return Ok();

        }
        public void InitConversationAttachmentTypeEnum()
        {
            List<ConversationAttachmentTypeDAO> ConversationAttachmentTypeEnumList = ConversationAttachmentTypeEnum.ConversationAttachmentTypeEnumList.Select(item => new ConversationAttachmentTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.ConversationAttachmentType.BulkSynchronize(ConversationAttachmentTypeEnumList);
        }
        public void InitGlobalUserTypeEnum()
        {
            List<GlobalUserTypeDAO> GlobalUserTypeDAOs = GlobalUserTypeEnum.GlobalUserTypeEnumList.Select(x => new GlobalUserTypeDAO
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            }).ToList();

            DataContext.GlobalUserType.BulkSynchronize(GlobalUserTypeDAOs);
        }
        public void InitRoleEnum()
        {
            List<RoleDAO> RoleDAOs = RoleEnum.RoleEnumList.Select(x => new RoleDAO
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            }).ToList();

            DataContext.Role.BulkSynchronize(RoleDAOs);
        }
        public void InitConnectionStatusEnum()
        {
            List<ConnectionStatusDAO> ConnectionStatuses = ConnectionStatusEnum.ConnectionStatusEnumList.Select(x => new ConnectionStatusDAO
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            }).ToList();

            DataContext.ConnectionStatus.BulkSynchronize(ConnectionStatuses);
        }
        public void InitConnectionTypeEnum()
        {
            List<ConnectionTypeDAO> ConnectionTypes = ConnectionTypeEnum.ConnectionTypeEnumList.Select(x => new ConnectionTypeDAO
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            }).ToList();

            DataContext.ConnectionType.BulkSynchronize(ConnectionTypes);
        }
        public void InitInformationTypeEnum()
        {
            List<InformationTypeDAO> InformationTypes = InformationTypeEnum.InformationTypeEnumList.Select(x => new InformationTypeDAO
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            }).ToList();

            DataContext.InformationType.BulkSynchronize(InformationTypes);
        }
        public void InitMbtiPersonalTypeEnum()
        {
            List<MbtiPersonalTypeDAO> MbtiPersonalTypes = MbtiPersonalTypeEnum.MbtiPersonalTypeEnumList.Select(x => new MbtiPersonalTypeDAO
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Value = x.Value
            }).ToList();

            DataContext.MbtiPersonalType.BulkSynchronize(MbtiPersonalTypes);
        }
        public void InitMbtiSingleTypeEnum()
        {
            List<MbtiSingleTypeDAO> MbtiSingleTypes = MbtiSingleTypeEnum.MbtiSingleTypeEnumList.Select(x => new MbtiSingleTypeDAO
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            }).ToList();

            DataContext.MbtiSingleType.BulkSynchronize(MbtiSingleTypes);
        }
        public void InitMentorApprovalStatusEnum()
        {
            List<MentorApprovalStatusDAO> MentorApprovalStatuses = MentorApprovalStatusEnum.MentorApprovalStatusEnumList.Select(x => new MentorApprovalStatusDAO
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            }).ToList();

            DataContext.MentorApprovalStatus.BulkSynchronize(MentorApprovalStatuses);
        }
        public void InitNewsStatusEnum()
        {
            List<NewsStatusDAO> NewsStatuses = NewsStatusEnum.NewsStatusEnumList.Select(x => new NewsStatusDAO
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            }).ToList();

            DataContext.NewsStatus.BulkSynchronize(NewsStatuses);
        }
        public void InitSexEnum()
        {
            List<SexDAO> Sexes = SexEnum.SexEnumList.Select(x => new SexDAO
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            }).ToList();

            DataContext.Sex.BulkSynchronize(Sexes);
        }
        public void InitUnitOfTimeEnum()
        {
            List<UnitOfTimeDAO> UnitOfTimes = UnitOfTimeEnum.UnitOfTimeEnumList.Select(x => new UnitOfTimeDAO
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            }).ToList();

            DataContext.UnitOfTime.BulkSynchronize(UnitOfTimes);
        }

    }
}
