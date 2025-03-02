using System.ComponentModel.DataAnnotations;

namespace DoAnLTW.Models
{
    public class Account
    {
        public int id { get; set; }


        [Required, StringLength(100)]
        public string username { get; set; }
        
        [Required, StringLength(100)]
        public string password { get; set; }
     

        [Required, StringLength(100)]
        public string email { get; set; }
       
    }
}
