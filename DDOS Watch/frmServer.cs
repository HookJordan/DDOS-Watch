using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DDOS_Watch
{
    public partial class frmServer : Form
    {
        public frmServer()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //cancel the new server window 
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            //validate information 
            int port;

            //check for valid port 
            if(!int.TryParse(txtPort.Text, out port))
            {
                MessageBox.Show("Invalid port. If you are unsure what a port is please seek further assistance from your network administrator.");
                return;
            }

            //check for ip / url to be entered 
            if(txtIP.Text == string.Empty)
            {
                MessageBox.Show("Please enter a server IP address or domain name!");
                return; 
            }

            //close window 
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void frmServer_Load(object sender, EventArgs e)
        {

        }
    }
}
