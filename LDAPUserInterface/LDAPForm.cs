using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Windows.Forms;

namespace LDAPUserInterface
{
    public partial class LDAPForm : Form
    {
        List<UserPrincipal> _list = new List<UserPrincipal>();
        private int _index = 0;
        public LDAPForm()
        {
            InitializeComponent();
        }

        
        private void btnStart_Click(object sender, EventArgs e)
        {

            // create LDAP connection object  
            // enter AD settings  
            PrincipalContext AD = new PrincipalContext(ContextType.Domain, ConfigurationSettings.AppSettings["ServerLDAP"], ConfigurationSettings.AppSettings["UserLDAP"], ConfigurationSettings.AppSettings["PasswordLDAP"]);
            
            //PrincipalContext AD = new PrincipalContext(ContextType.Domain, "backoffice-fax");  
            UserPrincipal u = new UserPrincipal(AD);  
            u.SamAccountName = txtUsername.Text;  
  
            // search for user  
            PrincipalSearcher search = new PrincipalSearcher(u);  
            UserPrincipal result = (UserPrincipal)search.FindOne();  
            search.Dispose();  

            _list = new List<UserPrincipal>();
            _list.Add(result);
            _index = 0;
            // show some details  
            txtCognome.Text = result.Surname;
            txtNome.Text = result.GivenName;
            txtEmail.Text = result.GivenName.ToLower() + "." + result.Surname.ToLower() + "@" + ConfigurationSettings.AppSettings["Domain"];
  
        }

        private void btnTutti_Click(object sender, EventArgs e)
        {
            try
            {
                PrincipalContext AD = new PrincipalContext(ContextType.Domain, ConfigurationSettings.AppSettings["ServerLDAP"], ConfigurationSettings.AppSettings["UserLDAP"], ConfigurationSettings.AppSettings["PasswordLDAP"]);
                UserPrincipal u = new UserPrincipal(AD);
                PrincipalSearcher search = new PrincipalSearcher(u);
                _list = new List<UserPrincipal>();
                _index = 0;
                foreach (UserPrincipal result in search.FindAll())
                {
                    _list.Add(result);
                    
                }
                foreach (UserPrincipal result in _list)
                {
                    if (_list[_index].Surname != null && _list[_index].GivenName != null)
                    {
                        txtCognome.Text = _list[_index].Surname;
                        txtNome.Text = _list[_index].GivenName;
                        txtEmail.Text = _list[_index].GivenName.ToLower() + "." + _list[_index].Surname.ToLower() + "@" + ConfigurationSettings.AppSettings["Domain"];
                        break;
                    }
                    _index++;
                }

                txtOutput.Text += "Load " + _list.Count + " users.\r\n";
                search.Dispose();
            }
            catch (Exception ex)
            {
                txtOutput.Text += ex
                    + "\r\n";
            }  
        }

        private void btnSkip_Click(object sender, EventArgs e)
        {
            txtOutput.Text += "Skipped user " + _list[_index].Name + "\r\n";
            _index++;
            BindData();
        }

        private void btnSalvaEmail_Click(object sender, EventArgs e)
        {
            _list[_index].EmailAddress = txtEmail.Text;
            _list[_index].Save();


            txtOutput.Text += "Email " + _list[_index].EmailAddress + " saved.\r\n";
            txtOutput.Text += "Other " + (_list.Count - _index) + " emails.\r\n";

            _index++;
            BindData();

        }
        private void BindData()
        {
            if (_index<= _list.Count - 1){
                if (_list[_index].Surname != null && _list[_index].GivenName != null && (_list[_index].EmailAddress==null || _list[_index].EmailAddress==""))//(_list[_index].EmailAddress == null || _list[_index].EmailAddress.Contains(" ") || _list[_index].EmailAddress.Contains("'") || _list[_index].Surname.Contains(_list[_index].GivenName)))
                {
                    txtCognome.Text = _list[_index].Surname;
                    txtNome.Text = _list[_index].GivenName;
                    txtEmail.Text = (_list[_index].GivenName.ToLower() + "." + _list[_index].Surname.ToLower() +
                                    "@" + ConfigurationSettings.AppSettings["Domain"]).Trim();
                }
                else
                {
                    _index++;
                    BindData();
                }
            }
        }


    }
}
