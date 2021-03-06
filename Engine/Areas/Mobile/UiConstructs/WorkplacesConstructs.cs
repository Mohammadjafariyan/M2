using System.Collections.Generic;
using Engine.Areas.Absence.Controllers;
using Engine.Areas.Mobile.Models;
using Engine.Areas.Mobile.ViewModel;
using Engine.Controllers.AbstractControllers;
using Engine.Controllers.AbstractControllers.ObjectBased;
using Engine.Entities.Models.Core.AppGeneration;
using Engine.Entities.Models.UiGeneratorModels;
using WebAppIDEEngine.Models.CoreEnum;
using WebAppIDEEngine.Models.UiGeneratorModels;
using NotImplementedException = System.NotImplementedException;

namespace Engine.Areas.Absence.UiConstructs
{
    public class WorkplacesConstructs : BaseConstructProvider, IFormConstructProvider, ITableConstructProvider
    {
        public override UiForm GetSaveForm()
        {
            return GetForm();
        }
        
        public override void GetTableUiItems(EjTable ejtable)
        {
            
            base.GetTableUiItems(ejtable);
            
            var GoToSave = new UiItem {Name = "محدوده مکانی در نقشه GPS", UiItemType = UiItemType.Link};

            GoToSave.CustomUrl = $@"/{CurrentArea}/{CurrentController}/WorkplaceInMap";
            //    Delete.CustomUrl =$@"/{CurrentArea}/Api/{CurrentController}Api/Delete";;

            ejtable.UiTableItems.Add(new UiTableItem {EjTable = ejtable, UiItem = GoToSave});

        }
        

        private UiForm GetForm()
        {
            var uiform = new UiForm();
            uiform.Name = "Workplaces";
            uiform.Translate = "ثبت مکان کاری جدید";

            Workplace sp;

            uiform.UiFormInputs.Add(new UiFormInput
                {UiInput = new UiInput {Name = "Name", Translate = "نام مکان", FieldType = FieldType.Text}});
            uiform.UiFormInputs.Add(new UiFormInput
            {
                UiInput = new UiInput
                {
                    Name = "oneDeviceEnabled", Translate = "استفاده از یک دستگاه برای ورود برای این گروه کاری ",
                    FieldType = FieldType.Text, InputType = InputType.Checkbox
                }
            });
            uiform.UiFormInputs.Add(new UiFormInput
            {
                UiInput = new UiInput
                {
                    Name = "IsNotificationsEnabled",
                    Translate = " خبر رسانی فعال باشد ",
                    FieldType = FieldType.Text,
                    InputType = InputType.Checkbox
                }
            });

            uiform.UiFormInputs.Add(new UiFormInput
            {
                UiInput = new UiInput
                {
                    Name = "IsFaceRecognationEnabled",
                    Translate = "تشخیص چهره فعال باشد",
                    FieldType = FieldType.Text,
                    InputType = InputType.Checkbox
                }
            });


            UiFormInput input = GetDropDownInput("UserClockTypesarr", "نواع ساعت زنی", FieldType.MultiSelect,
                GetEnumSelectList<ClockType>());


            uiform.UiFormInputs.Add(input);


            return uiform;
        }

        public override UiForm GetDataTableSearchForm()
        {
            return null;
        }
    }
}