Imports System
Imports System.ComponentModel
Imports System.Web.UI

Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Web.Editors.ASPx
Imports DevExpress.Web.ASPxGridView
Imports DevExpress.ExpressApp.Web
Imports DevExpress.ExpressApp.Web.Editors
Imports DevExpress.ExpressApp.Editors
Imports DevExpress.Web.ASPxClasses
Imports DevExpress.ExpressApp.DC
Imports DevExpress.Web.ASPxCallback
Imports DevExpress.ExpressApp.Model

Namespace WebExample.Module.Web
    Public Class MultiRowEditingController
        Inherits ViewController(Of ListView)
        Public Sub New()
            TargetViewId = "DomainObject1_ListView"
        End Sub
        Private Const CallbackArgumentFormat As String = "function (s, e) {{ {0}.PerformCallback(""{1}|{2}|"" + {3}); }}" ' ASPxCallback, key, fieldName, value
        Private callback As ASPxCallback
        Protected Overrides Sub OnActivated()
            MyBase.OnActivated()
            Dim listEditor As ASPxGridListEditor = TryCast(View.Editor, ASPxGridListEditor)
            If listEditor IsNot Nothing Then
                AddHandler listEditor.CustomCreateCellControl, AddressOf listEditor_CustomCreateCellControl
            End If
        End Sub
        Protected Overrides Sub OnViewControlsCreated()
            MyBase.OnViewControlsCreated()
            Dim listEditor As ASPxGridListEditor = TryCast(View.Editor, ASPxGridListEditor)
            If listEditor IsNot Nothing Then
                For Each column As IModelColumn In listEditor.Model.Columns
                    Dim propertyEditor As WebPropertyEditor = listEditor.FindPropertyEditor(column, ViewEditMode.View)
                    propertyEditor.ViewEditMode = ViewEditMode.Edit
                Next column
                callback = New ASPxCallback()
                callback.ID = View.ObjectTypeInfo.Type.Name & "aspxCallback1"
                callback.ClientInstanceName = View.ObjectTypeInfo.Type.Name & "_callback1"
                AddHandler callback.Callback, AddressOf callback_Callback
                CType(View.Control, Control).Controls.Add(callback)
            End If
        End Sub
        Private Sub listEditor_CustomCreateCellControl(ByVal sender As Object, ByVal e As CustomCreateCellControlEventArgs)
            If TypeOf e.PropertyEditor.Editor Is ASPxWebControl Then
                AddHandler e.PropertyEditor.Editor.Init, AddressOf Editor_Init
            End If
        End Sub
        Private Sub Editor_Init(ByVal sender As Object, ByVal e As EventArgs)
            Dim editor As ASPxWebControl = CType(sender, ASPxWebControl)
            RemoveHandler editor.Init, AddressOf Editor_Init
            editor.Attributes("onclick") = RenderHelper.EventCancelBubbleCommand
            Dim container As GridViewDataItemTemplateContainer = CType(editor.NamingContainer, GridViewDataItemTemplateContainer)
            editor.SetClientSideEventHandler("ValueChanged", String.Format(CallbackArgumentFormat, callback.ClientInstanceName, container.KeyValue, container.Column.FieldName, "s.GetValue()"))
        End Sub
        Private Sub callback_Callback(ByVal source As Object, ByVal e As DevExpress.Web.ASPxCallback.CallbackEventArgs)
            Dim p() As String = e.Parameter.Split("|"c)

            Dim key As Object = TypeDescriptor.GetConverter(View.ObjectTypeInfo.KeyMember.MemberType).ConvertFromString(p(0))
            Dim member As IMemberInfo = View.ObjectTypeInfo.FindMember(p(1))
            Dim value As Object = TypeDescriptor.GetConverter(member.MemberType).ConvertFromString(p(2))


            Dim obj As Object = ObjectSpace.GetObjectByKey(View.ObjectTypeInfo.Type, key)
            member.SetValue(obj, value)
            ObjectSpace.CommitChanges()
        End Sub
        Protected Overrides Sub OnDeactivated()
            MyBase.OnDeactivated()
            If View IsNot Nothing AndAlso TypeOf View.Editor Is ASPxGridListEditor Then
                RemoveHandler (CType(View.Editor, ASPxGridListEditor)).CustomCreateCellControl, AddressOf listEditor_CustomCreateCellControl
            End If
            If callback IsNot Nothing Then
                RemoveHandler callback.Callback, AddressOf callback_Callback
                callback = Nothing
            End If
        End Sub
    End Class
End Namespace