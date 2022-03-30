using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SAPbouiCOM.Framework;
using SAPbobsCOM;
using Application = SAPbouiCOM.Framework.Application;
using BoFormMode = SAPbouiCOM.BoFormMode;
using System.Net;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;

namespace JEC_SAP.Forms
{
    [FormAttribute("frmManualSending", "Forms/frmManualSending.b1f")]
    class frmManualSending : UserFormBase
    {
        private SAPbobsCOM.Documents oDocument { get; set; }
        public static SAPbobsCOM.Recordset oRS { get; set; }
        private SAPbouiCOM.Form oForm { get; set; }
        private SAPbouiCOM.Grid oGridData { get; set; }
        private SAPbouiCOM.Button btSend { get; set; }
        private SAPbouiCOM.Button btCancel { get; set; }
        private SAPbouiCOM.Button btSAll { get; set; }
        private SAPbouiCOM.Button btCAll { get; set; }
        private SAPbouiCOM.Button btRList { get; set; }
        private SAPbouiCOM.StaticText stStatus { get; set; }
        private SAPbouiCOM.StaticText stPoint { get; set; }
        private SAPbouiCOM.ComboBox cbPoint { get; set; }
        private SAPbouiCOM.ComboBox cbStatus { get; set; }
        private SAPbouiCOM.StaticText stLDate { get; set; }
        private SAPbouiCOM.StaticText stFilter { get; set; }
        private SAPbouiCOM.EditText edFilter { get; set; }
        private SAPbouiCOM.EditText edLDate { get; set; }
        private SAPbouiCOM.EditTextColumn oColumns { get; set; }
        public SAPbouiCOM.DataTable oDataTable { get; set; }
        private string oPDF { get; set; }
        private int RecordCount { get; set; }
        private int SuccessCount { get; set; }
        private int FailedCount { get; set; }
        private string oDocEntry { get; set; }
        private string oDocNum { get; set; }
        private string oCardCode { get; set; }
        private string oCardName { get; set; }
        private DateTime oDocDate { get; set; }
        private string oBPEmailAdd { get; set; }
        private string oBPEmailAddCC { get; set; }
        private string oBPEmailAddBCC { get; set; }
        private string ofolderName { get; set; }
        List<Models.API_BusinessPartners> listBusinessParters { get; set; }
        List<Models.API_Invoice> listInvoice { get; set; }
        List<Models.API_CreditNote> listCreditNote { get; set; }
        static List<Models.API_CreditRefund> listCreditRefund { get; set; }
        static List<Models.API_Receipt> listReceipt { get; set; }
        static List<Models.API_FinanceItem> listItem { get; set; }

        public override void OnInitializeComponent()
        {
            this.btSend = ((SAPbouiCOM.Button)(this.GetItem("btSend").Specific));
            this.btSend.PressedBefore += new SAPbouiCOM._IButtonEvents_PressedBeforeEventHandler(this.btnSend_PressedBefore);
            this.btSend.PressedAfter += new SAPbouiCOM._IButtonEvents_PressedAfterEventHandler(this.btSend_PressedAfter);
            this.btCancel = ((SAPbouiCOM.Button)(this.GetItem("2").Specific));
            this.oGridData = ((SAPbouiCOM.Grid)(this.GetItem("myGrid").Specific));
            this.btSAll = ((SAPbouiCOM.Button)(this.GetItem("btSAll").Specific));
            this.btSAll.PressedAfter += new SAPbouiCOM._IButtonEvents_PressedAfterEventHandler(this.btSAll_PressedAfter);
            this.btCAll = ((SAPbouiCOM.Button)(this.GetItem("btCAll").Specific));
            this.btCAll.PressedAfter += new SAPbouiCOM._IButtonEvents_PressedAfterEventHandler(this.btCAll_PressedAfter);
            this.btRList = ((SAPbouiCOM.Button)(this.GetItem("btRList").Specific));
            this.btRList.PressedBefore += new SAPbouiCOM._IButtonEvents_PressedBeforeEventHandler(this.btRList_PressedBefore);
            this.btRList.PressedAfter += new SAPbouiCOM._IButtonEvents_PressedAfterEventHandler(this.btRList_PressedAfter);
            this.stStatus = ((SAPbouiCOM.StaticText)(this.GetItem("stStatus").Specific));
            this.stPoint = ((SAPbouiCOM.StaticText)(this.GetItem("stPoint").Specific));
            this.cbPoint = ((SAPbouiCOM.ComboBox)(this.GetItem("cbPoint").Specific));
            this.cbStatus = ((SAPbouiCOM.ComboBox)(this.GetItem("cbStatus").Specific));
            this.stLDate = ((SAPbouiCOM.StaticText)(this.GetItem("stLDate").Specific));
            this.edLDate = ((SAPbouiCOM.EditText)(this.GetItem("edLDate").Specific));
            this.stFilter = ((SAPbouiCOM.StaticText)(this.GetItem("stFilter").Specific));
            this.edFilter = ((SAPbouiCOM.EditText)(this.GetItem("edFilter").Specific));
            this.OnCustomInitialize();
        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
        }

