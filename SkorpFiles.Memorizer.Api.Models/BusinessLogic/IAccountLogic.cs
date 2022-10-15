namespace SkorpFiles.Memorizer.Api.Models.BusinessLogic
{
    public interface IAccountLogic
    {
        Task RegisterUserActivityAsync(string userName, string userId);
    }
}
