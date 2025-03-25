using FirebaseAdmin;

namespace HomeCleaningService.Extensions
{
    public static class FirebaseExtension
    {
        public static IServiceCollection AddFirebaseService(this IServiceCollection services, IConfiguration config)
        {
            var projectId = config["Firebase:ProjectId"];
            var credentialPath = config["Firebase:CredentialPath"];
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);
            FirebaseApp.Create();
            return services;
        }
    }
}
