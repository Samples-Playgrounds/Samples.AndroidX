using System;

namespace Toggl.Core.Services
{
    public interface IErrorHandlingService
    {
        bool TryHandleDeprecationError(Exception error);
        bool TryHandleUnauthorizedError(Exception error);
        bool TryHandleNoWorkspaceError(Exception error);
        bool TryHandleNoDefaultWorkspaceError(Exception error);
    }
}
