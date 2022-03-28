using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Entities;

namespace TrueCareer.Repositories
{
    public interface IMentorRegisterRequestRepository
    {
        Task<int> CountAll(MentorRegisterRequestFilter MentorRegisterRequestFilter);
        Task<int> Count(MentorRegisterRequestFilter MentorRegisterRequestFilter);
        Task<List<MentorRegisterRequest>> List(MentorRegisterRequestFilter MentorRegisterRequestFilter);
        Task<List<MentorRegisterRequest>> List(List<long> Ids);
        Task<MentorRegisterRequest> Get(long Id);
        Task<bool> Create(MentorRegisterRequest MentorRegisterRequest);
        Task<bool> Update(MentorRegisterRequest MentorRegisterRequest);
        Task<bool> Delete(MentorRegisterRequest MentorRegisterRequest);
        Task<bool> BulkMerge(List<MentorRegisterRequest> MentorRegisterRequests);
        Task<bool> BulkDelete(List<MentorRegisterRequest> MentorRegisterRequests);
    }
    public class MentorRegisterRequestRepository : IMentorRegisterRequestRepository
    {
        public Task<bool> BulkDelete(List<MentorRegisterRequest> MentorRegisterRequests)
        {
            throw new NotImplementedException();
        }

        public Task<bool> BulkMerge(List<MentorRegisterRequest> MentorRegisterRequests)
        {
            throw new NotImplementedException();
        }

        public Task<int> Count(MentorRegisterRequestFilter MentorRegisterRequestFilter)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAll(MentorRegisterRequestFilter MentorRegisterRequestFilter)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Create(MentorRegisterRequest MentorRegisterRequest)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(MentorRegisterRequest MentorRegisterRequest)
        {
            throw new NotImplementedException();
        }

        public Task<MentorRegisterRequest> Get(long Id)
        {
            throw new NotImplementedException();
        }

        public Task<List<MentorRegisterRequest>> List(MentorRegisterRequestFilter MentorRegisterRequestFilter)
        {
            throw new NotImplementedException();
        }

        public Task<List<MentorRegisterRequest>> List(List<long> Ids)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(MentorRegisterRequest MentorRegisterRequest)
        {
            throw new NotImplementedException();
        }
    }
    
}
