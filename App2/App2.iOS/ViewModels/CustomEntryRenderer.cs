using System;
using UIKit;
using Xamarin.Forms;
using App2.ViewModels;
using App2.iOS.ViewModels;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;


[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]

namespace App2.iOS.ViewModels
{
    class CustomEntryRenderer : EntryRenderer
    {
       
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var view = (CustomEntry)Element;

                Control.LeftView = new UIView(new CGRect(0f, 0f, 9f, 20f));
                Control.LeftViewMode = UITextFieldViewMode.Always;

                Control.KeyboardAppearance = UIKeyboardAppearance.Dark;
                Control.ReturnKeyType = UIReturnKeyType.Done;

                Control.Layer.CornerRadius = Convert.ToSingle(view.CornerRadius);
                Control.Layer.BorderColor = view.BorderColor.ToCGColor();
                Control.Layer.BorderWidth = view.BorderWidth;
                Control.ClipsToBounds = true;
            }
        }
    }
}