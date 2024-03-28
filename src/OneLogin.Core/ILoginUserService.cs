using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLogin.Core
{
    public interface ILoginUserService
    {
        Task<ExecuteResult> ValidateAsync(string username, string password);
    }
}
