using System;
using System.Collections.Generic;
using System.Text;

using System.Threading; 

namespace DDOS_Watch.Watch
{
    class Watcher
    {
        /// <summary>
        /// The delay between checks 
        /// </summary>
        public int Delay { get; set; }
        
        /// <summary>
        /// List of email addresses to notify 
        /// </summary>
        public List<EmailAccount> Emails = new List<EmailAccount>();

        /// <summary>
        /// List of servers to watch 
        /// </summary>
        public List<Server> WatchList = new List<Server>();

        /// <summary>
        /// Thread to run on 
        /// </summary>
        private Thread t;

        /// <summary>
        /// Flag for running or not 
        /// </summary>
        private bool running; 

        /// <summary>
        /// Constructor for new watcher object 
        /// </summary>
        /// <param name="delay">The delay between checks </param>
        public Watcher(int delay)
        {
            //pass parameters
            this.Delay = Delay;

            //set not running 
            running = false;
        }

        /// <summary>
        /// Call this to start the watcher object 
        /// </summary>
        public void Run()
        {
            //if already running, exit 
            if (running)
                return;

            //else set the flag to running 
            running = true;

            //launch new background thread to perform checks 
            t = new Thread(doWork);
            t.IsBackground = true;
            t.Start();
        }

        /// <summary>
        /// This method runs in the background to check if servers are online or not 
        /// </summary>
        private void doWork()
        {
            //while enabled 
            while(running)
            {
                //loop through each server in the watch list 
                foreach(Server s in WatchList)
                {
                    //check if the server is online 
                    s.Check(); 

                    //if the servers status has changed since last check 
                    if(s.hasChanged())
                    {
                        //alert each email address in the emails list 
                        foreach(EmailAccount e in Emails)
                        {
                            if (s.isOnline)
                                //send new online alert 
                                e.SendNotificationEmail(s, Enums.NotificationType.Online);
                            else
                                //send new offline alert 
                                e.SendNotificationEmail(s, Enums.NotificationType.Offline);
                        }
                    }
                }

                //sleep for specified delay before checking account 
                Thread.Sleep(Delay);
            }
        }

        public void Stop()
        {
            //if already stopped, exit 
            if (!running)
                return;

            //else set the flag to off 
            running = false;

            //if the thread is not null 
            if(t != null)
            {
                //abort the thread to stop checks 
                t.Abort();

                //set thread to null 
                t = null;
            }
        }
    }
}
