using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DDOS_Watch.Watch;

using System.IO;

namespace DDOS_Watch
{
    public partial class frmMain : Form
    {
        //Watcher is the ddos watcher class 
        Watcher watch;

        //timer variables 
        int TimeToNextCheck = 0;
        int timePass = 0;
        public frmMain()
        {
            InitializeComponent();

            //setup new watcher with default settings 
            watch = new Watcher(30 * 1000);
            TimeToNextCheck = 30;
            watch.Run();

            //disable some buttons 
            editToolStripMenuItem.Enabled = false;
            removeToolStripMenuItem.Enabled = false;
            editToolStripMenuItem1.Enabled = false;
            removeToolStripMenuItem1.Enabled = false;
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //clear the lists 
            watch.WatchList.Clear();
            lstServer.Items.Clear();
            saveSettings();
        }

        private void clearToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //clear the lists 
            watch.Emails.Clear();
            lstEmail.Items.Clear();
            saveSettings();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //new server dialog 
            frmServer s = new frmServer();
            if(s.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //if a new server was entered correctly, create a new server object 
                Server _s = new Server();

                //assign server information 
                _s.IP = s.txtIP.Text;
                _s.PORT = Convert.ToInt32(s.txtPort.Text);

                //add server to list 
                addServer(_s);

                //save settings 
                saveSettings();
            }

            //clean up form 
            s.Dispose();
        }

        private void addServer(Server s)
        {
            //add server to lists 
            watch.WatchList.Add(s);
            lstServer.Items.Add(s.IP + ":" + s.PORT);
        }

        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //find the selected server 
            int index = lstServer.SelectedIndex;

            //create new server form 
            frmServer s = new frmServer();

            //add current server information to form 
            s.txtIP.Text = watch.WatchList[index].IP;
            s.txtPort.Text = watch.WatchList[index].PORT.ToString();

            //if the form is submitted 
            if(s.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //update server settings 
                lstServer.Items[index] = s.txtIP.Text + ":" + s.txtPort.Text;
                watch.WatchList[index].IP = s.txtIP.Text;
                watch.WatchList[index].PORT = Convert.ToInt32(s.txtPort.Text);

                //save settings 
                saveSettings();
            }

            //clean up 
            s.Dispose();
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //find the server to remove 
            int index = lstServer.SelectedIndex;

            //remove server from all lists 
            lstServer.Items.RemoveAt(index);
            watch.WatchList.RemoveAt(index);

            //save settings 
            saveSettings();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //set default refresh delay 
            watch.Delay = (int)numDelay.Value * 1000;

            //setup events for lst server 
            lstServer.DrawItem += lstServer_DrawItem;
            lstServer.DrawMode = DrawMode.OwnerDrawFixed;

            //setup events for draw email 
            lstEmail.DrawItem += lstEmail_DrawItem;
            lstEmail.DrawMode = DrawMode.OwnerDrawFixed;


            //check for settings file 
            if (File.Exists("Settings.dat"))
            {
                //create memory stream of settings information 
                using(MemoryStream ms = new MemoryStream(Convert.FromBase64String(File.ReadAllText("Settings.dat"))))
                {
                    //read the settings as binary information 
                    using(BinaryReader br = new BinaryReader(ms))
                    {
                        //read delay settings 
                        watch.Delay = br.ReadInt32();
                        numDelay.Value = watch.Delay / 1000;

                        //determine amount of email accounts to notify 
                        int count = br.ReadInt32();
                        for(int i = 0; i < count; i++)
                        {

                            //load each email account 
                            EmailAccount m = new EmailAccount();
                            m.Address = br.ReadString();
                            m.Password = br.ReadString();
                            m.SMTP_INDEX = br.ReadInt32();
                            m.SMTP_SERVER = br.ReadString();
                            m.SMTP_PORT = br.ReadInt32();
                            watch.Emails.Add(m);
                            lstEmail.Items.Add(m.Address);
                        }

                        //determine the amount of servers 
                        count = br.ReadInt32();
                        for(int i = 0; i < count; i++)
                        {
                            //load each server 
                            Server s = new Server();
                            s.IP = br.ReadString();
                            s.PORT = br.ReadInt32();

                            watch.WatchList.Add(s);
                            lstServer.Items.Add(s.IP + ":" + s.PORT);
                        }
                    }
                }
            }

