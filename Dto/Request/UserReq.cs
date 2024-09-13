using user_service_api.Models;

namespace user_service_api.Dto;

public class UserRegisterReq
{
    
    // 必填的需要填上其他的看情况
    public string UserName { get; set; }
    
    public string Phone { get; set; }
    
    public Enable Enable { get; set; }
    
    public ApproveStatus ApproveStatus { get; set; }
    
    public string Password { get; set; }
}

public class UserPage
{
    public PaginationReq PageInfo { get; set; }
    public UserListFilter? Filter { get; set; }

}

public class CanEditUserAttr
{
    public int? IsDeleted { get; set; } = 0; // 数据是否被删除; 0=未删除, 1=已删除
    public string? Username { get; set; } // 用户名

    public string? Phone { get; set; } // 用户手机号
    
    public Enable? Enable { get; set; } // 是否启用; 0=禁用, 1=启用

    public ApproveStatus? ApproveStatus { get; set; } // 审核状态; 0=无审核申请, 1=审核中, 2=审核驳回, 3=审核通过

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

// 编辑用户信息 需要 dto
public class UserUpdateReq
{
    public int Id { get; set; }

    public CanEditUserAttr UserInfo { get; set; }
}