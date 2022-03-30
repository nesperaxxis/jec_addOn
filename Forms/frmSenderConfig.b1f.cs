using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM.Framework;
using SAPbobsCOM;
using Application = SAPbouiCOM.Framework.Application;
using BoFormMode = SAPbouiCOM.BoFormMode;
using System.Net;
using System.Net.Mail;

namespace JEC_SAP.Forms
{
    [FormAttribute("frmSenderConfig", "Forms/frmSenderConfig.b1f")]
    class frmSenderConfig : UserFormBase
    {
        private static SAPbobsCOM.Recordset oRS { get; set; }
        private static SAPbobsCOM.GeneralService oGeneralService { get; set; }
        private static SAPbobsCOM.CompanyService oCompService { get; set; }
        private static SAPbobsCOM.GeneralData oGeneralData { get; set; }
        private static SAPbobsCOM.GeneralDataParams oGeneralParams { get; set; }

        private SAPbouiCOM.StaticText stEmail { get; set; }
        private SAPbouiCOM.StaticText stPW { get; set; }
        private SAPbouiCOM.EditText edPW { get; set; }
        private SAPbouiCOM.EditText edEmail { get; set; }
        private SAPbouiCOM.StaticText stTo { get; set; }
        private SAPbouiCOM.EditText edTo { get; set; }
        private SAPbouiCOM.Folder fdSender { get; set; }
        private SAPbouiCOM.Folder fdBody { get; set; }
        private SAPbouiCOM.StaticText stToNote { get; set; }
        private SAPbouiCOM.EditText edBody { get; set; }
        private SAPbouiCOM.StaticText stHost { get; set; }
        private SAPbouiCOM.StaticText stPort { get; set; }
        private SAPbouiCOM.EditText edHost { get; set; }
        private SAPbouiCOM.EditText edPort { get; set; }
        private SAPbouiCOM.Button btSave { get; set; }
        private SAPbouiCOM.Button btCancel { get; set; }
        private SAPbouiCOM.Button btMail { get; set; }
        private SAPbouiCOM.CheckBox chkSSL { get; set; }
        private SAPbouiCOM.StaticText stCc { get; set; }
        private SAPbouiCOM.EditText edCc { get; set; }
        private SAPbouiCOM.StaticText stBcc { get; set; }
        private SAPbouiCOM.EditText edBcc { get; set; }
        private SAPbouiCOM.Button btRec { get; set; }

        private SAPbouiCOM.Folder fdSalute { get; set; }
        private SAPbouiCOM.Folder fdCompli { get; set; }
        private SAPbouiCOM.EditText edSalute { get; set; }
        private SAPbouiCOM.EditText edCompli { get; set; }

