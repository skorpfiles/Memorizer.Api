using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using SkorpFiles.Memorizer.Api.DataAccess.DependencyInjection;
using SkorpFiles.Memorizer.Api.DataAccess.Mapping;

namespace SkorpFiles.Memorizer.Api.DataAccess.Tests.Infrastructure
{
    public abstract class IntegrationTestsBase : IDisposable
    {
        protected ApplicationDbContext DbContext { get; private set; }
        protected IMapper Mapper { get; private set; }
        protected IServiceProvider ServiceProvider { get; private set; }

        private readonly DbContextOptions<ApplicationDbContext> _options;

        public IntegrationTestsBase()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlServer()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

            var configurationBuilder = new ConfigurationBuilder().AddUserSecrets("a8ce15b5-6722-46fe-9f32-95ceee83f5be");
            var configuration = configurationBuilder.Build();

            builder.UseSqlServer(configuration["DatabaseConnectionString"])
                    .UseInternalServiceProvider(serviceProvider);

            _options = builder.Options;

            DbContext = new ApplicationDbContext(builder.Options);
            DbContext.Database.EnsureDeleted();
            DbContext.Database.Migrate();
            SeedTestData();

            var services = new ServiceCollection();
            services.AddRepositories();

            var opt = new DbContextOptionsBuilder<ApplicationDbContext>();
            opt.UseSqlServer(configuration["DatabaseConnectionString"]);

            var mapperExpression = new MapperConfigurationExpression();
            mapperExpression.AddProfile(new DataAccessMappingProfile());
            var mapperConfig = new MapperConfiguration(mapperExpression, NullLoggerFactory.Instance);

            Mapper = mapperConfig.CreateMapper();

            services.AddScoped(services => mapperConfig.CreateMapper());

            ServiceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// Fills the freshly migrated database from the shared
        /// SkorpFiles.Memorizer.Api.DataAccess/Scripts/TestData.sql, which is copied into the
        /// Infrastructure output folder at build time (see the csproj). The script is a single
        /// self-contained batch (no GO separators), so it runs as one command.
        /// </summary>
        private void SeedTestData()
        {
            var scriptPath = Path.Combine(AppContext.BaseDirectory, "Infrastructure", "TestData.sql");
            var script = File.ReadAllText(scriptPath);

            // Run the script over the raw connection rather than through ExecuteSqlRaw, which
            // would treat the literal { } in the question texts as composite-format placeholders.
            var connection = DbContext.Database.GetDbConnection();
            var wasClosed = connection.State != System.Data.ConnectionState.Open;
            if (wasClosed)
                connection.Open();
            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = script;
                command.CommandTimeout = 120;
                command.ExecuteNonQuery();
            }
            finally
            {
                if (wasClosed)
                    connection.Close();
            }
        }

        /// <summary>
        /// A separate context on the same database, for reading persisted state back without
        /// the change tracker of the context the repository under test wrote through.
        /// </summary>
        protected ApplicationDbContext FreshContext() => new(_options);

        public void Dispose()
        {
            DisposeDbContext();
            GC.SuppressFinalize(this);
        }

        private void DisposeDbContext()
        {
            DbContext.Database.EnsureDeleted();
        }
    }
}
