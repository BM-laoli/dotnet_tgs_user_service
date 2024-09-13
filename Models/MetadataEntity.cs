using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace user_service_api.Models;

public class MetadataEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } // 主键ID，自增

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // 数据创建时间

    [Column("updated_at")]
    [NotMapped] // 表示不要映射到 db中去
    public DateTime? UpdatedAt { get; set; } // 数据最后一次更新时间

    [Column("creator_id")]
    public int? CreatorId { get; set; } = 999999; // 数据创建者 999999 表示系统

    [Column("is_deleted")] public int? IsDeleted { get; set; } = 0; // 数据是否被删除; 0=未删除, 1=已删除
}