using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace user_service_api.Models;

public enum Enable
{
    Close = 0,  // 0=禁用, 1=启用
    Open = 1  // 
}

public enum ApproveStatus
{
 // 审核状态; 0=无审核申请, 1=审核中, 2=审核驳回, 3=审核通过
 NotApprove = 0,
 Approving = 1,
 ApproveSuccess = 2,
 ApproveReject = 3
}

/// <summary>
/// 用户信息模型
/// </summary>
public class UserInfo :MetadataEntity
{

    [Required]
    [StringLength(255)]
    public string Username { get; set; } // 用户名

    [Required]
    [StringLength(20)]
    public string Phone { get; set; } // 用户手机号
    
    [Required]
    public Enable Enable { get; set; } // 是否启用; 0=禁用, 1=启用

    [Required]
    [Column("approve_status")]
    public ApproveStatus ApproveStatus { get; set; } // 审核状态; 0=无审核申请, 1=审核中, 2=审核驳回, 3=审核通过

    [Required]
    [StringLength(255)]
    public string Password { get; set; } // 用户密码; 应该是哈希后的密码

    [Column("role_id")]
    public int? RoleId { get; set; } // 所具备的角色id

    [Column("community_id")]
    public int? CommunityId { get; set; } // 所属社区id

    [StringLength(255)]
    public string? Avatar { get; set; } // 用户头像


    [StringLength(255)]
    [Column("province_code")]
    public string? ProvinceCode { get; set; } // 省

    [StringLength(255)]
    [Column("city_code")]
    public string? CityCode { get; set; } // 市

    [StringLength(255)]
    [Column("district_code")]
    public string? DistrictCode { get; set; } // 区县

    [StringLength(255)]
    [Column("street_code")]
    public string? StreetCode { get; set; } // 街道

    [StringLength(255)]
    public string? Address { get; set; } // 具体的地址
    
    [Column("approve_reject_reason")]
    [StringLength(255)]
    public string? ApproveRejectReason { get; set; } // 创建时间
}