        public frmSenderConfig()
        {
            GetSenderSettings();
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.stEmail = ((SAPbouiCOM.StaticText)(this.GetItem("stFrom").Specific));
            this.stPW = ((SAPbouiCOM.StaticText)(this.GetItem("stPW").Specific));
            this.edPW = ((SAPbouiCOM.EditText)(this.GetItem("edPW").Specific));
            this.edEmail = ((SAPbouiCOM.EditText)(this.GetItem("edEmail").Specific));
            this.btSave = ((SAPbouiCOM.Button)(this.GetItem("1").Specific));
            this.btSave.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btSave_PressedBefore);
            this.btSave.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.btSave_PressedAfter);
            this.btCancel = ((SAPbouiCOM.Button)(this.GetItem("2").Specific));
            this.fdSender = ((SAPbouiCOM.Folder)(this.GetItem("fdSender").Specific));
            this.fdBody = ((SAPbouiCOM.Folder)(this.GetItem("fdBody").Specific));
            this.edBody = ((SAPbouiCOM.EditText)(this.GetItem("edBody").Specific));
            this.stHost = ((SAPbouiCOM.StaticText)(this.GetItem("stHost").Specific));
            this.stPort = ((SAPbouiCOM.StaticText)(this.GetItem("stPort").Specific));
            this.edHost = ((SAPbouiCOM.EditText)(this.GetItem("edHost").Specific));
            this.edPort = ((SAPbouiCOM.EditText)(this.GetItem("edPort").Specific));
            this.btMail = ((SAPbouiCOM.Button)(this.GetItem("btMail").Specific));
            this.btMail.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btMail_PressedBefore);
            this.btMail.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.btMail_PressedAfter);
            this.stTo = ((SAPbouiCOM.StaticText)(this.GetItem("stTo").Specific));
            this.edTo = ((SAPbouiCOM.EditText)(this.GetItem("btTo").Specific));
            this.stToNote = ((SAPbouiCOM.StaticText)(this.GetItem("stToNote").Specific));
            this.chkSSL = ((SAPbouiCOM.CheckBox)(this.GetItem("chkSSL").Specific));
            this.stCc = ((SAPbouiCOM.StaticText)(this.GetItem("stCc").Specific));
            this.edCc = ((SAPbouiCOM.EditText)(this.GetItem("edCc").Specific));
            this.stBcc = ((SAPbouiCOM.StaticText)(this.GetItem("stBcc").Specific));
            this.edBcc = ((SAPbouiCOM.EditText)(this.GetItem("edBcc").Specific));
            this.btRec = ((SAPbouiCOM.Button)(this.GetItem("btRec").Specific));
            this.btRec.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.btRec_PressedAfter);
            this.fdSalute = ((SAPbouiCOM.Folder)(this.GetItem("fdSalute").Specific));
            this.fdCompli = ((SAPbouiCOM.Folder)(this.GetItem("fdCompli").Specific));
            this.edSalute = ((SAPbouiCOM.EditText)(this.GetItem("edSalute").Specific));
            this.edCompli = ((SAPbouiCOM.EditText)(this.GetItem("edCompli").Specific));
            this.OnCustomInitialize();
        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
        }

        private void OnCustomInitialize()
        {
            Helpers.GlobalVar.vbWindowsHelper = new vbHelper_Library.Windows();
        }

        private void btSave_PressedBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (edEmail.Value == "" || edPW.Value == "")
                {
                    Application.SBO_Application.SetStatusBarMessage("Please enter valid email address and correct password.", SAPbouiCOM.BoMessageTime.bmt_Short, true);
                    BubbleEvent = false;
                }
                else
                {
                    int iReturn = Application.SBO_Application.MessageBox("Save Sender E-Mail Credentials, Do you want to continue?", 2, "Yes", "No");
                    if (!iReturn.Equals(1))
                    {
                        BubbleEvent = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.SetStatusBarMessage(ex.ToString());
            }
        }

        private void GetSenderSettings()
        {
            oRS = (Recordset)Helpers.GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            oRS.DoQuery("select * from \"@OEMAIL\"");
            if (oRS.RecordCount > 0)
            {
                edEmail.Value = Convert.ToString(oRS.Fields.Item("U_EMailAdd").Value);
                edPW.Value = Convert.ToString(Helpers.GlobalVar.vbWindowsHelper.Decrypt(oRS.Fields.Item("U_EMailPw").Value.ToString()));
                edHost.Value = Convert.ToString(oRS.Fields.Item("U_EMailHost").Value);
                edPort.Value = Convert.ToString(oRS.Fields.Item("U_EMailPort").Value);
                edBody.Value = Convert.ToString(oRS.Fields.Item("U_EMailBody").Value);
                edSalute.Value = Convert.ToString(oRS.Fields.Item("U_EMailSalute").Value);
                edCompli.Value = Convert.ToString(oRS.Fields.Item("U_EMailCompli").Value);
                edCc.Value = Convert.ToString(oRS.Fields.Item("U_EMailCc").Value);
                edBcc.Value = Convert.ToString(oRS.Fields.Item("U_EMailBcc").Value);
                if (Convert.ToString(oRS.Fields.Item("U_EMailSSL").Value) == "Y")
                {
                    chkSSL.Checked = true;
                }
                else
                {
                    chkSSL.Checked = false;
                }
            }
        }

        private void btRec_PressedAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                Application.SBO_Application.ActivateMenuItem(Helpers.GlobalVar.GetSubMenuID("EMAILMODULES - Recipient per Module Setup"));
            }
            catch (Exception ex)
            {
                Application.SBO_Application.SetStatusBarMessage(ex.ToString());
            }
        }

        private void btSave_PressedAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                if (this.edEmail.Value != "" && this.edPW.Value != "")
                {
                    oRS = (Recordset)Helpers.GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                    oCompService = Helpers.GlobalVar.myCompany.GetCompanyService();
                    oGeneralService = oCompService.GetGeneralService("OEMAIL");
                    oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);
                    oGeneralParams = (SAPbobsCOM.GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);

                    oRS.DoQuery("select * from \"@OEMAIL\"");
                    if (oRS.RecordCount > 0)
                    {
                        Helpers.GlobalVar.myCompany.StartTransaction();
                        //Setting Params
                        oGeneralParams.SetProperty("Code", oRS.Fields.Item("Code").Value);
                        oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                        //Setting Data to Master Data Table Fields
                        oGeneralData.SetProperty("U_EMailAdd", edEmail.Value);
                        oGeneralData.SetProperty("U_EMailPw", Helpers.GlobalVar.vbWindowsHelper.Encrypt(edPW.Value));
                        oGeneralData.SetProperty("U_EMailHost", edHost.Value);
                        oGeneralData.SetProperty("U_EMailPort", edPort.Value);
                        oGeneralData.SetProperty("U_EMailBody", edBody.Value);
                        oGeneralData.SetProperty("U_EMailSalute", edSalute.Value);
                        oGeneralData.SetProperty("U_EMailCompli", edCompli.Value);
                        oGeneralData.SetProperty("U_EMailCc", edCc.Value);
                        oGeneralData.SetProperty("U_EMailBcc", edBcc.Value);
                        if (chkSSL.Checked == true)
                        {
                            oGeneralData.SetProperty("U_EMailSSL", "Y");
                        }
                        else
                            oGeneralData.SetProperty("U_EMailSSL", "N");


                        //Updating Data to Master Data Table Fields
                        oGeneralService.Update(oGeneralData);
                        if (Helpers.GlobalVar.myCompany.InTransaction)
                        {
                            Helpers.GlobalVar.myCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                            Application.SBO_Application.SetStatusBarMessage("Successfully update credentials.", SAPbouiCOM.BoMessageTime.bmt_Short, false);
                        }
                    }
                    else
                    {
                        Helpers.GlobalVar.myCompany.StartTransaction();
                        //Setting Data to Master Data Table Fields
                        oGeneralData.SetProperty("Code", "Sender");
                        oGeneralData.SetProperty("Name", "Sender");
                        oGeneralData.SetProperty("U_EMailAdd", edEmail.Value);
                        oGeneralData.SetProperty("U_EMailPw", edPW.Value);
                        oGeneralData.SetProperty("U_EMailHost", edHost.Value);
                        oGeneralData.SetProperty("U_EMailPort", edPort.Value);
                        oGeneralData.SetProperty("U_EMailBody", edBody.Value);
                        oGeneralData.SetProperty("U_EMailSalute", edSalute.Value);
                        oGeneralData.SetProperty("U_EMailCompli", edCompli.Value);
                        oGeneralData.SetProperty("U_EMailCc", edCc.Value);
                        oGeneralData.SetProperty("U_EMailBcc", edBcc.Value);
                        if (chkSSL.Checked == true)
                        {
                            oGeneralData.SetProperty("U_EMailSSL", "Y");
                        }
                        else
                            oGeneralData.SetProperty("U_EMailSSL", "N");

                        //Adding Data to Master Data Table Fields
                        oGeneralService.Add(oGeneralData);
                        if (Helpers.GlobalVar.myCompany.InTransaction)
                        {
                            Helpers.GlobalVar.myCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                            Application.SBO_Application.SetStatusBarMessage("Successfully save credentials.", SAPbouiCOM.BoMessageTime.bmt_Short, false);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Application.SBO_Application.SetStatusBarMessage(ex.ToString());
            }
        }

        private void btMail_PressedBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (edEmail.Value == "" || edPW.Value == "" || edHost.Value == "" || edPort.Value == "")
                {
                    Application.SBO_Application.SetStatusBarMessage("Please enter valid email address and correct password.", SAPbouiCOM.BoMessageTime.bmt_Short, true);
                    BubbleEvent = false;
                }
                else
                {
                    int iReturn = Application.SBO_Application.MessageBox("Send Test Mail, Do you want to continue?", 2, "Yes", "No");
                    if (!iReturn.Equals(1))
                    {
                        BubbleEvent = false;
                    }
                    else
                    {
                        if (edTo.Value == "")
                        {
                            Application.SBO_Application.SetStatusBarMessage("Please enter valid email address in Email To.", SAPbouiCOM.BoMessageTime.bmt_Short, true);
                            BubbleEvent = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.SetStatusBarMessage(ex.ToString());
            }
        }

        private void btMail_PressedAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                if (this.edEmail.Value != "" && this.edPW.Value != "" && this.edHost.Value != "" && this.edPort.Value != "" && this.edTo.Value != "")
                {
                    Application.SBO_Application.SetStatusBarMessage("Sending Test E-Mail to " + edTo.Value, SAPbouiCOM.BoMessageTime.bmt_Short, false);

                    MailMessage mM = new MailMessage();
                    SmtpClient MailClient = new SmtpClient();

                    if (edPort.Value != "")
                        MailClient.Port = Convert.ToInt16(edPort.Value);

                    if (edHost.Value != "")
                        MailClient.Host = edHost.Value;

                    MailClient.Timeout = 100000;
                    MailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    MailClient.UseDefaultCredentials = false;

                    if (chkSSL.Checked == true)
                    {
                        MailClient.EnableSsl = true;
                    }
                    else
                        MailClient.EnableSsl = false;

                    MailClient.Credentials = new System.Net.NetworkCredential(edEmail.Value, edPW.Value);

                    if (edEmail.Value != "")
                        mM.From = new MailAddress(edEmail.Value);

                    if (edTo.Value != "")
                        mM.To.Add(edTo.Value);

                    mM.Subject = "Test E-Mail";
                    mM.Body = "Test E-Mail";

                    if (edCc.Value != "")
                        mM.CC.Add(edCc.Value);

                    if (edBcc.Value != "")
                        mM.Bcc.Add(edBcc.Value);

                    try
                    {
                        MailClient.Send(mM);
                        MailClient.Dispose();
                        mM.Dispose();
                        Application.SBO_Application.SetStatusBarMessage("Successfully send test mail to " + edTo.Value, SAPbouiCOM.BoMessageTime.bmt_Short, false);
                    }
                    catch (Exception ex)
                    {
                        Application.SBO_Application.SetStatusBarMessage(ex.ToString());
                    }
                }
            }

            catch (Exception ex)
            {
                Application.SBO_Application.SetStatusBarMessage(ex.ToString());
            }
        }
    }
}
