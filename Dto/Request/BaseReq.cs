using System.ComponentModel.DataAnnotations.Schema;

namespace user_service_api.Dto;

public class BaseReq
{
    [NotMapped]
    public int? Id { get; set; } // 主键ID，自增
    
    [NotMapped]
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // 数据创建时间
    
    [NotMapped]
    public DateTime? UpdatedAt { get; set; } // 数据最后一次更新时间

    [NotMapped]
    public int? CreatorId { get; set; } = 999999; // 数据创建者 999999 表示系统

    [NotMapped]
    public int? IsDeleted { get; set; } = 0; // 数据是否被删除; 0=未删除, 1=已删除
}