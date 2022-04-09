using DadataApiProbe.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace DadataApiProbe.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Account
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public string Fullname { get; set; }
        [Required]
        public AccountType Type { get; set; }
        [Required]
        public string Inn { get; set; }

        public string Kpp { get; set; }
    }


}
