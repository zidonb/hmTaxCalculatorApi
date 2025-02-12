using TaxCalculator.Application.Interfaces.Services;
using TaxCalculator.Infrastructure.Services;

namespace TaxCalculator;

public static class Startup {
    // Read keys from appsettings.json
    // Implementing IOptions design pattern
    public static IServiceCollection AddConfigureSettings(this IServiceCollection services) {
        services
            .AddOptions<AppSettings>()
            .BindConfiguration(nameof(AppSettings))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddDbConfigureSettings();

        services.AddImpersonationConfigureSettings();

        services.AddAppConfigureSettings();

        return services;
    }

    private static IServiceCollection AddDbConfigureSettings(this IServiceCollection services) {
        var configuration = services.GetConfiguration();

        services
            .Configure<DbSettings>
            // Key    ,//Bind from appsettings.json
            (DbSettingsConstants.DEV_TRAINING_DB_NAME, configuration.GetSection("DbSettings"));

        return services;
    }

    private static IServiceCollection AddImpersonationConfigureSettings(this IServiceCollection services) {
        services.AddOptions<ImpersonateSettings>()
            .BindConfiguration("ImpersonateSettings");

        return services;
    }

    private static IServiceCollection AddAppConfigureSettings(this IServiceCollection services) {
        return services;
    }

    public static IServiceCollection AddExternalServices(this IServiceCollection services) {
        // Get appSettings from IOption
        var appSettings = services.BuildServiceProvider()
           .GetRequiredService<IOptions<AppSettings>>()
           .Value;

        // Cross-origin
        services.AddCors();

        // Add AddFluentValidationRulesToSwagger
        services.AddFluentValidationRulesToSwagger();

        // Add Auto Mapper
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());




        // Register validations
        #region Validators

        services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters()
            .AddValidatorsFromAssemblyContaining(typeof(Program));

        #endregion

        
        

        return services;
    }

    public static IServiceCollection AddAppServices(this IServiceCollection services) {
        services.AddAppDbContexts();

        //Register services to dependency injection
        #region Services

        services.AddScoped<IWorkFlowsService, WorkFlowsService>();
        services.AddSingleton<IRuleEngineLogger, RuleEngineLogger>();
        services.AddSingleton<RuleEnginePerYearFactory>();
        services.AddHostedService<WorkFlowsLoader>();
        #endregion

        //Register repositories to dependency injection
        #region Repositories


        #endregion

        return services;
    }

    private static IServiceCollection AddAppDbContexts(this IServiceCollection services) {
        var optionsSnapshot = services
            .BuildServiceProvider()
            .GetRequiredService<IOptionsSnapshot<DbSettings>>();

        return services;
    }

    // Use middleware
    public static void UseMiddleware(this WebApplication app) {
        // Add Shaam exceptions to middleware
        app.UseExceptionHandler();



        app.UseAppMiddleware();
    }

    // Use App middleware
    public static void UseAppMiddleware(this WebApplication app) {

    }

    #region Private
    private static IConfiguration GetConfiguration(this IServiceCollection services) {
        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        return configuration;
    }
    #endregion
}
