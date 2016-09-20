using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DDOS_Watch
{
    public partial class Email : Form
    {
        public Email()
        {
            InitializeComponent();
        }

        private void cbHost_SelectedIndexChanged(object sender, EventArgs e)
        {
            //check if it's a custom smtp server 
            if(cbHost.Text == "CUSTOM")
            {
                //reset all settings 
                txtSMTPIP.ReadOnly = false;
                txtSMTPPORT.ReadOnly = false;
                txtSMTPIP.Text = "";
                txtSMTPPORT.Text = "";
            }
            else
            {
                //else check if it's a known email server 
                txtSMTPIP.ReadOnly = true;
                txtSMTPPORT.ReadOnly = true;

                //set known settings for popular servers 
                switch(cbHost.Text)
                {
                    case "HOTMAIL":
                        txtSMTPIP.Text = "smtp.live.com";
                        txtSMTPPORT.Text = "587";
                        break;
                    case "GMAIL":
                        txtSMTPIP.Text = "smtp.gmail.com";
                        txtSMTPPORT.Text = "587";
                        break;
                    case "YAHOO":
                        txtSMTPIP.Text = "smtp.mail.yahoo.com";
                        txtSMTPPORT.Text = "587";
                        break;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //cancel settings update 
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //If settings are invalid prompt user to fix, else close the settings window 
            if (txtEmail.Text != string.Empty && txtPassword.Text != string.Empty && txtSMTPIP.Text != string.Empty && txtSMTPPORT.Text != string.Empty)
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            else
                MessageBox.Show("Please fill in all fields before accepting!");
        }

        private void Email_Load(object sender, EventArgs e)
        {

        }
    }
}
