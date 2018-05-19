using System;
using System.ComponentModel;
using System.Web.UI;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web.ASPxGridView;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Editors;
using DevExpress.Web.ASPxClasses;
using DevExpress.ExpressApp.DC;
using DevExpress.Web.ASPxCallback;
using DevExpress.ExpressApp.Model;

namespace WebExample.Module.Web {
    public class MultiRowEditingController : ViewController<ListView> {
        public MultiRowEditingController() {
            TargetViewId = "DomainObject1_ListView";
        }
        const String CallbackArgumentFormat = "function (s, e) {{ {0}.PerformCallback(\"{1}|{2}|\" + {3}); }}"; // ASPxCallback, key, fieldName, value
        ASPxCallback callback;
        protected override void OnActivated() {
            base.OnActivated();
            ASPxGridListEditor listEditor = View.Editor as ASPxGridListEditor;
            if (listEditor != null) {
                listEditor.CustomCreateCellControl += new EventHandler<CustomCreateCellControlEventArgs>(listEditor_CustomCreateCellControl);
            }
        }
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            ASPxGridListEditor listEditor = View.Editor as ASPxGridListEditor;
            if (listEditor != null) {
                foreach (IModelColumn column in listEditor.Model.Columns) {
                    WebPropertyEditor propertyEditor = listEditor.FindPropertyEditor(column, ViewEditMode.View);
                    propertyEditor.ViewEditMode = ViewEditMode.Edit;
                }
                callback = new ASPxCallback();
                callback.ID = View.ObjectTypeInfo.Type.Name + "aspxCallback1";
                callback.ClientInstanceName = View.ObjectTypeInfo.Type.Name + "_callback1";
                callback.Callback += new CallbackEventHandler(callback_Callback);
                ((Control)View.Control).Controls.Add(callback);
            }
        }
        void listEditor_CustomCreateCellControl(object sender, CustomCreateCellControlEventArgs e) {
            if (e.PropertyEditor.Editor is ASPxWebControl) {
                e.PropertyEditor.Editor.Init += new EventHandler(Editor_Init);
            }
        }
        void Editor_Init(object sender, EventArgs e) {
            ASPxWebControl editor = (ASPxWebControl)sender;
            editor.Init -= Editor_Init;
            editor.Attributes["onclick"] = RenderHelper.EventCancelBubbleCommand;
            GridViewDataItemTemplateContainer container = (GridViewDataItemTemplateContainer)editor.NamingContainer;
            editor.SetClientSideEventHandler("ValueChanged", String.Format(CallbackArgumentFormat,
                callback.ClientInstanceName, container.KeyValue, container.Column.FieldName, "s.GetValue()"));
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
        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (View != null && View.Editor is ASPxGridListEditor) {
                ((ASPxGridListEditor)View.Editor).CustomCreateCellControl -= new EventHandler<CustomCreateCellControlEventArgs>(listEditor_CustomCreateCellControl);
            }
            if (callback != null) {
                callback.Callback -= callback_Callback;
                callback = null;
            }
        }
    }
}