Imports Microsoft.VisualBasic
Imports System
Imports System.ComponentModel
Imports System.Web.UI
Imports System.Web.UI.WebControls

Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Web.Editors.ASPx
Imports DevExpress.Web.ASPxGridView
Imports DevExpress.ExpressApp.Web
Imports DevExpress.ExpressApp.Web.Editors
Imports DevExpress.ExpressApp.Editors
Imports DevExpress.Web.ASPxClasses
Imports DevExpress.ExpressApp.DC
Imports DevExpress.Web.ASPxCallback

Namespace WebExample.Module.Web
	Public Class MultiRowEditingController
		Inherits ViewController
		Public Sub New()
			TargetViewId = "DomainObject1_ListView"
		End Sub
		Private Const CallbackArgumentFormat As String = "function (s, e) {{ {0}.PerformCallback(""{1}|{2}|"" + {3}); }}" ' ASPxCallback, key, fieldName, value
		Private callback As ASPxCallback
		Protected Overrides Sub OnViewControlsCreated()
			MyBase.OnViewControlsCreated()
			Dim listEditor As ASPxGridListEditor = TryCast((CType(View, ListView)).Editor, ASPxGridListEditor)
			If listEditor IsNot Nothing Then
				callback = New ASPxCallback()
				callback.ID = View.Id & "aspxCallback1"
				callback.ClientInstanceName = View.Id & "_callback1"
				AddHandler callback.Callback, AddressOf callback_Callback
				CType(View.Control, Control).Controls.Add(callback)
				Dim gridView As ASPxGridView = CType(listEditor.Grid, ASPxGridView)
				For Each column As GridViewColumn In gridView.VisibleColumns
					If TypeOf column Is GridViewDataColumn Then
						Dim dataColumn As GridViewDataColumn = CType(column, GridViewDataColumn)
						Dim defaultTemplate As DataItemTemplate = TryCast(dataColumn.DataItemTemplate, DataItemTemplate)
						If defaultTemplate IsNot Nothing Then
							Dim newTemplate As New EditItemTemplate(defaultTemplate.PropertyEditor, listEditor)
							AddHandler newTemplate.CustomCreateCellControl, AddressOf newTemplate_CustomCreateCellControl
							dataColumn.DataItemTemplate = newTemplate
						End If
					End If
				Next column
			End If
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
		Private Sub newTemplate_CustomCreateCellControl(ByVal sender As Object, ByVal e As CustomCreateCellControlEventArgs)
			If TypeOf e.PropertyEditor.Editor Is ASPxWebControl Then
				AddHandler e.PropertyEditor.Editor.Init, AddressOf Editor_Init
			End If
		End Sub
		Private Sub Editor_Init(ByVal sender As Object, ByVal e As EventArgs)
			Dim editor As ASPxWebControl = CType(sender, ASPxWebControl)
			Dim container As GridViewDataItemTemplateContainer = CType(editor.NamingContainer, GridViewDataItemTemplateContainer)
			editor.SetClientSideEventHandler("ValueChanged", String.Format(CallbackArgumentFormat, callback.ClientInstanceName, container.KeyValue, container.Column.FieldName, "s.GetValue()"))
		End Sub
	End Class
	Public Class EditItemTemplate
		Inherits DataItemTemplate
		Implements ITemplate
		Public Sub New(ByVal propertyEditor As WebPropertyEditor, ByVal provider As IDataItemTemplateInfoProvider)
			MyBase.New(propertyEditor, provider, ViewEditMode.Edit)
		End Sub
		Private Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn
			MyBase.InstantiateIn(container)
			PropertyEditor.Editor.Attributes("onclick") = RenderHelper.EventCancelBubbleCommand
		End Sub
	End Class
End Namespace