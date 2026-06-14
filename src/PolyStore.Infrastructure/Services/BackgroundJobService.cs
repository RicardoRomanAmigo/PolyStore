using PolyStore.Application.Abstractions.Services;
using System;
using System.Linq.Expressions;
using Hangfire;

namespace PolyStore.Infrastructure.Services;

public class BackgroundJobService : IBackgroundJobService
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public BackgroundJobService(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    public void Enqueue(Expression<Func<Task>> methodCall)
    {
        _backgroundJobClient.Enqueue(methodCall);
    }
}