using Microsoft.AspNetCore.Mvc;
using SkorpFiles.Memorizer.Api.Models.Exceptions;
using SkorpFiles.Memorizer.Api.Web.Exceptions;
using System.Reflection.Metadata;

namespace SkorpFiles.Memorizer.Api.Web.Controllers.Abstract
{
    public class ControllerWithActionsToBusinessLogic:Controller
    {
        protected private async Task<IActionResult> ExecuteActionToBusinessLogicAsync(Func<Task<IActionResult>> actionAsync)
        {
            IActionResult result;
            try
            {
                result = await actionAsync();
            }
            catch (AccessDeniedForUserException e)
            {
                result = Unauthorized(e.Message);
            }
            catch (BadRequestException e)
            {
                result = BadRequest(e.Message);
            }
            catch (ObjectNotFoundException e)
            {
                result = NotFound(e.Message);
            }
            return result;
        }

        protected private enum IdOrCode
        {
            Id,
            Code
        }

        protected private static IdOrCode GetIdType(string idOrCode, out object result)
        {
            bool isId = Guid.TryParse(idOrCode, out Guid guidResult);
            bool isCode = int.TryParse(idOrCode, out int intResult);
            if (isId)
            {
                result = guidResult;
                return IdOrCode.Id;
            }
            else if (isCode)
            {
                result = intResult;
                return IdOrCode.Code;
            }
            else
                throw new ArgumentException("The value is not a GUID ID or an int code.");
        }

        protected private async Task<IActionResult> SwitchIdOrCodeAndExecuteActionToBusinessLogicAsync<TBusinessLogicReturningType>(
            string idOrCode, Func<Guid, Task<TBusinessLogicReturningType>> actionIfIdAsync, Func<int, Task<TBusinessLogicReturningType>> actionIfCodeAsync,
            Func<TBusinessLogicReturningType, IActionResult> processResultAction)
        {
            object idOrCodeObj;
            IdOrCode isIdOrCode;
            IActionResult result;

            try
            {
                isIdOrCode = GetIdType(idOrCode, out idOrCodeObj);
            }
            catch (ArgumentException)
            {
                return BadRequest("Unsupported ID parameter: only GUID or integer are supported.");
            }

            result = await ExecuteActionToBusinessLogicAsync(async () =>
            {
                TBusinessLogicReturningType businessLogicResult;

                if (isIdOrCode == IdOrCode.Id)
                    businessLogicResult = await actionIfIdAsync((Guid)idOrCodeObj);
                else if (isIdOrCode == IdOrCode.Code)
                    businessLogicResult = await actionIfCodeAsync((int)idOrCodeObj);
                else
                    throw new NotImplementedException("Enum method is not implemented.");

                return processResultAction(businessLogicResult);
            });

            return result;
        }
    }
}
