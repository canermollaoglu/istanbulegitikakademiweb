using System.Collections.Generic;

namespace NitelikliBilisim.Core.ComplexTypes
{
    public class UserViewModel
    {
        public string Title { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string UserName { get; set; }
        public List<RoleModel> Roles { get; set; }
        public string FotoUrl { get; set; }
    }
}