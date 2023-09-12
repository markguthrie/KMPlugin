using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessObjects.Loans.Logging;
using EllieMae.Encompass.BusinessObjects.Loans;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KMPlugin
{
    public partial class MSAssignments : Form
    {
        private Loan loan = null;
        public MSAssignments()
        {
            InitializeComponent();
            try { 
                loan = EncompassApplication.CurrentLoan; 
            } catch(Exception ex) {
                MessageBox.Show($"Error grabbing current loan.{ex.Message}", "Error");
            }

            PopulateMilestoneAssigments();
        }

        public void PopulateMilestoneAssigments()
        {
            // Wipe out data
            gridMilestones.Columns.Clear();

            List<Models.MilestoneAssignment> msAssignments = new List<Models.MilestoneAssignment>();
                
            foreach (MilestoneEvent ms in loan.Log.MilestoneEvents)
            {
                Models.MilestoneAssignment msAssigned = new Models.MilestoneAssignment(ms.MilestoneName, ms.LoanAssociate.User.ID);
                if (!msAssignments.Contains(msAssigned))
                {
                    msAssignments.Add(msAssigned);
                }
            }

            gridMilestones.DataSource = msAssignments;


            //autoresize each column

            gridMilestones.AutoResizeColumn(0);

            gridMilestones.AutoResizeColumn(1);
        }
    }
}
