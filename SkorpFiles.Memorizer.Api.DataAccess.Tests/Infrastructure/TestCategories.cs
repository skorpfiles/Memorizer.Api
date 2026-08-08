namespace SkorpFiles.Memorizer.Api.DataAccess.Tests.Infrastructure
{
    /// <summary>
    /// Test category names used with <see cref="TestCategoryAttribute"/>.
    /// </summary>
    public static class TestCategories
    {
        /// <summary>
        /// Integration tests that need a real SQL Server database. They are excluded on CI,
        /// which runs <c>dotnet test --filter "TestCategory!=Integration"</c> (see the GitHub
        /// Actions workflows), and are meant to be run locally against a configured database.
        /// </summary>
        public const string Integration = "Integration";
    }
}
