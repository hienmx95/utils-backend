﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Utils.Hub
{
    [Authorize]
    public class UserNotificationHub : Microsoft.AspNetCore.SignalR.Hub
    {
    }
}
