using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todos.Common.Validation;

namespace Todos.Domain.Entities.Abstract
{
    public interface IEntity
    {
        public string Id { get; set; }

        public ValidationResult Validate();
    }

    public interface IRootEntity : IEntity
    {
    }

    public abstract class Entity : IEntity
    {
        public string Id { get; set; } = "";

        public virtual ValidationResult Validate()
        {
            if (string.IsNullOrEmpty(Id)) ValidationResult.Failure("Id must be not null or empty");

            return ValidationResult.Success;
        }
    }

    public abstract class RootEntity : Entity
    {
    }
}
