using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.AccessToSQLServer
{
    public class ImpersonationContext : IDisposable
    {
        private IntPtr tokenHandle = IntPtr.Zero;
        private WindowsImpersonationContext impersonationContext;

        public ImpersonationContext(string domain, string username, string password)
        {
            const int LOGON32_LOGON_NEW_CREDENTIALS = 9;
            const int LOGON32_PROVIDER_DEFAULT = 0;

            bool success = LogonUser(username, domain, password,
                LOGON32_LOGON_NEW_CREDENTIALS,
                LOGON32_PROVIDER_DEFAULT,
                out tokenHandle);

            if (!success)
            {
                int error = Marshal.GetLastWin32Error();
                throw new UnauthorizedAccessException($"LogonUser failed with error code: {error}");
            }

            WindowsIdentity identity = new WindowsIdentity(tokenHandle);
            impersonationContext = identity.Impersonate();
        }

        public void Dispose()
        {
            impersonationContext?.Undo();
            if (tokenHandle != IntPtr.Zero)
            {
                CloseHandle(tokenHandle);
            }
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword,
            int dwLogonType, int dwLogonProvider, out IntPtr phToken);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);
    }
}
