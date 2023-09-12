using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.ComponentModel;
using EllieMae.Encompass.BusinessObjects.ExternalOrganization;

namespace KMPlugin

{
    /// <summary>
    /// Plugin for Kensie Mae to demonstrate foundational abilities of SDK Plugins
    /// </summary>

    [Plugin]

    public class KMPlugin
    {
        private Models.TriggerFields triggerFields = new Models.TriggerFields();

        public KMPlugin()
        {
            EncompassApplication.Login += EncompassApplication_Login;
        }

        private void EncompassApplication_Login(object sender, EventArgs e)
        {
            EncompassApplication.LoanOpened += EncompassApplication_LoanOpened;
        }



        #region EncompassApplication_LoanOpened

        private void EncompassApplication_LoanOpened(object sender, EventArgs e)
        {

            // Subscribe to loan level events. 

            EncompassApplication.LoanClosing += EncompassApplication_LoanClosing;

            EncompassApplication.CurrentLoan.FieldChange += CurrentLoan_FieldChange;

            // Get initial values of fields
            GetCustomFieldValues(EncompassApplication.CurrentLoan);
        }

        private void GetCustomFieldValues(Loan loan)
        {
            // Set ID if undefined
            if (String.IsNullOrEmpty(triggerFields.CXMark1.ID.Trim()))
            {
                triggerFields.CXMark1.ID = "CX.MARK.1";

            }

            if (String.IsNullOrEmpty(triggerFields.CXMark2.ID.Trim()))
            {
                triggerFields.CXMark2.ID = "CX.MARK.2";

            }

            // Get value of field and record it to objecct
            triggerFields.CXMark1.Value = MarkUtilities.Encompass.GetField(triggerFields.CXMark1.ID, loan);
            triggerFields.CXMark2.Value = MarkUtilities.Encompass.GetField(triggerFields.CXMark2.ID, loan);

        } 
        #endregion



        #region EncompassApplication_LoanClosing

        private void EncompassApplication_LoanClosing(object sender, EventArgs e)

        {

            // make sure all subscribed to events have been unsibscribed to prevent

            // hanging/orphaned event subscriptions

            EncompassApplication.CurrentLoan.FieldChange -= CurrentLoan_FieldChange;
        }

        #endregion



        #region CurrentLoan_FieldChange

        private void CurrentLoan_FieldChange(object source, FieldChangeEventArgs e)
        {

            if (e.FieldID == triggerFields.CXMark1.ID && e.NewValue.ToLower().Trim() == "show")
            {
                using (MSAssignments assignedMilestones = new MSAssignments())
                {
                    assignedMilestones.Show();
                    assignedMilestones.Focus();
                }

                MarkUtilities.Encompass.SetField(EncompassApplication.CurrentLoan, triggerFields.CXMark1.ID, "");
            }


            if (e.FieldID == triggerFields.CXMark2.ID && !String.IsNullOrEmpty(e.NewValue.ToLower().Trim()))
            {
                StringBuilder borrowers = new StringBuilder();

                LoanBorrowerPairs pairs = EncompassApplication.CurrentLoan.BorrowerPairs;

                // Grab BorrowerPairs and put them in a list
                foreach (BorrowerPair pair in pairs)
                {
                    if (!String.IsNullOrEmpty(pair.Borrower.FirstName.Trim())) {
                        borrowers.Append($"{pair.Borrower.FirstName}, ");
                    }
                    if (!String.IsNullOrEmpty(pair.CoBorrower.FirstName.Trim())){
                        borrowers.Append($"{pair.CoBorrower.FirstName}, ");
                    }

                    // Trim last comma and space off the comma separated list
                    string output = borrowers.ToString().Length > 2 ? borrowers.ToString().Substring(0, -2) : borrowers.ToString();

                    // Display borrowers in a messagebox
                    System.Windows.Forms.MessageBox.Show(output, $"Loan #{EncompassApplication.CurrentLoan.LoanNumber} Borrowers", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
            }
        }
        #endregion

    }

}