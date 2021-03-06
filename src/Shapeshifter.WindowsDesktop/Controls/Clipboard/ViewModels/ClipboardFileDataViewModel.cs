﻿namespace Shapeshifter.WindowsDesktop.Controls.Clipboard.ViewModels
{
    using System.Windows.Forms;

    using Autofac;

    using Data.Interfaces;

    using Designer.Facades;
    using Designer.Helpers;

    using Infrastructure.Environment;
    using Infrastructure.Environment.Interfaces;

    class ClipboardFileDataViewModel: ClipboardDataViewModel<IClipboardFileData>
    {
        public ClipboardFileDataViewModel()
            : this(new EnvironmentInformation(true)) { }

        public ClipboardFileDataViewModel(
            IEnvironmentInformation environmentInformation)
        {
            if (environmentInformation.IsInDesignTime)
            {
                PrepareDesignerMode();
            }
        }

        void PrepareDesignerMode()
        {
            var container = DesignTimeContainerHelper.CreateDesignTimeContainer();
            Data = container.Resolve<DesignerClipboardFileDataFacade>();
        }
    }
}