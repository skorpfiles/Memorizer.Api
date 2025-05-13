using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SkorpFiles.Memorizer.Api.Models.Enums;
using SkorpFiles.Memorizer.Api.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SkorpFiles.Memorizer.Api.DataAccess
{
    public static class Utils
    {
        public static void CheckQuestionnaireAvailabilityForUser(Guid currentUserId, Guid ownerId, Availability availability)
        {
            CheckAvailabilityForUser(currentUserId, ownerId, "Unable to get details about a private questionnaire to a foreign user.", availability);
        }

        public static void CheckAvailabilityForUser(Guid currentUserId, Guid ownerId, string errorMessage, Availability? availability = null)
        {
            if (availability != null)
            {
                if (availability == Availability.Private && ownerId != currentUserId)
                    throw new AccessDeniedForUserException(errorMessage);
            }
            else
            {
                if (ownerId != currentUserId)
                    throw new AccessDeniedForUserException(errorMessage);
            }
        }

        public static async Task<bool> ExecuteInDangerOfMultipleAddUniqueRecordsAsync(Func<Task> asyncAction)
        {
            try
            {
                await asyncAction();
                return true;
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601) //already exists by index constraint
                {
                    //TODO add logging an attempt to add a duplicated record
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }
        public static string ReadEmbeddedTextFile(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"{assembly.GetName().Name}.{fileName}";

            using Stream? stream = assembly.GetManifestResourceStream(resourceName) ?? throw new FileNotFoundException($"The embedded resource '{resourceName}' was not found.");
            using StreamReader reader = new(stream);
            return reader.ReadToEnd();
        }
    }
}
