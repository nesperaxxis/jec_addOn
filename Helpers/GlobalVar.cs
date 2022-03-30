using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM.Framework;
using SAPbobsCOM;
using System.Reflection;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Data.SqlClient;
using Application = SAPbouiCOM.Framework.Application;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Net;
using System.Web.Http;
using System.Net.Http;
using System.Net.Http.Headers;

namespace JEC_SAP.Helpers
{
    class GlobalVar
    {
        #region Variables
        public static SAPbobsCOM.Company iCompany { get; set; }
        public static SAPbobsCOM.Recordset oRS { get; set; }
        public static SAPbobsCOM.Recordset oRSSelect { get; set; }
        public static SAPbobsCOM.Recordset oRSQuery { get; set; }
        public static SAPbobsCOM.Recordset oRSExec { get; set; }
        public static SAPbobsCOM.Recordset oRSLog { get; set; }
        public static vbHelper_Library.Windows vbWindowsHelper { get; set; }
        public static string oQuery { get; set; }

        public static SAPbobsCOM.Company myCompany { get; set; }
        public static SAPbobsCOM.Recordset myRS { get; set; }
        public static SAPbouiCOM.DBDataSource myDBds { get; set; }

        public static string oSERVERNAME { get; set; }
        public static string oSERVERDB { get; set; }
        public static string oSERVERUN { get; set; }
        public static string oSERVERPW { get; set; }
        public static string oSERVERTYPE { get; set; }
        public static string oB1UN { get; set; }
        public static string oB1PW { get; set; }

        public static string oEmailAdd { get; set; }
        public static string oEmailAddCC { get; set; }
        public static string oEmailAddBCC { get; set; }
        public static string oEmailAddPW { get; set; }
        public static string oEmailHost { get; set; }
        public static string oEmailPort { get; set; }
        public static string oEmailBody { get; set; }
        public static string oEmailSSL { get; set; }

        public static string oGlobalEmailCc { get; set; }
        public static string oGlobalEmailBcc { get; set; }

        public static string oSelected;

        public static string oCenterCode { get; set; }
        public static string oCurrency { get; set; }
        public static string oCountry { get; set; }
        public static string oGroup { get; set; }
        public static string oDivision { get; set; }
        public static string oProduct { get; set; }
        public static string api_key { get; set; }
        public static string client { get; set; }
        public static string base_url { get; set; }
        public static int pricelistcode { get; set; }
        public static string oItemDescription { get; set; }

        public static int oDepartmentCode { get; set; }
        public static Boolean oDepartmentFinance { get; set; }
        public static Boolean oSuperUser { get; set; }

        static SAPbobsCOM.BusinessPartners oBusinessPartners { get; set; }
        static SAPbobsCOM.Documents oInvoice { get; set; }
        static SAPbobsCOM.Documents oCreditNote { get; set; }
        static SAPbobsCOM.Payments oIncomingPayment { get; set; }
        static SAPbobsCOM.Payments oOutgoingPayment { get; set; }
        static List<Models.API_BusinessPartners> BusinessPartnersModel { get; set; }
        static List<Models.API_Invoice> InvoiceModelHeader { get; set; }
        static List<Models.API_InvoiceDetails> InvoiceModelDetails { get; set; }
        static List<Models.API_CreditNote> CreditNoteModelHeader { get; set; }
        static List<Models.API_CreditNoteDetails> CreditNoteModelDetails { get; set; }
        static List<Models.API_CreditRefund> CreditRefundModel { get; set; }
        static List<Models.API_Receipt> ReceiptModelHeader { get; set; }
        static List<Models.API_ReceiptDetails> ReceiptModelDetails { get; set; }
        static List<Models.API_FinanceItem> ItemModel { get; set; }
        static List<Models.API_FinanceItem> ItemMasterModel { get; set; }
        public static List<Models.API_FinanceItem> listItem { get; set; }
        public static List<Models.ResponseResultSuccess> listResponseResultSuccess { get; set; }
        public static List<Models.ResponseResultFailed> listResponseResultFailed { get; set; }

        static string lastMessage { get; set; }
        static string strQuery { get; set; }

        #endregion

        #region "Properties"
        public string LastErrorMessage
        {
            get
            {
                return lastMessage;
            }
        }

        public static object Iif(bool expression, object truePart, object falsePart)
        { return expression ? truePart : falsePart; }

        public static object TrimData(string oValue)
        { return oValue.Replace("'", "''"); }

        public static object RemoveSpaceData(string oValue)
        { return oValue.Replace(" ", ""); }

