using BuildingBlocks.Cache;
using Profiles.Domain.Abstractions;
using Profiles.Domain.ValueObjects;

namespace Profiles.Domain.Models;
public class Profile : Entity<ProfileId>, ICachable
{
    public string? AvatarUrl { get; private set; }
    public ProfileName ProfileName { get; private set; }
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? Description { get; private set; }
    public int FollowerCount { get; private set; }
    public int FollowingCount { get; private set; }
    public int PostCount { get; private set; }
    public int ViewCount { get; private set; }
    public DateTime ViewCountResetAt { get; private set; } = DateTime.UtcNow;
    public bool IsCached { get; private set; }
    
    private const int ViewCountUpdateTime = 7;
    private const int CountTrigger = 2;

    private Profile() { }
    
    public static Profile Create(ProfileId id, ProfileName profileName)
    {
        return new Profile
        {
            Id = id,
            ProfileName = profileName,
        };
    }
    public static Profile Create(ProfileId id, ProfileName profileName, string avatarUrl,
        string firstName, string lastName, string description, 
        int followerCount, int followingCount, int postCount)
    {
        return new Profile
        {
            Id = id,
            ProfileName = profileName,
            AvatarUrl = avatarUrl,
            FirstName = firstName,
            LastName = lastName,
            Description = description,
            FollowerCount = followerCount,
            FollowingCount = followingCount,
            PostCount = postCount
        };
    }
    
    public void Update(string? firstName, string? lastName, string? description)
    {
        FirstName = firstName;
        LastName = lastName;
        Description = description;
    }

    public void UpdateProfileName(ProfileName newProfileName) =>
        ProfileName = newProfileName;

    public void UpdateAvatar(string avatarUrl) =>
        AvatarUrl = avatarUrl;
    
    public void AddFollower() =>
        FollowerCount++;
    
    public void AddFollowing() =>
        FollowingCount++;   
    
    public void IncrementPostCount() =>
        PostCount++;
    
    public void DecrementPostCount() =>
        PostCount--;

    #region Caching
    public void UpdateViewCount()
    {
        ResetViewCountIfOutdated();
        ViewCount++;
        LastModified = DateTime.UtcNow;
    }

    private void ResetViewCountIfOutdated()
    {
        if ((DateTime.UtcNow - ViewCountResetAt).TotalDays > ViewCountUpdateTime)
            ResetViewCount();
    }

    private void ResetViewCount()
    {
        ViewCount = 0;
        ViewCountResetAt = DateTime.UtcNow;
    }

    public void SetIsCached(bool value) =>
        IsCached = value;

    public bool ShouldBeCached()
    {
        UpdateViewCount();

        if (ViewCount >= CountTrigger)
        {
            ResetViewCount();
            IsCached = true;
            return true;
        }
        
        return false;
    }
    #endregion
}