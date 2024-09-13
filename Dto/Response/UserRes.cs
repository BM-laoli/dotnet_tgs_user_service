using user_service_api.Models;

namespace user_service_api.Dto;

public class UserRes:BaseRes
{
    public string Username { get; set; } // 用户名

    public string Phone { get; set; } // 用户手机号
    
    public Enable Enable { get; set; } // 是否启用; 0=禁用, 1=启用

    public ApproveStatus ApproveStatus { get; set; } // 审核状态; 0=无审核申请, 1=审核中, 2=审核驳回, 3=审核通过

    public int? RoleId { get; set; } // 所具备的角色id

    public int? CommunityId { get; set; } // 所属社区id

    public string? Avatar { get; set; } // 用户头像

    public string? ProvinceCode { get; set; } // 省

    public string? CityCode { get; set; } // 市

    public string? DistrictCode { get; set; } // 区县

    public string? StreetCode { get; set; } // 街道

    public string? Address { get; set; } // 具体的地址

    public string? ApproveRejectReason { get; set; } // 创建时间
}