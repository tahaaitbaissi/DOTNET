using System;

namespace CarRental.Desktop.Services;

public static class ServiceLocator
{
    public static IServiceProvider Provider { get; set; }

    public static void Initialize(IServiceProvider serviceProvider)
    {
        Provider = serviceProvider;
    }

    public static T GetService<T>()
    {
        if (Provider == null)
            throw new InvalidOperationException("ServiceLocator n'est pas initialisé. Vérifiez App.xaml.cs.");

        var service = Provider.GetService(typeof(T));

        if (service == null)
            throw new InvalidOperationException($"Le service {typeof(T).Name} est introuvable. A-t-il été ajouté dans services.Add...() ?");

        return (T)service;
    }
}