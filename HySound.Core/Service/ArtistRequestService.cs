using HySound.Core.Service.IService;
using HySound.DataAccess.Repository.IRepository;
using HySound.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HySound.Core.Service
{
    public class ArtistRequestService : IArtistRequestService
    {
        IRepository<ArtistRequest> _requestRepository;
        IRepository<User> _userRepository;
        UserManager<IdentityUser> _userManager;

        public ArtistRequestService(UserManager<IdentityUser> userManager,IRepository<User> userRepository,IRepository<ArtistRequest> requestRepository)
        {
            _requestRepository = requestRepository;
            _userRepository = userRepository;
            _userManager = userManager;
        }

        public async Task<ArtistRequest> AddAsync(ArtistRequest request)
        {
            await _requestRepository.AddAsync(request);
            return request;
        }

        public async Task ApproveRequestAsync(int requestId, int adminId)
        {
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request == null || request.Status != "pending")
            {
                throw new InvalidOperationException("Request not found or already processed.");
            }

            var user = await _userManager.FindByIdAsync(request.IdentityUserId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            // Update request status
            request.Status = "approved";
            request.AdminId = adminId;

            // Assign "Artist" role and remove "User" role if present
            if (!await _userManager.IsInRoleAsync(user, "Artist"))
            {
                await _userManager.AddToRoleAsync(user, "Artist");
                if (await _userManager.IsInRoleAsync(user, "User"))
                {
                    await _userManager.RemoveFromRoleAsync(user, "User");
                }
            }

            await _requestRepository.UpdateAsync(request);
        }

        public async Task DenyRequestAsync(int requestId, int adminId)
        {
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request == null || request.Status != "pending")
            {
                throw new InvalidOperationException("Request not found or already processed.");
            }

            request.Status = "denied";
            request.AdminId = adminId;

            await _requestRepository.UpdateAsync(request);
        }

        public async Task<ArtistRequest> GetByIdAsync(int id)
        {
            return await _requestRepository.GetAll()
               .Include(x => x.User)
               .FirstOrDefaultAsync(x=>x.Id==id);
        }

        public async Task<List<ArtistRequest>> GetPendingRequestsAsync()
        {
            return await _requestRepository.GetAll()
                .Where(x => x.Status == "pending").Include(x => x.User)
                .ToListAsync();
        }

        public async Task<ArtistRequest> SubmitArtistRequestAsync(int userId,string identityId,string pfp, string name,string email, string bio, string password)
        {

            var request = new ArtistRequest
            {
                UserId = userId,
                IdentityUserId = identityId,
                ArtistUsername = name,
                Bio = bio,
                Password = password,
                Email = email,
                Status = "pending",
                ProfilePicture = pfp
            };

            await _requestRepository.AddAsync(request);
            return request;
        }

        public async Task UpdateAsync(ArtistRequest request)
        {
            await _requestRepository.UpdateAsync(request);
        }
    }
}
