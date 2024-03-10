using SkorpFiles.Memorizer.Api.Models.Enums;
using SkorpFiles.Memorizer.Api.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
