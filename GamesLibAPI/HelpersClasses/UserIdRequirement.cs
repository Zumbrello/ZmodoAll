using Microsoft.AspNetCore.Authorization;

public class UserIdRequirement : IAuthorizationRequirement
{
    public int UserId { get; }

    public UserIdRequirement(int userId)
    {
        UserId = userId;
    }
}