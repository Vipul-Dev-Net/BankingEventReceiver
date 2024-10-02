﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApi.EventReceiver
{
    public interface IEventMessageProcessor
    {
        Task StartProcessing(CancellationToken cancellation);
    }
}
