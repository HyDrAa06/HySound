using HySound.Models.Models;

namespace HySound.ViewModels.User
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePicture {  get; set; }

        public IFormFile? ImageFile {  get; set; }
        public ICollection<Followed>? Followers { get; set; }
        public ICollection<Followed>? Following { get; set; }
    }
}
