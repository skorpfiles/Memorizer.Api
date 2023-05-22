using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.DataAccess.Models
{
    [Table("jAuthenticationCache", Schema = Constants.MemorizerSchemaName)]
    public class AuthenticationCache
    {
        [Key]
        public string? Key { get; set; }
        public string? Value { get; set; }
    }
}
