using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApi.EventReceiver.Utility
{
    internal static class RetryHelper
    {
        internal static void Retry(Action action, TimeSpan timeSpan)
        {
            Thread.Sleep(timeSpan);
            action();
        }
    }
}
