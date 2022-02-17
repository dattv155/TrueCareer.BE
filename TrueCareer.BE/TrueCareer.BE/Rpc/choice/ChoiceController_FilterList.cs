using TrueSight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Common;
using TrueCareer.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using TrueCareer.Entities;
using TrueCareer.Services.MChoice;
using TrueCareer.Services.MMbtiSingleType;
using TrueCareer.Services.MQuestion;

namespace TrueCareer.Rpc.choice
{
    public partial class ChoiceController 
    {
        [Route(ChoiceRoute.FilterListMbtiSingleType), HttpPost]
        public async Task<List<Choice_MbtiSingleTypeDTO>> FilterListMbtiSingleType([FromBody] Choice_MbtiSingleTypeFilterDTO Choice_MbtiSingleTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MbtiSingleTypeFilter MbtiSingleTypeFilter = new MbtiSingleTypeFilter();
            MbtiSingleTypeFilter.Skip = 0;
            MbtiSingleTypeFilter.Take = int.MaxValue;
            MbtiSingleTypeFilter.Take = 20;
            MbtiSingleTypeFilter.OrderBy = MbtiSingleTypeOrder.Id;
            MbtiSingleTypeFilter.OrderType = OrderType.ASC;
            MbtiSingleTypeFilter.Selects = MbtiSingleTypeSelect.ALL;

            List<MbtiSingleType> MbtiSingleTypes = await MbtiSingleTypeService.List(MbtiSingleTypeFilter);
            List<Choice_MbtiSingleTypeDTO> Choice_MbtiSingleTypeDTOs = MbtiSingleTypes
                .Select(x => new Choice_MbtiSingleTypeDTO(x)).ToList();
            return Choice_MbtiSingleTypeDTOs;
        }
        [Route(ChoiceRoute.FilterListQuestion), HttpPost]
        public async Task<List<Choice_QuestionDTO>> FilterListQuestion([FromBody] Choice_QuestionFilterDTO Choice_QuestionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            QuestionFilter QuestionFilter = new QuestionFilter();
            QuestionFilter.Skip = 0;
            QuestionFilter.Take = 20;
            QuestionFilter.OrderBy = QuestionOrder.Id;
            QuestionFilter.OrderType = OrderType.ASC;
            QuestionFilter.Selects = QuestionSelect.ALL;
            QuestionFilter.Id = Choice_QuestionFilterDTO.Id;
            QuestionFilter.QuestionContent = Choice_QuestionFilterDTO.QuestionContent;
            QuestionFilter.Description = Choice_QuestionFilterDTO.Description;

            List<Question> Questions = await QuestionService.List(QuestionFilter);
            List<Choice_QuestionDTO> Choice_QuestionDTOs = Questions
                .Select(x => new Choice_QuestionDTO(x)).ToList();
            return Choice_QuestionDTOs;
        }
    }
}

