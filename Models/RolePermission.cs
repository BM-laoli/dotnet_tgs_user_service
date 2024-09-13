using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace user_service_api.Models;

public class RolePermission :MetadataEntity
{
    [Required]
    [Column("role_id")]
    public int RoleId { get; set; }
    
    [Required]
    [Column("permission_id")]
    public int PermissionId { get; set; }
}