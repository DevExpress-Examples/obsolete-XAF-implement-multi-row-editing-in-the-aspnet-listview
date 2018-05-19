using System;
using System.ComponentModel;
using System.Web.UI;

using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Web.Editors;

namespace WebExample.Module.Web {
    [ListEditor(typeof(object), false)]
    public class MultiEditASPxGridListEditor : ASPxGridListEditor {
        public MultiEditASPxGridListEditor(IModelListView model)
            : base(model) {
            CreateCustomDataItemTemplate += OnCreateCustomDataItemTemplate;
            CustomizeGridViewDataColumn += OnCustomizeGridViewDataColumn;
        }
        ASPxCallback callback;
        protected override object CreateControlsCore() {
            Panel panel = new Panel();
            callback = new ASPxCallback();
            callback.ID = Model.Id + "aspxCallback1";
            callback.ClientInstanceName = Model.Id + "_callback1";
            callback.Callback += new CallbackEventHandler(callback_Callback);
            panel.Controls.Add(callback);
            ASPxGridView grid = (ASPxGridView)base.CreateControlsCore();
            panel.Controls.Add(grid);
            return panel;
        }
        void OnCreateCustomDataItemTemplate(object sender, CreateCustomDataItemTemplateEventArgs e) {
            if (IsColumnSupported(e.ModelColumn)) {
                WebPropertyEditor propertyEditor = FindPropertyEditor(e.ModelColumn, ViewEditMode.Edit);
                if (propertyEditor == null) {
                    propertyEditor = DataItemTemplateFactory.CreateColumnTemplate(e.ModelColumn, e.DataItemTemplateInfoProvider, Application, ObjectTypeInfo, ObjectSpace, ViewEditMode.Edit).PropertyEditor;
                }
                propertyEditor.ImmediatePostData = false;
                e.Template = new MultiRowEditTemplate(propertyEditor, callback.ClientInstanceName);
                e.Handled = true;
            }
        }
        void callback_Callback(object source, DevExpress.Web.CallbackEventArgs e) {
            String[] p = e.Parameter.Split('|');
            Object key = TypeDescriptor.GetConverter(ObjectTypeInfo.KeyMember.MemberType).ConvertFromString(p[0]);
            IMemberInfo member = ObjectTypeInfo.FindMember(p[1]);
            Object value = TypeDescriptor.GetConverter(member.MemberType).ConvertFromString(p[2]); ;
            object obj = ObjectSpace.GetObjectByKey(ObjectTypeInfo.Type, key);
            member.SetValue(obj, value);
            ObjectSpace.CommitChanges();
        }
        Type[] supportedPropertyEditorTypes = new Type[]{
            typeof(ASPxStringPropertyEditor),
            typeof(ASPxIntPropertyEditor),
            typeof(ASPxBooleanPropertyEditor),
            typeof(ASPxEnumPropertyEditor)
        };
        protected virtual bool IsColumnSupported(IModelColumn model) {
            if (model.GroupIndex >= 0) {
                return false;
            }
            foreach (Type type in supportedPropertyEditorTypes) {
                if (type.IsAssignableFrom(model.PropertyEditorType)) {
                    return true;
                }
            }
            return false;
        }
        // Sorting and grouping are not supported
        void OnCustomizeGridViewDataColumn(object sender, CustomizeGridViewDataColumnEventArgs e) {
            if (IsColumnSupported(e.ModelColumn)) {
                e.Column.Settings.AllowSort = DevExpress.Utils.DefaultBoolean.False;
                e.Column.Settings.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
            }
        }
    }
    public class MultiRowEditTemplate : IBehaviourTemplate {
        WebPropertyEditor propertyEditor;
        string callbackName;
        const String CallbackArgumentFormat = "function (s, e) {{ {0}.PerformCallback(\"{1}|{2}|\" + {3}); }}"; // ASPxCallback, key, fieldName, value

        public MultiRowEditTemplate(WebPropertyEditor propertyEditor, string callbackName) {
            this.propertyEditor = propertyEditor;
            this.callbackName = callbackName;
        }
        #region IBehaviourTemplate Members

        public void InstantiateIn(Control container) {
            GridViewDataItemTemplateContainer gridContainer = container as GridViewDataItemTemplateContainer;
            if (gridContainer == null) {
                throw new NotSupportedException(container.GetType().FullName);
            }
            propertyEditor.CreateControl();
            Object obj = gridContainer.Grid.GetRow(gridContainer.VisibleIndex);
            propertyEditor.CurrentObject = obj;
            propertyEditor.ReadValue();
            if (propertyEditor.Editor is ASPxWebControl) {
                ((ASPxWebControl)propertyEditor.Editor).SetClientSideEventHandler("ValueChanged", String.Format(CallbackArgumentFormat,
                    callbackName, gridContainer.KeyValue, gridContainer.Column.FieldName, "s.GetValue()"));
            }
            container.Controls.Add((Control)propertyEditor.Control);
        }

        public bool CancelClickEventPropagation {
            get {
                return true;
            }
        }

        #endregion
    }
}