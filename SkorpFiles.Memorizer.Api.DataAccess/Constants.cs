namespace SkorpFiles.Memorizer.Api.DataAccess
{
    internal static class Constants
    {
        public const string MemorizerSchemaName = "memorizer";

        public static class ExceptionMessages
        {
            public const string IdOrCodeShouldNotBeNull = "Either Questionnaire ID or Code should not be null.";
            public const string IdOrCodeShouldBeNull = "Only one parameter of ID and Code should be defined.";
        }
    }
}
