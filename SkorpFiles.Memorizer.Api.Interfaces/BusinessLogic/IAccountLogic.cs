namespace SkorpFiles.Memorizer.Api.Interfaces.BusinessLogic
{
    public interface IAccountLogic
    {
        Task RegisterUserActivityAsync(string userName, string userId);
    }
}
