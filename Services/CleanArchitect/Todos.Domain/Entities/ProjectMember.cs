using Todos.Domain.Entities.Abstract;

namespace Todos.Domain.Entities;

public class ProjectMember : RootEntity
{
    public string UserId { get; set; } = "";

    public string ProjectId { get; set; } = "";

    public Roles Role { get; set; } = Roles.Member;

    public enum Roles
    {
        Member,
        ProjectOwner
    }
}