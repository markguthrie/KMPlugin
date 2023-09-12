using EllieMae.Encompass.Automation;
using EllieMae.Encompass.BusinessEnums;
using EllieMae.Encompass.BusinessObjects; 
using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.BusinessObjects.Loans.Logging;
using EllieMae.Encompass.BusinessObjects.Loans.Templates;
using EllieMae.Encompass.BusinessObjects.Users;
using EllieMae.Encompass.Client;
using EllieMae.Encompass.Collections;
using EllieMae.Encompass.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarkUtilities
{

    public class Encompass
    {

        private static Dictionary<string, string> input_form_templates = new Dictionary<string, string>();
     

        /// <summary>

        /// Gets Encompass Users

        /// </summary>

        /// <param name="user_id">string, the ID of the user you want to return</param>

        /// <param name="session">EllieMae.Encompass.Client.Session, the session you want to grab the user</param>

        /// <returns>EllieMae.Encompass.BusinessObject.Users.User</returns>

        public static User GetUser(string user_id, Session session)

        {

            User user = null;

            try

            {

                user = session.Users.GetUser(user_id);

            }
            catch (Exception ex)

            {

                System.Windows.Forms.MessageBox.Show("Error getting users. Error " + ex.ToString());

            }

            return user;

        }



        /// <summary>

        /// Grabs all user ID's and returns them as a list

        /// </summary>

        /// <param name="get_enabled_users">bool - true if you want to include enabled users</param>

        /// <param name="get_disbaled_users">bool - true if you want to include disabled users</param>

        /// <param name="include_locked_users">bool - true if you want locked users included</param>

        /// <returns>List<string></returns>

        public static List<string> GetAllUserIDs(bool get_enabled_users = true, bool get_disbaled_users = false, bool include_locked_users = true)

        {

            List<string> all_users = new List<string>();

            try

            {

                UserList users = EncompassApplication.Session.Users.GetAllUsers();

                foreach (User user in users)

                {

                    // CHECK IF USER IS ENABLED

                    if (user.Enabled)

                    {

                        // CHECK IF WE WANT ENABLED USERS

                        if (get_enabled_users)

                        {

                            // CHECK IF USER ACCOUNT IS LOCKED AND IF WE WANT TO INCLUDE LOCKED USERS

                            if (include_locked_users && user.AccountLocked)

                            {

                                // ADD TO LIST IF WE WANT ENABLED + LOCKED USERS

                                all_users.Add(user.ID);

                            }

                            else if (!user.AccountLocked)

                            {

                                // ADD TO LIST IF USER IS NOT LOCKED AND ENABLED

                                all_users.Add(user.ID);

                            }

                        }

                    }

                    else

                    {

                        // CHECK IF USER IS DISABLED

                        if (get_disbaled_users)

                        {

                            // ADD TO LIST IF WE WANT DISABLED USERS

                            // CHECK IF USER ACCOUNT IS LOCKED AND IF WE WANT TO INCLUDE LOCKED USERS

                            if (include_locked_users && user.AccountLocked)

                            {

                                // ADD TO LIST IF WE WANT DISABLED + LOCKED USERS

                                all_users.Add(user.ID);

                            }

                            else if (!user.AccountLocked)

                            {

                                // ADD TO LIST IF USER IS NOT LOCKED AND DISABLED

                                all_users.Add(user.ID);

                            }

                        }



                    }

                }

            }

            catch (Exception ex)

            {

                all_users.Clear();

                all_users.Add("Error populating user list. ERROR: " + ex.Message);

            }

            return all_users;



        }



        /// <summary>

        /// Grabs all personas and returns them as a list of strings

        /// </summary>

        public static List<string> GetAllPersonas()

        {

            List<string> all_personas = new List<string>();

            try

            {

                Personas personas = EncompassApplication.Session.Users.Personas;

                foreach (Persona persona in personas)

                {

                    all_personas.Add(persona.Name);

                }

            }

            catch (Exception ex)

            {

                all_personas.Clear();

                all_personas.Add("Error populating personas list. ERROR: " + ex.Message);

            }

            return all_personas;

        }



        /// <summary>

        /// Grabs all roles and returns them as a Dictionary. Key = Abbreviation, Value = Role Name

        /// </summary>

        /// <returns></returns>

        public static Dictionary<string, string> GetAllRoles()

        {

            Dictionary<string, string> all_roles = new Dictionary<string, string>();

            try

            {

                Roles roles = EncompassApplication.Session.Loans.Roles;

                foreach (Role role in roles)

                {

                    if (!all_roles.ContainsKey(role.Abbreviation))

                    {

                        all_roles.Add(role.Abbreviation, role.Abbreviation + " - " + role.Name);

                    }

                }

            }

            catch (Exception ex)

            {

                all_roles.Clear();

                all_roles.Add("ERROR", "Error populating roles list. ERROR: " + ex.Message);

            }

            return all_roles;



        }

        /// <summary>

        /// Gets all global input form set templates from Encompass and returns them as a dictionary (ID = Template Name, VAL = Template Path)

        /// </summary>

        public static Dictionary<string, string> GetAllInputFormTemplates(TemplateEntry template_entry)

        {

            input_form_templates.Clear();

            try

            {

                RunInputFormTemplates(template_entry);

            }
            catch (Exception ex)

            {

                input_form_templates.Clear();

                input_form_templates.Add("ERROR", "Error getting input form templates. ERROR: " + ex.Message);

            }

            return input_form_templates;

        }

        /// <summary>

        /// Get all input form templates and add them to dictionary

        /// </summary>

        /// <param name="parent">TemplateEntry </param>

        private static void RunInputFormTemplates(TemplateEntry parent)

        {

            try

            {

                // Retrieve the contents of the specified parent folder

                TemplateEntryList templateEntries = EncompassApplication.Session.Loans.Templates.GetTemplateFolderContents(TemplateType.InputFormSet, parent);

                // Iterate over each of the TemplateEntry records, each of which represents either

                // a Template or a subfolder of the parent folder.

                foreach (TemplateEntry e in templateEntries)

                {

                    AddTemplate(e);



                    // If the entry represents a subfolder, recurse into that folder

                    if (e.EntryType == TemplateEntryType.Folder)

                    {

                        RunInputFormTemplates(e);

                    }

                }

            }

            catch (Exception ex) { throw new Exception(ex.Message); }

        }



        /// <summary>

        /// Add an individual input form set template to dictionary

        /// </summary>

        /// <param name="e"></param>

        private static void AddTemplate(TemplateEntry e)

        {

            if (e.EntryType == TemplateEntryType.Template && !input_form_templates.ContainsKey(e.Name))

            {

                try

                {

                    input_form_templates.Add(e.Name, e.Path);

                }

                catch (Exception ex) { throw new Exception(ex.Message); }

            }

        }





        /// <summary>

        /// Adds an email entry to the Encompass conversation log under the last milestone. Note - Saves the loan afterwards so it takes a second to run.

        /// </summary>

        /// <param name="message">string - the body of the email. Can be text or HTML</param>

        /// <param name="sender_email">string - the email address of the sender (from email)</param>

        /// <param name="sender_name">string - the name of the sender (from)</param>

        /// <param name="recipient_email">string - the email addresses of the recipients (separated by semicolons)</param>

        /// <param name="recipient_name">string - the names of the recipients (separated by semicolons)</param>

        /// <param name="recipient_company_name">string - the company name of the recipient</param>

        /// <returns>boolean - if it fails adding to the log, it will return false. Otherwise it will return true</returns>

        public static bool LogEmailConversation(string message, string sender_email, string sender_name, string recipient_email, string recipient_name, string recipient_company_name)

        {

            try

            {

                Conversation conv = EncompassApplication.CurrentLoan.Log.Conversations.Add(DateTime.Now);

                conv.Comments = message.Trim();

                conv.EmailAddress = recipient_email.Trim();

                conv.Company = recipient_company_name;

                conv.ContactMethod = ConversationContactMethod.Email;

                conv.HeldWith = recipient_name.Trim();

                EncompassApplication.CurrentLoan.Commit();

                return true;

            }

            catch (Exception ex)

            {

                System.Windows.Forms.MessageBox.Show("Error adding to conversation log." + "Error " + ex.ToString());

                return false;

            }

        }



        /// <summary>

        /// Simple method that returns a string of an entire custom data object that is uploaded into Encompass

        /// </summary>

        /// <param name="file_name">The file name of the custom data object (Example: "users.csv")</param>

        /// <returns>String of the entire file defined</returns>

        public static string GetCustomObject(string file_name)

        {

            string result = "";

            DataObject ob = null;

            try

            {

                ob = EncompassApplication.Session.DataExchange.GetCustomDataObject(file_name);

                result = ob.ToString(Encoding.UTF8);

            }

            catch

            {

                result = "";

            }

            return result;

        }



        /// <summary>

        /// Saves a Global Custom Data Object to Encompass

        /// </summary>

        /// <param name="file_name">name of file</param>

        /// <param name="contents">contents of file</param>

        /// <param name="s">Session object used to save CDO</param>

        /// <returns></returns>

        public static Tuple<bool, string> SaveCDO(string file_name, string contents, Session s)

        {

            // Save XML settings to custom data object

            try

            {

                byte[] contents_byte = Encoding.ASCII.GetBytes(contents);

                DataObject cdo = new DataObject(contents_byte);

                if (s.IsConnected)

                {

                    s.DataExchange.SaveCustomDataObject(file_name, cdo);

                    return new Tuple<bool, string>(true, "Custom Data Object saved as \"" + file_name + "\"");

                }
                else

                {

                    throw new Exception("Encompass session not connected.");

                }

            }

            catch (Exception ex)

            {

                return new Tuple<bool, string>(false, "Error saving CDO as \"" + file_name + "\". ERROR: " + ex.Message);

            }

        }



        /// <summary>

        /// Returns a field of a CSV file that is uploaded to Encompass as a custom data object. Similar to HLookup in Excel.

        /// </summary>

        /// <param name="custom_data_object_name">The file name of the custom data object (Example: "users.csv")</param>

        /// <param name="search_column">Integer, this is the column number that your search value is in. "0" is the first column</param>

        /// <param name="search_value">This is the string value you're searching for in the previous column</param>

        /// <param name="result_column">Once the search value is found, a string value will be returned of the column index defined here. First columnn is also "0".</param>

        /// <returns>String of the field from the row you querying</returns>

        public static string SearchCSV(string custom_data_object_name, int search_column, string search_value, int result_column)

        {

            string result = "";

            DataObject text_input_csv = EncompassApplication.Session.DataExchange.GetCustomDataObject(custom_data_object_name);

            string[] csv = text_input_csv.ToString(Encoding.UTF8).Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in csv)

            {

                string[] field = line.Split(new string[] { ",", " ,", " , " }, StringSplitOptions.RemoveEmptyEntries);

                if (field[search_column] == search_value.Trim())

                {

                    result = field[result_column];

                    break;

                }

            }

            return result;

        }



        /// <summary>

        /// Test if one of the user's personas are in the list of allowed personas

        /// </summary>

        /// <param name="users_personas">Pass in the user's personas</param>

        /// <param name="allowed_personas">Pass in a list of the allowed personas</param>

        /// <returns>True or False</returns>



        public static bool CheckPermission(UserPersonas users_personas, List<Persona> allowed_personas)

        {

            bool output = false;

            foreach (Persona persona in allowed_personas)

            {

                if (users_personas.Contains(persona))

                {

                    return true;

                }

            }

            return output;

        }



        /// <summary>

        /// Returns a StringFieldCriterion

        /// </summary>

        /// <param name="field_id">ID of the field</param>

        /// <param name="val">value 0f the field</param>

        /// <param name="include">bool, if set to true, the parameter will be included, false and it will be excluded</param>

        /// <param name="match_type">StringFieldMatchType</param>

        /// <returns>StringFieldCriterion</returns>

        public static StringFieldCriterion CriString(string field_id, string val, bool include = true, StringFieldMatchType match_type = StringFieldMatchType.CaseInsensitive)

        {



            StringFieldCriterion cri = new StringFieldCriterion();

            if (field_id.Contains("Fields."))

            {

                cri.FieldName = field_id;

            }

            else

            {

                cri.FieldName = "Fields." + field_id;

            }

            cri.Value = val;

            cri.Include = include;

            cri.MatchType = match_type;

            return cri;

        }



        /// <summary>

        /// Returns a DateFieldCriterion

        /// </summary>

        /// <param name="field_id">ID of the field</param>

        /// <param name="val">DateTime of the value you want to compare against</param>

        /// <param name="match_type">OrdinalFieldMatchType, default to EQUALS</param>

        /// <returns>StringFieldCriterion</returns>

        public static DateFieldCriterion CriDate(string field_id, DateTime val, OrdinalFieldMatchType match_type = OrdinalFieldMatchType.Equals)

        {



            DateFieldCriterion cri = new DateFieldCriterion();

            if (field_id.Contains("Fields."))

            {

                cri.FieldName = field_id;

            }

            else

            {

                cri.FieldName = "Fields." + field_id;

            }

            cri.MatchType = match_type;

            cri.Value = val;

            return cri;

        }



        /// <summary>

        /// Returns a NumbericFieldCriterion

        /// </summary>

        /// <param name="field_id">ID of the field</param>

        /// <param name="val">Int of the value you want to compare against</param>

        /// <param name="match_type">OrdinalFieldMatchType, default to EQUALS</param>

        /// <returns>NumericFieldCriterion</returns>

        public static NumericFieldCriterion CriNumber(string field_id, int val, OrdinalFieldMatchType match_type = OrdinalFieldMatchType.Equals)

        {

            NumericFieldCriterion cri = new NumericFieldCriterion();

            if (field_id.Contains("Fields."))

            {

                cri.FieldName = field_id;

            }

            else

            {

                cri.FieldName = "Fields." + field_id;

            }

            cri.MatchType = match_type;

            cri.Value = val;

            return cri;

        }



        /// <summary>

        /// Create an Encompass Session and return it to the caller

        /// </summary>

        /// <param name="host">URL of Encompass server</param>

        /// <param name="id">User name</param>

        /// <param name="pw">Password</param>

        /// <returns>Encompass.Client.Session returned</returns>

        public static Session OpenSession(string host, string id, string pw)

        {

            Session s = new Session();

            try

            {

                s.Start(host, id, pw);

            }

            catch

            {

                s = null;

            }



            return s;

        }



        /// <summary>

        /// Sets a field in Encompass without any business rules being applied

        /// </summary>

        /// <param name="loan">Loan object you want to set ID of</param>

        /// <param name="sFieldID">ID of field to set</param>

        /// <param name="sValue">Value of field</param>

        public static void SetFieldNoRules(Loan loan, string sFieldID, string sValue)

        {

            string message = null;

            bool businessRulesEnabled = loan.BusinessRulesEnabled;

            if (businessRulesEnabled)

            {

                loan.BusinessRulesEnabled = false;

            }

            try

            {

                loan.Fields[sFieldID].Value = sValue;

            }

            catch (Exception ex)

            {

                message = ex.Message;

            }

            if (businessRulesEnabled)

            {

                loan.BusinessRulesEnabled = true;

            }



            if (message != null)

            {

                throw new Exception(message);

            }

        }



        /// <summary>

        /// Sets a field in Encompass without any business rules being applied

        /// </summary>

        /// <param name="loan">Loan object you want to set ID of</param>

        /// <param name="sFieldID">ID of field to set</param>

        /// <param name="sValue">Value of field</param>

        public static void SetField(Loan loan, string sFieldID, string sValue)

        {

            string message = null;

            try

            {

                loan.Fields[sFieldID].Value = sValue;

            }

            catch (Exception ex)

            {

                message = ex.Message;

            }

        }



        /// <summary>

        /// Checks if field is valid

        /// </summary>

        /// <param name="session">Encompass session object</param>

        /// <param name="sFieldID">Field ID you want to check</param>

        /// <returns>bool</returns>

        public static bool IsValidFieldID(Session session, string sFieldID)

        {

            try

            {

                if (session.Loans.FieldDescriptors[sFieldID] != null)

                {

                    return true;

                }

            }

            catch

            {

            }

            return false;

        }



        /// <summary>

        /// Gets an Encompass field and converts it to a date time. If it fails, Date.MinValue is returned.

        /// </summary>

        /// <param name="field_id">ID of field</param>

        /// <returns>DateTime</returns>

        public static DateTime GetDateField(string field_id, Loan loan = null)

        {

            DateTime dtval = DateTime.MinValue;

            if (loan == null) { return dtval; }

            try { dtval = DateTime.Parse(loan.Fields[field_id].UnformattedValue); } catch { }

            return dtval;

        }



        /// <summary>

        /// Gets a field value and returns it as a string. Returns blank if there's a problem.

        /// </summary>

        /// <param name="field_id">ID of field</param>

        /// <returns>string</returns>

        public static string GetField(string field_id, Loan loan = null)

        {

            if (loan == null) { return ""; }

            string val = "";

            try { val = loan.Fields[field_id].UnformattedValue.Trim(); } catch { }

            return val;



        }



        /// <summary>

        /// Gets a field value for a borrower pair and returns it as a string. Returns blank if there's a problem.

        /// </summary>

        /// <param name="field_id">ID of field</param>

        /// <param name="bp">Borrower Pair you want to grab the field for</param>

        /// <returns>string</returns>

        public static string GetBPField(string field_id, BorrowerPair bp, Loan loan = null)

        {

            if (loan == null) { return ""; }

            string val = "";

            try { val = loan.Fields[field_id].GetValueForBorrowerPair(bp); } catch { }

            return val;



        }



        /// <summary>

        /// Gets the local Encompass Data Directory for plugins, forms, and cache. Returns an object

        /// </summary>

        /// <param name="folder">The folder enumeration you would like to retrieve</param>

        /// <returns></returns>

        public static EncompassDir GetEncompassCacheDir()

        {

            EncompassDir encdir = new EncompassDir();

            StringBuilder pluginDir = new StringBuilder();

            string formsDir = "";



            try

            {



                foreach (string dir in System.IO.Directory.GetDirectories(EllieMae.Encompass.Client.Session.EncompassDataDirectory + @"\Settings\Cache"))

                {

                    pluginDir.Append(dir);

                    break;

                }



                formsDir = pluginDir.ToString() + @"\Forms";

                pluginDir.Append(@"\Plugins");

            }

            catch { }



            encdir.PluginsDir = pluginDir.ToString();

            encdir.FormsDir = formsDir;

            return encdir;

        }





    }



    public class EncompassDir

    {

        public string PluginsDir { get; set; }

        public string FormsDir { get; set; }

    }

}

