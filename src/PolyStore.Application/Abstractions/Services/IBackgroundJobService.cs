using System.Linq.Expressions;
using System;
using System.Threading.Tasks;

namespace PolyStore.Application.Abstractions.Services;

public interface IBackgroundJobService
{
    /// <summary>
    /// Encola una tarea para ser ejecutada inmediatamente en segundo plano
    /// <summary>
    void Enqueue(Expression<Func<Task>> methodCall);
}