            //add event for form close 
            this.FormClosing += frmMain_FormClosing;
        }

        void lstEmail_DrawItem(object sender, DrawItemEventArgs e)
        {
            if(lstEmail.Items.Count == 0)
                return;

            //customer draw email items (certain os by default wont draw colors) 
            Color c;
            e.DrawBackground();
            
            //determine if there is an error sending emails to certain address
            if (watch.Emails[e.Index].SendError)
                c = Color.Red; //red will signify error 
            else
                c = Color.Green; //green means the meial is okay 

            //highlight selected items 
            if (e.State == DrawItemState.Selected)
            {
                //use gray.. 
                e.Graphics.FillRectangle(Brushes.Gray, e.Bounds);
            }

            //draw the text of the list item with the appropriate color depending on if the email address is valid 
            e.Graphics.DrawString(lstEmail.Items[e.Index].ToString(),
                e.Font,
                new SolidBrush(c),
                0,
                e.Index * lstEmail.ItemHeight
            );

            //draw focus rectangly around selected item 
            e.DrawFocusRectangle();
        }

        void lstServer_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (lstServer.Items.Count == 0)
                return;

            //similar to emails... we will custom draw servers to use different colors 
            Color c;
            e.DrawBackground();
            
            //if a server is online 
            if (watch.WatchList[e.Index].isOnline)
                c = Color.Green; //draw it in green 
            else
                c = Color.Red; //otherwise draw it in red 


            //highlight gray when selected... 
            if (e.State == DrawItemState.Selected)
            {
                e.Graphics.FillRectangle(Brushes.Gray, e.Bounds);
            }


            //draw item... 
            e.Graphics.DrawString(lstServer.Items[e.Index].ToString(),
                e.Font,
                new SolidBrush(c),
                0,
                e.Index * lstServer.ItemHeight
            );

            e.DrawFocusRectangle();
        }

        void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //when user attempts to close program, prompt to hide program instead of clsoing 
            if (MessageBox.Show("Click yes to completely close this application. If you click anything else the program will be minimized to the task bar. In order to re-open it please double click on the icon!", "DDOS WATCH CLOSING", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
            {
                //if user wants to hide program, they can reopen it using the notify icon in the bottom right 
                e.Cancel = true;
                this.Hide();
            }

            //save settings one last time (incase something was changed?) 
            saveSettings();
        }
        private void saveSettings()
        {
            //create a new instance of settings 
            Settings set = new Settings();

            //set the settings based on delay and lists 
            set.CheckDelay = watch.Delay;
            set.Emails = watch.Emails;
            set.Servers = watch.WatchList;

            //create new memorystream to write to 
            using (MemoryStream ms = new MemoryStream())
            {
                //create new binarywriter to write with 
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    //write delay 
                    bw.Write(set.CheckDelay);

                    //write email count 
                    bw.Write(set.Emails.Count);

                    //loop through each email account 
                    for (int i = 0; i < set.Emails.Count; i++)
                    {
                        //write the email account address, password and email server settings 
                        bw.Write(set.Emails[i].Address);
                        bw.Write(set.Emails[i].Password);
                        bw.Write(set.Emails[i].SMTP_INDEX);
                        bw.Write(set.Emails[i].SMTP_SERVER);
                        bw.Write(set.Emails[i].SMTP_PORT);
                    }

                    //write the server count 
                    bw.Write(set.Servers.Count);

                    //loop through each server 
                    for (int i = 0; i < set.Servers.Count; i++)
                    {
                        //write the server settings 
                        bw.Write(set.Servers[i].IP);
                        bw.Write(set.Servers[i].PORT);
                    }
                }
                File.WriteAllText("Settings.dat", Convert.ToBase64String(ms.ToArray()));
            }
        }
        private void numDelay_ValueChanged(object sender, EventArgs e)
        {
            //when the delay is changed we need to update the watcher 
            watch.Delay = (int)numDelay.Value * 1000;

            //calculate time to next check 
            TimeToNextCheck = (int)numDelay.Value;
            timePass = 0;

            //restart the watcher 
            watch.Stop();
            watch.Run();
        }

        private void addToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //create a new email form 
            Email m = new Email();

            //if the email was added correctly 
            if(m.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //create new email object 
                EmailAccount acc = new EmailAccount();
                acc.Address = m.txtEmail.Text;
                acc.Password = m.txtPassword.Text;
                acc.SMTP_SERVER = m.txtSMTPIP.Text;
                acc.SMTP_PORT = Convert.ToInt32(m.txtSMTPPORT.Text);
                acc.SMTP_INDEX = m.cbHost.SelectedIndex;

                //add email to lists 
                watch.Emails.Add(acc);
                lstEmail.Items.Add(acc.Address);

                //save settings 
                saveSettings();
            }

            //clean up 
            m.Dispose();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //get selected email 
            int index = lstEmail.SelectedIndex;

            //create new email form 
            Email m = new Email();

            //copy over current email information 
            m.txtEmail.Text = watch.Emails[index].Address;
            m.txtPassword.Text = watch.Emails[index].Password;
            m.cbHost.SelectedIndex = watch.Emails[index].SMTP_INDEX;
            m.txtSMTPIP.Text = watch.Emails[index].SMTP_SERVER;
            m.txtSMTPPORT.Text = watch.Emails[index].SMTP_PORT.ToString();

            //if email is updated 
            if(m.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //set new email information 
                lstEmail.Items[index] = m.txtEmail.Text;

                watch.Emails[index].Address = m.txtEmail.Text;
                watch.Emails[index].Password = m.txtPassword.Text;
                watch.Emails[index].SMTP_INDEX = m.cbHost.SelectedIndex;
                watch.Emails[index].SMTP_SERVER = m.txtSMTPIP.Text;
                watch.Emails[index].SMTP_PORT = Convert.ToInt32(m.txtSMTPPORT.Text);

                //save settings 
                saveSettings();
            }

            //clean up 
            m.Dispose();
        }

        private void tmrCheck_Tick(object sender, EventArgs e)
        {
            //calculate time passed 
            timePass++;
            if(timePass == TimeToNextCheck)
            {
                //if it's time to run a check, restart the timer 
                timePass = 0;
            }

            //update time to next check text 
            txtCheck.Text = TimeToNextCheck - timePass + " SEC";
        }

        private void removeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //remove selected email 
            watch.Emails.RemoveAt(lstEmail.SelectedIndex);
            lstEmail.Items.RemoveAt(lstEmail.SelectedIndex);

            //save settings 
            saveSettings();
        }

        private void mnuServer_Opening(object sender, CancelEventArgs e)
        {
            //based on server count, enable or disable buttons 
            if (lstServer.SelectedItems.Count == 0)
            {
                editToolStripMenuItem1.Enabled = false;
                removeToolStripMenuItem.Enabled = false;
            }
            else
            {
                editToolStripMenuItem1.Enabled = true;
                removeToolStripMenuItem.Enabled = true;
            }
        }

        private void mnuEmail_Opening(object sender, CancelEventArgs e)
        {
            //based email count, enable or disable buttons 
            if (lstEmail.SelectedItems.Count == 0)
            {
                editToolStripMenuItem.Enabled = false;
                removeToolStripMenuItem1.Enabled = false;
            }
            else
            {
                editToolStripMenuItem.Enabled = true;
                removeToolStripMenuItem1.Enabled = true;
            }
        }

        private void iconDDOS_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //if you double click on the notify icon, show the form 
            this.Show();
        }
    }
}
