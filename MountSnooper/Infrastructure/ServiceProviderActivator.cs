using Hangfire;
using System;

public class ServiceProviderActivator : JobActivator
{
    private readonly IServiceProvider _serviceProvider;

    public ServiceProviderActivator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override object ActivateJob(Type jobType)
    {
        return _serviceProvider.GetService(jobType);
    }
}