        public static bool CheckDate(String date)
        {
            try
            {
                DateTime iDateTIme = DateTime.Parse(date);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static SAPbouiCOM.Item GetItem(string name, string form = "")
        {
            try
            {
                SAPbouiCOM.Item oItem;
                SAPbouiCOM.Form oForm;

                if (form == "")
                {
                    oForm = Application.SBO_Application.Forms.ActiveForm;
                }
                else
                {
                    oForm = Application.SBO_Application.Forms.GetForm(form, 1);
                }

                return oItem = oForm.Items.Item(name);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void releaseObject(object obj)
        {
            try
            {
                if (obj != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                    obj = null;
                }
            }
            catch (Exception)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }

        #endregion

        #region "Setting Value"
        public static void GetServerCredential()
        {
            try
            {
                oRS = (Recordset)myCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                Helpers.GlobalVar.oQuery = "select * from \"@OCRED\" where \"Code\" = '" + myCompany.CompanyDB + "'";

                oRS.DoQuery(Helpers.GlobalVar.oQuery);
                if (oRS.RecordCount > 0)
                {
                    oSERVERDB = myCompany.CompanyDB;
                    oSERVERTYPE = oRS.Fields.Item("U_ServerType").Value.ToString();
                    oSERVERNAME = oRS.Fields.Item("U_ServerName").Value.ToString();
                    oSERVERUN = oRS.Fields.Item("U_ServerUN").Value.ToString();
                    oSERVERPW = oRS.Fields.Item("U_ServerPW").Value.ToString();
                    oB1UN = oRS.Fields.Item("U_B1UN").Value.ToString();
                    oB1PW = oRS.Fields.Item("U_B1PW").Value.ToString();
                }
            }
            catch (Exception)
            { }
        }

        public static void GetEMailCredentials()
        {
            try
            {
                oRS = (Recordset)myCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                Helpers.GlobalVar.oQuery = "select * from \"@OEMAIL\"";

                oRS.DoQuery(Helpers.GlobalVar.oQuery);
                if (oRS.RecordCount > 0)
                {
                    oEmailAdd = oRS.Fields.Item("U_EMailAdd").Value.ToString();
                    oEmailAddPW = oRS.Fields.Item("U_EMailPw").Value.ToString();
                    oEmailAddCC = oRS.Fields.Item("U_EMailCC").Value.ToString();
                    oEmailAddBCC = oRS.Fields.Item("U_EMailBCC").Value.ToString();
                    oEmailHost = oRS.Fields.Item("U_EMailHost").Value.ToString();
                    oEmailPort = oRS.Fields.Item("U_EMailPort").Value.ToString();
                    oEmailSSL = oRS.Fields.Item("U_EMailSSL").Value.ToString();
                    oEmailBody = oRS.Fields.Item("U_EMailBody").Value.ToString();
                    oGlobalEmailCc = oRS.Fields.Item("U_EMailCc").Value.ToString();
                    oGlobalEmailBcc = oRS.Fields.Item("U_EMailBcc").Value.ToString();
                }
            }
            catch (Exception)
            { }
        }

        public static void GetIntegrationSetup()
        {
            try
            {
                oRS = (Recordset)myCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                Helpers.GlobalVar.oQuery = "select * from \"@INTEGRATIONSETUP\" where \"U_CompanyDB\" = '" + Helpers.GlobalVar.oSERVERDB + "'";

                oRS.DoQuery(Helpers.GlobalVar.oQuery);
                if (oRS.RecordCount > 0)
                {
                    oCenterCode = oRS.Fields.Item("Name").Value.ToString();
                    oCurrency = oRS.Fields.Item("U_Curr").Value.ToString();
                    oCountry = oRS.Fields.Item("U_Country").Value.ToString();
                    oGroup = oRS.Fields.Item("U_Group").Value.ToString();
                    oDivision = oRS.Fields.Item("U_Division").Value.ToString();
                    oProduct = oRS.Fields.Item("U_Product").Value.ToString();
                    api_key = oRS.Fields.Item("U_api_key").Value.ToString();
                    client = oRS.Fields.Item("Name").Value.ToString();
                    base_url = oRS.Fields.Item("U_base_url").Value.ToString();
                    pricelistcode = Convert.ToInt16(oRS.Fields.Item("U_pricelist_code").Value.ToString());
                }
            }
            catch (Exception)
            { }
        }

        public static int GetDepartmentCode()
        {
            try
            {
                Int16 Department = 0;
                oRS = (Recordset)myCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                Helpers.GlobalVar.oQuery = "select \"Department\" from \"OUSR\" where \"USER_CODE\" = '" + myCompany.UserName + "'";

                oRS.DoQuery(Helpers.GlobalVar.oQuery);
                if (oRS.RecordCount > 0)
                {
                    Department = Convert.ToInt16(oRS.Fields.Item(0).Value);
                }
                return Department;

            }
            catch (Exception)
            { return 0; }
        }

        public static bool IsDepartmentFinance()
        {
            try
            {
                Boolean DepartmentFinance = false;
                oRS = (Recordset)myCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                Helpers.GlobalVar.oQuery = "select \"Name\",\"Remarks\" from \"OUDP\" where \"Code\" = '" + oDepartmentCode + "'";

                oRS.DoQuery(Helpers.GlobalVar.oQuery);
                if (oRS.RecordCount > 0)
                {
                    if (oRS.Fields.Item("Name").Value.ToString().Contains("Finance") == true || oRS.Fields.Item("Remarks").Value.ToString().Contains("Finance") == true)
                    {
                        DepartmentFinance = true;
                    }
                }
                return DepartmentFinance;
            }
            catch (Exception)
            { return false; }
        }

        public static bool IsSuperUser()
        {
            try
            {
                Boolean oSuperUser = false;
                oRS = (Recordset)myCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                Helpers.GlobalVar.oQuery = "select \"SUPERUSER\" from \"OUSR\" where \"USER_CODE\" = '" + myCompany.UserName + "'";

                oRS.DoQuery(Helpers.GlobalVar.oQuery);
                if (oRS.RecordCount > 0)
                {
                    if (oRS.Fields.Item("SUPERUSER").Value.ToString() == "Y")
                    {
                        oSuperUser = true;
                    }
                }
                return oSuperUser;
            }
            catch (Exception)
            { return false; }
        }

        public static void InsertDatabase(string _companyDB = "")
        {
            try
            {
                Helpers.GlobalVar.oRSQuery = (Recordset)Helpers.GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                if (Helpers.GlobalVar.myCompany.DbServerType == BoDataServerTypes.dst_HANADB)
                {
                    if (_companyDB == "")
                    {
                        Helpers.GlobalVar.oQuery = "select " + Environment.NewLine +
                        "\"dbName\" \"CompanyDB\" " + Environment.NewLine +
                        "from \"SBOCOMMON\".\"SRGC\"";
                    }
                    else
                    {
                        Helpers.GlobalVar.oQuery = "select " + Environment.NewLine +
                        "\"dbName\" \"CompanyDB\" " + Environment.NewLine +
                        "from \"SBOCOMMON\".\"SRGC\" where \"dbName\" = '" + Helpers.GlobalVar.TrimData(_companyDB) + "'";                       
                    }
                    Helpers.GlobalVar.oRSQuery.DoQuery(Helpers.GlobalVar.oQuery);

                    while (!Helpers.GlobalVar.oRSQuery.EoF)
                    {
                        string companyDB = Helpers.GlobalVar.oRSQuery.Fields.Item("CompanyDB").Value.ToString();
                        Helpers.GlobalVar.oRS = (Recordset)Helpers.GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                        Helpers.GlobalVar.oQuery = "select * from \"" + companyDB + "\".\"@INTEGRATIONSETUP\" where \"U_CompanyDB\" = '" + Helpers.GlobalVar.TrimData(companyDB) + "'";
                        Helpers.GlobalVar.oRS.DoQuery(Helpers.GlobalVar.oQuery);
                        if (Helpers.GlobalVar.oRS.RecordCount > 0)
                        {
                            string clientName = Helpers.GlobalVar.oRS.Fields.Item("name").Value.ToString();
                            string Curr = Helpers.GlobalVar.oRS.Fields.Item("U_Curr").Value.ToString();
                            string Country = Helpers.GlobalVar.oRS.Fields.Item("U_Country").Value.ToString();
                            string Group = Helpers.GlobalVar.oRS.Fields.Item("U_Group").Value.ToString();
                            string Division = Helpers.GlobalVar.oRS.Fields.Item("U_Division").Value.ToString();
                            string Product = Helpers.GlobalVar.oRS.Fields.Item("U_Product").Value.ToString();
                            string api_key = Helpers.GlobalVar.oRS.Fields.Item("U_api_key").Value.ToString();
                            string base_url = Helpers.GlobalVar.oRS.Fields.Item("U_base_url").Value.ToString();
                            string pricelist_code = Helpers.GlobalVar.oRS.Fields.Item("U_pricelist_code").Value.ToString();
                            string activate = "N";

                            Helpers.GlobalVar.oRSSelect = (Recordset)Helpers.GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                            Helpers.GlobalVar.oQuery = "select * from \"TAIDII_SAP\".\"axxis_tb_IntegrationSetup\" where \"companyDB\" = '" + companyDB + "'";
                            Helpers.GlobalVar.oRSSelect.DoQuery(Helpers.GlobalVar.oQuery);

                            if (Helpers.GlobalVar.oRSSelect.RecordCount == 0)
                            {
                                Helpers.GlobalVar.oRSExec = (Recordset)Helpers.GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                                Helpers.GlobalVar.oQuery = "insert into \"TAIDII_SAP\".\"axxis_tb_IntegrationSetup\" " + Environment.NewLine +
                                "(\"companyDB\",\"clientName\",\"Curr\",\"Country\",\"Group\",\"Division\", " + Environment.NewLine +
                                "\"Product\",\"api_key\",\"base_url\",\"pricelist_code\",\"activate\") " + Environment.NewLine +
                                "values ('" + Helpers.GlobalVar.TrimData(companyDB) + "','" + Helpers.GlobalVar.TrimData(clientName) + "', " + Environment.NewLine +
                                "'" + Helpers.GlobalVar.TrimData(Curr) + "','" + Helpers.GlobalVar.TrimData(Country) + "', " + Environment.NewLine +
                                "'" + Helpers.GlobalVar.TrimData(Group) + "','" + Helpers.GlobalVar.TrimData(Division) + "', " + Environment.NewLine +
                                "'" + Helpers.GlobalVar.TrimData(Product) + "','" + Helpers.GlobalVar.TrimData(api_key) + "', " + Environment.NewLine +
                                "'" + Helpers.GlobalVar.TrimData(base_url) + "','" + Helpers.GlobalVar.TrimData(pricelist_code) + "','" + Helpers.GlobalVar.TrimData(activate) + "')";
                                Helpers.GlobalVar.oRSExec.DoQuery(Helpers.GlobalVar.oQuery);
                            }
                            else
                            {
                                Helpers.GlobalVar.oRSExec = (Recordset)Helpers.GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                                Helpers.GlobalVar.oQuery = "update \"TAIDII_SAP\".\"axxis_tb_IntegrationSetup\" set  " + Environment.NewLine +
                                "\"clientName\" = '" + Helpers.GlobalVar.TrimData(clientName) + "', " + Environment.NewLine +
                                "\"Curr\" = '" + Helpers.GlobalVar.TrimData(Curr) + "', " + Environment.NewLine +
                                "\"Country\" = '" + Helpers.GlobalVar.TrimData(Country) + "', " + Environment.NewLine +
                                "\"Group\" = '" + Helpers.GlobalVar.TrimData(Group) + "', " + Environment.NewLine +
                                "\"Division\" = '" + Helpers.GlobalVar.TrimData(Division) + "', " + Environment.NewLine +
                                "\"Product\" = '" + Helpers.GlobalVar.TrimData(Product) + "', " + Environment.NewLine +
                                "\"api_key\" = '" + Helpers.GlobalVar.TrimData(api_key) + "', " + Environment.NewLine +
                                "\"base_url\" = '" + Helpers.GlobalVar.TrimData(base_url) + "', " + Environment.NewLine +
                                "\"pricelist_code\" = '" + Helpers.GlobalVar.TrimData(pricelist_code) + "', " + Environment.NewLine +
                                "\"activate\" = '" + Helpers.GlobalVar.TrimData(activate) + "' " + Environment.NewLine +
                                "where \"companyDB\" = '" + Helpers.GlobalVar.TrimData(companyDB) + "'";
                                Helpers.GlobalVar.oRSExec.DoQuery(Helpers.GlobalVar.oQuery);
                            }
                        }
                        Helpers.GlobalVar.oRSQuery.MoveNext();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Insert Database Method - " + ex.Message);
            }
        }
        #endregion

        #region "Helpers"
        public static string ConvertToSAPDate(string Date)
        {
            string month, day, year;
            year = Date.Substring(0, 4);
            month = Date.Substring(4, 2);
            day = Date.Substring(6, 2);
            return day + "/" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt16(month)) + "/" + year;
        }

        public static string ConvertToValidDate(string Date)
        {
            string month, day, year;
            year = Date.Substring(0, 4);
            month = Date.Substring(4, 2);
            day = Date.Substring(6, 2);
            return month + "/" + day + "/" + year;
        }

        public static string GetSubMenuID(string menuDesc)
        {
            return Program._oSAP.GetSubMenuID_UserDefinedWindows(menuDesc);
        }

        #endregion

        #region "SBO Class"
        public static Boolean ConnectDI(string oServerType, string oServerName, string oServerUN, string oServerPW, string oCompanyDB, string oLicenseServer, string oB1Name, string oB1Password)
        {
            iCompany = new SAPbobsCOM.Company();

            switch (oServerType)
            {
                case "dst_MSSQL2005":
                    iCompany.DbServerType = BoDataServerTypes.dst_MSSQL2005;
                    break;
                case "dst_MSSQL2008":
                    iCompany.DbServerType = BoDataServerTypes.dst_MSSQL2008;
                    break;
                case "dst_MSSQL2012":
                    iCompany.DbServerType = BoDataServerTypes.dst_MSSQL2012;
                    break;
                case "dst_MSSQL2014":
                    iCompany.DbServerType = BoDataServerTypes.dst_MSSQL2014;
                    break;
                case "dst_HANADB":
                    iCompany.DbServerType = BoDataServerTypes.dst_HANADB;
                    break;
            }

            iCompany.Server = oServerName;
            iCompany.CompanyDB = oCompanyDB;
            iCompany.DbUserName = oServerUN;
            iCompany.DbPassword = oServerPW;
            iCompany.UserName = oB1Name;
            iCompany.Password = oB1Password;
            iCompany.UseTrusted = false;

            if (iCompany.Connect() != 0)
            {
                Application.SBO_Application.SetStatusBarMessage(iCompany.GetLastErrorDescription(), SAPbouiCOM.BoMessageTime.bmt_Short, true);
                return false;
            }
            else
            {
                if (iCompany.Connected == true)
                {
                    iCompany.Disconnect();
                    return true;
                }
            }
            return false;
        }

        public static bool SBOPostBusinessPartners(List<Models.API_BusinessPartners> listBusinessParters)
        {
            bool functionReturnValue = false;
            int lErrCode = 0;
            string oLogExist = string.Empty;
            string oCardCode = string.Empty;
            string oCountry = string.Empty;
            string oBPMaster = string.Empty;
            string GroupCode = string.Empty;
            SBOGetRecord clsSBOGetRecord = new SBOGetRecord();
            try
            {
                foreach (var iRowBP in listBusinessParters)
                {
                    try
                    {
                        oBPMaster = RemoveSpaceData(TrimData(iRowBP.BPMaster).ToString()).ToString();
                        oBusinessPartners = (BusinessPartners)GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.oBusinessPartners);

                        oCardCode = (String)clsSBOGetRecord.GetSingleValue("select \"CardCode\" from \"OCRD\" where \"CardCode\" = '" + oBPMaster + "'", GlobalVar.myCompany);
                        if (oCardCode == "" || oCardCode == "0")
                        {
                            ////**** Creation of Business Partners in SAP B1 ****\\\\
                            oBusinessPartners.CardType = BoCardTypes.cCustomer;

                            GroupCode = (String)clsSBOGetRecord.GetSingleValue("select \"GroupCode\" from \"OCRG\" where \"GroupName\" = 'STUDENT (Customer)'", GlobalVar.myCompany);

                            if (!string.IsNullOrEmpty(GroupCode))
                                oBusinessPartners.GroupCode = Convert.ToInt16(GroupCode);

                            if (!string.IsNullOrEmpty(oBPMaster))
                                oBusinessPartners.CardCode = oBPMaster;

                            if (!string.IsNullOrEmpty(iRowBP.fullname))
                                oBusinessPartners.CardName = iRowBP.fullname;

                            //if (iRowBP.country != "N.A")
                            //{
                            //    oCountry = (String)clsSBOGetRecord.GetSingleValue("select \"Code\" from \"OCRY\" where \"Name\" = '" + TrimData(iRowBP.country) + "'", sapCompany);
                            //    if (oCountry != "")
                            //    {
                            //        oBusinessPartners.Country = oCountry;
                            //    }
                            //    else
                            //    {
                            //        lastMessage = "Country is not found in SAP B1.";
                            //        sapRecSet.DoQuery("update " + iif(SBOConstantClass.ServerVersion != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'false',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + sapCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(SBOConstantClass.Database) + "' and \"module\" = 'Student' and \"uniqueId\" = '" + TrimData(iRowBP.BPMaster) + "'");

                            //        functionReturnValue = true;

                            //        goto isUpdateWithError;
                            //    }
                            //}

                            oBusinessPartners.Addresses.AddressType = BoAddressType.bo_BillTo;
                            oBusinessPartners.Addresses.AddressName = "Home Address";

                            if (!string.IsNullOrEmpty(iRowBP.address))
                                oBusinessPartners.Addresses.Street = iRowBP.address;

                            if (!string.IsNullOrEmpty(iRowBP.unit_no))
                                oBusinessPartners.Addresses.Block = iRowBP.unit_no;

                            if (!string.IsNullOrEmpty(iRowBP.postal_code))
                                oBusinessPartners.Addresses.ZipCode = iRowBP.postal_code;

                            ////**** Contact Person ****\\\\
                            if (!string.IsNullOrEmpty(iRowBP.contact_name))
                                oBusinessPartners.ContactEmployees.Name = iRowBP.contact_name;

                            if (!string.IsNullOrEmpty(iRowBP.contact_relation))
                                oBusinessPartners.ContactEmployees.Position = iRowBP.contact_relation;

                            if (!string.IsNullOrEmpty(iRowBP.contact_nric))
                                oBusinessPartners.ContactEmployees.FirstName = iRowBP.contact_nric;

                            if (!string.IsNullOrEmpty(iRowBP.contact_telephone))
                                oBusinessPartners.ContactEmployees.Phone1 = iRowBP.contact_telephone;

                            if (!string.IsNullOrEmpty(iRowBP.contact_email))
                                oBusinessPartners.ContactEmployees.E_Mail = iRowBP.contact_email;
                            ////**** Contact Person ****\\\\

                            ////**** User defined fields ****\\\\
                            if (!string.IsNullOrEmpty(iRowBP.country) && iRowBP.country != "N.A")
                                oBusinessPartners.UserFields.Fields.Item("U_Country").Value = iRowBP.country;

                            if (!string.IsNullOrEmpty(iRowBP.level))
                                oBusinessPartners.UserFields.Fields.Item("U_Level").Value = iRowBP.level;

                            if (!string.IsNullOrEmpty(iRowBP.nric))
                                oBusinessPartners.UserFields.Fields.Item("U_IC").Value = iRowBP.nric;

                            if (!string.IsNullOrEmpty(Convert.ToString(iRowBP.gender)))
                                oBusinessPartners.UserFields.Fields.Item("U_Gender").Value = iRowBP.gender;

                            if (!string.IsNullOrEmpty(iRowBP.dob))
                                oBusinessPartners.UserFields.Fields.Item("U_DOB").Value = Convert.ToDateTime(iRowBP.dob);

                            if (!string.IsNullOrEmpty(iRowBP.student_care_type))
                                oBusinessPartners.UserFields.Fields.Item("U_STD_CARE_TYPE").Value = iRowBP.student_care_type;

                            if (!string.IsNullOrEmpty(iRowBP.program_type))
                                oBusinessPartners.UserFields.Fields.Item("U_ProgramType").Value = iRowBP.program_type;

                            if (!string.IsNullOrEmpty(iRowBP.admission_date))
                                oBusinessPartners.UserFields.Fields.Item("U_AdmissionDate").Value = Convert.ToDateTime(iRowBP.admission_date);

                            if (!string.IsNullOrEmpty(iRowBP.registration_no))
                                oBusinessPartners.UserFields.Fields.Item("U_RegNo").Value = iRowBP.registration_no;

                            if (iRowBP.subsidy != 0)
                                oBusinessPartners.UserFields.Fields.Item("U_Subsidy").Value = iRowBP.subsidy;

                            if (iRowBP.additional_subsidy != 0)
                                oBusinessPartners.UserFields.Fields.Item("U_Add_Subsidy").Value = iRowBP.additional_subsidy;

                            if (iRowBP.financial_assistance != 0)
                                oBusinessPartners.UserFields.Fields.Item("U_FinAssist").Value = iRowBP.financial_assistance;

                            if (iRowBP.deposit != 0)
                                oBusinessPartners.UserFields.Fields.Item("U_Deposit").Value = iRowBP.deposit;

                            if (!string.IsNullOrEmpty(iRowBP.nationality))
                                oBusinessPartners.UserFields.Fields.Item("U_Nationality").Value = iRowBP.nationality;

                            if (!string.IsNullOrEmpty(iRowBP.race))
                                oBusinessPartners.UserFields.Fields.Item("U_Race").Value = iRowBP.race;

                            if (!string.IsNullOrEmpty(iRowBP.bank_name))
                                oBusinessPartners.ContactEmployees.UserFields.Fields.Item("U_Bankname").Value = iRowBP.bank_name;

                            if (!string.IsNullOrEmpty(iRowBP.account_name))
                                oBusinessPartners.ContactEmployees.UserFields.Fields.Item("U_AccName").Value = iRowBP.account_name;

                            if (!string.IsNullOrEmpty(iRowBP.cdac_bank_no))
                                oBusinessPartners.ContactEmployees.UserFields.Fields.Item("U_BankAccNo").Value = iRowBP.cdac_bank_no;

                            if (!string.IsNullOrEmpty(iRowBP.customer_ref_no))
                                oBusinessPartners.ContactEmployees.UserFields.Fields.Item("U_CusRefNo").Value = iRowBP.customer_ref_no;
                            ////**** User defined fields ****\\\\

                            lErrCode = oBusinessPartners.Add();

                            if (lErrCode == 0)
                            {
                                try
                                {
                                    oCardCode = GlobalVar.myCompany.GetNewObjectKey();
                                    lastMessage = "Successfully created Customer Code: " + oCardCode + " in SAP B1.";
                                    GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Posted',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oCardCode + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Student' and \"uniqueId\" = '" + TrimData(iRowBP.BPMaster) + "'");

                                    functionReturnValue = false;
                                }
                                catch
                                { }
                            }
                            else
                            {
                                lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'false',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Student' and \"uniqueId\" = '" + iRowBP.BPMaster + "'");

                                functionReturnValue = true;

                                goto isAddWithError;
                            }

                        isAddWithError: ;

                            ////**** Creation of Business Partners in SAP B1 ****\\\\
                        }
                        else
                        {
                            if (oBusinessPartners.GetByKey(oCardCode) == true)
                            {
                                if (!string.IsNullOrEmpty(iRowBP.fullname))
                                    oBusinessPartners.CardName = iRowBP.fullname;

                                //if (iRowBP.country != "N.A")
                                //{
                                //    oCountry = (String)clsSBOGetRecord.GetSingleValue("select \"Code\" from \"OCRY\" where \"Name\" = '" + TrimData(iRowBP.country) + "'", sapCompany);
                                //    if (oCountry != "")
                                //    {
                                //        oBusinessPartners.Country = oCountry;
                                //    }
                                //    else
                                //    {
                                //        lastMessage = "Country is not found in SAP B1.";
                                //        sapRecSet.DoQuery("update " + iif(SBOConstantClass.ServerVersion != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'false',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + sapCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(SBOConstantClass.Database) + "' and \"module\" = 'Student' and \"uniqueId\" = '" + TrimData(iRowBP.BPMaster) + "'");

                                //        functionReturnValue = true;

                                //        goto isUpdateWithError;
                                //    }
                                //}

                                oBusinessPartners.Addresses.AddressType = BoAddressType.bo_BillTo;
                                oBusinessPartners.Addresses.AddressName = "Home Address";

                                if (!string.IsNullOrEmpty(iRowBP.address))
                                    oBusinessPartners.Addresses.Street = iRowBP.address;

                                if (!string.IsNullOrEmpty(iRowBP.unit_no))
                                    oBusinessPartners.Addresses.Block = iRowBP.unit_no;

                                if (!string.IsNullOrEmpty(iRowBP.postal_code))
                                    oBusinessPartners.Addresses.ZipCode = iRowBP.postal_code;

                                ////**** Contact Person ****\\\\
                                if (!string.IsNullOrEmpty(iRowBP.contact_name))
                                    oBusinessPartners.ContactEmployees.Name = iRowBP.contact_name;

                                if (!string.IsNullOrEmpty(iRowBP.contact_relation))
                                    oBusinessPartners.ContactEmployees.Position = iRowBP.contact_relation;

                                if (!string.IsNullOrEmpty(iRowBP.contact_nric))
                                    oBusinessPartners.ContactEmployees.FirstName = iRowBP.contact_nric;

                                if (!string.IsNullOrEmpty(iRowBP.contact_telephone))
                                    oBusinessPartners.ContactEmployees.Phone1 = iRowBP.contact_telephone;

                                if (!string.IsNullOrEmpty(iRowBP.contact_email))
                                    oBusinessPartners.ContactEmployees.E_Mail = iRowBP.contact_email;
                                ////**** Contact Person ****\\\\

                                ////**** User defined fields ****\\\\
                                if (!string.IsNullOrEmpty(iRowBP.country) && iRowBP.country != "N.A")
                                    oBusinessPartners.UserFields.Fields.Item("U_Country").Value = iRowBP.country;

                                if (!string.IsNullOrEmpty(iRowBP.level))
                                    oBusinessPartners.UserFields.Fields.Item("U_Level").Value = iRowBP.level;

                                if (!string.IsNullOrEmpty(iRowBP.nric))
                                    oBusinessPartners.UserFields.Fields.Item("U_IC").Value = iRowBP.nric;

                                if (!string.IsNullOrEmpty(Convert.ToString(iRowBP.gender)))
                                    oBusinessPartners.UserFields.Fields.Item("U_Gender").Value = iRowBP.gender;

                                if (!string.IsNullOrEmpty(iRowBP.dob))
                                    oBusinessPartners.UserFields.Fields.Item("U_DOB").Value = Convert.ToDateTime(iRowBP.dob);

                                if (!string.IsNullOrEmpty(iRowBP.student_care_type))
                                    oBusinessPartners.UserFields.Fields.Item("U_STD_CARE_TYPE").Value = iRowBP.student_care_type;

                                if (!string.IsNullOrEmpty(iRowBP.program_type))
                                    oBusinessPartners.UserFields.Fields.Item("U_ProgramType").Value = iRowBP.program_type;

                                if (!string.IsNullOrEmpty(iRowBP.admission_date))
                                    oBusinessPartners.UserFields.Fields.Item("U_AdmissionDate").Value = Convert.ToDateTime(iRowBP.admission_date);

                                if (!string.IsNullOrEmpty(iRowBP.registration_no))
                                    oBusinessPartners.UserFields.Fields.Item("U_RegNo").Value = iRowBP.registration_no;

                                if (iRowBP.subsidy != 0)
                                    oBusinessPartners.UserFields.Fields.Item("U_Subsidy").Value = iRowBP.subsidy;

                                if (iRowBP.additional_subsidy != 0)
                                    oBusinessPartners.UserFields.Fields.Item("U_Add_Subsidy").Value = iRowBP.additional_subsidy;

                                if (iRowBP.financial_assistance != 0)
                                    oBusinessPartners.UserFields.Fields.Item("U_FinAssist").Value = iRowBP.financial_assistance;

                                if (iRowBP.deposit != 0)
                                    oBusinessPartners.UserFields.Fields.Item("U_Deposit").Value = iRowBP.deposit;

                                if (!string.IsNullOrEmpty(iRowBP.nationality))
                                    oBusinessPartners.UserFields.Fields.Item("U_Nationality").Value = iRowBP.nationality;

                                if (!string.IsNullOrEmpty(iRowBP.race))
                                    oBusinessPartners.UserFields.Fields.Item("U_Race").Value = iRowBP.race;

                                if (!string.IsNullOrEmpty(iRowBP.bank_name))
                                    oBusinessPartners.ContactEmployees.UserFields.Fields.Item("U_Bankname").Value = iRowBP.bank_name;

                                if (!string.IsNullOrEmpty(iRowBP.account_name))
                                    oBusinessPartners.ContactEmployees.UserFields.Fields.Item("U_AccName").Value = iRowBP.account_name;

                                if (!string.IsNullOrEmpty(iRowBP.cdac_bank_no))
                                    oBusinessPartners.ContactEmployees.UserFields.Fields.Item("U_BankAccNo").Value = iRowBP.cdac_bank_no;

                                if (!string.IsNullOrEmpty(iRowBP.customer_ref_no))
                                    oBusinessPartners.ContactEmployees.UserFields.Fields.Item("U_CusRefNo").Value = iRowBP.customer_ref_no;
                                ////**** User defined fields ****\\\\

                                lErrCode = oBusinessPartners.Update();
                                if (lErrCode == 0)
                                {
                                    try
                                    {
                                        oCardCode = GlobalVar.myCompany.GetNewObjectKey();
                                        lastMessage = "Successfully updated Customer Code: " + oCardCode + " in SAP B1.";
                                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Posted',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oCardCode + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Student' and \"uniqueId\" = '" + TrimData(iRowBP.BPMaster) + "'");

                                        functionReturnValue = false;
                                    }
                                    catch
                                    { }
                                }
                                else
                                {
                                    lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                    GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'false',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Student' and \"uniqueId\" = '" + TrimData(iRowBP.BPMaster) + "'");

                                    functionReturnValue = true;

                                    goto isUpdateWithError;
                                }
                            }

                        isUpdateWithError: ;

                        }
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(oBusinessPartners);
                    }
                    catch (Exception ex)
                    {
                        lastMessage = ex.ToString();
                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'false',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Student' and \"uniqueId\" = '" + TrimData(iRowBP.BPMaster) + "'");
                        functionReturnValue = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return functionReturnValue;
        }

        public static bool SBOPostInvoiceDraft(List<Models.API_Invoice> listInvoice)
        {
            bool functionReturnValue = false;
            int lErrCode = 0;
            int oId = 0;
            int oStatus = 0;
            string oLogExist = string.Empty;
            string oTransId = string.Empty;
            string oCardCode = string.Empty;
            string oCardName = string.Empty;
            string oDocEntry = string.Empty;
            string oDescription = string.Empty;
            string oItemCode = string.Empty;
            string oDocType = string.Empty;

            SBOGetRecord clsSBOGetRecord = new SBOGetRecord();
            try
            {
                foreach (var iRowInv in listInvoice)
                {
                    try
                    {
                        oId = iRowInv.id;
                        oStatus = iRowInv.status;

                        if (iRowInv.status == 1)
                        {
                            oTransId = (String)clsSBOGetRecord.GetSingleValue("select \"U_TransId\" from \"ODRF\" where \"U_TransId\" = '" + iRowInv.id + "' and \"CANCELED\" = 'N' and \"NumAtCard\" = '" + iRowInv.invoice_no + "' and \"ObjType\" = 13", GlobalVar.myCompany);
                            if (oTransId == "" || oTransId == "0")
                            {
                                oInvoice = (Documents)GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.oDrafts);
                                oInvoice.DocObjectCode = BoObjectTypes.oInvoices;

                                oCardCode = (String)clsSBOGetRecord.GetSingleValue("select \"CardCode\" from \"OCRD\" where \"CardCode\" = '" + TrimData(iRowInv.student) + "'", GlobalVar.myCompany);
                                if (oCardCode != "")
                                {
                                    oInvoice.CardCode = oCardCode;
                                }
                                else
                                {
                                    lastMessage = "Customer Code:" + iRowInv.student + " is not found in SAP B1";
                                    GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowInv.status == 1, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(GlobalVar.lastMessage) + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Invoice' and \"uniqueId\" = '" + iRowInv.id + "'");

                                    functionReturnValue = true;

                                    goto isAddWithError;
                                }

                                oInvoice.DocDate = Convert.ToDateTime(iRowInv.date_created);
                                oInvoice.NumAtCard = iRowInv.invoice_no;
                                oInvoice.DocDueDate = Convert.ToDateTime(iRowInv.date_due);

                                if (iRowInv.status == 1)
                                    oInvoice.Comments = iRowInv.remarks;
                                else
                                    oInvoice.Comments = iRowInv.void_remarks;

                                ////**** UDF *****\\\\
                                if (iRowInv.id != 0)
                                    oInvoice.UserFields.Fields.Item("U_TransId").Value = iRowInv.id.ToString();

                                if (iRowInv.level != "")
                                    oInvoice.UserFields.Fields.Item("U_Level").Value = iRowInv.level;

                                if (iRowInv.program_type != "")
                                    oInvoice.UserFields.Fields.Item("U_ProgramType").Value = iRowInv.program_type;
                                ////**** UDF *****\\\\

                                foreach (var iRowInvDtls in iRowInv.items.ToList())
                                {
                                    if (iRowInvDtls.item_code == "" || string.IsNullOrEmpty(iRowInvDtls.item_code))
                                    {
                                        oDocType = "dDocument_Service";
                                        string iReplaceDesc = " (" + TrimData(iRowInv.level) + " - " + TrimData(iRowInv.program_type) + ")";
                                        //oDescription = SBOstrManipulation.BeforeCharacter(iRowInvDtls.description, " (");
                                        oDescription = iRowInvDtls.description.Replace(iReplaceDesc, "");
                                        if (oDescription != "")
                                        {
                                            string description = oDescription;
                                            string iDescription = (String)clsSBOGetRecord.GetSingleValue("select \"U_Description\" from \"@GLACCTMAPPING\" where \"U_Description\" = '" + TrimData(description) + "' and \"U_Level\" = '" + TrimData(iRowInv.level) + "' and \"U_ProgramType\" = '" + TrimData(iRowInv.program_type) + "'", GlobalVar.myCompany);
                                            if (iDescription != "")
                                            {
                                                string idate_created = string.Empty;
                                                string idate_for = string.Empty;
                                                string iGLAccount = string.Empty;
                                                string oDateFor = string.Empty;

                                                if (!string.IsNullOrEmpty(iRowInvDtls.date_for))
                                                {
                                                    idate_for = iRowInvDtls.date_for;
                                                    oDateFor = Convert.ToDateTime(idate_for).ToString("MMM") + " " + Convert.ToDateTime(idate_for).Year.ToString();
                                                }
                                                else
                                                {
                                                    idate_for = iRowInv.date_created;
                                                    oDateFor = Convert.ToDateTime(idate_for).ToString("MMM") + " " + Convert.ToDateTime(idate_for).Year.ToString();
                                                }

                                                oCardName = (String)clsSBOGetRecord.GetSingleValue("select \"CardName\" from \"OCRD\" where \"CardCode\" = '" + TrimData(iRowInv.student) + "'", GlobalVar.myCompany);

                                                string oTaxCode = (String)clsSBOGetRecord.GetSingleValue("select \"U_TaxCode\" from \"@GLACCTMAPPING\" where \"U_Description\" = '" + TrimData(description) + "' and \"U_Level\" = '" + TrimData(iRowInv.level) + "' and \"U_ProgramType\" = '" + TrimData(iRowInv.program_type) + "'", GlobalVar.myCompany);

                                                if (!string.IsNullOrEmpty(oTaxCode))
                                                    oInvoice.Lines.VatGroup = oTaxCode;

                                                oItemDescription = oCardName + " - " + oDateFor + " - " + iRowInvDtls.description;
                                                oInvoice.Lines.UserFields.Fields.Item("U_Dscription").Value = oItemDescription;

                                                string Dscription = string.Empty;
                                                if (oItemDescription.Length > 100)
                                                {
                                                    Dscription = oItemDescription.Substring(0, 100);
                                                    oInvoice.Lines.ItemDescription = Dscription;
                                                }
                                                else
                                                {
                                                    oInvoice.Lines.ItemDescription = oItemDescription;
                                                }

                                                oInvoice.Lines.LineTotal = iRowInvDtls.unit_price;

                                                if (!string.IsNullOrEmpty(iRowInv.date_created))
                                                    idate_created = iRowInv.date_created;

                                                if (string.IsNullOrEmpty(Helpers.GlobalVar.oCountry) || string.IsNullOrEmpty(Helpers.GlobalVar.oGroup) || string.IsNullOrEmpty(Helpers.GlobalVar.oDivision) || string.IsNullOrEmpty(Helpers.GlobalVar.oProduct))
                                                {
                                                    lastMessage = "Cost Center is not defined in SAP B1. Please define in the integration setup.";
                                                    string oQuery = "update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowInv.status == 1, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Invoice' and \"uniqueId\" = '" + iRowInv.id + "'";
                                                    GlobalVar.oRS.DoQuery(oQuery);

                                                    functionReturnValue = true;

                                                    goto isAddWithError;
                                                }

                                                if (!string.IsNullOrEmpty(Helpers.GlobalVar.oCountry))
                                                    oInvoice.Lines.CostingCode = Helpers.GlobalVar.oCountry;

                                                if (!string.IsNullOrEmpty(Helpers.GlobalVar.oGroup))
                                                    oInvoice.Lines.CostingCode2 = Helpers.GlobalVar.oGroup;

                                                if (!string.IsNullOrEmpty(Helpers.GlobalVar.oDivision))
                                                    oInvoice.Lines.CostingCode3 = Helpers.GlobalVar.oDivision;

                                                if (!string.IsNullOrEmpty(Helpers.GlobalVar.oProduct))
                                                    oInvoice.Lines.CostingCode4 = Helpers.GlobalVar.oProduct;

                                                if (!string.IsNullOrEmpty(idate_for))
                                                    oInvoice.Lines.UserFields.Fields.Item("U_date_for").Value = Convert.ToDateTime(idate_for);

                                                if (CheckDate(idate_created) == true && CheckDate(idate_for) == true)
                                                {
                                                    if (Convert.ToDateTime(idate_for) > Convert.ToDateTime(idate_created))
                                                    {
                                                        iGLAccount = (String)clsSBOGetRecord.GetSingleValue("select \"U_FuturePeriod\" from \"@GLACCTMAPPING\" where \"U_Description\" = '" + TrimData(description) + "' and \"U_Level\" = '" + TrimData(iRowInv.level) + "' and \"U_ProgramType\" = '" + TrimData(iRowInv.program_type) + "'", GlobalVar.myCompany);
                                                    }
                                                    else
                                                    {
                                                        iGLAccount = (String)clsSBOGetRecord.GetSingleValue("select \"U_CurrentPeriod\" from \"@GLACCTMAPPING\" where \"U_Description\" = '" + TrimData(description) + "' and \"U_Level\" = '" + TrimData(iRowInv.level) + "' and \"U_ProgramType\" = '" + TrimData(iRowInv.program_type) + "'", GlobalVar.myCompany);
                                                    }
                                                }

                                                if (!string.IsNullOrEmpty(iGLAccount))
                                                    oInvoice.Lines.AccountCode = iGLAccount;

                                                oInvoice.Lines.Add();
                                            }
                                            else
                                            {
                                                lastMessage = "Description:" + iRowInvDtls.description + ", Level: " + iRowInv.level + " or Program type:" + iRowInv.program_type + " is not defined in SAP B1. Please define in the table.";
                                                string oQuery = "update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowInv.status == 1, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Invoice' and \"uniqueId\" = '" + iRowInv.id + "'";
                                                GlobalVar.oRS.DoQuery(oQuery);

                                                functionReturnValue = true;

                                                goto isAddWithError;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        oDocType = "dDocument_Items";
                                        oItemCode = string.Empty;
                                        oItemCode = (String)clsSBOGetRecord.GetSingleValue("select \"ItemCode\" from \"OITM\" where \"ItemCode\" = '" + TrimData(iRowInvDtls.item_code) + "'", GlobalVar.myCompany);
                                        if (oItemCode != "" || !string.IsNullOrEmpty(oItemCode))
                                        {
                                            oInvoice.Lines.ItemCode = iRowInvDtls.item_code;

                                            oInvoice.Lines.FreeText = iRowInvDtls.description;

                                            if (iRowInvDtls.quantity != 0)
                                                oInvoice.Lines.Quantity = iRowInvDtls.quantity;

                                            if (iRowInvDtls.unit_price != 0)
                                                oInvoice.Lines.UnitPrice = iRowInvDtls.unit_price;

                                            //Added August 7, 2019 becuase of the discount scenario
                                            oInvoice.Lines.LineTotal = iRowInvDtls.total;

                                            if (!string.IsNullOrEmpty(Helpers.GlobalVar.oCountry))
                                                oInvoice.Lines.CostingCode = Helpers.GlobalVar.oCountry;

                                            if (!string.IsNullOrEmpty(Helpers.GlobalVar.oGroup))
                                                oInvoice.Lines.CostingCode2 = Helpers.GlobalVar.oGroup;

                                            if (!string.IsNullOrEmpty(Helpers.GlobalVar.oDivision))
                                                oInvoice.Lines.CostingCode3 = Helpers.GlobalVar.oDivision;

                                            if (!string.IsNullOrEmpty(Helpers.GlobalVar.oProduct))
                                                oInvoice.Lines.CostingCode4 = Helpers.GlobalVar.oProduct;

                                            oInvoice.Lines.Add();
                                        }
                                        else
                                        {
                                            lastMessage = "ItemCode: " + iRowInvDtls.item_code + " does not exist in SAP B1.";
                                            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowInv.status == 1, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Invoice' and \"uniqueId\" = '" + iRowInv.id + "'");

                                            functionReturnValue = true;

                                            goto isAddWithError;
                                        }
                                    }
                                }

                                if (oDocType == "dDocument_Items")
                                    oInvoice.DocType = BoDocumentTypes.dDocument_Items;
                                else
                                    oInvoice.DocType = BoDocumentTypes.dDocument_Service;

                                lErrCode = oInvoice.Add();
                                if (lErrCode == 0)
                                {
                                    try
                                    {
                                        oDocEntry = GlobalVar.myCompany.GetNewObjectKey();
                                        lastMessage = "Successfully created Invoice (Draft) with Transaction Id:" + iRowInv.id + " in SAP B1.";
                                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Draft',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "',\"objType\" = 112 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Invoice' and \"uniqueId\" = '" + iRowInv.id + "'");

                                        functionReturnValue = false;
                                    }
                                    catch
                                    { }
                                }
                                else
                                {
                                    lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                    GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowInv.status == 1, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.myCompany.CompanyDB) + "' and \"module\" = 'Invoice' and \"uniqueId\" = '" + iRowInv.id + "'");

                                    functionReturnValue = true;

                                    goto isAddWithError;
                                }

                            isAddWithError: ;

                                System.Runtime.InteropServices.Marshal.ReleaseComObject(oInvoice);
                            }
                            else
                            {
                                oDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ODRF\" where \"U_TransId\" = '" + iRowInv.id + "' and \"CANCELED\" = 'N' and \"NumAtCard\" = '" + iRowInv.invoice_no + "' and \"ObjType\" = 13", GlobalVar.myCompany);

                                lastMessage = "Invoice with Transaction Id:" + iRowInv.id + " is already existing in SAP B1 Draft.";

                                GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Draft',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "',\"objType\" = 112 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Invoice' and \"uniqueId\" = '" + iRowInv.id + "'");

                                functionReturnValue = true;
                            }
                        }
                        else
                        {
                            oDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"OINV\" where \"U_TransId\" = '" + iRowInv.id + "' and \"CANCELED\" = 'N' and \"NumAtCard\" = '" + iRowInv.invoice_no + "'", GlobalVar.myCompany);
                            if (oDocEntry != "" && oDocEntry != "0")
                            {
                                oInvoice = (Documents)GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.oInvoices);
                                if (oInvoice.GetByKey(Convert.ToInt16(oDocEntry)) == true)
                                {
                                    oCreditNote = (Documents)GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.oDrafts);
                                    oCreditNote.DocObjectCode = BoObjectTypes.oCreditNotes;

                                    oCreditNote.CardCode = oInvoice.CardCode;
                                    oCreditNote.NumAtCard = oInvoice.NumAtCard;
                                    oCreditNote.DocDate = oInvoice.DocDate;
                                    oCreditNote.Comments = iRowInv.void_remarks;

                                    if (oInvoice.DocType == BoDocumentTypes.dDocument_Items)
                                    {
                                        oCreditNote.DocType = BoDocumentTypes.dDocument_Items;
                                    }
                                    else
                                    {
                                        oCreditNote.DocType = BoDocumentTypes.dDocument_Service;
                                    }

                                    ////**** UDF ****\\\\
                                    oCreditNote.UserFields.Fields.Item("U_TransId").Value = iRowInv.id.ToString();
                                    oCreditNote.UserFields.Fields.Item("U_Level").Value = oInvoice.UserFields.Fields.Item("U_Level").Value;
                                    oCreditNote.UserFields.Fields.Item("U_ProgramType").Value = oInvoice.UserFields.Fields.Item("U_ProgramType").Value;
                                    ////**** UDF ****\\\\

                                    for (int i = 0; i < oInvoice.Lines.Count; i++)
                                    {
                                        oCreditNote.Lines.BaseEntry = Convert.ToInt16(oDocEntry);
                                        oCreditNote.Lines.BaseType = (int)SAPbobsCOM.BoObjectTypes.oInvoices;
                                        oCreditNote.Lines.BaseLine = i;

                                        if (i < oInvoice.Lines.Count - 1)
                                        {
                                            oCreditNote.Lines.Add();
                                        }
                                    }

                                    lErrCode = oCreditNote.Add();
                                    if (lErrCode == 0)
                                    {
                                        try
                                        {
                                            oDocEntry = GlobalVar.myCompany.GetNewObjectKey();
                                            lastMessage = "Successfully created Credit Note (Draft) to void Invoice with Transaction Id: " + iRowInv.id + " in SAP B1. Subject for manual posting the Draft to cancel the Invoice.";
                                            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Draft',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Invoice' and \"uniqueId\" = '" + iRowInv.id + "'");

                                            functionReturnValue = false;
                                        }
                                        catch
                                        { }
                                    }
                                    else
                                    {
                                        lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowInv.status == 1, "Draft", "Void") + "',\"statusCode\" = 'Draft',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Invoice' and \"uniqueId\" = '" + iRowInv.id + "'");

                                        functionReturnValue = true;
                                    }
                                }
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(oInvoice);
                            }
                            else
                            {
                                Int16 iDocEntry = CreateInvoiceVoid(listInvoice);
                                if (iDocEntry != 0)
                                { functionReturnValue = false; }
                                else
                                    functionReturnValue = true;

                                //Auto Posting of Draft document created
                                //if (iDocEntry != 0)
                                //{
                                //    oInvoice = (Documents)GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.oInvoices);
                                //    if (oInvoice.GetByKey(iDocEntry) == true)
                                //    {
                                //        SAPbobsCOM.Documents oCancelDocs = oInvoice.CreateCancellationDocument();
                                //        lErrCode = oCancelDocs.Add();
                                //        if (lErrCode == 0)
                                //        {
                                //            try
                                //            {
                                //                lastMessage = "Successfully created Credit Note (Draft) with Transaction Id: " + iRowInv.id + " in SAP B1.";
                                //                GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Posted',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + iDocEntry + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Invoice' and \"uniqueId\" = '" + iRowInv.id + "'");

                                //                functionReturnValue = false;
                                //            }
                                //            catch
                                //            { }
                                //        }
                                //        else
                                //        {
                                //            lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                //            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowInv.status == 1, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Invoice' and \"uniqueId\" = '" + iRowInv.id + "'");

                                //            functionReturnValue = true;
                                //        }
                                //    }                                
                                //}
                                //else
                                //    functionReturnValue = true;

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lastMessage = ex.ToString();
                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowInv.status == 1, "Draft", "Void") + "',\"statusCode\" = 'Draft',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "'  and \"module\" = 'Invoice' and \"uniqueId\" = '" + iRowInv.id + "'");
                        functionReturnValue = true;
                    }
                }
            }
            catch (Exception ex)
            {
                lastMessage = ex.ToString();
                GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'false',\"statusCode\" = '" + Iif(oStatus == 1, "Draft", "Void") + "',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Invoice' and \"uniqueId\" = '" + oId + "'");
                functionReturnValue = true;
            }

            return functionReturnValue;
        }

        public static bool SBOPostInvoice(List<Models.API_Invoice> listInvoice)
        {
            bool functionReturnValue = false;
            int lErrCode = 0;
            int oId = 0;
            int oStatus = 0;
            string oLogExist = string.Empty;
            string oTransId = string.Empty;
            string oCardCode = string.Empty;
            string oCardName = string.Empty;
            string oDocEntry = string.Empty;
            string oDescription = string.Empty;
            string oItemCode = string.Empty;
            string oDocType = string.Empty;
            SBOGetRecord clsSBOGetRecord = new SBOGetRecord();
            try
            {
                foreach (var iRowInv in listInvoice)
                {
                    try
                    {
                        oId = iRowInv.id;
                        oStatus = iRowInv.status;

                        if (iRowInv.status == 1)
                        {
                            oTransId = (String)clsSBOGetRecord.GetSingleValue("select \"U_TransId\" from \"OINV\" where \"U_TransId\" = '" + iRowInv.id + "' and \"CANCELED\" = 'N' and \"NumAtCard\" = '" + iRowInv.invoice_no + "'", GlobalVar.myCompany);
                            if (oTransId == "" || oTransId == "0")
                            {
                                SAPbobsCOM.Documents oDraft = (Documents)GlobalVar.myCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                                oDraft.DocObjectCode = BoObjectTypes.oInvoices;
                                string oDraftDocEntry = string.Empty;
                                oDraftDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ODRF\" where \"U_TransId\" = '" + iRowInv.id + "' and \"CANCELED\" = 'N' and \"NumAtCard\" = '" + iRowInv.invoice_no + "' and \"ObjType\" = 13", GlobalVar.myCompany);
                                if (oDraftDocEntry != "")
                                {

                                    if (oDraft.GetByKey(Convert.ToInt16(oDraftDocEntry)))
                                    {
                                        lErrCode = oDraft.SaveDraftToDocument();
                                        if (lErrCode == 0)
                                        {
                                            oDocEntry = GlobalVar.myCompany.GetNewObjectKey();
                                            lastMessage = "Successfully created Invoice with Transaction Id:" + iRowInv.id + " in SAP B1.";
                                            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Posted',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "',\"objType\" = 13 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Invoice' and \"uniqueId\" = '" + iRowInv.id + "'");

                                            functionReturnValue = false;
                                        }
                                        else
                                        {
                                            lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'false',\"statusCode\" = 'Draft',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Invoice' and \"uniqueId\" = '" + iRowInv.id + "'");

                                            functionReturnValue = true;
                                        }
                                    }
                                }
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(oDraft);
                            }
                            else
                            {
                                oDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"OINV\" where \"U_TransId\" = '" + iRowInv.id + "' and \"CANCELED\" = 'N' and \"NumAtCard\" = '" + iRowInv.invoice_no + "'", GlobalVar.myCompany);
                                lastMessage = "Invoice with Transaction Id:" + iRowInv.id + " already exist in SAP B1.";

                                GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Posted',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "',\"objType\" = 13 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Invoice' and \"uniqueId\" = '" + iRowInv.id + "'");
                                functionReturnValue = true;
                            }
                        }
                        else
                        {
                            SAPbobsCOM.Documents oDraft = (Documents)GlobalVar.myCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                            oDraft.DocObjectCode = BoObjectTypes.oCreditNotes;
                            string oDraftDocEntry = string.Empty;
                            oDraftDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ODRF\" where \"U_TransId\" = '" + iRowInv.id + "' and \"CANCELED\" = 'N' and \"NumAtCard\" = '" + iRowInv.invoice_no + "' and \"ObjType\" = 14", GlobalVar.myCompany);
                            if (oDraftDocEntry != "")
                            {
                                if (oDraft.GetByKey(Convert.ToInt16(oDraftDocEntry)))
                                {
                                    lErrCode = oDraft.SaveDraftToDocument();
                                    if (lErrCode == 0)
                                    {
                                        oDocEntry = GlobalVar.myCompany.GetNewObjectKey();
                                        lastMessage = "Successfully created Credit Note with Transaction Id:" + iRowInv.id + " base in Invoice (Void) with Transaction Id:" + iRowInv.id + " in SAP B1.";
                                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Posted',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "',\"objType\" = 14 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Invoice' and \"uniqueId\" = '" + iRowInv.id + "'");

                                        functionReturnValue = false;
                                    }
                                    else
                                    {
                                        lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'false',\"statusCode\" = 'Draft',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Invoice' and \"uniqueId\" = '" + iRowInv.id + "'");

                                        functionReturnValue = true;
                                    }
                                }
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(oDraft);
                            }
                            else
                            {
                                lastMessage = "Credit Note with Transaction Id:" + iRowInv.id + " base in Invoice (Void) with Transaction Id:" + iRowInv.id + " does not exist in SAP B1.";
                                GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'false',\"statusCode\" = 'Draft',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Invoice' and \"uniqueId\" = '" + iRowInv.id + "'");

                                functionReturnValue = true;

                            }
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(oDraft);
                        }
                    }
                    catch (Exception ex)
                    {
                        lastMessage = ex.ToString();
                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowInv.status == 1, "Draft", "Void") + "',\"statusCode\" = 'Draft',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "'  and \"module\" = 'Invoice' and \"uniqueId\" = '" + iRowInv.id + "'");
                        functionReturnValue = true;
                    }
                }
            }
            catch (Exception ex)
            {
                lastMessage = ex.ToString();
                GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'false',\"statusCode\" = '" + Iif(oStatus == 1, "Draft", "Void") + "',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Invoice' and \"uniqueId\" = '" + oId + "'");
                functionReturnValue = true;
            }

            return functionReturnValue;
        }

        public static bool SBOPostCreditNoteDraft(List<Models.API_CreditNote> listCreditNote)
        {
            bool functionReturnValue = false;
            int lErrCode = 0;
            int oId = 0;
            int oStatus = 0;
            string oLogExist = string.Empty;
            string oTransId = string.Empty;
            string oCardCode = string.Empty;
            string oCardName = string.Empty;
            string oDocEntry = string.Empty;
            string oDescription = string.Empty;
            string oDocType = string.Empty;
            SBOGetRecord clsSBOGetRecord = new SBOGetRecord();
            try
            {
                foreach (var iRowCreditNote in listCreditNote)
                {
                    try
                    {
                        oId = iRowCreditNote.id;
                        oStatus = iRowCreditNote.status;

                        if (iRowCreditNote.status == 1)
                        {
                            oTransId = (String)clsSBOGetRecord.GetSingleValue("select \"U_TransId\" from \"ODRF\" where \"U_TransId\" = '" + iRowCreditNote.id + "' and \"CANCELED\" = 'N' and \"NumAtCard\" = '" + iRowCreditNote.credit_no + "' and \"ObjType\" = 14 and \"U_CreatedByVoucher\" = 0", GlobalVar.myCompany);
                            if (oTransId == "" || oTransId == "0")
                            {
                                oCreditNote = (Documents)GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.oDrafts);
                                oCreditNote.DocObjectCode = BoObjectTypes.oCreditNotes;

                                oCardCode = (String)clsSBOGetRecord.GetSingleValue("select \"CardCode\" from \"OCRD\" where \"CardCode\" = '" + iRowCreditNote.student + "'", GlobalVar.myCompany);
                                if (oCardCode != "")
                                {
                                    oCreditNote.CardCode = oCardCode;
                                }
                                else
                                {
                                    lastMessage = "Customer Code:" + iRowCreditNote.student + " is not found in SAP B1";
                                    GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditNote.status == 1, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + GlobalVar.oSERVERDB + "' and \"module\" = 'Credit Note' and \"uniqueId\" = '" + iRowCreditNote.id + "'");

                                    functionReturnValue = true;

                                    goto isAddWithError;
                                }

                                oCreditNote.DocDate = Convert.ToDateTime(iRowCreditNote.date_created);
                                oCreditNote.NumAtCard = iRowCreditNote.credit_no;

                                if (iRowCreditNote.status == 1)
                                    oCreditNote.Comments = iRowCreditNote.remarks;
                                else
                                    oCreditNote.Comments = iRowCreditNote.void_remarks;

                                ////**** UDF ****\\\\
                                if (iRowCreditNote.id != 0)
                                    oCreditNote.UserFields.Fields.Item("U_TransId").Value = iRowCreditNote.id.ToString();

                                if (iRowCreditNote.credit_type != -1)
                                    oCreditNote.UserFields.Fields.Item("U_CreditType").Value = iRowCreditNote.credit_type;

                                if (iRowCreditNote.type != -1)
                                    oCreditNote.UserFields.Fields.Item("U_Type").Value = iRowCreditNote.type;

                                if (iRowCreditNote.program_type != "")
                                    oCreditNote.UserFields.Fields.Item("U_ProgramType").Value = iRowCreditNote.program_type;

                                if (iRowCreditNote.level != "")
                                    oCreditNote.UserFields.Fields.Item("U_Level").Value = iRowCreditNote.level;
                                ////**** UDF ****\\\\

                                foreach (var iRowCreditNoteDtls in iRowCreditNote.items.ToList())
                                {
                                    if (iRowCreditNoteDtls.description != "")
                                    {
                                        string iReplaceDesc = " (" + TrimData(iRowCreditNote.level) + " - " + TrimData(iRowCreditNote.program_type) + ")";
                                        //oDescription = SBOstrManipulation.BeforeCharacter(iRowCreditNoteDtls.description, " (");
                                        oDescription = iRowCreditNoteDtls.description.Replace(iReplaceDesc, "");
                                        if (oDescription != "")
                                        {
                                            oDocType = "dDocument_Service";

                                            string description = oDescription;
                                            string iDescription = (String)clsSBOGetRecord.GetSingleValue("select \"U_Description\" from \"@GLACCTMAPPING\" where \"U_Description\" = '" + description + "' and \"U_Level\" = '" + iRowCreditNote.level + "' and \"U_ProgramType\" = '" + iRowCreditNote.program_type + "'", GlobalVar.myCompany);
                                            if (iDescription != "")
                                            {
                                                string idate_created = string.Empty;
                                                string idate_for = string.Empty;
                                                string iGLAccount = string.Empty;
                                                string oDateFor = string.Empty;

                                                if (string.IsNullOrEmpty(iRowCreditNoteDtls.date_for))
                                                {
                                                    idate_for = iRowCreditNote.date_created;
                                                    oDateFor = Convert.ToDateTime(idate_for).ToString("MMM") + " " + Convert.ToDateTime(idate_for).Year.ToString();
                                                }
                                                else
                                                {
                                                    idate_for = iRowCreditNoteDtls.date_for;
                                                    oDateFor = Convert.ToDateTime(idate_for).ToString("MMM") + " " + Convert.ToDateTime(idate_for).Year.ToString();
                                                }

                                                oCardName = (String)clsSBOGetRecord.GetSingleValue("select \"CardName\" from \"OCRD\" where \"CardCode\" = '" + TrimData(iRowCreditNote.student) + "'", GlobalVar.myCompany);

                                                string oTaxCode = (String)clsSBOGetRecord.GetSingleValue("select \"U_TaxCode\" from \"@GLACCTMAPPING\" where \"U_Description\" = '" + TrimData(description) + "' and \"U_Level\" = '" + TrimData(iRowCreditNote.level) + "' and \"U_ProgramType\" = '" + TrimData(iRowCreditNote.program_type) + "'", GlobalVar.myCompany);

                                                if (!string.IsNullOrEmpty(oTaxCode))
                                                    oCreditNote.Lines.VatGroup = oTaxCode;

                                                oItemDescription = oCardName + " - " + oDateFor + " - " + iRowCreditNoteDtls.description;

                                                oCreditNote.Lines.UserFields.Fields.Item("U_Dscription").Value = oItemDescription;

                                                string Dscription = string.Empty;
                                                if (oItemDescription.Length > 100)
                                                {
                                                    Dscription = oItemDescription.Substring(0, 100);
                                                    oCreditNote.Lines.ItemDescription = Dscription;

                                                }
                                                else
                                                {
                                                    oCreditNote.Lines.ItemDescription = oItemDescription;
                                                }

                                                oCreditNote.Lines.LineTotal = iRowCreditNoteDtls.amount;

                                                if (!string.IsNullOrEmpty(iRowCreditNote.date_created))
                                                    idate_created = iRowCreditNote.date_created;

                                                if (string.IsNullOrEmpty(Helpers.GlobalVar.oCountry) || string.IsNullOrEmpty(Helpers.GlobalVar.oGroup) || string.IsNullOrEmpty(Helpers.GlobalVar.oDivision) || string.IsNullOrEmpty(Helpers.GlobalVar.oProduct))
                                                {
                                                    lastMessage = "Cost Center is not defined in SAP B1. Please define in the integration setup.";
                                                    string oQuery = "update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditNote.status == 1, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Note' and \"uniqueId\" = '" + iRowCreditNote.id + "'";
                                                    GlobalVar.oRS.DoQuery(oQuery);

                                                    functionReturnValue = true;

                                                    goto isAddWithError;
                                                }

                                                if (!string.IsNullOrEmpty(Helpers.GlobalVar.oCountry))
                                                    oCreditNote.Lines.CostingCode = Helpers.GlobalVar.oCountry;

                                                if (!string.IsNullOrEmpty(Helpers.GlobalVar.oGroup))
                                                    oCreditNote.Lines.CostingCode2 = Helpers.GlobalVar.oGroup;

                                                if (!string.IsNullOrEmpty(Helpers.GlobalVar.oDivision))
                                                    oCreditNote.Lines.CostingCode3 = Helpers.GlobalVar.oDivision;

                                                if (!string.IsNullOrEmpty(Helpers.GlobalVar.oProduct))
                                                    oCreditNote.Lines.CostingCode4 = Helpers.GlobalVar.oProduct;

                                                if (CheckDate(idate_created) == true && CheckDate(idate_for) == true)
                                                {
                                                    if (Convert.ToDateTime(idate_for) > Convert.ToDateTime(idate_created))
                                                    {
                                                        iGLAccount = (String)clsSBOGetRecord.GetSingleValue("select \"U_FuturePeriod\" from \"@GLACCTMAPPING\" where \"U_Description\" = '" + description + "' and \"U_Level\" = '" + iRowCreditNote.level + "' and \"U_ProgramType\" = '" + iRowCreditNote.program_type + "'", GlobalVar.myCompany);
                                                    }
                                                    else
                                                    {
                                                        iGLAccount = (String)clsSBOGetRecord.GetSingleValue("select \"U_CurrentPeriod\" from \"@GLACCTMAPPING\" where \"U_Description\" = '" + description + "' and \"U_Level\" = '" + iRowCreditNote.level + "' and \"U_ProgramType\" = '" + iRowCreditNote.program_type + "'", GlobalVar.myCompany);
                                                    }
                                                }

                                                if (!string.IsNullOrEmpty(iGLAccount))
                                                    oCreditNote.Lines.AccountCode = iGLAccount;

                                                oCreditNote.Lines.Add();
                                            }
                                            else
                                            {
                                                lastMessage = "Description:" + iRowCreditNoteDtls.description + ", Level: " + iRowCreditNote.level + " or Program type:" + iRowCreditNote.program_type + " is not defined in SAP B1. Please define in the table.";
                                                GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditNote.status == 1, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + lastMessage + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + GlobalVar.oSERVERDB + "' and \"module\" = 'Credit Note' and \"uniqueId\" = '" + iRowCreditNote.id + "'");

                                                functionReturnValue = true;

                                                goto isAddWithError;
                                            }
                                        }
                                    }
                                }

                                if (oDocType == "dDocument_Items")
                                    oCreditNote.DocType = BoDocumentTypes.dDocument_Items;
                                else
                                    oCreditNote.DocType = BoDocumentTypes.dDocument_Service;

                                lErrCode = oCreditNote.Add();
                                if (lErrCode == 0)
                                {
                                    try
                                    {
                                        oDocEntry = GlobalVar.myCompany.GetNewObjectKey();
                                        lastMessage = "Successfully created Credit Note (Draft) with Transaction Id:" + iRowCreditNote.id + " in SAP B1.";
                                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Draft',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "',\"objType\" = 112 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Note' and \"uniqueId\" = '" + iRowCreditNote.id + "'");

                                        functionReturnValue = false;
                                    }
                                    catch
                                    { }
                                }
                                else
                                {
                                    lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                    GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditNote.status == 1, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + lastMessage + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Note' and \"uniqueId\" = '" + iRowCreditNote.id + "'");

                                    functionReturnValue = true;

                                    goto isAddWithError;
                                }

                            isAddWithError: ;

                                System.Runtime.InteropServices.Marshal.ReleaseComObject(oCreditNote);

                            }
                            else
                            {
                                oDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ODRF\" where \"U_TransId\" = '" + iRowCreditNote.id + "' and \"CANCELED\" = 'N' and \"NumAtCard\" = '" + iRowCreditNote.credit_no + "' and \"ObjType\" = 14 and \"U_CreatedByVoucher\" = 0", GlobalVar.myCompany);

                                lastMessage = "Credit Note with Transaction Id:" + iRowCreditNote.id + " is already existing in SAP B1 Draft.";

                                GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Draft',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "',\"objType\" = 112 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Note' and \"uniqueId\" = '" + iRowCreditNote.id + "'");

                                functionReturnValue = true;
                            }
                        }
                        else
                        {
                            oDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ORIN\" where \"U_TransId\" = '" + iRowCreditNote.id + "' and \"CANCELED\" = 'N' and \"NumAtCard\" = '" + iRowCreditNote.credit_no + "'", GlobalVar.myCompany);
                            if (oDocEntry != "" && oDocEntry != "0")
                            {
                                oCreditNote = (Documents)GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.oCreditNotes);
                                if (oCreditNote.GetByKey(Convert.ToInt16(oDocEntry)) == true)
                                {
                                    SAPbobsCOM.Documents oCancelDocs = oCreditNote.CreateCancellationDocument();
                                    lErrCode = oCancelDocs.Add();
                                    if (lErrCode == 0)
                                    {
                                        try
                                        {
                                            lastMessage = "Successfully canceled Credit Note with Transaction Id:" + iRowCreditNote.id + " in SAP B1.";
                                            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Posted',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "',\"objType\" = 14 where \"companyDB\" = '" + GlobalVar.oSERVERDB + "' and \"module\" = 'Credit Note' and \"uniqueId\" = '" + iRowCreditNote.id + "'");

                                            functionReturnValue = false;
                                        }
                                        catch
                                        { }
                                    }
                                    else
                                    {
                                        lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditNote.status == 1, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Note' and \"uniqueId\" = '" + iRowCreditNote.id + "'");

                                        functionReturnValue = true;
                                    }
                                }
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(oCreditNote);
                            }
                            else
                            {
                                Int16 iDocEntry = CreateCreditNoteVoid(listCreditNote);
                                if (iDocEntry != 0)
                                {
                                    oCreditNote = (Documents)GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.oCreditNotes);
                                    if (oCreditNote.GetByKey(iDocEntry) == true)
                                    {
                                        SAPbobsCOM.Documents oCancelDocs = oCreditNote.CreateCancellationDocument();
                                        lErrCode = oCancelDocs.Add();
                                        if (lErrCode == 0)
                                        {
                                            try
                                            {
                                                lastMessage = "Successfully canceled Credit Note with Transaction Id: " + iRowCreditNote.id + " in SAP B1.";
                                                GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Posted',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + iDocEntry + "',\"objType\" = 14 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Note' and \"uniqueId\" = '" + iRowCreditNote.id + "'");

                                                functionReturnValue = false;
                                            }
                                            catch
                                            { }
                                        }
                                        else
                                        {
                                            lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditNote.status == 1, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Note' and \"uniqueId\" = '" + iRowCreditNote.id + "'");

                                            functionReturnValue = true;
                                        }
                                    }
                                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oCreditNote);
                                }
                                else
                                    functionReturnValue = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lastMessage = ex.ToString();
                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditNote.status == 1, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + lastMessage + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + GlobalVar.oSERVERDB + "' and \"module\" = 'Credit Note' and \"uniqueId\" = '" + iRowCreditNote.id + "'");

                        functionReturnValue = true;
                    }
                }
            }
            catch (Exception ex)
            {
                lastMessage = ex.ToString();
                GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(oStatus == 1, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + lastMessage + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + GlobalVar.oSERVERDB + "' and \"module\" = 'Credit Note' and \"uniqueId\" = '" + oId + "'");

                functionReturnValue = true;
            }

            return functionReturnValue;
        }

        public static bool SBOPostCreditNote(List<Models.API_CreditNote> listCreditNote)
        {
            bool functionReturnValue = false;
            int lErrCode = 0;
            int oId = 0;
            int oStatus = 0;
            string oLogExist = string.Empty;
            string oTransId = string.Empty;
            string oCardCode = string.Empty;
            string oCardName = string.Empty;
            string oDocEntry = string.Empty;
            string oDescription = string.Empty;
            string oDocType = string.Empty;
            SBOGetRecord clsSBOGetRecord = new SBOGetRecord();
            try
            {
                foreach (var iRowCreditNote in listCreditNote)
                {
                    try
                    {
                        oId = iRowCreditNote.id;
                        oStatus = iRowCreditNote.status;

                        if (iRowCreditNote.status == 1)
                        {
                            oTransId = (String)clsSBOGetRecord.GetSingleValue("select \"U_TransId\" from \"ORIN\" where \"U_TransId\" = '" + iRowCreditNote.id + "' and \"CANCELED\" = 'N' and \"NumAtCard\" = '" + iRowCreditNote.credit_no + "'", GlobalVar.myCompany);
                            if (oTransId == "" || oTransId == "0")
                            {
                                SAPbobsCOM.Documents oDraft = (Documents)GlobalVar.myCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                                oDraft.DocObjectCode = BoObjectTypes.oCreditNotes;
                                string oDraftDocEntry = string.Empty;
                                oDraftDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ODRF\" where \"U_TransId\" = '" + iRowCreditNote.id + "' and \"CANCELED\" = 'N'and \"NumAtCard\" = '" + iRowCreditNote.credit_no + "' and \"ObjType\" = 14", GlobalVar.myCompany);
                                if (oDraftDocEntry != "")
                                {
                                    if (oDraft.GetByKey(Convert.ToInt16(oDraftDocEntry)))
                                    {
                                        lErrCode = oDraft.SaveDraftToDocument();
                                        if (lErrCode == 0)
                                        {
                                            oDocEntry = GlobalVar.myCompany.GetNewObjectKey();
                                            lastMessage = "Successfully created Credit Note with Transaction Id:" + iRowCreditNote.id + " in SAP B1.";
                                            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Posted',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "',\"objType\" = 14 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Note' and \"uniqueId\" = '" + iRowCreditNote.id + "'");

                                            functionReturnValue = false;
                                        }
                                        else
                                        {
                                            lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'false',\"statusCode\" = 'Draft',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Note' and \"uniqueId\" = '" + iRowCreditNote.id + "'");

                                            functionReturnValue = true;
                                        }
                                    }
                                }
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(oDraft);
                            }
                            else
                            {
                                oDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ORIN\" where \"U_TransId\" = '" + iRowCreditNote.id + "' and \"CANCELED\" = 'N' and \"NumAtCard\" = '" + iRowCreditNote.credit_no + "'", GlobalVar.myCompany);

                                lastMessage = "Credit Note with Transaction Id:" + iRowCreditNote.id + " already exist in SAP B1.";

                                GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Posted',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "',\"objType\" = 14 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Note' and \"uniqueId\" = '" + iRowCreditNote.id + "'");
                                functionReturnValue = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lastMessage = ex.ToString();
                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditNote.status == 1, "Draft", "Void") + "',\"statusCode\" = 'Draft',\"failDesc\" = '" + lastMessage + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + GlobalVar.oSERVERDB + "' and \"module\" = 'Credit Note' and \"uniqueId\" = '" + iRowCreditNote.id + "'");

                        functionReturnValue = true;
                    }
                }
            }
            catch (Exception ex)
            {
                lastMessage = ex.ToString();
                GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(oStatus == 1, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + lastMessage + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + GlobalVar.oSERVERDB + "' and \"module\" = 'Credit Note' and \"uniqueId\" = '" + oId + "'");

                functionReturnValue = true;
            }

            return functionReturnValue;
        }

        public static bool SBOPostReceiptDraft(List<Models.API_Receipt> listReceipt)
        {
            bool functionReturnValue = false;
            int lErrCode = 0;
            string oLogExist = string.Empty;
            string oTransId = string.Empty;
            string oCardCode = string.Empty;
            string oCardName = string.Empty;
            string oDocEntry = string.Empty;
            string oInvDocEntry = string.Empty;
            string oCreditNoteDocEntry = string.Empty;
            string oModeOfPayment = string.Empty;
            string oAcctCode = string.Empty;
            string oBankName = string.Empty;
            string oCheckBankName = string.Empty;
            string iReference = string.Empty;

            SBOGetRecord clsSBOGetRecord = new SBOGetRecord();
            try
            {
                foreach (var iRowReceipt in listReceipt)
                {
                    try
                    {
                        //0 = no offset
                        //1 = has both payment and offset
                        //2 = only offset type
                        if (iRowReceipt.payment_type == 0 || iRowReceipt.payment_type == 1)
                        {
                            oIncomingPayment = (Payments)GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.oPaymentsDrafts);

                            if (iRowReceipt.status == 0)
                            {
                                oTransId = (String)clsSBOGetRecord.GetSingleValue("select \"U_TransId\" from \"OPDF\" where \"U_TransId\" = '" + iRowReceipt.id + "' and \"ObjType\" = 24", GlobalVar.myCompany);
                                oIncomingPayment.DocObjectCode = BoPaymentsObjectType.bopot_IncomingPayments;
                                if (oTransId == "" || oTransId == "0")
                                {
                                    oCardCode = (String)clsSBOGetRecord.GetSingleValue("select \"CardCode\" from \"OCRD\" where \"CardCode\" = '" + TrimData(iRowReceipt.student) + "'", GlobalVar.myCompany);
                                    if (oCardCode != "")
                                    {
                                        oIncomingPayment.CardCode = oCardCode;

                                        oCardName = (String)clsSBOGetRecord.GetSingleValue("select \"CardName\" from \"OCRD\" where \"CardCode\" = '" + TrimData(iRowReceipt.student) + "'", GlobalVar.myCompany);
                                    }
                                    else
                                    {
                                        lastMessage = "Customer Code:" + iRowReceipt.student + " is not found in SAP B1";
                                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowReceipt.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");

                                        functionReturnValue = true;

                                        goto isAddWithError;
                                    }

                                    oIncomingPayment.DocTypte = BoRcptTypes.rCustomer;
                                    oIncomingPayment.DocDate = Convert.ToDateTime(iRowReceipt.date_created);

                                    ////**** UDF ****\\\\     
                                    oIncomingPayment.UserFields.Fields.Item("U_TransId").Value = iRowReceipt.id.ToString();
                                    oIncomingPayment.UserFields.Fields.Item("U_Status").Value = iRowReceipt.status.ToString();
                                    oIncomingPayment.UserFields.Fields.Item("U_Level").Value = iRowReceipt.level;
                                    oIncomingPayment.UserFields.Fields.Item("U_ProgramType").Value = iRowReceipt.program_type;
                                    oIncomingPayment.UserFields.Fields.Item("U_ReceiptNo").Value = iRowReceipt.receipt_no;
                                    ////**** UDF ****\\\\

                                    if (iRowReceipt.status == 0)

                                        if (iRowReceipt.remarks.Length >= 200)
                                        {
                                            oIncomingPayment.Remarks = oCardName.Substring(0, 50) + " " + iRowReceipt.remarks;
                                        }
                                        else
                                            oIncomingPayment.Remarks = oCardName + " " + iRowReceipt.remarks;
                                    else
                                    {
                                        if (iRowReceipt.void_remarks.Length >= 200)
                                        {
                                            oIncomingPayment.Remarks = oCardName.Substring(0, 50) + " " + iRowReceipt.void_remarks;
                                        }
                                        else
                                            oIncomingPayment.Remarks = oCardName + " " + iRowReceipt.void_remarks;
                                    }

                                    ////**** Adding of List of Invoice to Incoming Payment ****\\\\
                                    int invoiceCount = 0;
                                    int invPaidCount;
                                    foreach (var iRowReceiptInvDtl in iRowReceipt.invoice_id.ToList())
                                    {
                                        invoiceCount += 1;
                                        oInvDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"OINV\" " + Environment.NewLine +
                                        "where \"U_TransId\" = '" + iRowReceiptInvDtl.ToString() + "' and \"CANCELED\" = 'N'", GlobalVar.myCompany);
                                        if (oInvDocEntry != "" && oInvDocEntry != "0")
                                        {
                                            invPaidCount = 0;
                                            foreach (var iRowReceiptInvPaidDtl in iRowReceipt.invoice_paid.ToList())
                                            {
                                                invPaidCount += 1;
                                                if (invoiceCount == invPaidCount)
                                                {
                                                    oIncomingPayment.Invoices.DocEntry = Convert.ToInt16(oInvDocEntry);
                                                    oIncomingPayment.Invoices.InvoiceType = BoRcptInvTypes.it_Invoice;
                                                    oIncomingPayment.Invoices.SumApplied = Convert.ToDouble(iRowReceiptInvPaidDtl.ToString());
                                                    oIncomingPayment.Invoices.Add();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            lastMessage = "Invoice with Transaction id:" + iRowReceiptInvDtl.ToString() + " does not exist in SAP B1.";
                                            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowReceipt.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");

                                            functionReturnValue = true;

                                            goto isAddWithError;
                                        }
                                    }
                                    ////**** Adding of List of Invoice to Incoming Payment ****\\\\

                                    ////**** Adding of List of Credit Note to Incoming Payment ****\\\\
                                    iReference = string.Empty;
                                    foreach (var iRowReceiptInvDtls in iRowReceipt.payment_methods.ToList())
                                    {
                                        if (iRowReceiptInvDtls.method == 3 || iRowReceiptInvDtls.method == 8 || iRowReceiptInvDtls.method == 10) //**OFFSET_DEPOSIT = 3**\\
                                        {
                                            if (!string.IsNullOrEmpty(iRowReceiptInvDtls.reference_id) && iRowReceiptInvDtls.reference_id != "N.A")
                                            {
                                                oCreditNoteDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ORIN\" where \"U_TransId\" = '" + TrimData(iRowReceiptInvDtls.reference_id) + "' and \"CANCELED\" = 'N' and \"U_CreatedByVoucher\" = 0", GlobalVar.myCompany);
                                                if (oCreditNoteDocEntry != "" && oCreditNoteDocEntry != "0")
                                                {
                                                    oIncomingPayment.Invoices.DocEntry = Convert.ToInt16(oCreditNoteDocEntry);
                                                    oIncomingPayment.Invoices.InvoiceType = BoRcptInvTypes.it_CredItnote;
                                                    oIncomingPayment.Invoices.Add();
                                                }
                                                else
                                                {
                                                    string oDraftDocEntry = string.Empty;
                                                    oDraftDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ODRF\" where \"U_TransId\" = '" + iRowReceiptInvDtls.reference_id + "' and \"CANCELED\" = 'N' and \"ObjType\" = 14", GlobalVar.myCompany);
                                                    if (oDraftDocEntry != "" && oDraftDocEntry != "0")
                                                    {
                                                        SAPbobsCOM.Documents oDraft = (Documents)GlobalVar.myCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                                                        if (oDraft.GetByKey(Convert.ToInt16(oDraftDocEntry)))
                                                        {
                                                            int ErrCode = oDraft.SaveDraftToDocument();
                                                            if (ErrCode == 0)
                                                            {
                                                                oCreditNoteDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ORIN\" where \"U_TransId\" = '" + iRowReceiptInvDtls.reference_id + "' and \"CANCELED\" = 'N' and \"U_CreatedByVoucher\" = 0", GlobalVar.myCompany);
                                                                if (oCreditNoteDocEntry != "" && oCreditNoteDocEntry != "0")
                                                                {
                                                                    oIncomingPayment.Invoices.DocEntry = Convert.ToInt16(oCreditNoteDocEntry);
                                                                    oIncomingPayment.Invoices.InvoiceType = BoRcptInvTypes.it_CredItnote;
                                                                    oIncomingPayment.Invoices.Add();
                                                                }
                                                            }
                                                            else
                                                            {
                                                                lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                                                GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowReceipt.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");

                                                                functionReturnValue = true;

                                                                goto isAddWithError;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        lastMessage = "Credit Note with Reference Id:" + iRowReceiptInvDtls.reference_id + " does not exist in SAP B1 Drafts";
                                                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowReceipt.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");

                                                        functionReturnValue = true;

                                                        goto isAddWithError;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (iRowReceiptInvDtls.reference != "N.A")
                                                    iReference += iRowReceiptInvDtls.reference + ", ";
                                            }
                                        }
                                        else
                                        {
                                            if (iRowReceiptInvDtls.reference != "N.A")
                                                iReference += iRowReceiptInvDtls.reference + ", ";
                                        }
                                    }
                                    ////**** Adding of List of Credit Note to Incoming Payment ****\\\\

                                    string oJournalRemarks = string.Empty;
                                    if (!string.IsNullOrEmpty(iReference))
                                    {
                                        oJournalRemarks = iReference.Substring(0, iReference.Length - 2);
                                    }

                                    if (!string.IsNullOrEmpty(oJournalRemarks))
                                        oIncomingPayment.JournalRemarks = oJournalRemarks;

                                    ////**** Payment Means for the List of Invoices ****\\\\
                                    foreach (var iRowReceiptDtls in iRowReceipt.payment_methods.ToList())
                                    {
                                        if (string.IsNullOrEmpty(iRowReceiptDtls.reference_id) || iRowReceiptDtls.reference_id == "N.A")
                                        {
                                            oAcctCode = (String)clsSBOGetRecord.GetSingleValue("select \"U_GLAccount\" from \"@PAYMENTCODES\" where \"U_PaymentCodeMethod\" = " + iRowReceiptDtls.method + "", GlobalVar.myCompany);

                                            oModeOfPayment = (String)clsSBOGetRecord.GetSingleValue("select \"U_ModePayment\" from \"@PAYMENTCODES\" where \"U_PaymentCodeMethod\" = " + iRowReceiptDtls.method + "", GlobalVar.myCompany);

                                            if (oModeOfPayment == "CA")
                                            {
                                                if (!string.IsNullOrEmpty(oAcctCode))
                                                    oIncomingPayment.CashAccount = oAcctCode;

                                                if (iRowReceiptDtls.amount != 0)
                                                    oIncomingPayment.CashSum = iRowReceiptDtls.amount;
                                            }
                                            else if (oModeOfPayment == "CK")
                                            {
                                                if (!string.IsNullOrEmpty(oAcctCode))
                                                    oIncomingPayment.CheckAccount = oAcctCode;

                                                if (iRowReceiptDtls.amount != 0)
                                                    oIncomingPayment.Checks.CheckSum = iRowReceiptDtls.amount;

                                                oIncomingPayment.Checks.Add();
                                            }
                                            else if (oModeOfPayment == "BT")
                                            {
                                                oIncomingPayment.TransferReference = iRowReceiptDtls.reference;

                                                if (!string.IsNullOrEmpty(oAcctCode))
                                                    oIncomingPayment.TransferAccount = oAcctCode;

                                                if (iRowReceiptDtls.amount != 0)
                                                    oIncomingPayment.TransferSum = iRowReceiptDtls.amount;

                                            }
                                            else if (oModeOfPayment == "CC")
                                            {
                                                //string creditCardName = cls.GetSingleValue("SELECT \"CreditCard\" FROM OCRC WHERE \"CardName\" = '" + oIncomingPaymentLines.creditCardName + "'", company);
                                                //if (creditCardName != "")
                                                //{
                                                //    oIncomingPayment.CreditCards.CreditCard = Convert.ToInt16(creditCardName);
                                                //    oIncomingPayment.CreditCards.CardValidUntil = Convert.ToDateTime(oIncomingPaymentLines.creditCardValidDate);
                                                //    oIncomingPayment.CreditCards.CreditCardNumber = oIncomingPaymentLines.creditCardNumber;

                                                //    if (oIncomingPaymentLines.creditCardAmount != 0)
                                                //        oIncomingPayment.CreditCards.CreditSum = oIncomingPaymentLines.creditCardAmount;

                                                //    oIncomingPayment.CreditCards.VoucherNum = oIncomingPaymentLines.creditCardApproval;
                                                //    oIncomingPayment.CreditCards.Add();
                                                //}
                                            }
                                            else if (oModeOfPayment == "CN")
                                            {
                                                string oDocDate = string.Empty;
                                                string CNDesc = string.Empty;
                                                if (!string.IsNullOrEmpty(iRowReceiptDtls.reference_id))
                                                { }
                                                else
                                                {
                                                    string oVoucherTaxCode = (String)clsSBOGetRecord.GetSingleValue("select \"U_TaxCode\" from \"@PAYMENTCODES\" where \"U_PaymentCodeMethod\" = " + iRowReceiptDtls.method + "", GlobalVar.myCompany);

                                                    oCardName = (String)clsSBOGetRecord.GetSingleValue("select \"CardName\" from \"OCRD\" where \"CardCode\" = '" + TrimData(iRowReceipt.student) + "'", GlobalVar.myCompany);

                                                    CNDesc = oCardName + " Voucher " + Convert.ToDateTime(iRowReceipt.date_created).ToString("MMM") + " " + Convert.ToDateTime(iRowReceipt.date_created).Year + " " + iRowReceipt.level + " " + iRowReceipt.program_type;

                                                    Int16 CNDocEntry = CreateCreditNoteVoucher(oCardCode, iRowReceipt.receipt_no, iRowReceipt.date_created, CNDesc, oAcctCode, iRowReceiptDtls.amount, oVoucherTaxCode, iRowReceipt.invoice_no[0].ToString());
                                                    if (CNDocEntry != 0)
                                                    {
                                                        oIncomingPayment.Invoices.DocEntry = CNDocEntry;
                                                        oIncomingPayment.Invoices.InvoiceType = BoRcptInvTypes.it_CredItnote;
                                                        oIncomingPayment.Invoices.Add();
                                                    }
                                                    else
                                                    {
                                                        lastMessage = "Credit Note (Voucher) with Transaction id:" + iRowReceipt.id + " and Receipt No:" + iRowReceipt.receipt_no + " does not exist in SAP B1.";
                                                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowReceipt.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");

                                                        functionReturnValue = true;

                                                        goto isAddWithError;
                                                    }
                                                }
                                            }
                                            else if (oModeOfPayment == "NA")
                                            { }
                                            else
                                            { }
                                        }
                                    }
                                    ////**** Payment Means for the List of Invoices and Credit Note ****\\\\

                                    lErrCode = oIncomingPayment.Add();
                                    if (lErrCode == 0)
                                    {
                                        try
                                        {
                                            oDocEntry = GlobalVar.myCompany.GetNewObjectKey();
                                            lastMessage = "Successfully created Incoming Payment (Draft) with Transaction Id:" + iRowReceipt.id + " in SAP B1.";
                                            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Draft',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "',\"objType\" = 140 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");

                                            functionReturnValue = false;
                                        }
                                        catch
                                        { }
                                    }
                                    else
                                    {
                                        lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowReceipt.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");

                                        functionReturnValue = true;

                                        goto isAddWithError;
                                    }

                                isAddWithError: ;

                                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oIncomingPayment);

                                }
                                else
                                {
                                    oDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"OPDF\" where \"U_TransId\" = '" + iRowReceipt.id + "' and \"ObjType\" = 24", GlobalVar.myCompany);

                                    lastMessage = "Incoming Payment (Draft) with Transaction Id:" + iRowReceipt.id + " is already existing in SAP B1.";
                                    GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowReceipt.status == 0, "Draft", "Void") + "',\"statusCode\" = 'Draft',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "',\"objType\" = 140 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");

                                    functionReturnValue = true;
                                }
                            }
                            else
                            {
                                oDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ORCT\" where \"U_TransId\" = '" + iRowReceipt.id + "' and \"Canceled\" = 'N'", GlobalVar.myCompany);
                                if (oDocEntry != "" && oDocEntry != "0") //**** Voiding of Incoming Payment when it is already existing SAP B1. ****\\
                                {
                                    oIncomingPayment = (Payments)GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.oIncomingPayments);
                                    if (oIncomingPayment.GetByKey(Convert.ToInt16(oDocEntry)) == true)
                                    {
                                        lErrCode = oIncomingPayment.Cancel();
                                        if (lErrCode == 0)
                                        {
                                            try
                                            {
                                                lastMessage = "Successfully canceled Incoming Payment with Transaction Id:" + iRowReceipt.id + " in SAP B1.";
                                                GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Posted',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");

                                                functionReturnValue = false;
                                            }
                                            catch
                                            { }
                                        }
                                        else
                                        {
                                            lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowReceipt.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");

                                            functionReturnValue = true;
                                        }
                                        System.Runtime.InteropServices.Marshal.ReleaseComObject(oIncomingPayment);
                                    }
                                }
                                else //**** Creation of Incoming Payment in SAP B1 before voiding the Incoming Payment. ****\\
                                {
                                    Int16 oDocEntryORCT = 0;
                                    string oDocEntryOPDF = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"OPDF\" where \"U_TransId\" = '" + iRowReceipt.id + "' and \"ObjType\" = 24", GlobalVar.myCompany);
                                    if (oDocEntryOPDF == "" || oDocEntryOPDF == "0")
                                    {
                                        oDocEntryORCT = CreateReceiptVoid(listReceipt);
                                        SAPbobsCOM.Payments oIncomingDraft = (Payments)GlobalVar.myCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPaymentsDrafts);
                                        if (oDocEntryORCT != 0)
                                        {
                                            if (oIncomingDraft.GetByKey(oDocEntryORCT))
                                            {
                                                int ErrCode = oIncomingDraft.SaveDraftToDocument();
                                                if (ErrCode == 0)
                                                {
                                                    oDocEntry = GlobalVar.myCompany.GetNewObjectKey();
                                                    oIncomingPayment = (Payments)GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.oIncomingPayments);
                                                    if (oIncomingPayment.GetByKey(Convert.ToInt16(oDocEntry)) == true)
                                                    {
                                                        lErrCode = oIncomingPayment.Cancel();
                                                        if (lErrCode == 0)
                                                        {
                                                            try
                                                            {
                                                                oDocEntry = GlobalVar.myCompany.GetNewObjectKey();
                                                                lastMessage = "Successfully canceled Incoming Payment with Transaction Id:" + iRowReceipt.id + " in SAP B1.";
                                                                GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Posted',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "',\"objType\" = 24 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");

                                                                functionReturnValue = false;
                                                            }
                                                            catch
                                                            { }
                                                        }
                                                        else
                                                        {
                                                            lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                                            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowReceipt.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");

                                                            functionReturnValue = true;
                                                        }
                                                        System.Runtime.InteropServices.Marshal.ReleaseComObject(oIncomingPayment);
                                                    }
                                                }
                                            }
                                            System.Runtime.InteropServices.Marshal.ReleaseComObject(oIncomingDraft);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            //0 = no offset
                            //1 = has both payment and offset
                            //2 = only offset type
                            foreach (var iRowReceiptOffSetDtls in iRowReceipt.offset_references.ToList())
                            {
                                string oDocEntryORIN = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ORIN\" where \"U_TransId\" = '" + iRowReceiptOffSetDtls.ToString() + "' and \"CANCELED\" = 'N' and \"ObjType\" = 14", GlobalVar.myCompany);
                                if (oDocEntryORIN == "" || oDocEntryORIN == "0")
                                {
                                    string oDraftDocEntry = string.Empty;
                                    oDraftDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ODRF\" where \"U_TransId\" = '" + iRowReceiptOffSetDtls.ToString() + "' and \"CANCELED\" = 'N' and \"ObjType\" = 14", GlobalVar.myCompany);
                                    if (oDraftDocEntry != "" && oDraftDocEntry != "0")
                                    {
                                        SAPbobsCOM.Documents oDraft = (Documents)GlobalVar.myCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                                        if (oDraft.GetByKey(Convert.ToInt16(oDraftDocEntry)))
                                        {
                                            int ErrCode = oDraft.SaveDraftToDocument();
                                            if (ErrCode == 0)
                                            {
                                                oDocEntry = GlobalVar.myCompany.GetNewObjectKey();
                                                lastMessage = "Successfully created Credit Note (Deposit) with Transaction Id:" + iRowReceipt.id + " in SAP B1.";
                                                GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Posted',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "',\"objType\" = 14 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");

                                                functionReturnValue = false;
                                            }
                                            else
                                            {
                                                lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                                GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowReceipt.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");

                                                functionReturnValue = true;
                                            }
                                        }
                                        System.Runtime.InteropServices.Marshal.ReleaseComObject(oDraft);
                                    }
                                    else
                                    {
                                        lastMessage = "Credit Note (Deposit) with Reference Id:" + iRowReceiptOffSetDtls.ToString() + " does not exist in SAP B1.";
                                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowReceipt.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");

                                        functionReturnValue = true;
                                    }
                                }
                                else
                                {
                                    lastMessage = "Credit Note (Deposit) with Reference Id:" + iRowReceiptOffSetDtls.ToString() + " already posted in SAP B1.";
                                    GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowReceipt.status == 0, "Confirmed", "Void") + "',\"statusCode\" = 'Posted',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntryORIN + "',\"objType\" = 14 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");

                                    functionReturnValue = false;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lastMessage = ex.ToString();
                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowReceipt.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");
                        functionReturnValue = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return functionReturnValue;
        }

        public static bool SBOPostReceipt(List<Models.API_Receipt> listReceipt)
        {
            bool functionReturnValue = false;
            int lErrCode = 0;
            string oLogExist = string.Empty;
            string oTransId = string.Empty;
            string oCardCode = string.Empty;
            string oCardName = string.Empty;
            string oDocEntry = string.Empty;
            string oInvDocEntry = string.Empty;
            string oCreditNoteDocEntry = string.Empty;
            string oAcctCode = string.Empty;
            string oBankName = string.Empty;
            string oCheckBankName = string.Empty;
            string iReference = string.Empty;

            SBOGetRecord clsSBOGetRecord = new SBOGetRecord();
            try
            {
                foreach (var iRowReceipt in listReceipt)
                {
                    try
                    {
                        if (iRowReceipt.status == 0)
                        {
                            oTransId = (String)clsSBOGetRecord.GetSingleValue("select \"U_TransId\" from \"ORCT\" where \"U_TransId\" = '" + iRowReceipt.id + "' and \"Canceled\" = 'N' and \"ObjType\" = 24", GlobalVar.myCompany);
                            if (oTransId == "" || oTransId == "0")
                            {
                                SAPbobsCOM.Payments oIncomingDraft = (Payments)GlobalVar.myCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPaymentsDrafts);
                                string oIncomingDocEntry = string.Empty;
                                oIncomingDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"OPDF\" where \"U_TransId\" = '" + iRowReceipt.id + "' and \"ObjType\" = 24", GlobalVar.myCompany);
                                if (oIncomingDocEntry != "" && oIncomingDocEntry != "0")
                                {
                                    if (oIncomingDraft.GetByKey(Convert.ToInt16(oIncomingDocEntry)))
                                    {
                                        lErrCode = oIncomingDraft.SaveDraftToDocument();
                                        if (lErrCode == 0)
                                        {
                                            oDocEntry = GlobalVar.myCompany.GetNewObjectKey();

                                            lastMessage = "Successfully created Incoming Payment with Transaction Id:" + iRowReceipt.id + " in SAP B1.";

                                            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Posted',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "',\"objType\" = 24 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");

                                            functionReturnValue = false;
                                        }
                                        else
                                        {
                                            lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowReceipt.status == 0, "Draft", "Void") + "',\"statusCode\" = 'Draft',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");

                                            functionReturnValue = true;
                                        }
                                    }
                                }
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(oIncomingDraft);
                            }
                            else
                            {
                                oDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ORCT\" where \"U_TransId\" = '" + iRowReceipt.id + "' and \"Canceled\" = 'N' and \"ObjType\" = 24", GlobalVar.myCompany);

                                lastMessage = "Incoming Payment with Transaction Id:" + iRowReceipt.id + " already exist in SAP B1.";

                                GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Posted',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"sapCode\" = '" + oDocEntry + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"objType\" = 24 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");
                                functionReturnValue = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lastMessage = ex.ToString();
                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowReceipt.status == 0, "Draft", "Void") + "',\"statusCode\" = 'Draft',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");
                        functionReturnValue = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return functionReturnValue;
        }

        public static bool SBOPostCreditRefundDraft(List<Models.API_CreditRefund> listCreditRefund)
        {
            bool functionReturnValue = false;
            int lErrCode = 0;
            string oLogExist = string.Empty;
            string oTransId = string.Empty;
            string oCardCode = string.Empty;
            string oDocEntry = string.Empty;
            string oCreditNoteDocEntry = string.Empty;
            string oInvoiceDocEntry = string.Empty;
            string oModeOfPayment = string.Empty;
            string oCountryCod = string.Empty;
            string oAcctCode = string.Empty;
            string oBankCode = string.Empty;
            string oBankName = string.Empty;
            string oBranch = string.Empty;
            string oCheckAccount = string.Empty;
            string oCheckNumber = string.Empty;
            string iReference = string.Empty;
            SBOGetRecord clsSBOGetRecord = new SBOGetRecord();
            try
            {
                foreach (var iRowCreditRefund in listCreditRefund)
                {
                    try
                    {
                        oOutgoingPayment = (Payments)GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.oPaymentsDrafts);
                        if (iRowCreditRefund.status == 0)
                        {
                            oTransId = (String)clsSBOGetRecord.GetSingleValue("select \"U_TransId\" from \"OPDF\" where \"U_TransId\" = '" + iRowCreditRefund.id + "' and \"ObjType\" = 46", GlobalVar.myCompany);
                            oOutgoingPayment.DocObjectCode = BoPaymentsObjectType.bopot_OutgoingPayments;
                            if (oTransId == "" || oTransId == "0")
                            {
                                oCardCode = (String)clsSBOGetRecord.GetSingleValue("select \"CardCode\" from \"OCRD\" where \"CardCode\" = '" + TrimData(iRowCreditRefund.student) + "'", GlobalVar.myCompany);
                                if (oCardCode != "")
                                {
                                    oOutgoingPayment.CardCode = oCardCode;
                                }
                                else
                                {
                                    lastMessage = "Customer Code:" + iRowCreditRefund.student + " is not found in SAP B1";
                                    GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditRefund.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                    functionReturnValue = true;

                                    goto isAddWithError;
                                }

                                oOutgoingPayment.DocType = BoRcptTypes.rCustomer;
                                oOutgoingPayment.DocDate = Convert.ToDateTime(iRowCreditRefund.date_created);

                                if (iRowCreditRefund.payment_reference != "N.A" && !string.IsNullOrEmpty(iRowCreditRefund.payment_reference))
                                    oOutgoingPayment.JournalRemarks = iRowCreditRefund.payment_reference;

                                ////**** UDF ****\\\\
                                oOutgoingPayment.UserFields.Fields.Item("U_TransId").Value = iRowCreditRefund.id.ToString();
                                oOutgoingPayment.UserFields.Fields.Item("U_Status").Value = iRowCreditRefund.status.ToString();
                                oOutgoingPayment.UserFields.Fields.Item("U_CreditType").Value = iRowCreditRefund.credit_type;
                                ////**** UDF ****\\\\

                                if (iRowCreditRefund.status == 0)
                                    oOutgoingPayment.Remarks = iRowCreditRefund.remarks;
                                else
                                    oOutgoingPayment.Remarks = iRowCreditRefund.void_remarks;


                                if (iRowCreditRefund.overpaid_offsets == 0)
                                {
                                    //** credit id **//
                                    oCreditNoteDocEntry = string.Empty;
                                    oCreditNoteDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ORIN\" where \"U_TransId\" = '" + iRowCreditRefund.credit_id + "' and \"CANCELED\" = 'N'", GlobalVar.myCompany);
                                    if (oCreditNoteDocEntry != "" && oCreditNoteDocEntry != "0")
                                    {
                                        oOutgoingPayment.Invoices.DocEntry = Convert.ToInt16(oCreditNoteDocEntry);
                                        oOutgoingPayment.Invoices.InvoiceType = BoRcptInvTypes.it_CredItnote;
                                        oOutgoingPayment.Invoices.Add();
                                    }
                                    else
                                    {
                                        string oDraftDocEntry = string.Empty;
                                        oDraftDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ODRF\" where \"U_TransId\" = '" + iRowCreditRefund.credit_id + "' and \"CANCELED\" = 'N'", GlobalVar.myCompany);
                                        if (oDraftDocEntry != "" && oDraftDocEntry != "0")
                                        {
                                            SAPbobsCOM.Documents oDraft = (Documents)GlobalVar.myCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                                            if (oDraft.GetByKey(Convert.ToInt16(oDraftDocEntry)))
                                            {
                                                int ErrCode = oDraft.SaveDraftToDocument();
                                                if (ErrCode == 0)
                                                {
                                                    oCreditNoteDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ORIN\" where \"U_TransId\" = '" + iRowCreditRefund.credit_id + "' and \"CANCELED\" = 'N'", GlobalVar.myCompany);
                                                    if (oCreditNoteDocEntry != "" && oCreditNoteDocEntry != "0")
                                                    {
                                                        oOutgoingPayment.Invoices.DocEntry = Convert.ToInt16(oCreditNoteDocEntry);
                                                        oOutgoingPayment.Invoices.InvoiceType = BoRcptInvTypes.it_CredItnote;
                                                        oOutgoingPayment.Invoices.Add();
                                                    }
                                                }
                                                else
                                                {
                                                    lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                                    GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditRefund.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                                    functionReturnValue = true;

                                                    goto isAddWithError;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            lastMessage = "Credit Note (Draft) with Reference Id:" + iRowCreditRefund.credit_id + " does not exist in SAP B1.";
                                            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditRefund.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                            functionReturnValue = true;

                                            goto isAddWithError;

                                        }
                                    }
                                    //** credit id **//
                                }
                                else
                                {
                                    //**overpaid_offsets_credit_notes**//
                                    foreach (var iRowCreditRefundCN in iRowCreditRefund.overpaid_offsets_credit_notes.ToList())
                                    {
                                        oCreditNoteDocEntry = string.Empty;
                                        oCreditNoteDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ORIN\" where \"U_TransId\" = '" + iRowCreditRefundCN.ToString() + "' and \"CANCELED\" = 'N'", GlobalVar.myCompany);
                                        if (oCreditNoteDocEntry != "" && oCreditNoteDocEntry != "0")
                                        {
                                            oOutgoingPayment.Invoices.DocEntry = Convert.ToInt16(oCreditNoteDocEntry);
                                            oOutgoingPayment.Invoices.InvoiceType = BoRcptInvTypes.it_CredItnote;
                                            oOutgoingPayment.Invoices.Add();
                                        }
                                        else
                                        {
                                            string oDraftDocEntry = string.Empty;
                                            oDraftDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ODRF\" where \"U_TransId\" = '" + iRowCreditRefundCN.ToString() + "' and \"CANCELED\" = 'N' and \"ObjType\" = 14", GlobalVar.myCompany);
                                            if (oDraftDocEntry != "" && oDraftDocEntry != "0")
                                            {
                                                SAPbobsCOM.Documents oDraft = (Documents)GlobalVar.myCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                                                if (oDraft.GetByKey(Convert.ToInt16(oDraftDocEntry)))
                                                {
                                                    int ErrCode = oDraft.SaveDraftToDocument();
                                                    if (ErrCode == 0)
                                                    {
                                                        oCreditNoteDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ORIN\" where \"U_TransId\" = '" + iRowCreditRefundCN.ToString() + "' and \"CANCELED\" = 'N'", GlobalVar.myCompany);
                                                        if (oCreditNoteDocEntry != "" && oCreditNoteDocEntry != "0")
                                                        {
                                                            oOutgoingPayment.Invoices.DocEntry = Convert.ToInt16(oCreditNoteDocEntry);
                                                            oOutgoingPayment.Invoices.InvoiceType = BoRcptInvTypes.it_CredItnote;
                                                            oOutgoingPayment.Invoices.Add();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditRefund.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                                        functionReturnValue = true;

                                                        goto isAddWithError;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                lastMessage = "Credit Note (Draft) with Reference Id:" + iRowCreditRefundCN.ToString() + " does not exist in SAP B1.";
                                                GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditRefund.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                                functionReturnValue = true;

                                                goto isAddWithError;

                                            }
                                        }
                                    }
                                    //**overpaid_offsets_credit_notes**//
                                }

                                //**overpaid_offsets_invoices**//
                                foreach (var iRowCreditRefundInv in iRowCreditRefund.overpaid_offsets_invoices.ToList())
                                {
                                    oInvoiceDocEntry = string.Empty;
                                    oInvoiceDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"OINV\" where \"U_TransId\" = '" + iRowCreditRefundInv.ToString() + "' and \"CANCELED\" = 'N'", GlobalVar.myCompany);
                                    if (oInvoiceDocEntry != "" && oInvoiceDocEntry != "0")
                                    {
                                        oOutgoingPayment.Invoices.DocEntry = Convert.ToInt16(oInvoiceDocEntry);
                                        oOutgoingPayment.Invoices.InvoiceType = BoRcptInvTypes.it_Invoice;
                                        oOutgoingPayment.Invoices.Add();
                                    }
                                    else
                                    {
                                        string oDraftDocEntry = string.Empty;
                                        oDraftDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ODRF\" where \"U_TransId\" = '" + iRowCreditRefundInv.ToString() + "' and \"CANCELED\" = 'N' and \"ObjType\" = 13", GlobalVar.myCompany);
                                        if (oDraftDocEntry != "" && oDraftDocEntry != "0")
                                        {
                                            SAPbobsCOM.Documents oDraft = (Documents)GlobalVar.myCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                                            if (oDraft.GetByKey(Convert.ToInt16(oDraftDocEntry)))
                                            {
                                                int ErrCode = oDraft.SaveDraftToDocument();
                                                if (ErrCode == 0)
                                                {
                                                    oInvoiceDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"OINV\" where \"U_TransId\" = '" + iRowCreditRefundInv.ToString() + "' and \"CANCELED\" = 'N'", GlobalVar.myCompany);
                                                    if (oInvoiceDocEntry != "" && oInvoiceDocEntry != "0")
                                                    {
                                                        oOutgoingPayment.Invoices.DocEntry = Convert.ToInt16(oCreditNoteDocEntry);
                                                        oOutgoingPayment.Invoices.InvoiceType = BoRcptInvTypes.it_Invoice;
                                                        oOutgoingPayment.Invoices.Add();
                                                    }
                                                }
                                                else
                                                {
                                                    lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                                    GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditRefund.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                                    functionReturnValue = true;

                                                    goto isAddWithError;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            lastMessage = "Invoice (Draft) with Reference Id:" + iRowCreditRefundInv.ToString() + " does not exist in SAP B1.";
                                            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditRefund.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                            functionReturnValue = true;

                                            goto isAddWithError;

                                        }
                                    }
                                }
                                //**overpaid_offsets_invoices**//

                                ////**** Payment Means for the List of Credit Note ****\\\\
                                oAcctCode = (String)clsSBOGetRecord.GetSingleValue("select \"U_GLAccount\" from \"@PAYMENTCODES\" where \"U_PaymentCodeMethod\" = '" + iRowCreditRefund.payment_method + "'", GlobalVar.myCompany);
                                oModeOfPayment = (String)clsSBOGetRecord.GetSingleValue("select \"U_ModePayment\" from \"@PAYMENTCODES\" where \"U_PaymentCodeMethod\" = '" + iRowCreditRefund.payment_method + "'", GlobalVar.myCompany);

                                if (oModeOfPayment == "CA")
                                {
                                    if (!string.IsNullOrEmpty(oAcctCode))
                                        oOutgoingPayment.CashAccount = oAcctCode;

                                    if (iRowCreditRefund.amount != 0)
                                        oOutgoingPayment.CashSum = iRowCreditRefund.amount;
                                }
                                else if (oModeOfPayment == "CK")
                                {
                                    if (!string.IsNullOrEmpty(iRowCreditRefund.payment_reference))
                                    {
                                        oBankName = iRowCreditRefund.payment_reference.Substring(0, iRowCreditRefund.payment_reference.IndexOf(' '));

                                        oCheckNumber = iRowCreditRefund.payment_reference.Replace(oBankName, "");

                                        oBankCode = (String)clsSBOGetRecord.GetSingleValue("select \"BankCode\" from \"ODSC\" where \"BankName\" = '" + TrimData(oBankName) + "'",
                                            GlobalVar.myCompany);


                                        if (string.IsNullOrEmpty(oBankCode))
                                        {
                                            lastMessage = "Bank:" + oBankName + " is not found in SAP B1";
                                            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditRefund.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                            functionReturnValue = true;

                                            goto isAddWithError;
                                        }

                                        oCountryCod = (String)clsSBOGetRecord.GetSingleValue("select \"Country\" from \"DSC1\" where \"BankCode\" = '" + TrimData(oBankCode) + "'", GlobalVar.myCompany);

                                        oAcctCode = (String)clsSBOGetRecord.GetSingleValue("select \"GLAccount\" from \"DSC1\" where \"BankCode\" = '" + TrimData(oBankCode) + "'", GlobalVar.myCompany);

                                        oCheckAccount = (String)clsSBOGetRecord.GetSingleValue("select \"Account\" from \"DSC1\" where \"BankCode\" = '" + TrimData(oBankCode) + "'", GlobalVar.myCompany);

                                        oBranch = (String)clsSBOGetRecord.GetSingleValue("select \"Branch\" from \"DSC1\" where \"BankCode\" = '" + TrimData(oBankCode) + "'", GlobalVar.myCompany);
                                    }

                                    if (!string.IsNullOrEmpty(oCountryCod))
                                        oOutgoingPayment.Checks.CountryCode = oCountryCod;

                                    if (!string.IsNullOrEmpty(oBankCode))
                                        oOutgoingPayment.Checks.BankCode = oBankCode;

                                    if (!string.IsNullOrEmpty(oAcctCode))
                                        oOutgoingPayment.Checks.CheckAccount = oAcctCode;

                                    if (!string.IsNullOrEmpty(oCheckAccount))
                                        oOutgoingPayment.Checks.AccounttNum = oCheckAccount;

                                    if (!string.IsNullOrEmpty(oBranch))
                                        oOutgoingPayment.Checks.Branch = oBranch;

                                    if (!string.IsNullOrEmpty(oCheckNumber))
                                        oOutgoingPayment.Checks.CheckNumber = Convert.ToInt32(TrimData(oCheckNumber));

                                    if (iRowCreditRefund.amount != 0)
                                        oOutgoingPayment.Checks.CheckSum = iRowCreditRefund.amount;

                                    oOutgoingPayment.Checks.Add();
                                }
                                else if (oModeOfPayment == "BT")
                                {
                                    if (iReference != "N.A" && iReference != "")
                                        oOutgoingPayment.TransferReference = iReference;

                                    if (string.IsNullOrEmpty(oAcctCode))
                                        oOutgoingPayment.TransferAccount = oAcctCode;

                                    if (iRowCreditRefund.amount != 0)
                                        oOutgoingPayment.TransferSum = iRowCreditRefund.amount;
                                }
                                else if (oModeOfPayment == "CC")
                                {
                                    //    string creditCardName = cls.GetSingleValue("SELECT \"CreditCard\" FROM OCRC WHERE \"CardName\" = '" + oIncomingPaymentLines.creditCardName + "'", company);
                                    //    if (creditCardName != "")
                                    //    {
                                    //        oIncomingPayment.CreditCards.CreditCard = Convert.ToInt16(creditCardName);
                                    //        oIncomingPayment.CreditCards.CardValidUntil = Convert.ToDateTime(oIncomingPaymentLines.creditCardValidDate);
                                    //        oIncomingPayment.CreditCards.CreditCardNumber = oIncomingPaymentLines.creditCardNumber;

                                    //        if (oIncomingPaymentLines.creditCardAmount != 0)
                                    //            oIncomingPayment.CreditCards.CreditSum = oIncomingPaymentLines.creditCardAmount;

                                    //        oIncomingPayment.CreditCards.VoucherNum = oIncomingPaymentLines.creditCardApproval;
                                    //        oIncomingPayment.CreditCards.Add();
                                    //    }
                                }
                                else if (oModeOfPayment == "CN")
                                { }
                                else if (oModeOfPayment == "NA")
                                { }
                                else
                                { }

                                ////**** Payment Means for the List of Invoices ****\\\\

                                lErrCode = oOutgoingPayment.Add();
                                if (lErrCode == 0)
                                {
                                    try
                                    {
                                        oDocEntry = GlobalVar.myCompany.GetNewObjectKey();
                                        lastMessage = "Successfully created Outgoing Payment (Draft) with Transaction Id:" + iRowCreditRefund.id + " in SAP B1.";
                                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Draft',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "',\"objType\" = 140 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                        functionReturnValue = false;
                                    }
                                    catch
                                    { }
                                }
                                else
                                {
                                    lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                    GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditRefund.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + GlobalVar.oSERVERDB + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                    functionReturnValue = true;

                                    goto isAddWithError;
                                }

                            isAddWithError: ;

                                System.Runtime.InteropServices.Marshal.ReleaseComObject(oOutgoingPayment);

                            }
                            else
                            {
                                oDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"OPDF\" where \"U_TransId\" = '" + iRowCreditRefund.id + "' and \"ObjType\" = 46", GlobalVar.myCompany);

                                lastMessage = "Outgoing Payment (Draft) with Transaction Id:" + iRowCreditRefund.id + " is already existing in SAP B1.";

                                GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'Draft',\"statusCode\" = 'Draft',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "',\"objType\" = 140 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                functionReturnValue = true;
                            }
                        }
                        else
                        {
                            oDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"OVPM\" where \"U_TransId\" = '" + iRowCreditRefund.id + "' and \"Canceled\" = 'N'", GlobalVar.myCompany);
                            if (oDocEntry != "" && oDocEntry != "0")//**** Voiding of Outgoing Payment when it is already existing SAP B1. ****\\
                            {
                                oOutgoingPayment = (Payments)GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.oVendorPayments);
                                if (oOutgoingPayment.GetByKey(Convert.ToInt16(oDocEntry)) == true)
                                {
                                    lErrCode = oOutgoingPayment.Cancel();
                                    if (lErrCode == 0)
                                    {
                                        try
                                        {
                                            lastMessage = "Successfully canceled Outgoing Payment with Transaction Id:" + iRowCreditRefund.id + " in SAP B1.";
                                            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"docStatus\" = 'Void',\"statusCode\" = 'Posted',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "',\"objType\" = 46 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                            functionReturnValue = false;
                                        }
                                        catch
                                        { }
                                    }
                                    else
                                    {
                                        lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditRefund.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                        functionReturnValue = true;
                                    }
                                }
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(oOutgoingPayment);
                            }
                            else
                            {
                                Int16 oDocEntryOVPM = 0;
                                string oDocEntryOPDF = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"OPDF\" where \"U_TransId\" = '" + iRowCreditRefund.id + "' and \"ObjType\" = 46", GlobalVar.myCompany);
                                if (oDocEntryOPDF == "" || oDocEntryOPDF == "0")//**** Creation of Outgoing Payment in SAP B1 before voiding the Outgoing Payment. ****\\
                                {
                                    oDocEntryOVPM = CreateCreditRefundVoid(listCreditRefund);
                                    SAPbobsCOM.Payments oOutgoingDraft = (Payments)GlobalVar.myCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPaymentsDrafts);
                                    if (oDocEntryOVPM != 0)
                                    {
                                        if (oOutgoingDraft.GetByKey(oDocEntryOVPM))
                                        {
                                            int ErrCode = oOutgoingDraft.SaveDraftToDocument();
                                            if (ErrCode == 0)
                                            {
                                                oDocEntry = GlobalVar.myCompany.GetNewObjectKey();
                                                oOutgoingPayment = (Payments)GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.oVendorPayments);
                                                if (oOutgoingPayment.GetByKey(Convert.ToInt16(oDocEntry)) == true)
                                                {
                                                    lErrCode = oOutgoingPayment.Cancel();
                                                    if (lErrCode == 0)
                                                    {
                                                        try
                                                        {
                                                            oDocEntry = GlobalVar.myCompany.GetNewObjectKey();
                                                            lastMessage = "Successfully canceled Outgoing Payment with Transaction Id:" + iRowCreditRefund.id + " in SAP B1.";
                                                            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Posted',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "',\"objType\" = 46 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                                            functionReturnValue = false;
                                                        }
                                                        catch
                                                        { }
                                                    }
                                                    else
                                                    {
                                                        lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditRefund.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                                        functionReturnValue = true;
                                                    }
                                                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oOutgoingPayment);
                                                }
                                            }
                                            else
                                            {
                                                lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                                GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditRefund.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                                functionReturnValue = true;
                                            }
                                        }
                                        System.Runtime.InteropServices.Marshal.ReleaseComObject(oOutgoingDraft);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lastMessage = ex.ToString();
                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditRefund.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");
                        functionReturnValue = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return functionReturnValue;
        }

        public static bool SBOPostCreditRefund(List<Models.API_CreditRefund> listCreditRefund)
        {
            bool functionReturnValue = false;
            int lErrCode = 0;
            string oLogExist = string.Empty;
            string oTransId = string.Empty;
            string oCardCode = string.Empty;
            string oDocEntry = string.Empty;
            string oCreditNoteDocEntry = string.Empty;
            string oAcctCode = string.Empty;
            string oBankName = string.Empty;
            string oCheckBankName = string.Empty;
            string iReference = string.Empty;
            SBOGetRecord clsSBOGetRecord = new SBOGetRecord();
            try
            {
                foreach (var iRowCreditRefund in listCreditRefund)
                {
                    try
                    {
                        if (iRowCreditRefund.status == 0)
                        {
                            oTransId = (String)clsSBOGetRecord.GetSingleValue("select \"U_TransId\" from \"OVPM\" where \"U_TransId\" = '" + iRowCreditRefund.id + "' and \"Canceled\" = 'N'", GlobalVar.myCompany);
                            if (oTransId == "" || oTransId == "0")
                            {
                                SAPbobsCOM.Payments oOutgoingDraft = (Payments)GlobalVar.myCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPaymentsDrafts);
                                string oOutgoingDocEntry = string.Empty;
                                oOutgoingDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"OPDF\" where \"U_TransId\" = '" + iRowCreditRefund.id + "' and \"ObjType\" = 46", GlobalVar.myCompany);
                                if (oOutgoingDocEntry != "" && oOutgoingDocEntry != "0")
                                {
                                    if (oOutgoingDraft.GetByKey(Convert.ToInt16(oOutgoingDocEntry)))
                                    {
                                        lErrCode = oOutgoingDraft.SaveDraftToDocument();
                                        if (lErrCode == 0)
                                        {
                                            oDocEntry = GlobalVar.myCompany.GetNewObjectKey();
                                            lastMessage = "Successfully created Outgoing Payment with Transaction Id:" + iRowCreditRefund.id + " in SAP B1.";

                                            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Posted',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "',\"objType\" = 46 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                            functionReturnValue = false;
                                        }
                                        else
                                        {
                                            lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'false',\"statusCode\" = 'Draft',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                            functionReturnValue = true;
                                        }
                                    }
                                }
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(oOutgoingDraft);
                            }
                            else
                            {
                                oDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"U_TransId\" from \"OVPM\" where \"U_TransId\" = '" + iRowCreditRefund.id + "' and \"Canceled\" = 'N'", GlobalVar.myCompany);

                                lastMessage = "Outgoing Payment with Transaction Id:" + iRowCreditRefund.id + " already exist in SAP B1.";

                                GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Posted',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"sapCode\" = '" + oDocEntry + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"objType\" = 46 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");
                                functionReturnValue = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lastMessage = ex.ToString();
                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'false',\"statusCode\" = 'Draft',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");
                        functionReturnValue = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return functionReturnValue;
        }

        public static Int16 CreateInvoiceVoid(List<Models.API_Invoice> olistInvoice)
        {
            {
                string ifunctionReturnValue = "0";
                string ilastErrorMessage = string.Empty; ;
                int olErrCode;
                string iTransId = string.Empty;
                string iCardCode = string.Empty;
                string iCardName = string.Empty;
                string iDocEntry = string.Empty;
                string iDescription = string.Empty;
                string iItemCode = string.Empty;
                string iDocType = string.Empty;
                SBOGetRecord clsSBOGetRecord = new SBOGetRecord();
                foreach (var oRowInv in olistInvoice)
                {
                    try
                    {
                        if (oRowInv.status == 2)
                        {
                            iDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ODRF\" where \"U_TransId\" = '" + oRowInv.id + "' and \"CANCELED\" = 'N' and \"ObjType\" = 13", GlobalVar.myCompany);
                            if (iDocEntry == "" || iDocEntry == "0")
                            {
                                oInvoice = (Documents)GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.oDrafts);
                                oInvoice.DocObjectCode = BoObjectTypes.oInvoices;

                                iCardCode = (String)clsSBOGetRecord.GetSingleValue("select \"CardCode\" from \"OCRD\" where \"CardCode\" = '" + TrimData(oRowInv.student) + "'", GlobalVar.myCompany);
                                if (iCardCode != "")
                                {
                                    oInvoice.CardCode = iCardCode;
                                }
                                else
                                {
                                    lastMessage = "Customer Code:" + oRowInv.student + " is not found in SAP B1";
                                    GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(oRowInv.status == 1, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "' where \"module\" = 'Invoice' and \"uniqueId\" = '" + oRowInv.id + "' and \"companyDB\" = '" + TrimData(Helpers.GlobalVar.oSERVERDB) + "'");

                                    ifunctionReturnValue = "0";

                                    goto isAddWithError;
                                }

                                oInvoice.DocDate = Convert.ToDateTime(oRowInv.date_created);
                                oInvoice.NumAtCard = oRowInv.invoice_no;
                                oInvoice.DocDueDate = Convert.ToDateTime(oRowInv.date_due);

                                if (oRowInv.status == 1)
                                    oInvoice.Comments = oRowInv.remarks;
                                else
                                    oInvoice.Comments = oRowInv.void_remarks;

                                ////**** UDF *****\\\\
                                if (oRowInv.id != 0)
                                    oInvoice.UserFields.Fields.Item("U_TransId").Value = oRowInv.id.ToString();

                                if (oRowInv.level != "")
                                    oInvoice.UserFields.Fields.Item("U_Level").Value = oRowInv.level;

                                if (oRowInv.program_type != "")
                                    oInvoice.UserFields.Fields.Item("U_ProgramType").Value = oRowInv.program_type;
                                ////**** UDF *****\\\\

                                foreach (var oRowInvDtls in oRowInv.items.ToList())
                                {
                                    if (oRowInvDtls.item_code == "" || string.IsNullOrEmpty(oRowInvDtls.item_code))
                                    {
                                        iDocType = "dDocument_Service";
                                        string iReplaceDesc = " (" + TrimData(oRowInv.level) + " - " + TrimData(oRowInv.program_type) + ")";
                                        //iDescription = SBOstrManipulation.BeforeCharacter(oRowInvDtls.description, " (");
                                        iDescription = oRowInvDtls.description.Replace(iReplaceDesc, "");

                                        if (iDescription != "")
                                        {
                                            string description = iDescription;
                                            string oDescription = (String)clsSBOGetRecord.GetSingleValue("select \"U_Description\" from \"@GLACCTMAPPING\" where \"U_Description\" = '" + TrimData(description) + "' and \"U_Level\" = '" + TrimData(oRowInv.level) + "' and \"U_ProgramType\" = '" + TrimData(oRowInv.program_type) + "'", GlobalVar.myCompany);
                                            if (oDescription != "")
                                            {
                                                string idate_created = string.Empty;
                                                string idate_for = string.Empty;
                                                string iGLAccount = string.Empty;
                                                string oDateFor = string.Empty;

                                                if (!string.IsNullOrEmpty(oRowInvDtls.date_for))
                                                {
                                                    idate_for = oRowInvDtls.date_for;
                                                    oDateFor = Convert.ToDateTime(idate_for).ToString("MMM") + " " + Convert.ToDateTime(idate_for).Year.ToString();
                                                }
                                                else
                                                {
                                                    idate_for = oRowInv.date_created;
                                                    oDateFor = Convert.ToDateTime(idate_for).ToString("MMM") + " " + Convert.ToDateTime(idate_for).Year.ToString();
                                                }

                                                iCardName = (String)clsSBOGetRecord.GetSingleValue("select \"CardName\" from \"OCRD\" where \"CardCode\" = '" + TrimData(oRowInv.student) + "'", GlobalVar.myCompany);

                                                string oTaxCode = (String)clsSBOGetRecord.GetSingleValue("select \"U_TaxCode\" from \"@GLACCTMAPPING\" where \"U_Description\" = '" + TrimData(description) + "' and \"U_Level\" = '" + TrimData(oRowInv.level) + "' and \"U_ProgramType\" = '" + TrimData(oRowInv.program_type) + "'", GlobalVar.myCompany);

                                                if (!string.IsNullOrEmpty(oTaxCode))
                                                    oInvoice.Lines.VatGroup = oTaxCode;

                                                oItemDescription = iCardName + " - " + oDateFor + " - " + oRowInvDtls.description;

                                                oInvoice.Lines.UserFields.Fields.Item("U_Dscription").Value = oItemDescription;

                                                string Dscription = string.Empty;
                                                if (oItemDescription.Length > 100)
                                                {
                                                    Dscription = oItemDescription.Substring(0, 100);
                                                    oInvoice.Lines.ItemDescription = Dscription;
                                                }
                                                else
                                                {
                                                    oInvoice.Lines.ItemDescription = oItemDescription;
                                                }

                                                oInvoice.Lines.LineTotal = oRowInvDtls.unit_price;

                                                if (!string.IsNullOrEmpty(oRowInv.date_created))
                                                    idate_created = oRowInv.date_created;

                                                if (string.IsNullOrEmpty(Helpers.GlobalVar.oCountry) || string.IsNullOrEmpty(Helpers.GlobalVar.oGroup) || string.IsNullOrEmpty(Helpers.GlobalVar.oDivision) || string.IsNullOrEmpty(Helpers.GlobalVar.oProduct))
                                                {
                                                    lastMessage = "Cost Center is not defined in SAP B1. Please define in the integration setup.";
                                                    string oQuery = "update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(oRowInv.status == 1, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Invoice' and \"uniqueId\" = '" + oRowInv.id + "'";
                                                    GlobalVar.oRS.DoQuery(oQuery);

                                                    ifunctionReturnValue = "0";

                                                    goto isAddWithError;
                                                }

                                                if (!string.IsNullOrEmpty(Helpers.GlobalVar.oCountry))
                                                    oInvoice.Lines.CostingCode = Helpers.GlobalVar.oCountry;

                                                if (!string.IsNullOrEmpty(Helpers.GlobalVar.oGroup))
                                                    oInvoice.Lines.CostingCode2 = Helpers.GlobalVar.oGroup;

                                                if (!string.IsNullOrEmpty(Helpers.GlobalVar.oDivision))
                                                    oInvoice.Lines.CostingCode3 = Helpers.GlobalVar.oDivision;

                                                if (!string.IsNullOrEmpty(Helpers.GlobalVar.oProduct))
                                                    oInvoice.Lines.CostingCode4 = Helpers.GlobalVar.oProduct;

                                                if (!string.IsNullOrEmpty(idate_for))
                                                    oInvoice.Lines.UserFields.Fields.Item("U_date_for").Value = Convert.ToDateTime(idate_for);

                                                if (CheckDate(idate_created) == true && CheckDate(idate_for) == true)
                                                {
                                                    if (Convert.ToDateTime(idate_for) > Convert.ToDateTime(idate_created))
                                                    {
                                                        iGLAccount = (String)clsSBOGetRecord.GetSingleValue("select \"U_FuturePeriod\" from \"@GLACCTMAPPING\" where \"U_Description\" = '" + TrimData(description) + "' and \"U_Level\" = '" + TrimData(oRowInv.level) + "' and \"U_ProgramType\" = '" + TrimData(oRowInv.program_type) + "'", GlobalVar.myCompany);
                                                    }
                                                    else
                                                    {
                                                        iGLAccount = (String)clsSBOGetRecord.GetSingleValue("select \"U_CurrentPeriod\" from \"@GLACCTMAPPING\" where \"U_Description\" = '" + TrimData(description) + "' and \"U_Level\" = '" + TrimData(oRowInv.level) + "' and \"U_ProgramType\" = '" + TrimData(oRowInv.program_type) + "'", GlobalVar.myCompany);
                                                    }
                                                }

                                                if (!string.IsNullOrEmpty(iGLAccount))
                                                    oInvoice.Lines.AccountCode = iGLAccount;

                                                oInvoice.Lines.Add();
                                            }
                                            else
                                            {
                                                lastMessage = "Description:" + oRowInvDtls.description + ", Level: " + oRowInv.level + " or Program type:" + oRowInv.program_type + " is not defined in SAP B1. Please define in the table.";
                                                string oQuery = "update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(oRowInv.status == 1, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Invoice' and \"uniqueId\" = '" + oRowInv.id + "'";
                                                GlobalVar.oRS.DoQuery(oQuery);

                                                ifunctionReturnValue = "0";

                                                goto isAddWithError;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        iDocType = "dDocument_Items";
                                        iItemCode = string.Empty;
                                        iItemCode = (String)clsSBOGetRecord.GetSingleValue("select \"ItemCode\" from \"OITM\" where \"ItemCode\" = '" + TrimData(oRowInvDtls.item_code) + "'", GlobalVar.myCompany);
                                        if (iItemCode != "")
                                        {
                                            oInvoice.Lines.ItemCode = oRowInvDtls.item_code;

                                            if (oRowInvDtls.quantity != 0)
                                                oInvoice.Lines.Quantity = oRowInvDtls.quantity;

                                            if (oRowInvDtls.unit_price != 0)
                                                oInvoice.Lines.UnitPrice = oRowInvDtls.unit_price;

                                            //Added August 7, 2019 becuase of the discount scenario
                                            oInvoice.Lines.LineTotal = oRowInvDtls.total;

                                            if (!string.IsNullOrEmpty(Helpers.GlobalVar.oCountry))
                                                oInvoice.Lines.CostingCode = Helpers.GlobalVar.oCountry;

                                            if (!string.IsNullOrEmpty(Helpers.GlobalVar.oGroup))
                                                oInvoice.Lines.CostingCode2 = Helpers.GlobalVar.oGroup;

                                            if (!string.IsNullOrEmpty(Helpers.GlobalVar.oDivision))
                                                oInvoice.Lines.CostingCode3 = Helpers.GlobalVar.oDivision;

                                            if (!string.IsNullOrEmpty(Helpers.GlobalVar.oProduct))
                                                oInvoice.Lines.CostingCode4 = Helpers.GlobalVar.oProduct;

                                            oInvoice.Lines.Add();
                                        }
                                        else
                                        {
                                            lastMessage = "ItemCode: " + oRowInvDtls.item_code + " does not exist in SAP B1.";
                                            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(oRowInv.status == 1, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Invoice' and \"uniqueId\" = '" + oRowInv.id + "'");

                                            ifunctionReturnValue = "0";

                                            goto isAddWithError;
                                        }
                                    }
                                }

                                if (iDocType == "dDocument_Items")
                                    oInvoice.DocType = BoDocumentTypes.dDocument_Items;
                                else
                                    oInvoice.DocType = BoDocumentTypes.dDocument_Service;

                                olErrCode = oInvoice.Add();
                                if (olErrCode == 0)
                                {
                                    try
                                    {
                                        iDocEntry = GlobalVar.myCompany.GetNewObjectKey();
                                        lastMessage = "Successfully created Invoice (Draft) with Transaction Id: " + oRowInv.id + " in SAP B1. Subject for manual posting and cancellation.";
                                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(oRowInv.status == 1, "Draft", "Void") + "',\"statusCode\" = 'Draft',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + iDocEntry + "',\"objType\" = 112 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Invoice' and \"uniqueId\" = '" + oRowInv.id + "'");

                                        ifunctionReturnValue = iDocEntry;
                                    }
                                    catch
                                    { }
                                }
                                else
                                {
                                    lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                    GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(oRowInv.status == 1, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.myCompany.CompanyDB) + "' and \"module\" = 'Invoice' and \"uniqueId\" = '" + oRowInv.id + "'");

                                    ifunctionReturnValue = "0";

                                    goto isAddWithError;
                                }

                            isAddWithError: ;

                                System.Runtime.InteropServices.Marshal.ReleaseComObject(oInvoice);
                            }
                            else
                            {
                                ifunctionReturnValue = iDocEntry;

                                lastMessage = "Invoice (Draft) with Transaction Id: " + oRowInv.id + " already exist in SAP B1. Subject for manual posting and manual cancellation.";
                                GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(oRowInv.status == 1, "Draft", "Void") + "',\"statusCode\" = 'Posted',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + iDocEntry + "',\"objType\" = 112 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Invoice' and \"uniqueId\" = '" + oRowInv.id + "'");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lastMessage = ex.ToString();
                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(oRowInv.status == 1, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "'  and \"module\" = 'Invoice' and \"uniqueId\" = '" + oRowInv.id + "'");

                        ifunctionReturnValue = "0";
                    }
                }
                return Convert.ToInt16(ifunctionReturnValue);
            }
        }

        public static Int16 CreateCreditNoteVoid(List<Models.API_CreditNote> listCreditNote)
        {
            {
                string ifunctionReturnValue = "0";
                string ilastErrorMessage = string.Empty; ;
                int olErrCode;
                string iTransId = string.Empty;
                string iCardCode = string.Empty;
                string iCardName = string.Empty;
                string iDocEntry = string.Empty;
                string iDescription = string.Empty;
                string iItemCode = string.Empty;
                string iDocType = string.Empty;
                SBOGetRecord clsSBOGetRecord = new SBOGetRecord();
                foreach (var oRowCreditNote in listCreditNote)
                {
                    try
                    {
                        if (oRowCreditNote.status == 2)
                        {
                            iDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ORIN\" where \"U_TransId\" = '" + oRowCreditNote.id + "' and \"CANCELED\" = 'N' and \"NumAtCard\" = '" + oRowCreditNote.credit_no + "'", GlobalVar.myCompany);
                            if (iDocEntry == "" || iDocEntry == "0")
                            {
                                oCreditNote = (Documents)GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.oCreditNotes);

                                iCardCode = (String)clsSBOGetRecord.GetSingleValue("select \"CardCode\" from \"OCRD\" where \"CardCode\" = '" + TrimData(oRowCreditNote.student) + "'", GlobalVar.myCompany);
                                if (iCardCode != "")
                                {
                                    oCreditNote.CardCode = iCardCode;
                                }
                                else
                                {
                                    lastMessage = "Customer Code:" + oRowCreditNote.student + " is not found in SAP B1";
                                    GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(oRowCreditNote.status == 1, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "' where \"module\" = 'Credit Note' and \"uniqueId\" = '" + oRowCreditNote.id + "' and \"companyDB\" = '" + TrimData(Helpers.GlobalVar.oSERVERDB) + "'");

                                    ifunctionReturnValue = "0";

                                    goto isAddWithError;
                                }

                                oCreditNote.DocDate = Convert.ToDateTime(oRowCreditNote.date_created);
                                oCreditNote.NumAtCard = oRowCreditNote.credit_no;

                                if (oRowCreditNote.status == 1)
                                    oCreditNote.Comments = oRowCreditNote.remarks;
                                else
                                    oCreditNote.Comments = oRowCreditNote.void_remarks;

                                ////**** UDF *****\\\\/
                                if (oRowCreditNote.id != 0)
                                    oCreditNote.UserFields.Fields.Item("U_TransId").Value = oRowCreditNote.id.ToString();

                                if (oRowCreditNote.level != "")
                                    oCreditNote.UserFields.Fields.Item("U_Level").Value = oRowCreditNote.level;

                                if (oRowCreditNote.program_type != "")
                                    oCreditNote.UserFields.Fields.Item("U_ProgramType").Value = oRowCreditNote.program_type;
                                ////**** UDF *****\\\\/

                                foreach (var oRowCreditNoteDtls in oRowCreditNote.items.ToList())
                                {
                                    if (oRowCreditNoteDtls.description != "")
                                    {
                                        iDocType = "dDocument_Service";
                                        string iReplaceDesc = " (" + TrimData(oRowCreditNote.level) + " - " + TrimData(oRowCreditNote.program_type) + ")";
                                        //iDescription = SBOstrManipulation.BeforeCharacter(oRowCreditNoteDtls.description, " (");
                                        iDescription = oRowCreditNoteDtls.description.Replace(iReplaceDesc, "");

                                        if (iDescription != "")
                                        {
                                            string description = iDescription;
                                            string oDescription = (String)clsSBOGetRecord.GetSingleValue("select \"U_Description\" from \"@GLACCTMAPPING\" where \"U_Description\" = '" + TrimData(description) + "' and \"U_Level\" = '" + TrimData(oRowCreditNote.level) + "' and \"U_ProgramType\" = '" + TrimData(oRowCreditNote.program_type) + "'", GlobalVar.myCompany);
                                            if (oDescription != "")
                                            {
                                                string idate_created = string.Empty;
                                                string idate_for = string.Empty;
                                                string iGLAccount = string.Empty;
                                                string oDateFor = string.Empty;

                                                if (!string.IsNullOrEmpty(oRowCreditNoteDtls.date_for))
                                                {
                                                    oDateFor = Convert.ToDateTime(oRowCreditNoteDtls.date_for).ToString("MMM") + " " + Convert.ToDateTime(oRowCreditNoteDtls.date_for).Year.ToString();
                                                    idate_for = oRowCreditNoteDtls.date_for;
                                                }
                                                else
                                                {
                                                    idate_for = oRowCreditNote.date_created;
                                                    oDateFor = Convert.ToDateTime(idate_for).ToString("MMM") + " " + Convert.ToDateTime(idate_for).Year.ToString();
                                                }

                                                iCardName = (String)clsSBOGetRecord.GetSingleValue("select \"CardName\" from \"OCRD\" where \"CardCode\" = '" + TrimData(oRowCreditNote.student) + "'", GlobalVar.myCompany);

                                                string oTaxCode = (String)clsSBOGetRecord.GetSingleValue("select \"U_TaxCode\" from \"@GLACCTMAPPING\" where \"U_Description\" = '" + TrimData(description) + "' and \"U_Level\" = '" + TrimData(oRowCreditNote.level) + "' and \"U_ProgramType\" = '" + TrimData(oRowCreditNote.program_type) + "'", GlobalVar.myCompany);

                                                if (oTaxCode != "")
                                                    oCreditNote.Lines.VatGroup = oTaxCode;

                                                oItemDescription = iCardName + " - " + oDateFor + " - " + oRowCreditNoteDtls.description;

                                                oCreditNote.Lines.UserFields.Fields.Item("U_Dscription").Value = oItemDescription;

                                                string Dscription = string.Empty;
                                                if (oItemDescription.Length > 100)
                                                {
                                                    Dscription = oItemDescription.Substring(0, 100);
                                                    oCreditNote.Lines.ItemDescription = Dscription;
                                                }
                                                else
                                                {
                                                    oCreditNote.Lines.ItemDescription = oItemDescription;
                                                }

                                                oCreditNote.Lines.LineTotal = oRowCreditNoteDtls.amount;

                                                if (!string.IsNullOrEmpty(oRowCreditNote.date_created))
                                                    idate_created = oRowCreditNote.date_created;

                                                if (!string.IsNullOrEmpty(oRowCreditNoteDtls.date_for))
                                                    idate_for = oRowCreditNoteDtls.date_for;

                                                if (!string.IsNullOrEmpty(Helpers.GlobalVar.oCountry))
                                                    oCreditNote.Lines.CostingCode = Helpers.GlobalVar.oCountry;

                                                if (!string.IsNullOrEmpty(Helpers.GlobalVar.oGroup))
                                                    oCreditNote.Lines.CostingCode2 = Helpers.GlobalVar.oGroup;

                                                if (!string.IsNullOrEmpty(Helpers.GlobalVar.oDivision))
                                                    oCreditNote.Lines.CostingCode3 = Helpers.GlobalVar.oDivision;

                                                if (!string.IsNullOrEmpty(Helpers.GlobalVar.oProduct))
                                                    oCreditNote.Lines.CostingCode4 = Helpers.GlobalVar.oProduct;

                                                if (idate_for != "")
                                                    oCreditNote.Lines.UserFields.Fields.Item("U_date_for").Value = Convert.ToDateTime(idate_for);

                                                if (CheckDate(idate_created) == true && CheckDate(idate_for) == true)
                                                {
                                                    if (Convert.ToDateTime(idate_for) > Convert.ToDateTime(idate_created))
                                                    {
                                                        iGLAccount = (String)clsSBOGetRecord.GetSingleValue("select \"U_FuturePeriod\" from \"@GLACCTMAPPING\" where \"U_Description\" = '" + TrimData(description) + "' and \"U_Level\" = '" + TrimData(oRowCreditNote.level) + "' and \"U_ProgramType\" = '" + TrimData(oRowCreditNote.program_type) + "'", GlobalVar.myCompany);
                                                    }
                                                    else
                                                    {
                                                        iGLAccount = (String)clsSBOGetRecord.GetSingleValue("select \"U_CurrentPeriod\" from \"@GLACCTMAPPING\" where \"U_Description\" = '" + TrimData(description) + "' and \"U_Level\" = '" + TrimData(oRowCreditNote.level) + "' and \"U_ProgramType\" = '" + TrimData(oRowCreditNote.program_type) + "'", GlobalVar.myCompany);
                                                    }
                                                }

                                                if (!string.IsNullOrEmpty(iGLAccount))
                                                    oCreditNote.Lines.AccountCode = iGLAccount;

                                                oCreditNote.Lines.Add();
                                            }
                                            else
                                            {
                                                lastMessage = "Description:" + oRowCreditNoteDtls.description + ", Level: " + oRowCreditNote.level + " or Program type:" + oRowCreditNote.program_type + " is not defined in SAP B1. Please define in the table.";
                                                string oQuery = "update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(oRowCreditNote.status == 1, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Note' and \"uniqueId\" = '" + oRowCreditNote.id + "'";
                                                GlobalVar.oRS.DoQuery(oQuery);

                                                ifunctionReturnValue = "0";

                                                goto isAddWithError;
                                            }
                                        }
                                    }
                                }

                                if (iDocType == "dDocument_Items")
                                    oCreditNote.DocType = BoDocumentTypes.dDocument_Items;
                                else
                                    oCreditNote.DocType = BoDocumentTypes.dDocument_Service;

                                olErrCode = oCreditNote.Add();
                                if (olErrCode == 0)
                                {
                                    try
                                    {
                                        iDocEntry = GlobalVar.myCompany.GetNewObjectKey();
                                        ifunctionReturnValue = iDocEntry;
                                    }
                                    catch
                                    { }
                                }
                                else
                                {
                                    lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                    GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(oRowCreditNote.status == 1, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.myCompany.CompanyDB) + "' and \"module\" = 'Credit Note' and \"uniqueId\" = '" + oRowCreditNote.id + "'");

                                    ifunctionReturnValue = "0";

                                    goto isAddWithError;
                                }

                            isAddWithError: ;

                                System.Runtime.InteropServices.Marshal.ReleaseComObject(oCreditNote);
                            }
                            else
                                ifunctionReturnValue = iDocEntry;
                        }
                    }
                    catch (Exception ex)
                    {
                        lastMessage = ex.ToString();
                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(oRowCreditNote.status == 1, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "'  and \"module\" = 'Credit Note' and \"uniqueId\" = '" + oRowCreditNote.id + "'");

                        ifunctionReturnValue = "0";
                    }
                }
                return Convert.ToInt16(ifunctionReturnValue);
            }
        }

        public static Int16 CreateReceiptVoid(List<Models.API_Receipt> listReceipt)
        {
            string functionReturnValue = "";
            int lErrCode = 0;
            string oLogExist = string.Empty;
            string oTransId = string.Empty;
            string oCardCode = string.Empty;
            string oCardName = string.Empty;
            string oDocEntry = string.Empty;
            string oInvDocEntry = string.Empty;
            string oCreditNoteDocEntry = string.Empty;
            string oModeOfPayment = string.Empty;
            string oAcctCode = string.Empty;
            string oBankName = string.Empty;
            string oCheckBankName = string.Empty;
            string iReference = string.Empty;

            SBOGetRecord clsSBOGetRecord = new SBOGetRecord();
            try
            {
                foreach (var iRowReceipt in listReceipt)
                {
                    try
                    {
                        //0 = no offset
                        //1 = has both payment and offset
                        //2 = only offset type

                        if (iRowReceipt.payment_type == 0 || iRowReceipt.payment_type == 1)
                        {
                            oIncomingPayment = (Payments)GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.oPaymentsDrafts);
                            if (iRowReceipt.status == 1)
                            {
                                oTransId = (String)clsSBOGetRecord.GetSingleValue("select \"U_TransId\" from \"OPDF\" where \"U_TransId\" = '" + iRowReceipt.id + "' and \"ObjType\" = 24", GlobalVar.myCompany);
                                oIncomingPayment.DocObjectCode = BoPaymentsObjectType.bopot_IncomingPayments;
                                if (oTransId == "" || oTransId == "0")
                                {
                                    oCardCode = (String)clsSBOGetRecord.GetSingleValue("select \"CardCode\" from \"OCRD\" where \"CardCode\" = '" + TrimData(iRowReceipt.student) + "'", GlobalVar.myCompany);
                                    if (oCardCode != "")
                                    {
                                        oIncomingPayment.CardCode = oCardCode;
                                    }
                                    else
                                    {
                                        lastMessage = "Customer Code:" + iRowReceipt.student + " is not found in SAP B1";
                                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowReceipt.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");

                                        functionReturnValue = "0";

                                        goto isAddWithError;
                                    }

                                    oIncomingPayment.DocTypte = BoRcptTypes.rCustomer;
                                    oIncomingPayment.DocDate = Convert.ToDateTime(iRowReceipt.date_created);

                                    ////**** UDF ****\\\\     
                                    oIncomingPayment.UserFields.Fields.Item("U_TransId").Value = iRowReceipt.id.ToString();
                                    oIncomingPayment.UserFields.Fields.Item("U_Status").Value = iRowReceipt.status.ToString();
                                    oIncomingPayment.UserFields.Fields.Item("U_Level").Value = iRowReceipt.level;
                                    oIncomingPayment.UserFields.Fields.Item("U_ProgramType").Value = iRowReceipt.program_type;
                                    oIncomingPayment.UserFields.Fields.Item("U_ReceiptNo").Value = iRowReceipt.receipt_no;
                                    ////**** UDF ****\\\\

                                    if (iRowReceipt.status == 0)
                                        oIncomingPayment.Remarks = iRowReceipt.remarks;
                                    else
                                        oIncomingPayment.Remarks = iRowReceipt.void_remarks;

                                    ////**** Adding of List of Invoice to Incoming Payment ****\\\\
                                    int invoiceCount = 0;
                                    int invPaidCount;
                                    foreach (var iRowReceiptInvDtl in iRowReceipt.invoice_id.ToList())
                                    {
                                        invoiceCount += 1;
                                        oInvDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"OINV\" where \"U_TransId\" = '" + iRowReceiptInvDtl.ToString() + "' and \"CANCELED\" = 'N'", GlobalVar.myCompany);
                                        if (oInvDocEntry != "" && oInvDocEntry != "0")
                                        {
                                            invPaidCount = 0;
                                            foreach (var iRowReceiptInvPaidDtl in iRowReceipt.invoice_paid.ToList())
                                            {
                                                invPaidCount += 1;
                                                if (invoiceCount == invPaidCount)
                                                {
                                                    oIncomingPayment.Invoices.DocEntry = Convert.ToInt16(oInvDocEntry);
                                                    oIncomingPayment.Invoices.InvoiceType = BoRcptInvTypes.it_Invoice;
                                                    oIncomingPayment.Invoices.SumApplied = Convert.ToDouble(iRowReceiptInvPaidDtl.ToString());
                                                    oIncomingPayment.Invoices.Add();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            lastMessage = "Invoice with Transaction id:" + iRowReceiptInvDtl.ToString() + " does not exist in SAP B1.";
                                            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowReceipt.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");

                                            functionReturnValue = "0";

                                            goto isAddWithError;
                                        }
                                    }
                                    ////**** Adding of List of Invoice to Incoming Payment ****\\\\

                                    ////**** Adding of List of Credit Note to Incoming Payment ****\\\\
                                    iReference = string.Empty;
                                    foreach (var iRowReceiptInvDtls in iRowReceipt.payment_methods.ToList())
                                    {
                                        if (iRowReceiptInvDtls.method == 3 || iRowReceiptInvDtls.method == 8 || iRowReceiptInvDtls.method == 10) //**OFFSET_DEPOSIT = 3**\\
                                        {
                                            if (!string.IsNullOrEmpty(iRowReceiptInvDtls.reference_id) && iRowReceiptInvDtls.reference_id != "N.A")
                                            {
                                                oCreditNoteDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ORIN\" where \"U_TransId\" = '" + TrimData(iRowReceiptInvDtls.reference_id) + "' and \"CANCELED\" = 'N' and \"U_CreatedByVoucher\" = 0", GlobalVar.myCompany);
                                                if (oCreditNoteDocEntry != "" && oCreditNoteDocEntry != "0")
                                                {
                                                    oIncomingPayment.Invoices.DocEntry = Convert.ToInt16(oCreditNoteDocEntry);
                                                    oIncomingPayment.Invoices.InvoiceType = BoRcptInvTypes.it_CredItnote;
                                                    oIncomingPayment.Invoices.Add();
                                                }
                                                else
                                                {
                                                    string oDraftDocEntry = string.Empty;
                                                    oDraftDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ODRF\" where \"U_TransId\" = '" + iRowReceiptInvDtls.reference_id + "' and \"CANCELED\" = 'N' and \"ObjType\" = 14", GlobalVar.myCompany);
                                                    if (oDraftDocEntry != "" && oDraftDocEntry != "0")
                                                    {
                                                        SAPbobsCOM.Documents oDraft = (Documents)GlobalVar.myCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                                                        if (oDraft.GetByKey(Convert.ToInt16(oDraftDocEntry)))
                                                        {
                                                            int ErrCode = oDraft.SaveDraftToDocument();
                                                            if (ErrCode == 0)
                                                            {
                                                                oCreditNoteDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ORIN\" where \"U_TransId\" = '" + iRowReceiptInvDtls.reference_id + "' and \"CANCELED\" = 'N' and \"U_CreatedByVoucher\" = 0", GlobalVar.myCompany);
                                                                if (oCreditNoteDocEntry != "" && oCreditNoteDocEntry != "0")
                                                                {
                                                                    oIncomingPayment.Invoices.DocEntry = Convert.ToInt16(oCreditNoteDocEntry);
                                                                    oIncomingPayment.Invoices.InvoiceType = BoRcptInvTypes.it_CredItnote;
                                                                    oIncomingPayment.Invoices.Add();
                                                                }
                                                            }
                                                            else
                                                            {
                                                                lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                                                GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowReceipt.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");

                                                                functionReturnValue = "0";

                                                                goto isAddWithError;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        lastMessage = "Credit Note with Reference Id:" + iRowReceiptInvDtls.reference_id + " does not exist in SAP B1 Drafts";
                                                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowReceipt.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");

                                                        functionReturnValue = "0";

                                                        goto isAddWithError;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (iRowReceiptInvDtls.reference != "N.A")
                                                    iReference += iRowReceiptInvDtls.reference + ", ";
                                            }
                                        }
                                        else
                                        {
                                            if (iRowReceiptInvDtls.reference != "N.A")
                                                iReference += iRowReceiptInvDtls.reference + ", ";
                                        }
                                    }
                                    ////**** Adding of List of Credit Note to Incoming Payment ****\\\\

                                    string oJournalRemarks = string.Empty;
                                    if (!string.IsNullOrEmpty(iReference))
                                    {
                                        oJournalRemarks = iReference.Substring(0, iReference.Length - 2);
                                    }

                                    if (!string.IsNullOrEmpty(oJournalRemarks))
                                        oIncomingPayment.JournalRemarks = oJournalRemarks;

                                    ////**** Payment Means for the List of Invoices ****\\\\
                                    foreach (var iRowReceiptDtls in iRowReceipt.payment_methods.ToList())
                                    {
                                        if (string.IsNullOrEmpty(iRowReceiptDtls.reference_id) || iRowReceiptDtls.reference_id == "N.A")
                                        {
                                            oAcctCode = (String)clsSBOGetRecord.GetSingleValue("select \"U_GLAccount\" from \"@PAYMENTCODES\" where \"U_PaymentCodeMethod\" = " + iRowReceiptDtls.method + "", GlobalVar.myCompany);
                                            oModeOfPayment = (String)clsSBOGetRecord.GetSingleValue("select \"U_ModePayment\" from \"@PAYMENTCODES\" where \"U_PaymentCodeMethod\" = " + iRowReceiptDtls.method + "", GlobalVar.myCompany);

                                            if (oModeOfPayment == "CA")
                                            {
                                                if (!string.IsNullOrEmpty(oAcctCode))
                                                    oIncomingPayment.CashAccount = oAcctCode;

                                                if (iRowReceiptDtls.amount != 0)
                                                    oIncomingPayment.CashSum = iRowReceiptDtls.amount;
                                            }
                                            else if (oModeOfPayment == "CK")
                                            {
                                                if (!string.IsNullOrEmpty(oAcctCode))
                                                    oIncomingPayment.CheckAccount = oAcctCode;

                                                if (iRowReceiptDtls.amount != 0)
                                                    oIncomingPayment.Checks.CheckSum = iRowReceiptDtls.amount;

                                                oIncomingPayment.Checks.Add();
                                            }
                                            else if (oModeOfPayment == "BT")
                                            {
                                                oIncomingPayment.TransferReference = iRowReceiptDtls.reference;

                                                if (!string.IsNullOrEmpty(oAcctCode))
                                                    oIncomingPayment.TransferAccount = oAcctCode;

                                                if (iRowReceiptDtls.amount != 0)
                                                    oIncomingPayment.TransferSum = iRowReceiptDtls.amount;
                                            }
                                            else if (oModeOfPayment == "CC")
                                            {
                                                //string creditCardName = cls.GetSingleValue("SELECT \"CreditCard\" FROM OCRC WHERE \"CardName\" = '" + oIncomingPaymentLines.creditCardName + "'", company);
                                                //if (creditCardName != "")
                                                //{
                                                //    oIncomingPayment.CreditCards.CreditCard = Convert.ToInt16(creditCardName);
                                                //    oIncomingPayment.CreditCards.CardValidUntil = Convert.ToDateTime(oIncomingPaymentLines.creditCardValidDate);
                                                //    oIncomingPayment.CreditCards.CreditCardNumber = oIncomingPaymentLines.creditCardNumber;

                                                //    if (oIncomingPaymentLines.creditCardAmount != 0)
                                                //        oIncomingPayment.CreditCards.CreditSum = oIncomingPaymentLines.creditCardAmount;

                                                //    oIncomingPayment.CreditCards.VoucherNum = oIncomingPaymentLines.creditCardApproval;
                                                //    oIncomingPayment.CreditCards.Add();
                                                //}
                                            }
                                            else if (oModeOfPayment == "CN")
                                            {
                                                string oDocDate = string.Empty;
                                                string CNDesc = string.Empty;
                                                if (!string.IsNullOrEmpty(iRowReceiptDtls.reference_id))
                                                { }
                                                else
                                                {
                                                    string oVoucherTaxCode = (String)clsSBOGetRecord.GetSingleValue("select \"U_TaxCode\" from \"@PAYMENTCODES\" where \"U_PaymentCodeMethod\" = " + iRowReceiptDtls.method + "", GlobalVar.myCompany);

                                                    oCardName = (String)clsSBOGetRecord.GetSingleValue("select \"CardName\" from \"OCRD\" where \"CardCode\" = '" + TrimData(iRowReceipt.student) + "'", GlobalVar.myCompany);

                                                    CNDesc = oCardName + " Voucher " + Convert.ToDateTime(iRowReceipt.date_created).ToString("MMM") + " " + Convert.ToDateTime(iRowReceipt.date_created).Year + " " + iRowReceipt.level + " " + iRowReceipt.program_type;

                                                    Int16 CNDocEntry = CreateCreditNoteVoucher(oCardCode, iRowReceipt.receipt_no, iRowReceipt.date_created, CNDesc, oAcctCode, iRowReceiptDtls.amount, oVoucherTaxCode, iRowReceipt.invoice_no[0].ToString());
                                                    if (CNDocEntry != 0)
                                                    {
                                                        oIncomingPayment.Invoices.DocEntry = CNDocEntry;
                                                        oIncomingPayment.Invoices.InvoiceType = BoRcptInvTypes.it_CredItnote;
                                                        oIncomingPayment.Invoices.Add();
                                                    }
                                                    else
                                                    {
                                                        lastMessage = "Credit Note (Voucher) with Transaction id:" + iRowReceipt.id + " and Receipt No:" + iRowReceipt.receipt_no + " does not exist in SAP B1.";
                                                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowReceipt.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");

                                                        functionReturnValue = "0";

                                                        goto isAddWithError;
                                                    }
                                                }
                                            }
                                            else if (oModeOfPayment == "NA")
                                            { }
                                            else
                                            { }
                                        }
                                    }
                                    ////**** Payment Means for the List of Invoices and Credit Note ****\\\\

                                    lErrCode = oIncomingPayment.Add();

                                    if (lErrCode == 0)
                                    {
                                        try
                                        {
                                            oDocEntry = GlobalVar.myCompany.GetNewObjectKey();
                                            lastMessage = "Successfully created Incoming Payment (Draft) with Transaction Id:" + iRowReceipt.id + " in SAP B1.";
                                            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Draft',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "',\"objType\" = 24 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");

                                            functionReturnValue = oDocEntry;
                                        }
                                        catch
                                        { }
                                    }
                                    else
                                    {
                                        lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowReceipt.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");

                                        functionReturnValue = "0";

                                        goto isAddWithError;
                                    }

                                isAddWithError: ;

                                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oIncomingPayment);

                                }
                                else
                                {
                                    oDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"OPDF\" where \"U_TransId\" = '" + iRowReceipt.id + "' and \"ObjType\" = 24", GlobalVar.myCompany);

                                    lastMessage = "Incoming Payment with Transaction Id:" + iRowReceipt.id + " is already existing in SAP B1.";
                                    GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowReceipt.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "',\"objType\" = 24 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");

                                    functionReturnValue = "0";
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lastMessage = ex.ToString();
                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowReceipt.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Receipt' and \"uniqueId\" = '" + iRowReceipt.id + "'");
                        functionReturnValue = "0";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Convert.ToInt16(functionReturnValue);
        }

        public static Int16 CreateCreditNoteVoucher(string student, string receipt_no, string create_date, string description, string account, float amount, string vatGroup, string invoice_no)
        {
            string functionReturnValue = string.Empty;
            string lastErrorMessage = string.Empty;
            string oDocEntry = string.Empty;
            int lErrCode;
            SBOGetRecord clsSBOGetValue = new SBOGetRecord();
            try
            {
                oDocEntry = (String)clsSBOGetValue.GetSingleValue("select \"DocEntry\" from \"ORIN\" where \"U_ReceiptNo\" = '" + receipt_no + "' and \"CANCELED\" = 'N' and \"U_CreatedByVoucher\" = 1", GlobalVar.myCompany);
                if (oDocEntry == "" || oDocEntry == "0")
                {
                    oCreditNote = (Documents)GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.oCreditNotes);

                    oCreditNote.CardCode = student;
                    oCreditNote.NumAtCard = receipt_no;
                    oCreditNote.DocDate = Convert.ToDateTime(create_date);
                    oCreditNote.DocType = BoDocumentTypes.dDocument_Service;
                    oCreditNote.Comments = "Invoice No:" + invoice_no + ", Receipt No:" + receipt_no;

                    oCreditNote.UserFields.Fields.Item("U_ReceiptNo").Value = receipt_no;
                    oCreditNote.UserFields.Fields.Item("U_CreatedByVoucher").Value = 1;

                    if (!string.IsNullOrEmpty(Helpers.GlobalVar.oCountry))
                        oCreditNote.Lines.CostingCode = Helpers.GlobalVar.oCountry;

                    if (!string.IsNullOrEmpty(Helpers.GlobalVar.oGroup))
                        oCreditNote.Lines.CostingCode2 = Helpers.GlobalVar.oGroup;

                    if (!string.IsNullOrEmpty(Helpers.GlobalVar.oDivision))
                        oCreditNote.Lines.CostingCode3 = Helpers.GlobalVar.oDivision;

                    if (!string.IsNullOrEmpty(Helpers.GlobalVar.oProduct))
                        oCreditNote.Lines.CostingCode4 = Helpers.GlobalVar.oProduct;

                    oCreditNote.Lines.AccountCode = account;
                    oCreditNote.Lines.UserFields.Fields.Item("U_Dscription").Value = description;

                    string Dscription = string.Empty;
                    if (description.Length > 100)
                    {
                        Dscription = description.Substring(0, 100);
                        oCreditNote.Lines.ItemDescription = Dscription;
                    }
                    else
                    {
                        oCreditNote.Lines.ItemDescription = description;
                    }

                    oCreditNote.Lines.VatGroup = vatGroup;
                    oCreditNote.Lines.LineTotal = amount / 1.07;

                    lErrCode = oCreditNote.Add();
                    if (lErrCode == 0)
                    {
                        try
                        {
                            functionReturnValue = GlobalVar.myCompany.GetNewObjectKey();
                        }
                        catch
                        { }
                    }
                    else
                    {
                        lastErrorMessage = GlobalVar.myCompany.GetLastErrorDescription();
                        functionReturnValue = "0";
                    }
                }
                else
                    functionReturnValue = oDocEntry;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Convert.ToInt16(functionReturnValue);
        }

        public static Int16 CreateCreditRefundVoid(List<Models.API_CreditRefund> listCreditRefund)
        {
            string functionReturnValue = "";
            int lErrCode = 0;
            string oLogExist = string.Empty;
            string oTransId = string.Empty;
            string oCardCode = string.Empty;
            string oDocEntry = string.Empty;
            string oCreditNoteDocEntry = string.Empty;
            string oInvoiceDocEntry = string.Empty;
            string oCountryCod = string.Empty;
            string oModeOfPayment = string.Empty;
            string oAcctCode = string.Empty;
            string oBankCode = string.Empty;
            string oBankName = string.Empty;
            string oBranch = string.Empty;
            string oCheckAccount = string.Empty;
            string oCheckNumber = string.Empty;
            string iReference = string.Empty;
            SBOGetRecord clsSBOGetRecord = new SBOGetRecord();
            try
            {
                foreach (var iRowCreditRefund in listCreditRefund)
                {
                    try
                    {
                        oOutgoingPayment = (Payments)GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.oPaymentsDrafts);
                        if (iRowCreditRefund.status == 1)
                        {
                            oTransId = (String)clsSBOGetRecord.GetSingleValue("select \"U_TransId\" from \"OPDF\" where \"U_TransId\" = '" + iRowCreditRefund.id + "' and \"ObjType\" = 46", GlobalVar.myCompany);
                            oOutgoingPayment.DocObjectCode = BoPaymentsObjectType.bopot_OutgoingPayments;
                            if (oTransId == "" || oTransId == "0")
                            {
                                oCardCode = (String)clsSBOGetRecord.GetSingleValue("select \"CardCode\" from \"OCRD\" where \"CardCode\" = '" + TrimData(iRowCreditRefund.student) + "'", GlobalVar.myCompany);
                                if (oCardCode != "")
                                {
                                    oOutgoingPayment.CardCode = oCardCode;
                                }
                                else
                                {
                                    lastMessage = "Customer Code:" + iRowCreditRefund.student + " is not found in SAP B1";
                                    GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditRefund.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                    functionReturnValue = "0";

                                    goto isAddWithError;
                                }

                                oOutgoingPayment.DocType = BoRcptTypes.rCustomer;
                                oOutgoingPayment.DocDate = Convert.ToDateTime(iRowCreditRefund.date_created);

                                if (iRowCreditRefund.payment_reference != "N.A" && !string.IsNullOrEmpty(iRowCreditRefund.payment_reference))
                                    oOutgoingPayment.JournalRemarks = iRowCreditRefund.payment_reference;

                                ////**** UDF ****\\\\
                                oOutgoingPayment.UserFields.Fields.Item("U_TransId").Value = iRowCreditRefund.id.ToString();
                                oOutgoingPayment.UserFields.Fields.Item("U_Status").Value = iRowCreditRefund.status.ToString();
                                oOutgoingPayment.UserFields.Fields.Item("U_CreditType").Value = iRowCreditRefund.credit_type;
                                ////**** UDF ****\\\\

                                if (iRowCreditRefund.status == 0)
                                    oOutgoingPayment.Remarks = iRowCreditRefund.remarks;
                                else
                                    oOutgoingPayment.Remarks = iRowCreditRefund.void_remarks;


                                if (iRowCreditRefund.overpaid_offsets == 0)
                                {
                                    //** credit id **//
                                    oCreditNoteDocEntry = string.Empty;
                                    oCreditNoteDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ORIN\" where \"U_TransId\" = '" + iRowCreditRefund.credit_id + "' and \"CANCELED\" = 'N'", GlobalVar.myCompany);
                                    if (oCreditNoteDocEntry != "" && oCreditNoteDocEntry != "0")
                                    {
                                        oOutgoingPayment.Invoices.DocEntry = Convert.ToInt16(oCreditNoteDocEntry);
                                        oOutgoingPayment.Invoices.InvoiceType = BoRcptInvTypes.it_CredItnote;
                                        oOutgoingPayment.Invoices.Add();
                                    }
                                    else
                                    {
                                        string oDraftDocEntry = string.Empty;
                                        oDraftDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ODRF\" where \"U_TransId\" = '" + iRowCreditRefund.credit_id + "' and \"CANCELED\" = 'N'", GlobalVar.myCompany);
                                        if (oDraftDocEntry != "" && oDraftDocEntry != "0")
                                        {
                                            SAPbobsCOM.Documents oDraft = (Documents)GlobalVar.myCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                                            if (oDraft.GetByKey(Convert.ToInt16(oDraftDocEntry)))
                                            {
                                                int ErrCode = oDraft.SaveDraftToDocument();
                                                if (ErrCode == 0)
                                                {
                                                    oCreditNoteDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ORIN\" where \"U_TransId\" = '" + iRowCreditRefund.credit_id + "' and \"CANCELED\" = 'N'", GlobalVar.myCompany);
                                                    if (oCreditNoteDocEntry != "" && oCreditNoteDocEntry != "0")
                                                    {
                                                        oOutgoingPayment.Invoices.DocEntry = Convert.ToInt16(oCreditNoteDocEntry);
                                                        oOutgoingPayment.Invoices.InvoiceType = BoRcptInvTypes.it_CredItnote;
                                                        oOutgoingPayment.Invoices.Add();
                                                    }
                                                }
                                                else
                                                {
                                                    lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                                    GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditRefund.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                                    functionReturnValue = "0";

                                                    goto isAddWithError;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            lastMessage = "Credit Note (Draft) with Reference Id:" + iRowCreditRefund.credit_id + " does not exist in SAP B1.";
                                            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditRefund.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                            functionReturnValue = "0";

                                            goto isAddWithError;

                                        }
                                    }
                                    //** credit id **//
                                }
                                else
                                {
                                    //**overpaid_offsets_credit_notes**//
                                    foreach (var iRowCreditRefundCN in iRowCreditRefund.overpaid_offsets_credit_notes.ToList())
                                    {
                                        oCreditNoteDocEntry = string.Empty;
                                        oCreditNoteDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ORIN\" where \"U_TransId\" = '" + iRowCreditRefundCN.ToString() + "' and \"CANCELED\" = 'N'", GlobalVar.myCompany);
                                        if (oCreditNoteDocEntry != "" && oCreditNoteDocEntry != "0")
                                        {
                                            oOutgoingPayment.Invoices.DocEntry = Convert.ToInt16(oCreditNoteDocEntry);
                                            oOutgoingPayment.Invoices.InvoiceType = BoRcptInvTypes.it_CredItnote;
                                            oOutgoingPayment.Invoices.Add();
                                        }
                                        else
                                        {
                                            string oDraftDocEntry = string.Empty;
                                            oDraftDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ODRF\" where \"U_TransId\" = '" + iRowCreditRefundCN.ToString() + "' and \"CANCELED\" = 'N' and \"ObjType\" = 14", GlobalVar.myCompany);
                                            if (oDraftDocEntry != "" && oDraftDocEntry != "0")
                                            {
                                                SAPbobsCOM.Documents oDraft = (Documents)GlobalVar.myCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                                                if (oDraft.GetByKey(Convert.ToInt16(oDraftDocEntry)))
                                                {
                                                    int ErrCode = oDraft.SaveDraftToDocument();
                                                    if (ErrCode == 0)
                                                    {
                                                        oCreditNoteDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ORIN\" where \"U_TransId\" = '" + iRowCreditRefundCN.ToString() + "' and \"CANCELED\" = 'N'", GlobalVar.myCompany);
                                                        if (oCreditNoteDocEntry != "" && oCreditNoteDocEntry != "0")
                                                        {
                                                            oOutgoingPayment.Invoices.DocEntry = Convert.ToInt16(oCreditNoteDocEntry);
                                                            oOutgoingPayment.Invoices.InvoiceType = BoRcptInvTypes.it_CredItnote;
                                                            oOutgoingPayment.Invoices.Add();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditRefund.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                                        functionReturnValue = "0";

                                                        goto isAddWithError;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                lastMessage = "Credit Note (Draft) with Reference Id:" + iRowCreditRefundCN.ToString() + " does not exist in SAP B1.";
                                                GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditRefund.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                                functionReturnValue = "0";

                                                goto isAddWithError;

                                            }
                                        }
                                    }
                                    //**overpaid_offsets_credit_notes**//
                                }

                                //**overpaid_offsets_invoices**//
                                foreach (var iRowCreditRefundInv in iRowCreditRefund.overpaid_offsets_invoices.ToList())
                                {
                                    oInvoiceDocEntry = string.Empty;
                                    oInvoiceDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"OINV\" where \"U_TransId\" = '" + iRowCreditRefundInv.ToString() + "' and \"CANCELED\" = 'N'", GlobalVar.myCompany);
                                    if (oInvoiceDocEntry != "" && oInvoiceDocEntry != "0")
                                    {
                                        oOutgoingPayment.Invoices.DocEntry = Convert.ToInt16(oInvoiceDocEntry);
                                        oOutgoingPayment.Invoices.InvoiceType = BoRcptInvTypes.it_Invoice;
                                        oOutgoingPayment.Invoices.Add();
                                    }
                                    else
                                    {
                                        string oDraftDocEntry = string.Empty;
                                        oDraftDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"ODRF\" where \"U_TransId\" = '" + iRowCreditRefundInv.ToString() + "' and \"CANCELED\" = 'N' and \"ObjType\" = 13", GlobalVar.myCompany);
                                        if (oDraftDocEntry != "" && oDraftDocEntry != "0")
                                        {
                                            SAPbobsCOM.Documents oDraft = (Documents)GlobalVar.myCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                                            if (oDraft.GetByKey(Convert.ToInt16(oDraftDocEntry)))
                                            {
                                                int ErrCode = oDraft.SaveDraftToDocument();
                                                if (ErrCode == 0)
                                                {
                                                    oInvoiceDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"OINV\" where \"U_TransId\" = '" + iRowCreditRefundInv.ToString() + "' and \"CANCELED\" = 'N'", GlobalVar.myCompany);
                                                    if (oInvoiceDocEntry != "" && oInvoiceDocEntry != "0")
                                                    {
                                                        oOutgoingPayment.Invoices.DocEntry = Convert.ToInt16(oCreditNoteDocEntry);
                                                        oOutgoingPayment.Invoices.InvoiceType = BoRcptInvTypes.it_Invoice;
                                                        oOutgoingPayment.Invoices.Add();
                                                    }
                                                }
                                                else
                                                {
                                                    lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                                    GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditRefund.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                                    functionReturnValue = "0";

                                                    goto isAddWithError;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            lastMessage = "Invoice (Draft) with Reference Id:" + iRowCreditRefundInv.ToString() + " does not exist in SAP B1.";
                                            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditRefund.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                            functionReturnValue = "0";

                                            goto isAddWithError;

                                        }
                                    }
                                }
                                //**overpaid_offsets_invoices**//


                                ////**** Payment Means for the List of Credit Note ****\\\\
                                oAcctCode = (String)clsSBOGetRecord.GetSingleValue("select \"U_GLAccount\" from \"@PAYMENTCODES\" where \"U_PaymentCodeMethod\" = '" + iRowCreditRefund.payment_method + "'", GlobalVar.myCompany);
                                oModeOfPayment = (String)clsSBOGetRecord.GetSingleValue("select \"U_ModePayment\" from \"@PAYMENTCODES\" where \"U_PaymentCodeMethod\" = '" + iRowCreditRefund.payment_method + "'", GlobalVar.myCompany);

                                if (oModeOfPayment == "CA")
                                {
                                    if (!string.IsNullOrEmpty(oAcctCode))
                                        oOutgoingPayment.CashAccount = oAcctCode;

                                    if (iRowCreditRefund.amount != 0)
                                        oOutgoingPayment.CashSum = iRowCreditRefund.amount;
                                }
                                else if (oModeOfPayment == "CK")
                                {
                                    if (!string.IsNullOrEmpty(iRowCreditRefund.payment_reference))
                                    {
                                        oBankName = iRowCreditRefund.payment_reference.Substring(0, iRowCreditRefund.payment_reference.IndexOf(' '));

                                        oCheckNumber = iRowCreditRefund.payment_reference.Replace(oBankName, "");

                                        oBankCode = (String)clsSBOGetRecord.GetSingleValue("select \"BankCode\" from \"ODSC\" where \"BankName\" = '" + TrimData(oBankName) + "'", GlobalVar.myCompany);

                                        if (string.IsNullOrEmpty(oBankCode))
                                        {
                                            lastMessage = "Bank:" + oBankName + " is not found in SAP B1";
                                            GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditRefund.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                            functionReturnValue = "0";

                                            goto isAddWithError;
                                        }

                                        oCountryCod = (String)clsSBOGetRecord.GetSingleValue("select \"Country\" from \"DSC1\" where \"BankCode\" = '" + TrimData(oBankCode) + "'", GlobalVar.myCompany);

                                        oAcctCode = (String)clsSBOGetRecord.GetSingleValue("select \"GLAccount\" from \"DSC1\" where \"BankCode\" = '" + TrimData(oBankCode) + "'", GlobalVar.myCompany);

                                        oCheckAccount = (String)clsSBOGetRecord.GetSingleValue("select \"Account\" from \"DSC1\" where \"BankCode\" = '" + TrimData(oBankCode) + "'", GlobalVar.myCompany);

                                        oBranch = (String)clsSBOGetRecord.GetSingleValue("select \"Branch\" from \"DSC1\" where \"BankCode\" = '" + TrimData(oBankCode) + "'", GlobalVar.myCompany);
                                    }

                                    if (!string.IsNullOrEmpty(oCountryCod))
                                        oOutgoingPayment.Checks.CountryCode = oCountryCod;

                                    if (!string.IsNullOrEmpty(oBankCode))
                                        oOutgoingPayment.Checks.BankCode = oBankCode;

                                    if (!string.IsNullOrEmpty(oAcctCode))
                                        oOutgoingPayment.Checks.CheckAccount = oAcctCode;

                                    if (!string.IsNullOrEmpty(oCheckAccount))
                                        oOutgoingPayment.Checks.AccounttNum = oCheckAccount;

                                    if (!string.IsNullOrEmpty(oBranch))
                                        oOutgoingPayment.Checks.Branch = oBranch;

                                    if (!string.IsNullOrEmpty(oCheckNumber))
                                        oOutgoingPayment.Checks.CheckNumber = Convert.ToInt32(TrimData(oCheckNumber));

                                    if (iRowCreditRefund.amount != 0)
                                        oOutgoingPayment.Checks.CheckSum = iRowCreditRefund.amount;

                                    oOutgoingPayment.Checks.Add();
                                }
                                else if (oModeOfPayment == "BT")
                                {
                                    if (iReference != "N.A" && iReference != "")
                                        oOutgoingPayment.TransferReference = iReference;

                                    if (string.IsNullOrEmpty(oAcctCode))
                                        oOutgoingPayment.TransferAccount = oAcctCode;

                                    if (iRowCreditRefund.amount != 0)
                                        oOutgoingPayment.TransferSum = iRowCreditRefund.amount;
                                }
                                else if (oModeOfPayment == "CC")
                                {
                                    //string creditCardName = cls.GetSingleValue("SELECT \"CreditCard\" FROM OCRC WHERE \"CardName\" = '" + oIncomingPaymentLines.creditCardName + "'", company);
                                    //if (creditCardName != "")
                                    //{
                                    //    oIncomingPayment.CreditCards.CreditCard = Convert.ToInt16(creditCardName);
                                    //    oIncomingPayment.CreditCards.CardValidUntil = Convert.ToDateTime(oIncomingPaymentLines.creditCardValidDate);
                                    //    oIncomingPayment.CreditCards.CreditCardNumber = oIncomingPaymentLines.creditCardNumber;

                                    //    if (oIncomingPaymentLines.creditCardAmount != 0)
                                    //        oIncomingPayment.CreditCards.CreditSum = oIncomingPaymentLines.creditCardAmount;

                                    //    oIncomingPayment.CreditCards.VoucherNum = oIncomingPaymentLines.creditCardApproval;
                                    //    oIncomingPayment.CreditCards.Add();
                                    //}
                                }
                                else if (oModeOfPayment == "CN")
                                { }
                                else if (oModeOfPayment == "NA")
                                { }
                                else
                                { }

                                ////**** Payment Means for the List of Invoices ****\\\\

                                lErrCode = oOutgoingPayment.Add();
                                if (lErrCode == 0)
                                {
                                    try
                                    {
                                        oDocEntry = GlobalVar.myCompany.GetNewObjectKey();
                                        lastMessage = "Successfully created Outgoing Payment (Draft) with Transaction Id:" + iRowCreditRefund.id + " in SAP B1.";
                                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Draft',\"failDesc\" = '',\"successDesc\" = '" + TrimData(lastMessage) + "',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "',\"objType\" = 46 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                        functionReturnValue = oDocEntry;
                                    }
                                    catch
                                    { }
                                }
                                else
                                {
                                    lastMessage = GlobalVar.myCompany.GetLastErrorDescription();
                                    GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditRefund.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + GlobalVar.oSERVERDB + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                    functionReturnValue = "0";

                                    goto isAddWithError;
                                }

                            isAddWithError: ;

                                System.Runtime.InteropServices.Marshal.ReleaseComObject(oOutgoingPayment);

                            }
                            else
                            {
                                oDocEntry = (String)clsSBOGetRecord.GetSingleValue("select \"DocEntry\" from \"OPDF\" where \"U_TransId\" = '" + iRowCreditRefund.id + "' and \"ObjType\" = 46", GlobalVar.myCompany);

                                lastMessage = "Outgoing Payment with Transaction Id:" + iRowCreditRefund.id + " is already existing in SAP B1.";
                                GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Posted',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + oDocEntry + "',\"objType\" = 46 where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");

                                functionReturnValue = "0";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lastMessage = ex.ToString();
                        GlobalVar.oRS.DoQuery("update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = '" + Iif(iRowCreditRefund.status == 0, "Draft", "Void") + "',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Credit Refund' and \"uniqueId\" = '" + iRowCreditRefund.id + "'");
                        functionReturnValue = "0";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Convert.ToInt16(functionReturnValue);
        }

        public static bool SBOPostFinanceItem(string id, string uniqueId, string oJSONResult)
        {
            try
            {
                string BaseUrl = string.Empty;
                string MethodUrl = string.Empty;
                string JSONResult = string.Empty;
                string oResponseResult = string.Empty;

                listResponseResultSuccess = new List<Models.ResponseResultSuccess>();
                listResponseResultFailed = new List<Models.ResponseResultFailed>();

                //Set Base URL Address for API Call
                BaseUrl = Helpers.GlobalVar.base_url;

                //Set Method for the API Call
                MethodUrl = "centeritem/create/";

                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(BaseUrl);

                HttpContent content = new FormUrlEncodedContent(
                new List<KeyValuePair<string, string>> { 
                        new KeyValuePair<string, string>("api_key", Helpers.GlobalVar.api_key),
                        new KeyValuePair<string,string>("client",Helpers.GlobalVar.client),
                        new KeyValuePair<string,string>("items",oJSONResult)
                    });

                HttpResponseMessage Response = httpClient.PostAsync(MethodUrl, content).Result;
                if (Response.IsSuccessStatusCode)
                {
                    oResponseResult = Response.Content.ReadAsStringAsync().Result;
                    if (oResponseResult.Contains("id") == true)
                    {
                        lastMessage = string.Empty;
                        listResponseResultSuccess = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.ResponseResultSuccess>>(oResponseResult);

                        foreach (var iRowSuccess in listResponseResultSuccess)
                        {
                            if (iRowSuccess.status == 1)
                            {
                                GlobalVar.strQuery = "update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'true',\"statusCode\" = 'Posted',\"failDesc\" = '',\"successDesc\" = 'Successfully " + Iif(iRowSuccess.log == "new", "created new Item", "updated existing item") + " in TAIDII Portal.',\"logDate\" = '" + Helpers.GlobalVar.myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + uniqueId + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Product' and \"uniqueId\" = '" + uniqueId + "'";
                            }
                            GlobalVar.oRS.DoQuery(GlobalVar.strQuery);
                        }
                        return false;
                    }
                    else
                    {
                        listResponseResultFailed = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.ResponseResultFailed>>(oResponseResult);
                        foreach (var iRowFailed in listResponseResultFailed)
                        {
                            if (iRowFailed.status == 0)
                            {
                                int errCnt = iRowFailed.errors.Count;
                                int counter = 0;
                                lastMessage = string.Empty;
                                foreach (var iRowFailedDtl in iRowFailed.errors.ToList())
                                {
                                    if (counter == 0 && errCnt != counter)
                                    {
                                        lastMessage += iRowFailedDtl.ToString() + ", ";
                                    }
                                    else
                                        lastMessage += iRowFailedDtl.ToString();

                                    counter += 1;
                                }

                                GlobalVar.strQuery = "update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"status\" = 'false',\"statusCode\" = 'For Process',\"failDesc\" = '" + TrimData(lastMessage) + "',\"successDesc\" = '',\"logDate\" = '" + Helpers.GlobalVar.myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "',\"sapCode\" = '" + uniqueId + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Product' and \"uniqueId\" = '" + uniqueId + "'";
                                GlobalVar.oRS.DoQuery(GlobalVar.strQuery);
                            }
                        }
                        return true;
                    }
                }
                else
                {
                    oResponseResult = Response.Content.ReadAsStringAsync().Result;
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<Models.API_FinanceItem> ItemMasterData(int PriceList = 0, string ItemCode = "")
        {
            try
            {
                //Declarations
                string oQuery = string.Empty;
                string oLogExist = string.Empty;
                SBOGetRecord clsSBOGetRecord = new SBOGetRecord();

                int oItmsGrpCod = Convert.ToInt16(clsSBOGetRecord.GetSingleValue("select \"ItmsGrpCod\" from \"OITB\" where \"ItmsGrpNam\" like '%MERCHANDISE%'", myCompany));

                ////** Declarations **//////
                ItemModel = new List<Models.API_FinanceItem>();

                if (ItemCode == "")
                {
                    oQuery = "select " + Environment.NewLine +
                   "\"a\".\"ItemCode\" \"item_code\", " + Environment.NewLine +
                   "\"a\".\"ItemName\" \"description\", " + Environment.NewLine +
                   "\"b\".\"ItmsGrpNam\" \"type\", " + Environment.NewLine +
                   "\"c\".\"Price\" \"unit_price\", " + Environment.NewLine +
                   "\"a\".\"FrgnName\" \"remarks\", " + Environment.NewLine +
                   "1 \"tax\" " + Environment.NewLine +
                   "from \"OITM\" \"a\" " + Environment.NewLine +
                   "left join \"OITB\" \"b\" on \"b\".\"ItmsGrpCod\" = \"a\".\"ItmsGrpCod\" " + Environment.NewLine +
                   "left join \"ITM1\" \"c\" on \"c\".\"ItemCode\" = \"a\".\"ItemCode\" " + Environment.NewLine +
                   "where \"c\".\"PriceList\" = " + PriceList + " " + Environment.NewLine +
                   "and \"a\".\"CreateDate\" = '" + Helpers.GlobalVar.myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' " + Environment.NewLine +
                   "and \"a\".\"ItmsGrpCod\" = " + oItmsGrpCod + " " + Environment.NewLine +
                   "or \"c\".\"PriceList\" = " + PriceList + " " + Environment.NewLine +
                   "and \"a\".\"UpdateDate\" = '" + Helpers.GlobalVar.myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' " + Environment.NewLine +
                   "and \"a\".\"ItmsGrpCod\" = " + oItmsGrpCod + "";
                }
                else
                {
                    oQuery = "select " + Environment.NewLine +
                    "\"a\".\"ItemCode\" \"item_code\", " + Environment.NewLine +
                    "\"a\".\"ItemName\" \"description\", " + Environment.NewLine +
                    "\"b\".\"ItmsGrpNam\" \"type\", " + Environment.NewLine +
                    "\"c\".\"Price\" \"unit_price\", " + Environment.NewLine +
                    "\"a\".\"FrgnName\" \"remarks\", " + Environment.NewLine +
                    "1 \"tax\" " + Environment.NewLine +
                    "from \"OITM\" \"a\" " + Environment.NewLine +
                    "left join \"OITB\" \"b\" on \"b\".\"ItmsGrpCod\" = \"a\".\"ItmsGrpCod\" " + Environment.NewLine +
                    "left join \"ITM1\" \"c\" on \"c\".\"ItemCode\" = \"a\".\"ItemCode\" " + Environment.NewLine +
                    "where \"c\".\"PriceList\" = " + PriceList + " " + Environment.NewLine +
                    "and \"a\".\"CreateDate\" = '" + Helpers.GlobalVar.myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' " + Environment.NewLine +
                    "and \"a\".\"ItemCode\" = '" + ItemCode + "' and \"a\".\"ItmsGrpCod\" = " + oItmsGrpCod + " " + Environment.NewLine +
                    "or \"c\".\"PriceList\" = " + PriceList + " " + Environment.NewLine +
                    "and \"a\".\"UpdateDate\" = '" + Helpers.GlobalVar.myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' " + Environment.NewLine +
                    "and \"a\".\"ItemCode\" = '" + ItemCode + "' " + Environment.NewLine +
                    "and \"a\".\"ItmsGrpCod\" = " + oItmsGrpCod + "";
                }

                GlobalVar.oRS.DoQuery(oQuery);

                ////** Parse Business Partners **//////
                XDocument xItemMasterData = XDocument.Parse(GlobalVar.oRS.GetAsXML());
                if (xItemMasterData.Root != null)
                {
                    ItemModel = xItemMasterData.Descendants("row").Select(oItemMaster =>
                    new Models.API_FinanceItem
                    {
                        item_code = oItemMaster.Element("item_code").Value,
                        description = oItemMaster.Element("description").Value,
                        type = oItemMaster.Element("type").Value,
                        unit_price = Convert.ToDouble(oItemMaster.Element("unit_price").Value),
                        remarks = oItemMaster.Element("remarks").Value,
                        tax = Convert.ToInt16(oItemMaster.Element("tax").Value)
                    }).ToList();
                }

                ////**** Create a list of Products ****\\\\
                foreach (var iRowItems in ItemModel)
                {
                    ItemMasterModel = new List<Models.API_FinanceItem>();
                    ItemMasterModel.Add(new Models.API_FinanceItem()
                    {
                        item_code = iRowItems.item_code,
                        description = iRowItems.description,
                        type = iRowItems.type,
                        unit_price = Convert.ToDouble(iRowItems.unit_price),
                        remarks = iRowItems.remarks,
                        tax = iRowItems.tax
                    });

                    string strJSON = JsonConvert.SerializeObject(ItemMasterModel);
                    oLogExist = (String)clsSBOGetRecord.GetSingleValue("select * from " + Iif(GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Product' and \"uniqueId\" = '" + TrimData(iRowItems.item_code) + "' ", myCompany);

                    if (oLogExist == "" || oLogExist == "0")
                    {
                        strQuery = "insert into " + Iif(GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " (\"lastTimeStamp\",\"companyDB\",\"module\",\"uniqueId\",\"docStatus\",\"status\",\"JSON\",\"statusCode\",\"successDesc\",\"failDesc\",\"logDate\") select '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "','" + TrimData(GlobalVar.oSERVERDB) + "','Product','" + TrimData(iRowItems.item_code) + "','Confirmed','','" + TrimData(strJSON) + "','','','',null" + Iif(GlobalVar.oSERVERTYPE != "dst_HANADB", "", " from dummy;") + "";
                        GlobalVar.oRS.DoQuery(strQuery);
                    }
                    else
                    {
                        strQuery = "update " + Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " set \"JSON\" = '" + TrimData(strJSON) + "', \"logDate\" = '" + myCompany.GetDBServerDate().ToString("yyyy-MM-dd") + "' where \"companyDB\" = '" + TrimData(GlobalVar.oSERVERDB) + "' and \"module\" = 'Product' and \"uniqueId\" = '" + iRowItems.item_code + "'";
                        GlobalVar.oRS.DoQuery(strQuery);
                    }
                }
                ////**** Create a list of Products ****\\\\
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ItemMasterModel;
        }
        #endregion
    }

    public static class SBOstrManipulation
    {
        /// <summary>
        /// Get string value after [first] a.
        /// </summary>
        public static string BeforeCharacter(this string value, string a)
        {
            int posA = value.IndexOf(a);
            if (posA == -1)
            {
                return "";
            }
            return value.Substring(0, posA);
        }

        /// <summary>
        /// Get string value after [last] a.
        /// </summary>
        public static string AfterCharacter(this string value, string a)
        {
            int posA = value.LastIndexOf(a);
            if (posA == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= value.Length)
            {
                return "";
            }
            return value.Substring(adjustedPosA);
        }
    }

}