using System;
using System.Collections.Generic;
using System.Text;

namespace DDOS_Watch.Watch
{
    //structure for how settings are stored 
    struct Settings
    {
        public int CheckDelay;
        public List<EmailAccount> Emails;
        public List<Server> Servers;
    }
}
