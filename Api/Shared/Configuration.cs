namespace Shared;

public class Configuration
{
    public static List<AppModule> Modules { get; private set; } = [];
    public static List<TransactionType> TransactionTypes { get; private set; } = [];

    public static void AddModule(AppModule module)
    {
        if (Modules.Any(p => p.Id == module.Id))
            throw new Exception($"Duplicate Module Id: {module.Id}");

        module.Features ??= [];
        module.Features = [..module.Features.Select(p => new AppFeature {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                ModuleId = p.ModuleId = module.Id})];
        Modules.Add(module);
    }

    public static void AddModules(IEnumerable<AppModule> modules)
    {
        foreach (var module in modules)
        {
            if (Modules.Any(p => p.Id == module.Id))
                throw new Exception($"Duplicate Module Id: {module.Id}");

            module.Features ??= [];
            module.Features = [..module.Features.Select(p => new AppFeature {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                ModuleId = p.ModuleId = module.Id})];
            Modules.Add(module);
        }
    }

    public static void AddFeature(AppFeature feature)
    {
        var modIdx = Modules.FindIndex(p => p.Id == feature.ModuleId);
        if (modIdx == -1)
            throw new Exception($"Module Id:{feature.ModuleId} not registered");

        Modules[modIdx].Features.Add(feature);
    }

    public static void AddFeatures(IEnumerable<AppFeature> features)
    {
        foreach (var feature in features) 
        {
            var modIdx = Modules.FindIndex(p => p.Id == feature.ModuleId);
            if (modIdx == -1)
                throw new Exception($"Module Id:{feature.ModuleId} not registered");

            Modules[modIdx].Features.Add(feature);
        }
    }

    public static void AddTransactionTypes(IEnumerable<TransactionType> transactionTypes)
    {
        TransactionTypes.AddRange(transactionTypes);
    }

    public static void AddTransactionType(TransactionType transactionType)
    {
        TransactionTypes.Add(transactionType);
    }
}

public class TransactionType
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ModuleId { get; set; }
}

public class AppModule
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public List<AppFeature> Features { get; set; } = [];
}

public class AppFeature
{
    public string Id { get; set; }
    public string ModuleId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
