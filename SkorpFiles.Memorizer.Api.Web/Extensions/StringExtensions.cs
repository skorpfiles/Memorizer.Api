namespace SkorpFiles.Memorizer.Api.Web.Extensions
{
    public static class StringExtensions
    {
        public static string PascalCaseToLowerCamelCase(this string source) =>
            char.ToLower(source[0]) + source[1..];
    }
}
