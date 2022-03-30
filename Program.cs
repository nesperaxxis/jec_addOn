using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using SAPbobsCOM;
using SAPbouiCOM;
using Application = SAPbouiCOM.Framework.Application;
using Company = SAPbobsCOM.Company;
using System.Xml;
using System.Xml.Linq;
using System.Threading;
using Excel = Microsoft.Office.Interop.Excel;

namespace JEC_SAP
{
    public class PaymentMethod
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }


    class Program
    {
        private static Company _oCompany { get; set; }
        public static SAP_Helper.SAP _oSAP { get; set; }
        private static SAPbobsCOM.Payments oIncomingPayment { get; set; }
        private static SAPbobsCOM.Payments oOutgoingPayment { get; set; }
        private static SAPbobsCOM.Documents oInvoice { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Application oApp = null;
                if (args.Length < 1)
                {
                    oApp = new Application();
                }
                else
                {
                    oApp = new Application(args[0]);
                }

                try
                {
                    Application.SBO_Application.Menus.RemoveEx("TAIDII_SAP");
                }
                catch (Exception)
                { }

                _oCompany = new Company();
                _oCompany = (SAPbobsCOM.Company)Application.SBO_Application.Company.GetDICompany();

                _oSAP = new SAP_Helper.SAP();
                _oSAP.oCompany = new Company();
                _oSAP.oCompany = _oCompany;

                List<string> findcols = new List<string>();
                List<string> listGender = new List<string>();
                List<string> listCreditType = new List<string>();
                List<string> listType = new List<string>();
                List<string> listPaymentStatus = new List<string>();
                List<string> listModule = new List<string>();
                List<string> listYesNo = new List<string>();
                List<string> listModePayment = new List<string>();


                listGender = new List<string>();
                listGender.Add("0,Male"); // 0 = Male
                listGender.Add("1,Female"); // 1 = Female

                listYesNo = new List<string>();
                listYesNo.Add("0,No"); // 0 = No
                listYesNo.Add("1,Yes"); // 1 = Yes

                listCreditType = new List<string>();
                listCreditType.Add("0,TYPE_CREDIT");  //TYPE_CREDIT = 0
                listCreditType.Add("1,TYPE_DEPOSIT"); //TYPE_DEPOSIT = 1
                listCreditType.Add("2,TYPE_ADVANCE_PAYMENT"); //TYPE_ADVANCE_PAYMENT = 2
                listCreditType.Add("3,TYPE_OVER_PAYMENT"); //TYPE_OVER_PAYMENT = 3

                listType = new List<string>();
                listType.Add("0,Credit Note");  //Credit Note = 0
                listType.Add("1,Deposit"); //Deposit = 1

                listPaymentStatus = new List<string>();
                listPaymentStatus.Add("0,Confirmed"); // 0 = Confirmed
                listPaymentStatus.Add("1,Void"); // 1 = Void

                listModule = new List<string>();
                listModule.Add("1,Student Master (Student List)");
                listModule.Add("2,Invoices (Invoice List)");
                listModule.Add("3,Credit Notes (Credit Note List)");
                listModule.Add("4,Receipts (Receipt List)");
                listModule.Add("5,Credit Refunds (Credit Refund List)");
                listModule.Add("6,Products (Create Finance Item)");

                listModePayment = new List<string>();
                listModePayment.Add("NA,Not Applicable");
                listModePayment.Add("CA,Cash");
                listModePayment.Add("CK,Check");
                listModePayment.Add("BT,Bank Transfer");
                listModePayment.Add("CC,Credit Card");
                listModePayment.Add("CN,CN Offset");

                #region Create UDF Business Partners Master Data
                _oSAP.CreateUDF("OCRD", "date_of_withdrawal", "Date of Withdrawal", BoFieldTypes.db_Date);
                _oSAP.CreateUDF("OCRD", "IC", "NRIC", BoFieldTypes.db_Alpha, 132);
                _oSAP.CreateUDF("OCRD", "Gender", "Gender", BoFieldTypes.db_Numeric, 10, "", "", BoFldSubTypes.st_None, listGender);
                _oSAP.CreateUDF("OCRD", "DOB", "Date of Birth", BoFieldTypes.db_Date);
                _oSAP.CreateUDF("OCRD", "STD_CARE_TYPE", "Student Care Type", BoFieldTypes.db_Alpha, 232);
                _oSAP.CreateUDF("OCRD", "ProgramType", "Program Type", BoFieldTypes.db_Alpha, 132);
                _oSAP.CreateUDF("OCRD", "AdmissionDate", "Admission Date", BoFieldTypes.db_Date);
                _oSAP.CreateUDF("OCRD", "RegNo", "Registration No", BoFieldTypes.db_Alpha, 50);
                _oSAP.CreateUDF("OCRD", "Subsidy", "Subsidy", BoFieldTypes.db_Float, 10, "", "", BoFldSubTypes.st_Sum);
                _oSAP.CreateUDF("OCRD", "Add_Subsidy", "Additional Subsidy", BoFieldTypes.db_Float, 10, "", "", BoFldSubTypes.st_Sum);
                _oSAP.CreateUDF("OCRD", "FinAssist", "Financial Assistance", BoFieldTypes.db_Float, 10, "", "", BoFldSubTypes.st_Sum);
                _oSAP.CreateUDF("OCRD", "Deposit", "Deposit", BoFieldTypes.db_Float, 10, "", "", BoFldSubTypes.st_Sum);
                _oSAP.CreateUDF("OCRD", "Nationality", "Nationality", BoFieldTypes.db_Alpha, 232);
                _oSAP.CreateUDF("OCRD", "Race", "Race", BoFieldTypes.db_Alpha, 132);
                _oSAP.CreateUDF("OCRD", "Country", "Country", BoFieldTypes.db_Alpha, 254);
                _oSAP.CreateUDF("OCRD", "Level", "Level", BoFieldTypes.db_Alpha, 132);
                #endregion

                #region Create UDF Contact Employee
                _oSAP.CreateUDF("OCPR", "Bankname", "Bank Name", BoFieldTypes.db_Alpha, 232);
                _oSAP.CreateUDF("OCPR", "AccName", "Account Name", BoFieldTypes.db_Alpha, 232);
                _oSAP.CreateUDF("OCPR", "BankAccNo", "CDAC/Bank Account No.", BoFieldTypes.db_Alpha, 132);
                _oSAP.CreateUDF("OCPR", "CusRefNo", "Customer Reference No.", BoFieldTypes.db_Alpha, 132);
                #endregion

                #region Create UDF Invoice, Credit Note, Incoming Payment and Outgoing Payment
                _oSAP.CreateUDF("OINV", "TransId", "Transaction Id", BoFieldTypes.db_Alpha, 32);
                _oSAP.CreateUDF("OINV", "Level", "Level", BoFieldTypes.db_Alpha, 132);
                _oSAP.CreateUDF("OINV", "ProgramType", "Program Type", BoFieldTypes.db_Alpha, 132);
                _oSAP.CreateUDF("OINV", "DiscName", "Discount Name", BoFieldTypes.db_Alpha, 232);
                _oSAP.CreateUDF("INV1", "date_for", "Date For", BoFieldTypes.db_Date);
                _oSAP.CreateUDF("INV1", "Dscription", "Description", BoFieldTypes.db_Memo);

                _oSAP.CreateUDF("ORIN", "CreditType", "Credit Type", BoFieldTypes.db_Numeric, 10, "", "", BoFldSubTypes.st_None, listCreditType);
                _oSAP.CreateUDF("ORIN", "Type", "Type", BoFieldTypes.db_Numeric, 10, "", "", BoFldSubTypes.st_None, listType);
                _oSAP.CreateUDF("ORIN", "ReceiptNo", "Receipt No", BoFieldTypes.db_Alpha, 32);
                _oSAP.CreateUDF("ORIN", "CreatedByVoucher", "Created By Voucher", BoFieldTypes.db_Numeric, 10, "0", "", BoFldSubTypes.st_None, listYesNo);
                _oSAP.CreateUDF("ORCT", "TransId", "Transaction Id", BoFieldTypes.db_Alpha, 32);
                _oSAP.CreateUDF("ORCT", "Status", "Status", BoFieldTypes.db_Alpha, 10, "", "", BoFldSubTypes.st_None, listPaymentStatus);
                _oSAP.CreateUDF("ORCT", "Level", "Level", BoFieldTypes.db_Alpha, 132);
                _oSAP.CreateUDF("ORCT", "ProgramType", "Program Type", BoFieldTypes.db_Alpha, 132);
                _oSAP.CreateUDF("ORCT", "ReceiptNo", "Receipt No", BoFieldTypes.db_Alpha, 132);
                _oSAP.CreateUDF("OVPM", "CreditType", "Credit Type", BoFieldTypes.db_Numeric, 10, "", "", BoFldSubTypes.st_None, listCreditType);
                #endregion

                #region Integration Setup
                _oSAP.CreateUDT("INTEGRATIONSETUP", "Integration Setup", BoUTBTableType.bott_NoObjectAutoIncrement);
                _oSAP.CreateUDF("@INTEGRATIONSETUP", "CompanyDB", "Company Database", BoFieldTypes.db_Alpha, 132);
                _oSAP.CreateUDF("@INTEGRATIONSETUP", "Curr", "Currency", BoFieldTypes.db_Alpha, 10);
                _oSAP.CreateUDF("@INTEGRATIONSETUP", "Country", "Country", BoFieldTypes.db_Alpha, 50);
                _oSAP.CreateUDF("@INTEGRATIONSETUP", "Group", "Group", BoFieldTypes.db_Alpha, 50);
                _oSAP.CreateUDF("@INTEGRATIONSETUP", "Division", "Division", BoFieldTypes.db_Alpha, 50);
                _oSAP.CreateUDF("@INTEGRATIONSETUP", "Product", "Product", BoFieldTypes.db_Alpha, 50);
                _oSAP.CreateUDF("@INTEGRATIONSETUP", "api_key", "api key", BoFieldTypes.db_Alpha, 254);
                _oSAP.CreateUDF("@INTEGRATIONSETUP", "base_url", "base url", BoFieldTypes.db_Alpha, 254);
                _oSAP.CreateUDF("@INTEGRATIONSETUP", "pricelist_code", "Price List Code (Item Create)", BoFieldTypes.db_Alpha, 10);
                #endregion

                #region GL Account Mapping (Invoice and Credit Notes)
                _oSAP.CreateUDT("GLACCTMAPPING", "G/L Account Mapping Setup", BoUTBTableType.bott_NoObjectAutoIncrement);
                _oSAP.CreateUDF("@GLACCTMAPPING", "Description", "Description", BoFieldTypes.db_Alpha, 254);
                _oSAP.CreateUDF("@GLACCTMAPPING", "Level", "Level", BoFieldTypes.db_Alpha, 50);
                _oSAP.CreateUDF("@GLACCTMAPPING", "ProgramType", "Program Type", BoFieldTypes.db_Alpha, 254);
                _oSAP.CreateUDF("@GLACCTMAPPING", "FuturePeriod", "Future Period", BoFieldTypes.db_Alpha, 50);
                _oSAP.CreateUDF("@GLACCTMAPPING", "CurrentPeriod", "Current Period", BoFieldTypes.db_Alpha, 50);
                _oSAP.CreateUDF("@GLACCTMAPPING", "TaxCode", "Tax Code", BoFieldTypes.db_Alpha, 132);
                #endregion

                #region Recipient per Module Setup
                _oSAP.CreateUDT("EMAILMODULES", "Recipient per Module Setup", BoUTBTableType.bott_NoObjectAutoIncrement);
                _oSAP.CreateUDF("@EMAILMODULES", "Module", "Module", BoFieldTypes.db_Numeric, 10, "", "", BoFldSubTypes.st_None, listModule);
                _oSAP.CreateUDF("@EMAILMODULES", "EMailRecipient", "Recipient (E-Mail Address)", BoFieldTypes.db_Alpha, 132);
                #endregion

                #region Payment Method Codes
                var ListPaymentMethod = new List<PaymentMethod>
                {
                    new PaymentMethod() { Code="0", Name="CHEQUE"},
                    new PaymentMethod() { Code="1", Name="CASH"},
                    new PaymentMethod() { Code="2", Name="GIRO"},
                    new PaymentMethod() { Code="3", Name="OFFSET_DEPOSIT"},
                    new PaymentMethod() { Code="4", Name="CDA"},
                    new PaymentMethod() { Code="5", Name="NETS"},
                    new PaymentMethod() { Code="6", Name="BANK_TRANSFER"},
                    new PaymentMethod() { Code="7", Name="VOUCHER"},
                    new PaymentMethod() { Code="8", Name="OFFSET_CREDIT_NOTE"},
                    new PaymentMethod() { Code="9", Name="SUBSIDY"},
                    new PaymentMethod() { Code="10", Name="OFFSET_ADVANCE_PAYMENT"},
                    new PaymentMethod() { Code="11", Name="OTHERS"},
                    new PaymentMethod() { Code="12", Name="NULL"},
                    new PaymentMethod() { Code="13", Name="INTERNAL_TRANSFER"}
                };

                _oSAP.CreateUDT("PAYMENTMETHOD", "Payment Method Setup", BoUTBTableType.bott_NoObject);
                _oSAP.CreateUDT("PAYMENTCODES", "Payment Code Setup", BoUTBTableType.bott_NoObjectAutoIncrement);
                _oSAP.CreateUDF("@PAYMENTCODES", "PaymentCodeMethod", "Payment Code Method", BoFieldTypes.db_Alpha, 254, "", "PAYMENTMETHOD");
                _oSAP.CreateUDF("@PAYMENTCODES", "GLAccount", "G/L Account", BoFieldTypes.db_Alpha, 50);
                _oSAP.CreateUDF("@PAYMENTCODES", "TaxCode", "Tax Code", BoFieldTypes.db_Alpha, 10);
                _oSAP.CreateUDF("@PAYMENTCODES", "ModePayment", "Mode of Payment", BoFieldTypes.db_Alpha, 10, "", "", BoFldSubTypes.st_None, listModePayment);
                #endregion

                #region Sender E-Mail Credentials UDO and UDF
                _oSAP.CreateUDT("OEMAIL", "Sender E-Mail Credentials", BoUTBTableType.bott_MasterData);
                _oSAP.CreateUDF("@OEMAIL", "EMailAdd", "E-Mail Address", BoFieldTypes.db_Alpha, 254);
                _oSAP.CreateUDF("@OEMAIL", "EMailPw", "E-Mail Password", BoFieldTypes.db_Alpha, 50);
                _oSAP.CreateUDF("@OEMAIL", "EMailHost", "E-Mail Host", BoFieldTypes.db_Alpha, 254);
                _oSAP.CreateUDF("@OEMAIL", "EMailPort", "E-Mail Port", BoFieldTypes.db_Alpha, 254);
                _oSAP.CreateUDF("@OEMAIL", "EMailSSL", "Enable SSL Required", BoFieldTypes.db_Alpha, 1);
                _oSAP.CreateUDF("@OEMAIL", "EMailBody", "Email Body", BoFieldTypes.db_Memo);
                _oSAP.CreateUDF("@OEMAIL", "EMailCc", "E-Mail Cc", BoFieldTypes.db_Alpha, 254);
                _oSAP.CreateUDF("@OEMAIL", "EMailBcc", "E-Mail Bcc", BoFieldTypes.db_Alpha, 254);
                _oSAP.CreateUDF("@OEMAIL", "EMailSalute", "Email Salutation", BoFieldTypes.db_Memo);
                _oSAP.CreateUDF("@OEMAIL", "EMailCompli", "Email Complimentary", BoFieldTypes.db_Memo);
                #endregion

                #region Recipient E-Mail Register UDO
                findcols = new List<string>();
                findcols.Add("Code,Code");
                findcols.Add("Name,Name");
                _oSAP.InitializeUDO(BoUDOObjType.boud_MasterData, "OEMAIL", "Sender E-Mail Credentials", "OEMAIL", null, BoYesNoEnum.tNO, BoYesNoEnum.tNO, BoYesNoEnum.tNO, BoYesNoEnum.tYES, findcols);
                _oSAP.RegisterUDOFinal();
                #endregion

                #region Server Credentials UDO and UDF
                _oSAP.CreateUDT("OCRED", "Server and SAP B1 Credentials", BoUTBTableType.bott_MasterData);
                _oSAP.CreateUDF("@OCRED", "ServerType", "Server Type", BoFieldTypes.db_Alpha, 50);
                _oSAP.CreateUDF("@OCRED", "ServerName", "Server Name", BoFieldTypes.db_Alpha, 132);
                _oSAP.CreateUDF("@OCRED", "ServerUN", "Server Username", BoFieldTypes.db_Alpha, 50);
                _oSAP.CreateUDF("@OCRED", "ServerPW", "Server Password", BoFieldTypes.db_Alpha, 50);
                _oSAP.CreateUDF("@OCRED", "B1UN", "B1 Username", BoFieldTypes.db_Alpha, 50);
                _oSAP.CreateUDF("@OCRED", "B1PW", "B1 Password", BoFieldTypes.db_Alpha, 50);
                #endregion

                #region SQL Server Credentials Register UDO
                findcols = new List<string>();
                findcols.Add("Code,Code");
                findcols.Add("Name,Name");
                _oSAP.InitializeUDO(BoUDOObjType.boud_MasterData, "OCRED", "Server and SAP B1 Credentials", "OCRED", null, BoYesNoEnum.tNO, BoYesNoEnum.tNO, BoYesNoEnum.tNO, BoYesNoEnum.tYES, findcols);
                _oSAP.RegisterUDOFinal();
                #endregion

                Helpers.GlobalVar.myCompany = new Company();
                Helpers.GlobalVar.myCompany = _oCompany;
                Helpers.GlobalVar.oRS = (Recordset)_oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                Helpers.GlobalVar.oRSQuery = (Recordset)_oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                Helpers.GlobalVar.oRSSelect = (Recordset)_oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                Helpers.GlobalVar.oRSExec = (Recordset)_oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                Helpers.GlobalVar.GetServerCredential();
                Helpers.GlobalVar.GetIntegrationSetup();
                Helpers.GlobalVar.oDepartmentCode = Helpers.GlobalVar.GetDepartmentCode();
                Helpers.GlobalVar.oDepartmentFinance = Helpers.GlobalVar.IsDepartmentFinance();
                Helpers.GlobalVar.oSuperUser = Helpers.GlobalVar.IsSuperUser();

                CreateObject();
                Helpers.GlobalVar.InsertDatabase();

                #region Insert Data to Payment Method
                Helpers.GlobalVar.oQuery = "select count(*) \"Rows\" from \"@PAYMENTMETHOD\"";
                Helpers.GlobalVar.oRS.DoQuery(Helpers.GlobalVar.oQuery);
                if (Convert.ToInt16(Helpers.GlobalVar.oRS.Fields.Item("Rows").Value) == 0)
                {
                    SAPbobsCOM.UserTable oDT = Helpers.GlobalVar.myCompany.UserTables.Item("PAYMENTMETHOD");

                    foreach (PaymentMethod PM in ListPaymentMethod)
                    {
                        string Code = PM.Code;
                        string Name = PM.Name;

                        oDT.Code = Code;
                        oDT.Name = Name;

                        if (oDT.Add() != 0)
                            Application.SBO_Application.SetStatusBarMessage(Helpers.GlobalVar.myCompany.GetLastErrorDescription().ToString());
                    }
                }
                #endregion

                Menu myMenu = new Menu();
                myMenu.oRS = _oSAP.oRecordset();
                myMenu.AddMenuItems();
                oApp.RegisterMenuEventHandler(myMenu.SBO_Application_MenuEvent);
                Application.SBO_Application.AppEvent += new SAPbouiCOM._IApplicationEvents_AppEventEventHandler(SBO_Application_AppEvent);
                Application.SBO_Application.SetStatusBarMessage("TAIDII_SAP Integration Add-on successfully connected.", SAPbouiCOM.BoMessageTime.bmt_Short, false);
                oApp.Run();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        static void CreateIncomingPayment()
        {
            int lErrCode;
            string oDocEntry = string.Empty;
            string oLastErrorMessage = string.Empty;
            try
            {
                oIncomingPayment = (SAPbobsCOM.Payments)_oCompany.GetBusinessObject(BoObjectTypes.oPaymentsDrafts);
                oIncomingPayment.DocObjectCode = BoPaymentsObjectType.bopot_IncomingPayments;

                oIncomingPayment.CardCode = "C20000";

                oIncomingPayment.DocTypte = BoRcptTypes.rCustomer;
                oIncomingPayment.DocDate = Convert.ToDateTime("07/01/2018");
                oIncomingPayment.CounterReference = "342";

                oIncomingPayment.Invoices.DocEntry = 342;
                oIncomingPayment.Invoices.InvoiceType = BoRcptInvTypes.it_Invoice;
                oIncomingPayment.Invoices.Add();


                oIncomingPayment.CashAccount = "_SYS00000000001";
                oIncomingPayment.CashSum = 2703.00;

                lErrCode = oIncomingPayment.Add();

                if (lErrCode == 0)
                {
                    try
                    {
                        oDocEntry = _oCompany.GetNewObjectKey();

                    }
                    catch
                    { }
                }
                else
                {
                    oLastErrorMessage = _oCompany.GetLastErrorDescription();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
        static void CancelIncomingPayment()
        {
            int lErrCode;
            string oDocEntry = string.Empty;
            string oLastErrorMessage = string.Empty;
            try
            {
                oIncomingPayment = (SAPbobsCOM.Payments)_oCompany.GetBusinessObject(BoObjectTypes.oIncomingPayments);

                if (oIncomingPayment.GetByKey(228) == true)
                {
                    lErrCode = oIncomingPayment.Cancel();

                    if (lErrCode == 0)
                    {
                        try
                        {
                            oDocEntry = _oCompany.GetNewObjectKey();

                        }
                        catch
                        { }
                    }
                    else
                    {
                        oLastErrorMessage = _oCompany.GetLastErrorDescription();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
        static void PostDraftIncoming()
        {
            SAPbobsCOM.Payments oOutgoingDraft = (Payments)_oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPaymentsDrafts);

            if (oOutgoingDraft.GetByKey(4))
            {
                int lErrCode = oOutgoingDraft.SaveDraftToDocument();
                if (lErrCode == 0)
                {
                    string oDocEntry = _oCompany.GetNewObjectKey();

                }
                else
                {
                    string lastMessage = _oCompany.GetLastErrorDescription();

                }
            }
        }
        static void CreateOutgoingPayment()
        {
            int lErrCode;
            string oDocEntry = string.Empty;
            string oLastErrorMessage = string.Empty;
            try
            {
                oOutgoingPayment = (SAPbobsCOM.Payments)_oCompany.GetBusinessObject(BoObjectTypes.oPaymentsDrafts);
                oOutgoingPayment.DocObjectCode = BoPaymentsObjectType.bopot_OutgoingPayments;

                oOutgoingPayment.CardCode = "C20000";

                oOutgoingPayment.DocTypte = BoRcptTypes.rCustomer;
                oOutgoingPayment.DocDate = Convert.ToDateTime("07/01/2018");
                oOutgoingPayment.CounterReference = "342";

                oOutgoingPayment.Invoices.DocEntry = 342;
                oOutgoingPayment.Invoices.InvoiceType = BoRcptInvTypes.it_Invoice;
                oOutgoingPayment.Invoices.Add();


                oOutgoingPayment.CashAccount = "_SYS00000000001";
                oOutgoingPayment.CashSum = 2703.00;

                lErrCode = oOutgoingPayment.Add();

                if (lErrCode == 0)
                {
                    try
                    {
                        oDocEntry = _oCompany.GetNewObjectKey();
                    }
                    catch
                    { }
                }
                else
                {
                    oLastErrorMessage = _oCompany.GetLastErrorDescription();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }


        }
        static void CancelOutgoingPayment()
        {
            int lErrCode;
            string oDocEntry = string.Empty;
            string oLastErrorMessage = string.Empty;
            try
            {
                oOutgoingPayment = (SAPbobsCOM.Payments)_oCompany.GetBusinessObject(BoObjectTypes.oVendorPayments);

                if (oOutgoingPayment.GetByKey(256) == true)
                {
                    lErrCode = oOutgoingPayment.Cancel();

                    if (lErrCode == 0)
                    {
                        try
                        {
                            oDocEntry = _oCompany.GetNewObjectKey();

                        }
                        catch
                        { }
                    }
                    else
                    {
                        oLastErrorMessage = _oCompany.GetLastErrorDescription();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
        static void CancelARInvoice()
        {
            int lErrCode;
            string oDocEntry = string.Empty;
            string oLastErrorMessage = string.Empty;
            try
            {
                oInvoice = (SAPbobsCOM.Documents)_oCompany.GetBusinessObject(BoObjectTypes.oInvoices);

                if (oInvoice.GetByKey(17239) == true)
                {
                    lErrCode = oInvoice.Cancel();

                    if (lErrCode == 0)
                    {
                        try
                        {
                            oDocEntry = _oCompany.GetNewObjectKey();
                        }
                        catch
                        { }
                    }
                    else
                    {
                        oLastErrorMessage = _oCompany.GetLastErrorDescription();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
        static void CreateObject()
        {
            try
            {
                if (Helpers.GlobalVar.myCompany.DbServerType == BoDataServerTypes.dst_HANADB)
                {
                    _oSAP.query = "select * from \"PUBLIC\".\"SCHEMAS\" where \"SCHEMA_NAME\" = 'TAIDII_SAP'";
                    _oSAP.oRS.DoQuery(_oSAP.query);

                    if (_oSAP.oRS.RecordCount == 0)
                    {
                        Helpers.GlobalVar.oQuery = "create schema \"TAIDII_SAP\" ";
                        _oSAP.oRecordset().DoQuery(Helpers.GlobalVar.oQuery);
                    }

                    _oSAP.query = "select * from \"PUBLIC\".\"PROCEDURES\" where \"PROCEDURE_NAME\" = 'axxis_sp_GetDocumentForEmail' and \"SCHEMA_NAME\" = '" + Helpers.GlobalVar.myCompany.CompanyDB + "'";
                    _oSAP.oRS.DoQuery(_oSAP.query);
                    if (_oSAP.oRS.RecordCount == 0)
                    {
                        Helpers.GlobalVar.oQuery = "create procedure \"axxis_sp_GetDocumentForEmail\" (in CompanyDB nvarchar(32), in Modules nvarchar(32)) " + Environment.NewLine +
                    "        as " + Environment.NewLine +
                    "        begin " + Environment.NewLine +
                    "" + Environment.NewLine +
                    "        -- Selection result as HTML " + Environment.NewLine +
                    "        declare html nvarchar(5000); " + Environment.NewLine +
                    "        declare Dtl nvarchar(5000); " + Environment.NewLine +
                    "        declare tableDtl nvarchar(5000); " + Environment.NewLine +
                    "        declare Salutation nvarchar(5000); " + Environment.NewLine +
                    "        declare Body nvarchar(5000); " + Environment.NewLine +
                    "        declare Complimentary nvarchar(5000); " + Environment.NewLine +
                    "        declare iModules nvarchar(5000); " + Environment.NewLine +
                    "        declare table_row int; " + Environment.NewLine +
                    "" + Environment.NewLine +
                    "        declare EXIT HANDLER for SQL_ERROR_CODE 1299 " + Environment.NewLine +
                    "        select ::SQL_ERROR_CODE, ::SQL_ERROR_MESSAGE from DUMMY; " + Environment.NewLine +
                    "" + Environment.NewLine +
                    "        CREATE LOCAL TEMPORARY TABLE #TempDtl( " + Environment.NewLine +
                    "        \"Data\" nvarchar(5000) " + Environment.NewLine +
                    "        ); " + Environment.NewLine +
                    "" + Environment.NewLine +
                    "        select case " + Environment.NewLine +
                    "                when :Modules = 1 then 'Student' " + Environment.NewLine +
                    "                when :Modules = 2 then 'Invoice' " + Environment.NewLine +
                    "                when :Modules = 3 then 'Credit Note' " + Environment.NewLine +
                    "                when :Modules = 4 then 'Receipt' " + Environment.NewLine +
                    "                when :Modules = 5 then 'Credit Refund' " + Environment.NewLine +
                    "                when :Modules = 6 then 'Product' " + Environment.NewLine +
                    "        end into iModules from DUMMY; " + Environment.NewLine +
                    "" + Environment.NewLine +
                    "        select count(*) into table_row from \"@OEMAIL\" where ifnull(cast(\"U_EMailSalute\" as varchar(5000)), '') != ''; " + Environment.NewLine +
                    "        if (:table_row != 0) then " + Environment.NewLine +
                    "            select ifnull(\"U_EMailSalute\", '') INTO Salutation FROM \"@OEMAIL\"; " + Environment.NewLine +
                    "        end if; " + Environment.NewLine +
                    "" + Environment.NewLine +
                    "        select count(*) into table_row from \"@OEMAIL\" where ifnull(cast(\"U_EMailBody\" as varchar(5000)), '') != ''; " + Environment.NewLine +
                    "        if (:table_row != 0) then " + Environment.NewLine +
                    "            select ifnull(\"U_EMailBody\", '') into Body from \"@OEMAIL\"; " + Environment.NewLine +
                    "        end if; " + Environment.NewLine +
                    "" + Environment.NewLine +
                    "        select count(*) into table_row from \"@OEMAIL\" where ifnull(cast(\"U_EMailCompli\" as varchar(5000)), '') != ''; " + Environment.NewLine +
                    "        if (:table_row != 0) then " + Environment.NewLine +
                    "            select ifnull(\"U_EMailCompli\", '') into Complimentary from \"@OEMAIL\"; " + Environment.NewLine +
                    "        end if; " + Environment.NewLine +
                    "" + Environment.NewLine +
                    "        select '<h6 style=\"font-family:verdana;\">' || :Salutation || '</h6>' into html from DUMMY; " + Environment.NewLine +
                    "        select '<h6 style=\"font-family:verdana;\">' || :Body || '</h6>' into Body from DUMMY; " + Environment.NewLine +
                    "        select '<h6 style=\"font-family:verdana;\">' || :Complimentary || '</h6>' into Complimentary from DUMMY; " + Environment.NewLine +
                    "" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "        select '<table style=\"font-family:verdana;font-size:70%;\"; cellspacing=\"0\" cellpadding=\"1\" border=\"1\" bgcolor=\"#ffffff\">' || " + Environment.NewLine +
                    "        '<tr bgcolor=\"#cccccc\">' || " + Environment.NewLine +
                    "            '<th width=\\100\\>Time Stamp</th>' || " + Environment.NewLine +
                    "            '<th width=\\100\\>Company DB</th>' || " + Environment.NewLine +
                    "            '<th width=\\100\\>Module</th>' || " + Environment.NewLine +
                    "            '<th width=\\100\\>Reference Id</th>' || " + Environment.NewLine +
                    "            '<th width=\\100\\>Unique Id</th>' || " + Environment.NewLine +
                    "            '<th width=\\100\\>Error Message</th>' || " + Environment.NewLine +
                    "            '</tr>' || CHAR(10) into Dtl from DUMMY; " + Environment.NewLine +
                    "" + Environment.NewLine +
                    "        insert into #TempDtl (\"Data\") " + Environment.NewLine +
                    "        select top 10 " + Environment.NewLine +
                    "          '<tr>' || '<td>' || TO_NVARCHAR(\"a\".\"lastTimeStamp\", 'DD/MM/YYYY') || '</td>' || " + Environment.NewLine +
                    "          '<td>' ||  \"a\".\"companyDB\" || '</td>' || " + Environment.NewLine +
                    "           '<td>' ||  \"a\".\"module\" || '</td>' || " + Environment.NewLine +
                    "          '<td>' ||  \"a\".\"reference\" || '</td>' || " + Environment.NewLine +
                    "          '<td>' ||  \"a\".\"uniqueId\" || '</td>' || " + Environment.NewLine +
                    "          '<td>' ||  \"a\".\"failDesc\" || '</td>' || '</tr>' " + Environment.NewLine +
                    "        from \"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" " + Environment.NewLine +
                    "        where \"failDesc\" != '' and \"status\" != 'true' and \"module\" = :iModules and \"companyDB\" = :CompanyDB; " + Environment.NewLine +
                    "" + Environment.NewLine +
                    "        --select Salutation,html,Body,Complimentary,Body,Dtl,iModules from DUMMY; " + Environment.NewLine +
                    "        select STRING_AGG(\"Data\") into tableDtl  from #TempDtl; " + Environment.NewLine +
                    " " + Environment.NewLine +
                    "        select " + Environment.NewLine +
                    "            :html || " + Environment.NewLine +
                    "            :Dtl || " + Environment.NewLine +
                    "            :tableDtl || char(10) || '</table>' || :Complimentary " + Environment.NewLine +
                    "        into html " + Environment.NewLine +
                    "        from DUMMY; " + Environment.NewLine +
                    "" + Environment.NewLine +
                    "        -- Final selection to return result. " + Environment.NewLine +
                    "        select html \"HTML\" from DUMMY; " + Environment.NewLine +
                    "" + Environment.NewLine +
                    "        drop table #TempDtl; " + Environment.NewLine +
                    "" + Environment.NewLine +
                    "        end;";
                        _oSAP.oRS.DoQuery(Helpers.GlobalVar.oQuery);
                    }

                    _oSAP.query = "select * from \"PUBLIC\".\"VIEWS\" where \"VIEW_NAME\" = 'axxis_vw_TaidiiTransaction' and \"SCHEMA_NAME\" = '" + Helpers.GlobalVar.myCompany.CompanyDB + "'";
                    _oSAP.oRS.DoQuery(_oSAP.query);
                    if (_oSAP.oRS.RecordCount == 0)
                    {
                        Helpers.GlobalVar.oQuery = "create view \"axxis_vw_TaidiiTransaction\" " + Environment.NewLine +
                    "       as " + Environment.NewLine +
                    "       select " + Environment.NewLine +
                    "            \"a\".\"CreateDate\" \"createDate\",\"a\".\"CardCode\",\"a\".\"CardName\" \"bpName\", " + Environment.NewLine +
                    "            0 \"amount\", 0 \"GST\", 0 \"totalAmount\", " + Environment.NewLine +
                    "            'Student' \"module\", \"a\".\"CardCode\" \"uniqueId\", \"a\".\"ObjType\" " + Environment.NewLine +
                    "       from \"OCRD\" \"a\" " + Environment.NewLine +
                    "       union all " + Environment.NewLine +
                    "       select " + Environment.NewLine +
                    "            \"a\".\"DocDate\" \"createDate\",\"a\".\"CardCode\",\"a\".\"CardName\" \"bpName\", " + Environment.NewLine +
                    "            \"a\".\"DocTotal\" - \"a\".\"VatSum\" \"amount\", \"a\".\"VatSum\" \"GST\", \"a\".\"DocTotal\" \"totalAmount\", " + Environment.NewLine +
                    "            'Invoice' \"module\", \"U_TransId\" \"uniqueId\", \"a\".\"ObjType\" " + Environment.NewLine +
                    "       from \"OINV\" \"a\" " + Environment.NewLine +
                    "       where \"a\".\"U_TransId\" is not null and \"a\".\"CANCELED\" = 'N' " + Environment.NewLine +
                    "       union all " + Environment.NewLine +
                    "       select " + Environment.NewLine +
                    "            \"a\".\"DocDate\" \"createDate\",\"a\".\"CardCode\",\"a\".\"CardName\" \"bpName\", " + Environment.NewLine +
                    "            \"a\".\"DocTotal\" - \"a\".\"VatSum\" \"amount\", \"a\".\"VatSum\" \"GST\", \"a\".\"DocTotal\" \"totalAmount\", " + Environment.NewLine +
                    "            'Credit Note' \"module\", \"U_TransId\" \"uniqueId\", \"a\".\"ObjType\" " + Environment.NewLine +
                    "       from \"ORIN\" \"a\" " + Environment.NewLine +
                    "       where \"a\".\"U_TransId\" is not null and \"a\".\"CANCELED\" = 'N' " + Environment.NewLine +
                    "       union all " + Environment.NewLine +
                    "       select " + Environment.NewLine +
                    "            \"a\".\"DocDate\" \"createDate\",\"a\".\"CardCode\",\"a\".\"CardName\" \"bpName\", " + Environment.NewLine +
                    "            \"a\".\"DocTotal\" - \"a\".\"VatSum\" \"amount\", \"a\".\"VatSum\" \"GST\", \"a\".\"DocTotal\" \"totalAmount\", " + Environment.NewLine +
                    "            'Invoice' \"module\", \"U_TransId\" \"uniqueId\", \"a\".\"ObjType\" " + Environment.NewLine +
                    "       from \"ODRF\" \"a\" " + Environment.NewLine +
                    "       where \"a\".\"U_TransId\" is not null and \"a\".\"ObjType\" in (13) and \"a\".\"DocStatus\" = 'O' " + Environment.NewLine +
                    "       union all " + Environment.NewLine +
                    "       select " + Environment.NewLine +
                    "            \"a\".\"DocDate\" \"createDate\",\"a\".\"CardCode\",\"a\".\"CardName\" \"bpName\", " + Environment.NewLine +
                    "            \"a\".\"DocTotal\" - \"a\".\"VatSum\" \"amount\", \"a\".\"VatSum\" \"GST\", \"a\".\"DocTotal\" \"totalAmount\", " + Environment.NewLine +
                    "            'Credit Note' \"module\", \"U_TransId\" \"uniqueId\", \"a\".\"ObjType\" " + Environment.NewLine +
                    "       from \"ODRF\" \"a\" " + Environment.NewLine +
                    "       where \"a\".\"U_TransId\" is not null and \"a\".\"ObjType\" in (14) and \"a\".\"DocStatus\" = 'O' " + Environment.NewLine +
                    "       union all " + Environment.NewLine +
                    "       select " + Environment.NewLine +
                    "            \"DocDate\" \"createDate\",\"CardCode\",\"CardName\" \"bpName\", " + Environment.NewLine +
                    "            \"DocTotal\" \"amount\", 0 \"GST\", \"DocTotal\" \"totalAmount\", " + Environment.NewLine +
                    "            'Receipt' \"module\", \"U_TransId\" \"uniqueId\", \"ObjType\" " + Environment.NewLine +
                    "       from \"ORCT\" " + Environment.NewLine +
                    "       where \"U_TransId\" is not null and \"Canceled\" = 'N' " + Environment.NewLine +
                    "       union all " + Environment.NewLine +
                    "       select " + Environment.NewLine +
                    "            \"DocDate\" \"createDate\",\"CardCode\",\"CardName\" \"bpName\", " + Environment.NewLine +
                    "            \"DocTotal\" \"amount\", 0 \"GST\", \"DocTotal\" \"totalAmount\", " + Environment.NewLine +
                    "            'Credit Refund' \"module\", \"U_TransId\" \"uniqueId\", \"ObjType\" " + Environment.NewLine +
                    "       from \"OPDF\" " + Environment.NewLine +
                    "       where \"U_TransId\" is not null and \"Canceled\" = 'N' and \"ObjType\" = 24 " + Environment.NewLine +
                    "       union all " + Environment.NewLine +
                    "       select " + Environment.NewLine +
                    "            \"DocDate\" \"createDate\",\"CardCode\",\"CardName\" \"bpName\", " + Environment.NewLine +
                    "            \"DocTotal\" \"amount\", 0 \"GST\", \"DocTotal\" \"totalAmount\",  " + Environment.NewLine +
                    "            'Credit Refund' \"module\", \"U_TransId\" \"uniqueId\", \"ObjType\" " + Environment.NewLine +
                    "       from \"OVPM\" " + Environment.NewLine +
                    "       where \"U_TransId\" is not null and \"Canceled\" = 'N' " + Environment.NewLine +
                    "       union all " + Environment.NewLine +
                    "       select " + Environment.NewLine +
                    "            \"DocDate\" \"createDate\",\"CardCode\",\"CardName\" \"bpName\", " + Environment.NewLine +
                    "            \"DocTotal\" \"amount\", 0 \"GST\", \"DocTotal\" \"totalAmount\", " + Environment.NewLine +
                    "            'Credit Refund' \"module\", \"U_TransId\" \"uniqueId\", \"ObjType\" " + Environment.NewLine +
                    "       from \"OPDF\" " + Environment.NewLine +
                    "       where \"U_TransId\" is not null and \"Canceled\" = 'N' and \"ObjType\" = 46;";
                        _oSAP.oRS.DoQuery(Helpers.GlobalVar.oQuery);
                    }
                }
                else
                {
                    _oSAP.query = @"IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'TAIDII_SAP') 
                BEGIN
	                EXEC 
                    ('	
                        CREATE DATABASE TAIDII_SAP
                    ')
                END";
                    _oSAP.oRecordset().DoQuery(_oSAP.query);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
        static void SBO_Application_AppEvent(SAPbouiCOM.BoAppEventTypes eventType)
        {
            try
            {
                switch (eventType)
                {
                    case SAPbouiCOM.BoAppEventTypes.aet_ShutDown:
                        //Exit Add-On
                        System.Windows.Forms.Application.Exit();
                        break;
                    case SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged:
                        System.Windows.Forms.Application.Exit();
                        break;
                    case SAPbouiCOM.BoAppEventTypes.aet_FontChanged:
                        break;
                    case SAPbouiCOM.BoAppEventTypes.aet_LanguageChanged:
                        break;
                    case SAPbouiCOM.BoAppEventTypes.aet_ServerTerminition:
                        System.Windows.Forms.Application.Exit();
                        break;
                    default:
                        break;
                }
            }
            catch
            { }
        }
    }
}