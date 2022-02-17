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
using TrueCareer.Services.MQuestion;
using TrueCareer.Services.MChoice;
using TrueCareer.Services.MMbtiSingleType;

namespace TrueCareer.Rpc.question
{
    public partial class QuestionController 
    {
        [Route(QuestionRoute.FilterListChoice), HttpPost]
        public async Task<List<Question_ChoiceDTO>> FilterListChoice([FromBody] Question_ChoiceFilterDTO Question_ChoiceFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ChoiceFilter ChoiceFilter = new ChoiceFilter();
            ChoiceFilter.Skip = 0;
            ChoiceFilter.Take = 20;
            ChoiceFilter.OrderBy = ChoiceOrder.Id;
            ChoiceFilter.OrderType = OrderType.ASC;
            ChoiceFilter.Selects = ChoiceSelect.ALL;
            ChoiceFilter.Id = Question_ChoiceFilterDTO.Id;
            ChoiceFilter.ChoiceContent = Question_ChoiceFilterDTO.ChoiceContent;
            ChoiceFilter.Description = Question_ChoiceFilterDTO.Description;
            ChoiceFilter.QuestionId = Question_ChoiceFilterDTO.QuestionId;
            ChoiceFilter.MbtiSingleTypeId = Question_ChoiceFilterDTO.MbtiSingleTypeId;

            List<Choice> Choices = await ChoiceService.List(ChoiceFilter);
            List<Question_ChoiceDTO> Question_ChoiceDTOs = Choices
                .Select(x => new Question_ChoiceDTO(x)).ToList();
            return Question_ChoiceDTOs;
        }
        [Route(QuestionRoute.FilterListMbtiSingleType), HttpPost]
        public async Task<List<Question_MbtiSingleTypeDTO>> FilterListMbtiSingleType([FromBody] Question_MbtiSingleTypeFilterDTO Question_MbtiSingleTypeFilterDTO)
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
            List<Question_MbtiSingleTypeDTO> Question_MbtiSingleTypeDTOs = MbtiSingleTypes
                .Select(x => new Question_MbtiSingleTypeDTO(x)).ToList();
            return Question_MbtiSingleTypeDTOs;
        }
    }
}

