namespace SkorpFiles.Memorizer.Api.Models.Interfaces.BusinessLogic
{
    public interface IAccountLogic
    {
        Task RegisterUserActivityAsync(string userName, string userId);
    }
}
