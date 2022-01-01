using System.ComponentModel.DataAnnotations;

namespace SaaSPOCModel.UserInfo
{
    public class FavoritePL: BaseEntity
    {
        [Required(AllowEmptyStrings = false)]
        public string FavoriteProgrammingLanguages { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string FavoriteIDEs { get; set; }
    }

    
}
