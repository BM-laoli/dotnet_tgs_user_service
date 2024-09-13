using System.ComponentModel.DataAnnotations;

namespace user_service_api.Models;

// 现在的角色只有三种 1.普通团购用户 2. 社区团长 3. 系统管理员
public class Role :MetadataEntity
{
    
    [Required]
    [StringLength(255)]
    public string Name { get; set; }
    
    [Required]
    public int? Enable { get; set; }
    public string? Icon { get; set; }
    public string? Description { get; set; }
}