using System.Linq.Expressions;
using Todos.Common.Extensions;
using Todos.Common.Validation;
using Todos.Common.Validation.Abstract;
using Todos.Domain.Entities.Abstract;
using Todos.Domain.Exception;

namespace Todos.Domain.Entities;

public class Project : RootEntity
{
    public string Name { get; set; } = "";

    public string? Description { get; set; }

    public string CreatedBy { get; set; } = "";

    public List<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();

    public static ExpressionValidator<Project> HasSaveProjectPermissionValidator(string auditUserId)
    {
        return new ExpressionValidator<Project>(
            validExpr: project => project.CreatedBy == auditUserId,
            errorMsg: "Only project creator can add/update tasks.");
    }

    public Project AddNewTask(string name, string? description, string auditUserId)
    {
        ValidateHasSaveProjectPermission(auditUserId)
            .EnsureValid(exceptionForError: error => new UnauthorizedDomainException(error));

        Tasks.Add(new ProjectTask()
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Description = description,
            CreatedBy = auditUserId,
            ProjectId = Id,
            Status = ProjectTask.Statuses.New
        });

        return this;
    }

    public Project UpdateTask(ProjectTask projectTask, string currentUserId)
    {
        ValidateHasSaveProjectPermission(currentUserId)
            .EnsureValid(exceptionForError: error => new UnauthorizedDomainException(error));

        var foundReplacedTasks = Tasks.ReplaceWhere(p => p.Id == projectTask.Id, projectTask);

        if (foundReplacedTasks.IsEmpty()) throw new DomainException("Task not found");

        return this;
    }

    public ValidationResult ValidateHasSaveProjectPermission(string currentUserId)
    {
        return HasSaveProjectPermissionValidator(currentUserId).Validate(this);
    }

    public override ValidationResult Validate()
    {
        if (string.IsNullOrEmpty(Name)) return ValidationResult.Failure("Name must be not null or empty.");

        return base.Validate();
    }
}

public class ProjectTask : Entity
{
    public static ExpressionValidator<ProjectTask> CanAssignProjectMemberValidator(
        ProjectMember auditMember,
        ProjectMember projectMember)
    {
        return new ExpressionValidator<ProjectTask>(
            validExpr: task => task.ProjectId == projectMember.ProjectId &&
                          (auditMember.Role == ProjectMember.Roles.ProjectOwner || task.AssignedProjectMemberId == null) &
                          task.Status != Statuses.Finished,
            errorMsg: "Only ProjectMember can assign themselves to an unassigned task or ProjectOwner can change assignees and task must be not Finished.");
    }

    public static ExpressionValidator<ProjectTask> CanUpdateStatusValidator(
        ProjectMember auditMember)
    {
        return new ExpressionValidator<ProjectTask>(
            validExpr: task => task.AssignedProjectMemberId == auditMember.Id,
            errorMsg: "Only the task assignee can update task status.");
    }

    public static ExpressionValidator<ProjectTask> NameValidator()
    {
        return new ExpressionValidator<ProjectTask>(
            validExpr: task => !string.IsNullOrEmpty(task.Name),
            errorMsg: "Name must be not null or empty.");
    }

    public static ExpressionValidator<ProjectTask> ProjectIdValidator()
    {
        return new ExpressionValidator<ProjectTask>(
            validExpr: task => !string.IsNullOrEmpty(task.ProjectId),
            errorMsg: "ProjectId must be not null or empty.");
    }

    public string Name { get; set; } = "";

    public string? Description { get; set; }

    public string ProjectId { get; set; } = "";

    public string CreatedBy { get; set; } = "";

    public string? AssignedProjectMemberId { get; set; }

    public Statuses Status { get; set; }

    public override ValidationResult Validate()
    {
        return ValidationResult.Valid()
            .And(() => NameValidator().Validate(this))
            .And(() => ProjectIdValidator().Validate(this));
    }

    public ProjectTask AssignProjectMember(
        ProjectMember auditMember,
        ProjectMember projectMember)
    {
        ValidateCanAssignProjectMember(auditMember, projectMember)
            .EnsureValid(error => new DomainException(error));

        AssignedProjectMemberId = projectMember.Id;

        return this;
    }

    public ValidationResult ValidateCanAssignProjectMember(
        ProjectMember auditMember,
        ProjectMember projectMember)
    {
        return CanAssignProjectMemberValidator(auditMember, projectMember).Validate(this);
    }

    public ProjectTask UpdateStatus(
        Statuses status,
        ProjectMember auditMember)
    {
        CanUpdateStatusValidator(auditMember).Validate(this)
            .EnsureValid(error => new DomainException(error));

        Status = status;

        return this;
    }

    public enum Statuses
    {
        New,
        Inprogress,
        Finished
    }
}