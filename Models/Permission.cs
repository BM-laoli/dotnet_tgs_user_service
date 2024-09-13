using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace user_service_api.Models;

public enum PermissionType
{
    Menu = 1,  // 表示菜单权限
    Button = 2  // 表示按钮权限
}

public class Permission :MetadataEntity
{    
    [Required]
    [StringLength(255)]
    public string Name { get; set; }
 
    public string Icon { get; set; }

    [StringLength(255)]
    public string Description { get; set; }
    
    public PermissionType Type { get; set; } // 假设枚举类型定义为 PermissionType

    [Column("page_url")]
    [StringLength(255)]
    public string PageUrl { get; set; }
    
    [Column("parent_id")]
    public int? ParentId { get; set; }
}