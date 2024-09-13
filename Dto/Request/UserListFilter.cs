namespace user_service_api.Dto;

public static class Sort
{
    public const string Acs = "acs";
    public const string Desc = "desc";
}

// 筛选项 用户手机号 用户名 用户所属社区 用户所在地区 
public class UserListFilter
{
    public string? Phone { get; set; }
    public string? Username { get; set; }
    public int? CommunityId { get; set; }
    public string? ProvinceCode { get; set; }
    public string? Sort { get; set; }    
}