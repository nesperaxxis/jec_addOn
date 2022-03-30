using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM.Framework;

namespace Booking_Shipping_Order
{
    [FormAttribute("139", "Forms/Sales Order.b1f")]
    class Sales_Order : SystemFormBase
    {
        private SAPbobsCOM.Company oCompany;
        private SAPbouiCOM.Form oForm;
        private SAPbobsCOM.Recordset oRS;

        public Sales_Order()
        {
            //oCompany = (SAPbobsCOM.Company)Application.SBO_Application.Company.GetDICompany();
            //oRS = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);               
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.DataLoadBefore += new SAPbouiCOM.Framework.FormBase.DataLoadBeforeHandler(this.Form_DataLoadBefore);
            this.DataLoadAfter += new DataLoadAfterHandler(this.Form_DataLoadAfter);

        }

        private void OnCustomInitialize()
        {

        }

        private void Form_DataLoadBefore(ref SAPbouiCOM.BusinessObjectInfo pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                oForm = Application.SBO_Application.Forms.GetFormByTypeAndCount(-139, 1);
                //oForm.Items.Item("U_DocOfficer").Enabled = false;
            }
            catch (Exception)
            {
            }
        }

        private void Form_DataLoadAfter(ref SAPbouiCOM.BusinessObjectInfo pVal)
        {
            try
            {
                oForm = Application.SBO_Application.Forms.GetFormByTypeAndCount(-139, 1);
                //oForm.Items.Item("U_DocOfficer").Enabled = false;
            }
            catch (Exception)
            {
            }
        }
    }
}
