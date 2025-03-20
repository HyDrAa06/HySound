using HySound.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HySound.Core.Service.IService
{
    public interface IArtistRequestService
    {
        Task<ArtistRequest> AddAsync(ArtistRequest request);
        Task<List<ArtistRequest>> GetPendingRequestsAsync();
        Task<ArtistRequest> GetByIdAsync(int id);
        Task UpdateAsync(ArtistRequest request);
        Task DeleteAsync(int userId);

        Task<ArtistRequest> SubmitArtistRequestAsync(int userId, string identityId,string pfp, string name, string email, string bio, string password);
        Task ApproveRequestAsync(int requestId, int adminId);
        Task DenyRequestAsync(int requestId, int adminId);
    }
}