        public frmManualSending()
        {
            Helpers.GlobalVar.GetServerCredential();
            Helpers.GlobalVar.GetEMailCredentials();
            Helpers.GlobalVar.GetIntegrationSetup();
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary> 
        /// 

        private void btnSend_PressedBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                int iReturn = Application.SBO_Application.MessageBox("Process selected item(s), Do you want to continue?", 2, "Yes", "No");
                if (!iReturn.Equals(1))
                {
                    if (Helpers.GlobalVar.oSERVERNAME == "")
                    {
                        Application.SBO_Application.SetStatusBarMessage("Setup Server and SAP B1 credential to proceed sending invoice thru e-mail.", SAPbouiCOM.BoMessageTime.bmt_Short);
                        BubbleEvent = false;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox(ex.ToString());
            }
        }

        private void btSend_PressedAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                Application.SBO_Application.SetStatusBarMessage("Processing request, Please wait...", SAPbouiCOM.BoMessageTime.bmt_Short, false);
                oForm = Application.SBO_Application.Forms.ActiveForm;
                ((SAPbouiCOM.Button)(oForm.Items.Item("btSend").Specific)).Item.Enabled = false;
                oForm.Freeze(true);
                string JSON1 = string.Empty;
                string JSON2 = string.Empty;
                string JSON3 = string.Empty;
                string JSON4 = string.Empty;
                string JSON5 = string.Empty;
                string JSON6 = string.Empty;
                string JSON7 = string.Empty;
                string JSON8 = string.Empty;
                string JSON9 = string.Empty;
                string JSON10 = string.Empty;
                string JSONResult = string.Empty;
                string oJSON = string.Empty;
                string Id = string.Empty;
                string uniqueId = string.Empty;
                string module = string.Empty;
                string status = string.Empty;

                RecordCount = 0;
                SuccessCount = 0;
                FailedCount = 0;

                if (oRS.RecordCount == 0)
                {
                    Application.SBO_Application.SetStatusBarMessage("No " + cbPoint.Selected.Value + " to send.", SAPbouiCOM.BoMessageTime.bmt_Short);
                    oForm.Freeze(false);
                    return;
                }

                for (int i = 0; i < this.oGridData.DataTable.Rows.Count; i++)
                {
                    Helpers.GlobalVar.oSelected = this.oGridData.DataTable.Columns.Item(1).Cells.Item(i).Value.ToString();
                    Id = this.oGridData.DataTable.Columns.Item(16).Cells.Item(i).Value.ToString();
                    uniqueId = this.oGridData.DataTable.Columns.Item(3).Cells.Item(i).Value.ToString();
                    module = this.oGridData.DataTable.Columns.Item(17).Cells.Item(i).Value.ToString();
                    status = this.oGridData.DataTable.Columns.Item(7).Cells.Item(i).Value.ToString();
                    JSONResult = this.oGridData.DataTable.Columns.Item(15).Cells.Item(i).Value.ToString();

                    if (Helpers.GlobalVar.oSelected == "Y")
                    {
                        if (status != "Posted")
                        {
                            RecordCount += 1;
                            Application.SBO_Application.SetStatusBarMessage("Currently processing selected transaction with uniqueId: " + uniqueId + ", Please wait...(" + string.Format("{0:n0}", RecordCount) + "/" + oRS.RecordCount + ")", SAPbouiCOM.BoMessageTime.bmt_Short, false);

                            if (Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE == "dst_HANADB", "true", "false").ToString() == "false")
                            {
                                string strQuery = "select substring(\"JSON\",1,200) \"JSON1\",substring(\"JSON\",201,200) \"JSON2\",substring(\"JSON\",401,200) \"JSON3\",substring(\"JSON\",601,200) \"JSON4\",substring(\"JSON\",801,200) \"JSON5\",substring(\"JSON\",1001,200) \"JSON6\",substring(\"JSON\",1201,200) \"JSON7\",substring(\"JSON\",1401,200) \"JSON8\",substring(\"JSON\",1601,200) \"JSON9\",substring(\"JSON\",2001,200) \"JSON10\" from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\"") + " where \"uniqueId\" = '" + uniqueId + "' and \"module\" = '" + module + "' and \"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"id\" = '" + Id + "'";

                                Helpers.GlobalVar.oRS.DoQuery(strQuery);

                                if (Helpers.GlobalVar.oRS.RecordCount > 0)
                                {
                                    JSON1 = Helpers.GlobalVar.oRS.Fields.Item(0).Value.ToString();
                                    JSON2 = Helpers.GlobalVar.oRS.Fields.Item(1).Value.ToString();
                                    JSON3 = Helpers.GlobalVar.oRS.Fields.Item(2).Value.ToString();
                                    JSON4 = Helpers.GlobalVar.oRS.Fields.Item(3).Value.ToString();
                                    JSON5 = Helpers.GlobalVar.oRS.Fields.Item(4).Value.ToString();
                                    JSON6 = Helpers.GlobalVar.oRS.Fields.Item(5).Value.ToString();
                                    JSON7 = Helpers.GlobalVar.oRS.Fields.Item(6).Value.ToString();
                                    JSON8 = Helpers.GlobalVar.oRS.Fields.Item(7).Value.ToString();
                                    JSON9 = Helpers.GlobalVar.oRS.Fields.Item(8).Value.ToString();
                                    JSON10 = Helpers.GlobalVar.oRS.Fields.Item(9).Value.ToString();
                                }
                            }

                            oJSON = string.Empty;

                            if (Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE == "dst_HANADB", "true", "false").ToString() == "true")
                            {
                                oJSON = JSONResult;
                            }
                            else
                            {
                                oJSON = JSON1 + JSON2 + JSON3 + JSON4 + JSON5 + JSON6 + JSON7 + JSON8 + JSON9 + JSON10;
                            }

                            //** "1", "Student Master (Student List)" **//
                            //** "2", "Invoices (Invoice List)" **//
                            //** "3", "Credit Note (Credit Note List)" **//
                            //** "4", "Credit Note Void (Credit Note List)" **//
                            //** "5", "Invoice Void (Invoice List)" **//
                            //** "6", "Receipts (Receipt List)" **//
                            //** "7", "Receipts Void (Receipt List)" **//
                            //** "8", "Receipts with Applied Deposits (Credit Refund List) " **//
                            //** "9", "Receipts with Applied Deposits Void (Credit Refund List)" **//
                            //** "10", "Products")" **//
                            //** "11", "All")" **//
                            //** "12", "Deposit (Credit Note List)" **//
                            //** "13", "Deposit Void (Credit Note List)" **//

                            if (oJSON != "")
                            {
                                if (cbPoint.Value == "1")
                                {
                                    listBusinessParters = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.API_BusinessPartners>>(oJSON);
                                    if (Helpers.GlobalVar.SBOPostBusinessPartners(listBusinessParters) == false)
                                        SuccessCount += 1;
                                    else
                                        FailedCount += 1;
                                }
                                else if (cbPoint.Value == "2" || cbPoint.Value == "5")
                                {
                                    listInvoice = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.API_Invoice>>(oJSON);
                                    if (cbStatus.Value == "1")
                                    {
                                        if (Helpers.GlobalVar.SBOPostInvoiceDraft(listInvoice) == false)
                                            SuccessCount += 1;
                                        else
                                            FailedCount += 1;
                                    }
                                    else if (cbStatus.Value == "2")
                                    {
                                        if (Helpers.GlobalVar.SBOPostInvoice(listInvoice) == false)
                                            SuccessCount += 1;
                                        else
                                            FailedCount += 1;
                                    }
                                }
                                else if (cbPoint.Value == "3" || cbPoint.Value == "4" || cbPoint.Value == "12" || cbPoint.Value == "13")
                                {
                                    listCreditNote = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.API_CreditNote>>(oJSON);
                                    if (cbStatus.Value == "1")
                                    {
                                        if (Helpers.GlobalVar.SBOPostCreditNoteDraft(listCreditNote) == false)
                                            SuccessCount += 1;
                                        else
                                            FailedCount += 1;
                                    }
                                    else if (cbStatus.Value == "2")
                                    {
                                        if (Helpers.GlobalVar.SBOPostCreditNote(listCreditNote) == false)
                                            SuccessCount += 1;
                                        else
                                            FailedCount += 1;
                                    }
                                }
                                else if (cbPoint.Value == "6" || cbPoint.Value == "7")
                                {
                                    listReceipt = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.API_Receipt>>(oJSON);
                                    if (cbStatus.Value == "1")
                                    {
                                        if (Helpers.GlobalVar.SBOPostReceiptDraft(listReceipt) == false)
                                            SuccessCount += 1;
                                        else
                                            FailedCount += 1;
                                    }
                                    else if (cbStatus.Value == "2")
                                    {
                                        if (Helpers.GlobalVar.SBOPostReceipt(listReceipt) == false)
                                            SuccessCount += 1;
                                        else
                                            FailedCount += 1;
                                    }

                                }
                                else if (cbPoint.Value == "8" || cbPoint.Value == "9")
                                {
                                    listCreditRefund = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.API_CreditRefund>>(oJSON);
                                    if (cbStatus.Value == "1")
                                    {
                                        if (Helpers.GlobalVar.SBOPostCreditRefundDraft(listCreditRefund) == false)
                                            SuccessCount += 1;
                                        else
                                            FailedCount += 1;
                                    }
                                    else if (cbStatus.Value == "2")
                                    {
                                        if (Helpers.GlobalVar.SBOPostCreditRefund(listCreditRefund) == false)
                                            SuccessCount += 1;
                                        else
                                            FailedCount += 1;
                                    }
                                }
                                else if (cbPoint.Value == "10")
                                {
                                    if (Helpers.GlobalVar.SBOPostFinanceItem(Id, uniqueId, oJSON) == false)
                                        SuccessCount += 1;
                                    else
                                        FailedCount += 1;

                                }
                            }
                        }
                    }
                }

                Application.SBO_Application.SetStatusBarMessage("Successfull: " + Convert.ToString(SuccessCount) + " and failed: " + Convert.ToString(FailedCount) + " process.", SAPbouiCOM.BoMessageTime.bmt_Short, false);

                oForm.Freeze(false);

                LoadGrid();
            }
            catch (Exception ex)
            {
                oForm.Freeze(false);
                ((SAPbouiCOM.Button)(oForm.Items.Item("btSend").Specific)).Item.Enabled = true;
                Application.SBO_Application.MessageBox(ex.ToString());
            }
        }

        private void LoadGrid()
        {
            try
            {
                oForm = Application.SBO_Application.Forms.ActiveForm;
                ((SAPbouiCOM.Button)(oForm.Items.Item("btRList").Specific)).Item.Enabled = false;
                oForm.Freeze(true);
                try
                {
                    oDataTable = oForm.DataSources.DataTables.Item("oDT");
                }
                catch (Exception)
                {
                    oDataTable = oForm.DataSources.DataTables.Add("oDT");
                }

                oGridData = ((SAPbouiCOM.Grid)(oForm.Items.Item("myGrid").Specific));

                string strField = "row_number() over (order by \"a\".\"lastTimeStamp\" desc,\"a\".\"uniqueId\" asc,\"a\".\"module\" asc) as \"#\",'N' \"(Y/N)\",\"a\".\"lastTimeStamp\",\"a\".\"uniqueId\",\"a\".\"reference\",\"a\".\"sapCode\",\"a\".\"docStatus\",\"a\".\"statusCode\" \"status\",\"b\".\"createDate\",\"b\".\"bpName\",\"b\".\"amount\",\"b\".\"GST\",\"b\".\"totalAmount\",case when \"a\".\"successDesc\" is null or \"a\".\"successDesc\" = '' then \"a\".\"failDesc\" else \"a\".\"successDesc\" end \"remarks\",\"a\".\"logDate\",\"a\".\"JSON\",\"a\".\"id\",\"a\".\"module\"";

                //**Equal to Integration Point = All, Status = All and Date is not null **//
                if (cbPoint.Value == "11" && cbStatus.Value == "4" && edLDate.Value != "")
                {

                    if (edFilter.Value == "")
                    {
                        Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                    }
                    else
                    {
                        Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";

                    }
                }
                //**Equal to Integration Point = All, Status = All and Date is null**//
                else if (cbPoint.Value == "11" && cbStatus.Value == "4" && edLDate.Value == "")
                {
                    if (edFilter.Value == "")
                    {
                        Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                    }
                    else
                    {
                        Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                    }
                }
                //**Equal to Integration Point != All, Status = All and Date is null**//
                else if (cbPoint.Value != "11" && cbStatus.Value == "4" && edLDate.Value == "")
                {

                    ////**** Equal to Integration Point == 1.	Student Master (Student List) ****\\\\
                    if (cbPoint.Value == "1" && cbStatus.Value == "4") //Status == all and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Student' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Student' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 1.	Student Master (Student List) ****\\\\

                    ////**** Equal to Integration Point == 2.	Invoices (Invoice List) ****\\\\
                    else if (cbPoint.Value == "2" && cbStatus.Value == "4") //Status == all and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }

                    }
                    ////**** Equal to Integration Point == 2.	Invoices (Invoice List) ****\\\\

                    ////**** Equal to Integration Point == 3.	Credit Note (Credit Note List) ****\\\\
                    else if (cbPoint.Value == "3" && cbStatus.Value == "4") //Status == all and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 3.	Credit Note (Credit Note List) ****\\\\

                    ////**** Equal to Integration Point == 4.	 Credit Note Void (Credit Note List) ****\\\\
                    else if (cbPoint.Value == "4" && cbStatus.Value == "4") //Status == all and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";

                        }
                    }
                    ////**** Equal to Integration Point == 4.	 Credit Note Void (Credit Note List) ****\\\\

                    ////**** Equal to Integration Point == 5.	Invoice Void (Invoice List) ****\\\\
                    else if (cbPoint.Value == "5" && cbStatus.Value == "4") //Status == all and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Void' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Void' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 5.	Invoice Void (Invoice List) ****\\\\

                    ////**** Equal to Integration Point == 6.	Receipts (Receipt List) ****\\\\
                    else if (cbPoint.Value == "6" && cbStatus.Value == "4") //Status == all and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";

                        }
                    }
                    ////**** Equal to Integration Point == 6.	Receipts (Receipt List) ****\\\\

                    ////**** Equal to Integration Point == 7.	Receipts Void (Receipt List) ****\\\\
                    else if (cbPoint.Value == "7" && cbStatus.Value == "4") //Status == all and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Void' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Void' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 7.	Receipts Void (Receipt List) ****\\\\

                    ////**** Equal to Integration Point == 8.	Receipts with Applied Deposits (Credit Refund List)  ****\\\\
                    else if (cbPoint.Value == "8" && cbStatus.Value == "4") //Status == all and Date is null
                    {


                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 8.	Receipts with Applied Deposits (Credit Refund List)  ****\\\\

                    ////**** Equal to Integration Point == 9.	Receipts with Applied Deposits Void (Credit Refund List)  ****\\\\
                    else if (cbPoint.Value == "9" && cbStatus.Value == "4") //Status == all and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Void' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Void' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 9.	Receipts with Applied Deposits Void (Credit Refund List)  ****\\\\

                    ////**** Equal to Integration Point == 10.	Products (Create Finance Item) ****\\\\
                    else if (cbPoint.Value == "10" && cbStatus.Value == "4") //Status == all and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Product' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Product' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 10.	Products (Create Finance Item) ****\\\\

                    ////**** Equal to Integration Point == 12.	Deposit (Credit Note List) ****\\\\
                    else if (cbPoint.Value == "12" && cbStatus.Value == "4") //Status == all and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 12.	Deposit (Credit Note List) ****\\\\

                    ////**** Equal to Integration Point == 13.	 Deposit Void (Credit Note List) ****\\\\
                    else if (cbPoint.Value == "13" && cbStatus.Value == "4") //Status == all and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 13.	 Deposit Void (Credit Note List) ****\\\\
                }
                else if (cbPoint.Value != "11" && cbStatus.Value != "4" && edLDate.Value == "") //Equal to Integration Point != All, Status != All and Date is null
                {
                    ////**** Equal to Integration Point == 1.	Student Master (Student List) ****\\\\
                    if (cbPoint.Value == "1" && cbStatus.Value == "1") //Status == For Process and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Student' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Student' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "1" && cbStatus.Value == "3") //Status == Posted and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Student' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Student' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 1.	Student Master (Student List) ****\\\\

                    ////**** Equal to Integration Point == 2.	Invoices (Invoice List) ****\\\\
                    else if (cbPoint.Value == "2" && cbStatus.Value == "1") //Status == For Process and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "2" && cbStatus.Value == "2") //Status == Draft and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "2" && cbStatus.Value == "3") //Status == Posted and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 2.	Invoices (Invoice List) ****\\\\

                    ////**** Equal to Integration Point == 3.	Credit Note (Credit Note List) ****\\\\
                    else if (cbPoint.Value == "3" && cbStatus.Value == "1") //Status == For Process and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "3" && cbStatus.Value == "2") //Status == Draft and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "3" && cbStatus.Value == "3") //Status == Posted and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 3.	Credit Note (Credit Note List) ****\\\\

                    ////**** Equal to Integration Point == 4.	 Credit Note Void (Credit Note List) ****\\\\
                    else if (cbPoint.Value == "4" && cbStatus.Value == "1") //Status == For Process and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "4" && cbStatus.Value == "2") //Status == Draft and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "4" && cbStatus.Value == "3") //Status == Posted and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 4.	 Credit Note Void (Credit Note List) ****\\\\

                    ////**** Equal to Integration Point == 5.	Invoice Void (Invoice List) ****\\\\
                    else if (cbPoint.Value == "5" && cbStatus.Value == "1") //Status == For Process and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "5" && cbStatus.Value == "2") //Status == Draft and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "5" && cbStatus.Value == "3") //Status == Posted and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";

                        }
                    }
                    ////**** Equal to Integration Point == 5.	Invoice Void (Invoice List) ****\\\\

                    ////**** Equal to Integration Point == 6.	Receipts (Receipt List) ****\\\\
                    else if (cbPoint.Value == "6" && cbStatus.Value == "1") //Status == For Process and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "6" && cbStatus.Value == "2") //Status == Draft and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "6" && cbStatus.Value == "3") //Status == Posted and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 6.	Receipts (Receipt List) ****\\\\

                    ////**** Equal to Integration Point == 7.	Receipts Void (Receipt List) ****\\\\
                    else if (cbPoint.Value == "7" && cbStatus.Value == "1") //Status == For Process and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "7" && cbStatus.Value == "2") //Status == Draft and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "7" && cbStatus.Value == "3") //Status == Posted and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 7.	Receipts Void (Receipt List) ****\\\\

                    ////**** Equal to Integration Point == 8.	Receipts with Applied Deposits (Credit Refund List)  ****\\\\
                    else if (cbPoint.Value == "8" && cbStatus.Value == "1") //Status == For Process and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "8" && cbStatus.Value == "2") //Status == Draft and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "8" && cbStatus.Value == "3") //Status == Posted and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 8.	Receipts with Applied Deposits (Credit Refund List)  ****\\\\

                    ////**** Equal to Integration Point == 9.	Receipts with Applied Deposits Void (Credit Refund List)  ****\\\\
                    else if (cbPoint.Value == "9" && cbStatus.Value == "1") //Status == For Process and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "9" && cbStatus.Value == "2") //Status == Draft and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "9" && cbStatus.Value == "3") //Status == Posted and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 9.	Receipts with Applied Deposits Void (Credit Refund List)  ****\\\\

                    ////**** Equal to Integration Point == 10.	Products (Create Finance Item) ****\\\\
                    else if (cbPoint.Value == "10" && cbStatus.Value == "1") //Status == For Process and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Product' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Product' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "10" && cbStatus.Value == "2") //Status == Draft and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Product' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Product' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "10" && cbStatus.Value == "3") //Status == Posted and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Product' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Product' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 10.	Products (Create Finance Item) ****\\\\

                    ////**** Equal to Integration Point == 12.	Deposit (Credit Note List) ****\\\\
                    else if (cbPoint.Value == "12" && cbStatus.Value == "1") //Status == For Process and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";

                        }
                    }
                    else if (cbPoint.Value == "12" && cbStatus.Value == "2") //Status == Draft and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "12" && cbStatus.Value == "3") //Status == Posted and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 12.	Deposit (Credit Note List) ****\\\\

                    ////**** Equal to Integration Point == 13.	 Deposit Void (Credit Note List) ****\\\\
                    else if (cbPoint.Value == "13" && cbStatus.Value == "1") //Status == For Process and Date is null
                    {


                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "13" && cbStatus.Value == "2") //Status == Draft and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "13" && cbStatus.Value == "3") //Status == Posted and Date is null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 13.	 Deposit Void (Credit Note List) ****\\\\
                }
                else if (cbPoint.Value != "11" && cbStatus.Value != "4" && edLDate.Value != "") //Equal to Integration Point != All, Status != All and Date is not null
                {
                    ////**** Equal to Integration Point == 1.	Student Master (Student List) ****\\\\
                    if (cbPoint.Value == "1" && cbStatus.Value == "1") //Status == For Process and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Student' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Student' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "1" && cbStatus.Value == "2") //Status == Draft and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Student' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Student' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "1" && cbStatus.Value == "3") //Status == Posted and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Student' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Student' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 1.	Student Master (Student List) ****\\\\

                    ////**** Equal to Integration Point == 2.	Invoices (Invoice List) ****\\\\
                    else if (cbPoint.Value == "2" && cbStatus.Value == "1") //Status == For Process and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'For Process' and \"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'For Process' and \"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "2" && cbStatus.Value == "2") //Status == Draft and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "2" && cbStatus.Value == "3") //Status == Posted and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 2.	Invoices (Invoice List) ****\\\\

                    ////**** Equal to Integration Point == 3.	Credit Note (Credit Note List) ****\\\\
                    else if (cbPoint.Value == "3" && cbStatus.Value == "1") //Status == For Process and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "3" && cbStatus.Value == "2") //Status == Draft and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "3" && cbStatus.Value == "3") //Status == Posted and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 3.	Credit Note (Credit Note List) ****\\\\

                    ////**** Equal to Integration Point == 4.	 Credit Note Void (Credit Note List) ****\\\\
                    else if (cbPoint.Value == "4" && cbStatus.Value == "1") //Status == For Process and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "4" && cbStatus.Value == "2") //Status == Draft and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "4" && cbStatus.Value == "3") //Status == Posted and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" not like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 4.	 Credit Note Void (Credit Note List) ****\\\\

                    ////**** Equal to Integration Point == 5.	Invoice Void (Invoice List) ****\\\\
                    else if (cbPoint.Value == "5" && cbStatus.Value == "1") //Status == For Process and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "5" && cbStatus.Value == "2") //Status == Draft and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "5" && cbStatus.Value == "3") //Status == Posted and Date is not null
                    {         
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Invoice' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 5.	Invoice Void (Invoice List) ****\\\\

                    ////**** Equal to Integration Point == 6.	Receipts (Receipt List) ****\\\\
                    else if (cbPoint.Value == "6" && cbStatus.Value == "1") //Status == For Process and Date is not null
                    {                       
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "6" && cbStatus.Value == "2") //Status == Draft and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "6" && cbStatus.Value == "3") //Status == Posted and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 6.	Receipts (Receipt List) ****\\\\

                    ////**** Equal to Integration Point == 7.	Receipts Void (Receipt List) ****\\\\
                    else if (cbPoint.Value == "7" && cbStatus.Value == "1") //Status == For Process and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "7" && cbStatus.Value == "2") //Status == Draft and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "7" && cbStatus.Value == "3") //Status == Posted and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Receipt' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 7.	Receipts Void (Receipt List) ****\\\\

                    ////**** Equal to Integration Point == 8.	Receipts with Applied Deposits (Credit Refund List)  ****\\\\
                    else if (cbPoint.Value == "8" && cbStatus.Value == "1") //Status == For Process and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "8" && cbStatus.Value == "2") //Status == Draft and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "8" && cbStatus.Value == "3") //Status == Posted and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 8.	Receipts with Applied Deposits (Credit Refund List)  ****\\\\

                    ////**** Equal to Integration Point == 9.	Receipts with Applied Deposits Void (Credit Refund List)  ****\\\\
                    else if (cbPoint.Value == "9" && cbStatus.Value == "1") //Status == For Process and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "9" && cbStatus.Value == "2") //Status == Draft and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "9" && cbStatus.Value == "3") //Status == Posted and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Refund' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }                      
                    }
                    ////**** Equal to Integration Point == 9.	Receipts with Applied Deposits Void (Credit Refund List)  ****\\\\

                    ////**** Equal to Integration Point == 10.	Products (Create Finance Item) ****\\\\
                    else if (cbPoint.Value == "10" && cbStatus.Value == "1") //Status == For Process and Date is not null
                    {                      
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Product' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Product' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "10" && cbStatus.Value == "2") //Status == Draft and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Product' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Product' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "10" && cbStatus.Value == "3") //Status == Posted and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Product' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Product' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 10.	Products (Create Finance Item) ****\\\\

                     ////**** Equal to Integration Point == 12.	Deposit (Credit Note List) ****\\\\
                    else if (cbPoint.Value == "12" && cbStatus.Value == "1") //Status == For Process and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "12" && cbStatus.Value == "2") //Status == Draft and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "12" && cbStatus.Value == "3") //Status == Posted and Date is not null
                    { 
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Confirmed' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 12.	Deposit (Credit Note List) ****\\\\

                    ////**** Equal to Integration Point == 13.	 Deposit Void (Credit Note List) ****\\\\
                    else if (cbPoint.Value == "13" && cbStatus.Value == "1") //Status == For Process and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'For Process' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "13" && cbStatus.Value == "2") //Status == Draft and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Draft' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    else if (cbPoint.Value == "13" && cbStatus.Value == "3") //Status == Posted and Date is not null
                    {
                        if (edFilter.Value == "")
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "'";
                        }
                        else
                        {
                            Helpers.GlobalVar.oQuery = "select " + strField + " from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.oSERVERTYPE != "dst_HANADB", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" collate database_default = \"a\".\"module\" and \"b\".\"uniqueId\" collate database_default = \"a\".\"uniqueId\"", "\"TAIDII_SAP\".\"axxis_tb_IntegrationLog\" \"a\" left join axxis_vw_TaidiiTransaction \"b\" on \"b\".\"module\" = \"a\".\"module\" and \"b\".\"uniqueId\" = \"a\".\"uniqueId\"") + " where \"a\".\"module\" = 'Credit Note' and \"a\".\"JSON\" like '%\"credit_type\":1%' and \"a\".\"docStatus\" = 'Void' and \"a\".\"statusCode\" = 'Posted' and \"a\".\"companyDB\" = '" + Helpers.GlobalVar.TrimData(Helpers.GlobalVar.oSERVERDB) + "' and \"a\".\"lastTimeStamp\" = '" + edLDate.Value + "' and (\"a\".\"uniqueId\" like '%" + edFilter.Value + "%' or \"a\".\"reference\" like '%" + edFilter.Value + "%' or \"b\".\"bpName\" like '%" + edFilter.Value + "%')";
                        }
                    }
                    ////**** Equal to Integration Point == 13.	 Deposit Void (Credit Note List) ****\\\\
                }

                oRS = (Recordset)Helpers.GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                oRS.DoQuery(Helpers.GlobalVar.oQuery);
                oDataTable.ExecuteQuery(Helpers.GlobalVar.oQuery);
                oGridData.DataTable = oDataTable;

                if (oRS.RecordCount > 0)
                {
                    oGridData.Columns.Item(1).Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox;
                    oColumns = ((SAPbouiCOM.EditTextColumn)(oGridData.Columns.Item(5)));

                    if (cbPoint.Value == "1")
                    {
                        oColumns.LinkedObjectType = "2";
                    }
                    else if (cbPoint.Value == "2")
                    {
                        if (cbStatus.Value == "2")
                            oColumns.LinkedObjectType = "112";
                        else
                            oColumns.LinkedObjectType = "13";
                    }
                    else if (cbPoint.Value == "5")
                    {
                        if (cbStatus.Value == "2")
                            oColumns.LinkedObjectType = "112";
                        else
                            oColumns.LinkedObjectType = "14";
                    }
                    else if (cbPoint.Value == "3" || cbPoint.Value == "4" || cbPoint.Value == "12" || cbPoint.Value == "13")
                    {
                        if (cbStatus.Value == "2")
                            oColumns.LinkedObjectType = "112";
                        else
                            oColumns.LinkedObjectType = "14";
                    }
                    else if (cbPoint.Value == "6" || cbPoint.Value == "7")
                    {
                        if (cbStatus.Value == "2")
                            oColumns.LinkedObjectType = "140";
                        else
                            oColumns.LinkedObjectType = "24";
                    }
                    else if (cbPoint.Value == "8" || cbPoint.Value == "9")
                    {
                        if (cbStatus.Value == "2")
                            oColumns.LinkedObjectType = "140";
                        else
                            oColumns.LinkedObjectType = "46";
                    }
                    else if (cbPoint.Value == "10")
                    {
                        oColumns.LinkedObjectType = "4";
                    }
                }

                for (int i = 0; i < oGridData.Columns.Count; i++)
                {
                    if (i == 1)
                        oGridData.Columns.Item(i).Editable = true;
                    else
                        oGridData.Columns.Item(i).Editable = false;
                }

                oForm.Freeze(false);

                ((SAPbouiCOM.Button)(oForm.Items.Item("btRList").Specific)).Item.Enabled = true;

                if (cbStatus.Value == "1" || cbStatus.Value == "2" || cbStatus.Value == "4")
                    ((SAPbouiCOM.Button)(oForm.Items.Item("btSend").Specific)).Item.Enabled = true;
                else
                    ((SAPbouiCOM.Button)(oForm.Items.Item("btSend").Specific)).Item.Enabled = false;


            }
            catch (Exception ex)
            {
                oForm.Freeze(false);
                ((SAPbouiCOM.Button)(oForm.Items.Item("btSend").Specific)).Item.Enabled = true;
                Application.SBO_Application.SetStatusBarMessage(ex.ToString());
            }
        }

        private void btSAll_PressedAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            oForm = Application.SBO_Application.Forms.ActiveForm;
            oForm.Freeze(true);

            btSAll.Item.Enabled = false;
            for (int i = 0; i < this.oGridData.DataTable.Rows.Count; i++)
            {
                this.oGridData.DataTable.SetValue("(Y/N)", i, "Y");
            }
            btSAll.Item.Enabled = true;
            btCAll.Item.Enabled = true;
            oForm.Freeze(false);
        }

        private void btCAll_PressedAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            oForm = Application.SBO_Application.Forms.ActiveForm;
            oForm.Freeze(true);
            btCAll.Item.Enabled = false;
            for (int i = 0; i < this.oGridData.DataTable.Rows.Count; i++)
            {
                this.oGridData.DataTable.SetValue("(Y/N)", i, "N");
            }
            btCAll.Item.Enabled = true;
            btSAll.Item.Enabled = true;
            oForm.Freeze(false);
        }

        private void btRList_PressedBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (cbPoint.Value == "")
                {
                    Application.SBO_Application.SetStatusBarMessage("Select Integration Point.", SAPbouiCOM.BoMessageTime.bmt_Short);
                    BubbleEvent = false;
                    return;
                }

                if (cbStatus.Value == "")
                {
                    Application.SBO_Application.SetStatusBarMessage("Select Status.", SAPbouiCOM.BoMessageTime.bmt_Short);
                    BubbleEvent = false;
                    return;
                }

                if (Helpers.GlobalVar.oSERVERNAME == "")
                {
                    Application.SBO_Application.SetStatusBarMessage("Setup Server and SAP B1 credentials before you can proceed .", SAPbouiCOM.BoMessageTime.bmt_Short);
                    BubbleEvent = false;
                    return;
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox(ex.ToString());
            }
        }

        private void btRList_PressedAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            Application.SBO_Application.SetStatusBarMessage("Refreshing the List, Please wait...", SAPbouiCOM.BoMessageTime.bmt_Short, false);
            LoadGrid();
        }

        private void OnCustomInitialize()
        {

        }
    }
}
