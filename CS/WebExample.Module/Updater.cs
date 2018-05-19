using System;

using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;

namespace WebExample.Module
{
    public class Updater : ModuleUpdater
    {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
            CreateDomainObject1("a", 0);
            CreateDomainObject1("b", 1);
            CreateDomainObject1("c", 2);
            CreateDomainObject1("d", 3);
        }
        private void CreateDomainObject1(string name, int value) {
            DomainObject1 do1 = ObjectSpace.FindObject<DomainObject1>(new BinaryOperator("Name", name));
            if (do1 == null) {
                do1 = ObjectSpace.CreateObject<DomainObject1>();
                do1.Name = name;
                do1.Value = value;
                do1.Save();
            }
        }
    }
}
