using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM.Framework;
using SAPbobsCOM;
using BoFormMode = SAPbouiCOM.BoFormMode;
using Application = SAPbouiCOM.Framework.Application;

namespace JEC_SAP.Forms
{
    [FormAttribute("frmCRED", "Forms/frmCRED.b1f")]
    class frmCRED : UserFormBase
    {
        private static SAPbobsCOM.Company oCompany { get; set; }
        private static SAPbobsCOM.Recordset oRS { get; set; }

        private static SAPbobsCOM.GeneralService oGeneralService;
        private static SAPbobsCOM.CompanyService oCompService;
        private static SAPbobsCOM.GeneralData oGeneralData;
        private static SAPbobsCOM.GeneralDataParams oGeneralParams;

        private SAPbouiCOM.StaticText stServer;
        private SAPbouiCOM.EditText edServer;
        private SAPbouiCOM.StaticText stUN;
        private SAPbouiCOM.EditText edUN;
        private SAPbouiCOM.StaticText stPW;
        private SAPbouiCOM.EditText edPW;
        private SAPbouiCOM.StaticText stType;
        private SAPbouiCOM.ComboBox cmbType;
        private SAPbouiCOM.StaticText stB1UN;
        private SAPbouiCOM.EditText edB1UN;
        private SAPbouiCOM.StaticText stB1PW;
        private SAPbouiCOM.EditText edB1PW;
        private SAPbouiCOM.Button btSave;
        private SAPbouiCOM.Button btCancel;

