using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web.ASPxGridView;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Editors;
using DevExpress.Web.ASPxClasses;
using DevExpress.ExpressApp.DC;
using DevExpress.Web.ASPxCallback;

namespace WebExample.Module.Web {
    public class MultiRowEditingController : ViewController {
        public MultiRowEditingController() {
            TargetViewId = "DomainObject1_ListView";
        }
        const String CallbackArgumentFormat = "function (s, e) {{ {0}.PerformCallback(\"{1}|{2}|\" + {3}); }}"; // ASPxCallback, key, fieldName, value
        ASPxCallback callback;
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            ASPxGridListEditor listEditor = ((ListView)View).Editor as ASPxGridListEditor;
            if (listEditor != null) {
                callback = new ASPxCallback();
                callback.ID = View.Id + "aspxCallback1";
                callback.ClientInstanceName = View.Id + "_callback1";
                callback.Callback += new CallbackEventHandler(callback_Callback);
                ((Control)View.Control).Controls.Add(callback);
                ASPxGridView gridView = (ASPxGridView)listEditor.Grid;
                foreach (GridViewColumn column in gridView.VisibleColumns) {
                    if (column is GridViewDataColumn) {
                        GridViewDataColumn dataColumn = (GridViewDataColumn)column;
                        DataItemTemplate defaultTemplate = dataColumn.DataItemTemplate as DataItemTemplate;
                        if (defaultTemplate != null) {
                            EditItemTemplate newTemplate = new EditItemTemplate(defaultTemplate.PropertyEditor, listEditor);
                            newTemplate.CustomCreateCellControl += new EventHandler<CustomCreateCellControlEventArgs>(newTemplate_CustomCreateCellControl);
                            dataColumn.DataItemTemplate = newTemplate;
                        }
                    }
                }
            }
        }
        void callback_Callback(object source, DevExpress.Web.ASPxCallback.CallbackEventArgs e) {
            String[] p = e.Parameter.Split('|');

            Object key = TypeDescriptor.GetConverter(View.ObjectTypeInfo.KeyMember.MemberType).ConvertFromString(p[0]);
            IMemberInfo member = View.ObjectTypeInfo.FindMember(p[1]);
            Object value = TypeDescriptor.GetConverter(member.MemberType).ConvertFromString(p[2]); ;

            object obj = ObjectSpace.GetObjectByKey(View.ObjectTypeInfo.Type, key);
            member.SetValue(obj, value);
            ObjectSpace.CommitChanges();
        }
        void newTemplate_CustomCreateCellControl(object sender, CustomCreateCellControlEventArgs e) {
            if (e.PropertyEditor.Editor is ASPxWebControl) {
                e.PropertyEditor.Editor.Init += new EventHandler(Editor_Init);
            }
        }
        void Editor_Init(object sender, EventArgs e) {
            ASPxWebControl editor = (ASPxWebControl)sender;
            GridViewDataItemTemplateContainer container = (GridViewDataItemTemplateContainer)editor.NamingContainer;
            editor.SetClientSideEventHandler("ValueChanged", String.Format(CallbackArgumentFormat,
                callback.ClientInstanceName, container.KeyValue, container.Column.FieldName, "s.GetValue()"));
        }
    }
    public class EditItemTemplate : DataItemTemplate, ITemplate {
        public EditItemTemplate(WebPropertyEditor propertyEditor, IDataItemTemplateInfoProvider provider) : base(propertyEditor, provider, ViewEditMode.Edit) { }
        void ITemplate.InstantiateIn(Control container) {
            base.InstantiateIn(container);
            PropertyEditor.Editor.Attributes["onclick"] = RenderHelper.EventCancelBubbleCommand;
        }
    }
}