namespace VerifyCallsOrder
{
    /// <summary>
    /// Since the default access level for Program is internal,
    /// we need a partial class to allow the web application factory to use program for integration tests.
    /// Nothing should be added to this class, only the Program.cs file should be updated.
    /// Another alternative is described here,
    /// https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-6.0#basic-tests-with-the-default-webapplicationfactory
    /// </summary>
    public partial class Program { }
}
