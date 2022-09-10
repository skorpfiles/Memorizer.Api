namespace SkorpFiles.Memorizer.Api.Enums
{
    //
    // Summary:
    //     Possible results from a sign in attempt
    public enum SignInStatus
    {
        //
        // Summary:
        //     Sign in was successful
        Success,
        //
        // Summary:
        //     User is locked out
        LockedOut,
        //
        // Summary:
        //     Sign in requires addition verification (i.e. two factor)
        RequiresVerification,
        //
        // Summary:
        //     Sign in failed
        Failure
    }
}