        public frmCRED()
        {
            GetServerCredentials();
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.stServer = ((SAPbouiCOM.StaticText)(this.GetItem("stServer").Specific));
            this.edServer = ((SAPbouiCOM.EditText)(this.GetItem("edServer").Specific));
            this.btSave = ((SAPbouiCOM.Button)(this.GetItem("1").Specific));
            this.btSave.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btSave_PressedBefore);
            this.btSave.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.btSave_PressedAfter);
            this.btCancel = ((SAPbouiCOM.Button)(this.GetItem("2").Specific));
            this.stUN = ((SAPbouiCOM.StaticText)(this.GetItem("stServer").Specific));
            this.stPW = ((SAPbouiCOM.StaticText)(this.GetItem("stPW").Specific));
            this.edUN = ((SAPbouiCOM.EditText)(this.GetItem("edUN").Specific));
            this.edPW = ((SAPbouiCOM.EditText)(this.GetItem("edPW").Specific));
            this.stType = ((SAPbouiCOM.StaticText)(this.GetItem("stType").Specific));
            this.edB1UN = ((SAPbouiCOM.EditText)(this.GetItem("edB1UN").Specific));
            this.stB1UN = ((SAPbouiCOM.StaticText)(this.GetItem("stB1UN").Specific));
            this.edB1PW = ((SAPbouiCOM.EditText)(this.GetItem("edB1PW").Specific));
            this.stB1PW = ((SAPbouiCOM.StaticText)(this.GetItem("stB1PW").Specific));
            this.cmbType = ((SAPbouiCOM.ComboBox)(this.GetItem("cmbType").Specific));
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
                if (cmbType.Value == "" || edServer.Value == "" || edUN.Value == "" || edPW.Value == "" || edB1UN.Value == "" || edB1PW.Value == "")
                {
                    Application.SBO_Application.SetStatusBarMessage("Please complete following credentials.", SAPbouiCOM.BoMessageTime.bmt_Short, false);
                    BubbleEvent = false;
                }
                else
                {
                    int iReturn = Application.SBO_Application.MessageBox("Save Credentials, Do you want to continue?", 2, "Yes", "No");
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

        private void GetServerCredentials()
        {
            oCompany = new Company();
            oCompany = (Company)Application.SBO_Application.Company.GetDICompany();
            oRS = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            oRS.DoQuery("select * from \"@OCRED\" where \"Code\" = '" + oCompany.CompanyDB + "'");
            if (oRS.RecordCount > 0)
            {
                cmbType.Select(Convert.ToString(oRS.Fields.Item("U_ServerType").Value), SAPbouiCOM.BoSearchKey.psk_ByValue);
                edServer.Value = oCompany.Server;
                edUN.Value = Convert.ToString(oRS.Fields.Item("U_ServerUN").Value);
                edPW.Value = Convert.ToString(Helpers.GlobalVar.vbWindowsHelper.Decrypt(oRS.Fields.Item("U_ServerPW").Value.ToString()));
                edB1UN.Value = Convert.ToString(oRS.Fields.Item("U_B1UN").Value);
                edB1PW.Value = Convert.ToString(Helpers.GlobalVar.vbWindowsHelper.Decrypt(oRS.Fields.Item("U_B1PW").Value.ToString()));
            }
            else
            {
                edServer.Value = oCompany.Server;
                cmbType.Select(oCompany.DbServerType.ToString(), SAPbouiCOM.BoSearchKey.psk_ByValue);
                edUN.Value = oCompany.DbUserName;
                edB1UN.Value = oCompany.UserName;
            }
        }

        private void btSave_PressedAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                if (cmbType.Value != "" || edServer.Value != "" || edUN.Value != "" || edPW.Value != "" || edB1UN.Value != "" || edB1PW.Value != "")
                {
                    //if (Helpers.GlobalVar.ConnectDI(cmbType.Value, edServer.Value, edUN.Value, edPW.Value, oCompany.CompanyDB, oCompany.LicenseServer, edB1UN.Value, edB1PW.Value) == false)
                    //{
                    //    return;
                    //}

                    oCompany = new Company();
                    oCompany = (Company)Application.SBO_Application.Company.GetDICompany();
                    oRS = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                    oCompService = oCompany.GetCompanyService();
                    oGeneralService = oCompService.GetGeneralService("OCRED");
                    oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);
                    oGeneralParams = (SAPbobsCOM.GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);

                    oRS.DoQuery("select * from \"@OCRED\" where \"Code\" = '" + oCompany.CompanyDB + "'");
                    if (oRS.RecordCount > 0)
                    {
                        oCompany.StartTransaction();
                        //Setting Params
                        oGeneralParams.SetProperty("Code", oCompany.CompanyDB);
                        oGeneralData = oGeneralService.GetByParams(oGeneralParams);

                        //Setting Data to Master Data Table Fields
                        oGeneralData.SetProperty("Name", oCompany.CompanyDB);   
                        oGeneralData.SetProperty("U_ServerType", cmbType.Value);
                        oGeneralData.SetProperty("U_ServerName", edServer.Value);
                        oGeneralData.SetProperty("U_ServerUN", edUN.Value);
                        oGeneralData.SetProperty("U_ServerPW", Helpers.GlobalVar.vbWindowsHelper.Encrypt(edPW.Value));
                        oGeneralData.SetProperty("U_B1UN", edB1UN.Value);
                        oGeneralData.SetProperty("U_B1PW", Helpers.GlobalVar.vbWindowsHelper.Encrypt(edB1PW.Value));

                        //Updating Data to Master Data Table Fields
                        oGeneralService.Update(oGeneralData);
                        if (oCompany.InTransaction)
                        {
                            oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                            Application.SBO_Application.SetStatusBarMessage("Successfully update credentials.", SAPbouiCOM.BoMessageTime.bmt_Short, false);
                        }
                    }
                    else
                    {
                        oCompany.StartTransaction();
                        //Setting Data to Master Data Table Fields
                        oGeneralData.SetProperty("Code", oCompany.CompanyDB);
                        oGeneralData.SetProperty("Name", oCompany.CompanyDB);
                        oGeneralData.SetProperty("U_ServerType", cmbType.Value);
                        oGeneralData.SetProperty("U_ServerName", edServer.Value);
                        oGeneralData.SetProperty("U_ServerUN", edUN.Value);
                        oGeneralData.SetProperty("U_ServerPW", Helpers.GlobalVar.vbWindowsHelper.Encrypt(edPW.Value));
                        oGeneralData.SetProperty("U_B1UN", edB1UN.Value);
                        oGeneralData.SetProperty("U_B1PW", Helpers.GlobalVar.vbWindowsHelper.Encrypt(edB1PW.Value));

                        //Adding Data to Master Data Table Fields
                        oGeneralService.Add(oGeneralData);
                        if (oCompany.InTransaction)
                        {
                            oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
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
    }
}
