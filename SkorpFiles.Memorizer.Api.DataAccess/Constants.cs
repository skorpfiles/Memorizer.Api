namespace SkorpFiles.Memorizer.Api.DataAccess
{
    internal static class Constants
    {
        public const string MemorizerSchemaName = "memorizer";

        public static class ExceptionMessages
        {
            public const string IdOrCodeShouldNotBeNull = "Either ID or Code of an object should not be null.";
            public const string IdOrCodeShouldBeNull = "Only one parameter of ID and Code of an object should be defined.";

            public const string UserCannotChangeQuestionnaire = "Current user cannot change this questionnaire.";
        }
    }
}
