using System;
using System.Collections.Generic;
using System.Text;
using SAPbouiCOM.Framework;
using SAPbobsCOM;
using Application = SAPbouiCOM.Framework.Application;
using BoFormMode = SAPbouiCOM.BoFormMode;

namespace JEC_SAP
{
    class Menu
    {
        private SAPbouiCOM.Form oForm { get; set; }
        private SAPbouiCOM.Grid iGridData { get; set; }
        private SAPbouiCOM.ComboBox icbPoint { get; set; }
        private SAPbouiCOM.EditTextColumn oColumns { get; set; }
        private SAPbouiCOM.Button ibtSend { get; set; }
        public SAPbobsCOM.Recordset oRS { get; set; }
        public SAPbouiCOM.DataTable oDataTable { get; set; }

        public void AddMenuItems()
        {

            SAPbouiCOM.Menus oMenus = null;
            SAPbouiCOM.MenuItem oMenuItem = null;

            oMenus = Application.SBO_Application.Menus;

            SAPbouiCOM.MenuCreationParams oCreationPackageMain = null;
            oCreationPackageMain = ((SAPbouiCOM.MenuCreationParams)(Application.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)));
            SAPbouiCOM.MenuCreationParams oCreationPackage = null;
            oCreationPackage = ((SAPbouiCOM.MenuCreationParams)(Application.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)));
            oMenuItem = Application.SBO_Application.Menus.Item("43520"); // modules'

            oCreationPackageMain.Type = SAPbouiCOM.BoMenuType.mt_POPUP;
            oCreationPackageMain.UniqueID = "TAIDII_SAP";
            oCreationPackageMain.String = "TAIDII_SAP Integration";
            oCreationPackageMain.Enabled = true;
            oCreationPackageMain.Position = -1;
            oCreationPackageMain.Image = System.Windows.Forms.Application.StartupPath + "\\icon.png";

            oMenus = oMenuItem.SubMenus;

            try
            {
                //  If the manu already exists this code will fail
                oMenus.AddEx(oCreationPackageMain);
            }
            catch { }

            try
            {
                // Get the menu collection of the newly added pop-up item
                oMenuItem = Application.SBO_Application.Menus.Item("TAIDII_SAP");
                oMenus = oMenuItem.SubMenus;

                // Create sub menu
                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                oCreationPackage.UniqueID = "OINTEGSETUP";
                oCreationPackage.String = "Integration Setup";
                oMenus.AddEx(oCreationPackage);

                // Create sub menu
                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                oCreationPackage.UniqueID = "OACTIVATE";
                oCreationPackage.String = "Activate Company Setup";
                oMenus.AddEx(oCreationPackage);

                // Create sub menu
                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                oCreationPackage.UniqueID = "OGLMAPPING";
                oCreationPackage.String = "G/L Account Mapping Setup (Invoice and Credit Note)";
                oMenus.AddEx(oCreationPackage);

                // Create sub menu
                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                oCreationPackage.UniqueID = "OPAYCODE";
                oCreationPackage.String = "Payment Codes Setup";
                oMenus.AddEx(oCreationPackage);

                // Create sub menu
                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                oCreationPackage.UniqueID = "OSENDER";
                oCreationPackage.String = "Sender E-Mail Credentials Setup";
                oMenus.AddEx(oCreationPackage);

                // Create sub menu
                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                oCreationPackage.UniqueID = "OCRED";
                oCreationPackage.String = "Server and SAP B1 Credentials Setup";
                oMenus.AddEx(oCreationPackage);

                // Create sub menu
                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                oCreationPackage.UniqueID = "OMANUALSEND";
                oCreationPackage.String = "TAIDII - SAP B1 Integration Log";
                oMenus.AddEx(oCreationPackage);
            }
            catch (Exception)
            { //  Menu already exists
                //Application.SBO_Application.SetStatusBarMessage("Menu Already Exists", SAPbouiCOM.BoMessageTime.bmt_Short, true);
            }
        }

        public void SBO_Application_MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                try
                {
                    oForm = Application.SBO_Application.Forms.ActiveForm;
                }
                catch
                { }
                switch (pVal.BeforeAction)
                {
                    case false:
                        switch (pVal.MenuUID)
                        {
                            case "OINTEGSETUP":
                                Application.SBO_Application.ActivateMenuItem(Helpers.GlobalVar.GetSubMenuID("INTEGRATIONSETUP - Integration Setup"));
                                break;
                            case "OGLMAPPING":
                                Application.SBO_Application.ActivateMenuItem(Helpers.GlobalVar.GetSubMenuID("GLACCTMAPPING - G/L Account Mapping Setup"));
                                break;
                            case "OPAYCODE":
                                Application.SBO_Application.ActivateMenuItem(Helpers.GlobalVar.GetSubMenuID("PAYMENTCODES - Payment Code Setup"));
                                break;
                            case "OMANUALSEND":

                                JEC_SAP.Forms.frmManualSending ofrmManualSending = new JEC_SAP.Forms.frmManualSending();
                                ofrmManualSending.Show();
                                oForm = Application.SBO_Application.Forms.ActiveForm;

                                oForm.Freeze(true);

                                icbPoint = ((SAPbouiCOM.ComboBox)(oForm.Items.Item("cbPoint").Specific));

                                icbPoint.ValidValues.Add("1", "Student Master (Student List)");
                                icbPoint.ValidValues.Add("2", "Invoices (Invoice List)");
                                icbPoint.ValidValues.Add("3", "Credit Note (Credit Note List)");
                                icbPoint.ValidValues.Add("4", "Credit Note Void (Credit Note List)");
                                icbPoint.ValidValues.Add("5", "Invoice Void (Invoice List)");
                                icbPoint.ValidValues.Add("6", "Receipts (Receipt List)");
                                icbPoint.ValidValues.Add("7", "Receipts Void (Receipt List)");
                                //icbPoint.ValidValues.Add("8", "Receipts with Applied Deposits (Credit Refund List) ");
                                //icbPoint.ValidValues.Add("9", "Receipts with Applied Deposits Void (Credit Refund List)");
                                icbPoint.ValidValues.Add("10", "Products");
                                icbPoint.ValidValues.Add("11", "All");
                                //icbPoint.ValidValues.Add("12", "Deposit (Credit Note List)");
                                //icbPoint.ValidValues.Add("13", "Deposit Void (Credit Note List)");

                                oForm.Freeze(false);

                                break;
                            case "OACTIVATE":
                                JEC_SAP.Forms.frmActivateCompany ofrmActivateCompany = new JEC_SAP.Forms.frmActivateCompany();
                                ofrmActivateCompany.Show();
                                oForm = Application.SBO_Application.Forms.ActiveForm;
                                break;
                            case "OSENDER":
                                JEC_SAP.Forms.frmSenderConfig ofrmSenderConfig = new JEC_SAP.Forms.frmSenderConfig();
                                oForm = Application.SBO_Application.Forms.ActiveForm;
                                oForm.ReportType = "RCRI";
                                ofrmSenderConfig.Show();
                                break;
                            case "OCRED":
                                JEC_SAP.Forms.frmCRED ofrmSQL = new JEC_SAP.Forms.frmCRED();
                                oForm = Application.SBO_Application.Forms.ActiveForm;
                                oForm.ReportType = "RCRI";
                                ofrmSQL.Show();
                                break;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                oForm.Freeze(false);
                Application.SBO_Application.MessageBox(ex.ToString(), 1, "Ok", "", "");
            }
        }
    }
}
