using HySound.Models.Models;

namespace HySound.Models.User
{
    public class UserViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePicture {  get; set; }
        public ICollection<Follower>? Followers { get; set; }
        public ICollection<Follower>? Following { get; set; }
    }
}
