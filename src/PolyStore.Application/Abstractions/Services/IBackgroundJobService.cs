using System.Linq.Expressions;
using System;
using System.Threading.Tasks;

namespace PolyStore.Application.Abstractions.Services;

public interface IBackgroundJobService
{
    /// <summary>
    /// Encola una tarea para ser ejecutada inmediatamente en segundo plano
    /// <summary>
     
    // Usamos un genérico <T> para que Hangfire sepa qué servicio debe resolver
    void Enqueue<T>(Expression<Func<T, Task>> methodCall);
}