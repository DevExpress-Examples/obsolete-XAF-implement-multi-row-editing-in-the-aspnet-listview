Imports Microsoft.VisualBasic
Imports System

Imports DevExpress.ExpressApp.Updating
Imports DevExpress.Xpo
Imports DevExpress.Data.Filtering
Imports DevExpress.ExpressApp
Imports DevExpress.Persistent.BaseImpl

Namespace WebExample.Module
	Public Class Updater
		Inherits ModuleUpdater
		Public Sub New(ByVal objectSpace As IObjectSpace, ByVal currentDBVersion As Version)
			MyBase.New(objectSpace, currentDBVersion)
		End Sub
		Public Overrides Sub UpdateDatabaseAfterUpdateSchema()
			MyBase.UpdateDatabaseAfterUpdateSchema()
			CreateDomainObject1("a", 0)
			CreateDomainObject1("b", 1)
			CreateDomainObject1("c", 2)
			CreateDomainObject1("d", 3)
		End Sub
		Private Sub CreateDomainObject1(ByVal name As String, ByVal value As Integer)
			Dim do1 As DomainObject1 = ObjectSpace.FindObject(Of DomainObject1)(New BinaryOperator("Name", name))
			If do1 Is Nothing Then
				do1 = ObjectSpace.CreateObject(Of DomainObject1)()
				do1.Name = name
				do1.Value = value
				do1.Save()
			End If
		End Sub
	End Class
End Namespace
