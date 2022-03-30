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
    [FormAttribute("frmActivateCompany", "Forms/frmActivateCompany.b1f")]
    class frmActivateCompany : UserFormBase
    {
        private SAPbouiCOM.Button btSave { get; set; }
        private SAPbouiCOM.Button btCancel { get; set; }
        private SAPbouiCOM.Grid iGrid { get; set; }
        private SAPbouiCOM.DataTable oDataTable { get; set; }
        private SAPbouiCOM.EditTextColumn oColumns { get; set; }

        public frmActivateCompany()
        {

        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.iGrid = ((SAPbouiCOM.Grid)(this.GetItem("iGrid").Specific));
            this.btSave = ((SAPbouiCOM.Button)(this.GetItem("1").Specific));
            this.btSave.PressedBefore += new SAPbouiCOM._IButtonEvents_PressedBeforeEventHandler(this.btSave_PressedBefore);
            this.btSave.PressedAfter += new SAPbouiCOM._IButtonEvents_PressedAfterEventHandler(this.btSave_PressedAfter);
            this.btCancel = ((SAPbouiCOM.Button)(this.GetItem("2").Specific));
        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {

        }

        private void OnCustomInitialize()
        {
            try
            {
                Application.SBO_Application.Forms.ActiveForm.Freeze(true);
                Helpers.GlobalVar.InsertDatabase(Helpers.GlobalVar.myCompany.CompanyDB);
                try
                {
                    oDataTable = Application.SBO_Application.Forms.ActiveForm.DataSources.DataTables.Item("CompanyList");
                }
                catch (Exception)
                {
                    oDataTable = Application.SBO_Application.Forms.ActiveForm.DataSources.DataTables.Add("CompanyList");
                }

                Helpers.GlobalVar.oRSQuery = (Recordset)Helpers.GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                if (Helpers.GlobalVar.myCompany.DbServerType == BoDataServerTypes.dst_HANADB)
                {
                    Helpers.GlobalVar.oQuery = "SELECT " + Environment.NewLine +
                    "\"companyDB\" \"Company DB\", " + Environment.NewLine +
                    "\"clientName\" \"Client Name\", " + Environment.NewLine +
                    "\"Curr\" \"Currency\", " + Environment.NewLine +
                    "\"Country\" \"Country\", " + Environment.NewLine +
                    "\"Group\" \"Group\", " + Environment.NewLine +
                    "\"Division\" \"Division\", " + Environment.NewLine +
                    "\"Product\" \"Product\", " + Environment.NewLine +
                    "\"api_key\" \"API Key\", " + Environment.NewLine +
                    "\"base_url\" \"Base URL\", " + Environment.NewLine +
                    "\"pricelist_code\" \"Pricelist Code\", " + Environment.NewLine +
                    "\"activate\" \"Activate\" " + Environment.NewLine +
                    "FROM " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.myCompany.DbServerType == BoDataServerTypes.dst_HANADB, "\"TAIDII_SAP\".\"axxis_tb_IntegrationSetup\"", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationSetup\"") + "";
                    oDataTable.ExecuteQuery(Helpers.GlobalVar.oQuery);
                    this.iGrid.DataTable = oDataTable;
                }

                for (int i = 0; i < this.iGrid.Columns.Count; i++)
                {
                    if (i == 0)
                    {
                        this.iGrid.Columns.Item(i).Editable = false;
                    }
                }

                oColumns = (SAPbouiCOM.EditTextColumn)this.iGrid.Columns.Item(10);
                oColumns.Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox;

                BindActivate();
                Application.SBO_Application.Forms.ActiveForm.Freeze(false);
            }
            catch (Exception ex)
            {
                Application.SBO_Application.Forms.ActiveForm.Freeze(false);
                Application.SBO_Application.MessageBox(ex.ToString());
            }
        }

        private void BindActivate()
        {
            SAPbouiCOM.ComboBoxColumn oComboBoxColumn;
            this.iGrid.Columns.Item(2).Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox;
            oComboBoxColumn = ((SAPbouiCOM.ComboBoxColumn)this.iGrid.Columns.Item(2));

            Helpers.GlobalVar.oRS = (Recordset)Helpers.GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            if (Helpers.GlobalVar.myCompany.DbServerType == BoDataServerTypes.dst_HANADB)
            {
                Helpers.GlobalVar.oQuery = "SELECT 'N' \"Code\", 'No' \"Name\" from DUMMY UNION ALL SELECT 'Y' \"Code\", 'Yes' \"Name\" FROM DUMMY";
            }
            Helpers.GlobalVar.oRS.DoQuery(Helpers.GlobalVar.oQuery);
            while (!Helpers.GlobalVar.oRS.EoF)
            {
                oComboBoxColumn.ValidValues.Add(Helpers.GlobalVar.oRS.Fields.Item("Code").Value.ToString(), Helpers.GlobalVar.oRS.Fields.Item("Name").Value.ToString());
                Helpers.GlobalVar.oRS.MoveNext();
            }
        }

        private void btSave_PressedBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (btSave.Caption != "OK")
                {
                    int iReturn = Application.SBO_Application.MessageBox(btSave.Caption + " " + this.UIAPIRawForm.Title + " , Do you want to continue?", 2, "Yes", "No");
                    if (!iReturn.Equals(1))
                    {
                        BubbleEvent = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox(ex.ToString());
            }
        }

        private void btSave_PressedAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                for (int i = 0; i < this.iGrid.DataTable.Rows.Count; i++)
                {
                    string companyDB = this.iGrid.DataTable.Columns.Item(0).Cells.Item(i).Value.ToString();
                    string clientName = this.iGrid.DataTable.Columns.Item(1).Cells.Item(i).Value.ToString();
                    string Curr = this.iGrid.DataTable.Columns.Item(2).Cells.Item(i).Value.ToString();
                    string Country = this.iGrid.DataTable.Columns.Item(3).Cells.Item(i).Value.ToString();
                    string Group = this.iGrid.DataTable.Columns.Item(4).Cells.Item(i).Value.ToString();
                    string Division = this.iGrid.DataTable.Columns.Item(5).Cells.Item(i).Value.ToString();
                    string Product = this.iGrid.DataTable.Columns.Item(6).Cells.Item(i).Value.ToString();
                    string api_key = this.iGrid.DataTable.Columns.Item(7).Cells.Item(i).Value.ToString();
                    string base_url = this.iGrid.DataTable.Columns.Item(8).Cells.Item(i).Value.ToString();
                    string pricelist_code = this.iGrid.DataTable.Columns.Item(9).Cells.Item(i).Value.ToString();
                    string activate = this.iGrid.DataTable.Columns.Item(10).Cells.Item(i).Value.ToString();

                    Helpers.GlobalVar.oRS = (Recordset)Helpers.GlobalVar.myCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                    Helpers.GlobalVar.oQuery = "select * from " + Helpers.GlobalVar.Iif(Helpers.GlobalVar.myCompany.DbServerType == BoDataServerTypes.dst_HANADB, "\"TAIDII_SAP\".\"axxis_tb_IntegrationSetup\"", "\"TAIDII_SAP\"..\"axxis_tb_IntegrationSetup\"") + " " + Environment.NewLine +
                    "where \"companyDB\" = '" + Helpers.GlobalVar.TrimData(companyDB) + "'";
                    Helpers.GlobalVar.oRS.DoQuery(Helpers.GlobalVar.oQuery);

                    if (Helpers.GlobalVar.oRS.RecordCount == 0)
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
                this.OnCustomInitialize();
            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox(ex.ToString());
            }
        }
    }
}
