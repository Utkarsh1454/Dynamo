﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dynamo.Core;
using Dynamo.Interfaces;
using Dynamo.Wpf.Properties;
using DelegateCommand = Dynamo.UI.Commands.DelegateCommand;
using Dynamo.Models;
using System.Windows.Data;
using System.Globalization;
using System.Linq;
using Dynamo.Configuration;
using DynamoUtilities;
using Dynamo.Wpf.Utilities;
using Dynamo.Logging;
using System.Windows;

namespace Dynamo.ViewModels
{

    public class TrustedPathEventArgs : EventArgs
    {
        /// <summary>
        /// Indicate whether user wants to add the current path to the list
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Indicate the path to be added to Dynamo's trusted locations
        /// </summary>
        public string Path { get; set; }
    }

    public class TrustedPathViewModel : ViewModelBase
    {
        private PreferenceSettings settings;
        private DynamoLogger logger;

        public ObservableCollection<string> TrustedLocations { get; private set; }
       
        public event EventHandler<TrustedPathEventArgs> RequestShowFileDialog;
        public virtual void OnRequestShowFileDialog(object sender, TrustedPathEventArgs e)
        {
            if (RequestShowFileDialog != null)
            {
                RequestShowFileDialog(sender, e);
            }
        }

        public DelegateCommand AddPathCommand { get; private set; }
        public DelegateCommand DeletePathCommand { get; private set; }
        public DelegateCommand UpdatePathCommand { get; private set; }
        public DelegateCommand SaveSettingCommand { get; private set; }

        /// <summary>
        /// The main constructor of the TrustedPathViewModel class.
        /// </summary>
        /// <param name="settings">Dynamo's preference settings</param>
        /// <param name="logger">Dynamo's logging tool</param>
        public TrustedPathViewModel(PreferenceSettings settings, DynamoLogger logger)
        {
            this.settings = settings;
            this.logger = logger;
            InitializeTrustedLocations();
            InitializeCommands();
        }

        private void InitializeCommands() 
        {
            AddPathCommand = new DelegateCommand(p => InsertPath());
            DeletePathCommand = new DelegateCommand(p => RemovePathAt(ConvertPathToIndex(p)), p => CanDelete(ConvertPathToIndex(p)));
            UpdatePathCommand = new DelegateCommand(p => UpdatePathAt(ConvertPathToIndex(p)), p => CanUpdate(ConvertPathToIndex(p)));
            SaveSettingCommand = new DelegateCommand(CommitChanges);
        }

        private int ConvertPathToIndex(object path)
        {
            return TrustedLocations.IndexOf(path as string);
        }

        private void RaiseCanExecuteChanged()
        {
            AddPathCommand.RaiseCanExecuteChanged();
            DeletePathCommand.RaiseCanExecuteChanged();
            UpdatePathCommand.RaiseCanExecuteChanged();
        }

        private bool CanDelete(int param)
        {
            return TrustedLocations.Count > 1;
        }

        private bool CanUpdate(int param)
        {
            //add any exceptions or built in trusted locations here
            return true;
        }

        private void InsertPath()
        {
            var args = new TrustedPathEventArgs();

            ShowFileDialog(args);

            if (args.Cancel)
                return;

            try
            {
                PathHelper.ValidateDirectory(args.Path);
            }
            catch(Exception ex)
            {
                if (string.IsNullOrEmpty(args.Path))
                {
                    this.logger?.LogError("Failed to add trusted location because the selected path was null or empty");
                }
                else
                {
                    this.logger?.LogError($"Failed to add trusted location ${args.Path} due to the following error: {ex.Message}");
                }

                string errorMessage = string.Format(Resources.PackageFolderNotAccessible, args.Path);
                MessageBoxService.Show(errorMessage, Resources.UnableToAccessPackageDirectory, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            TrustedLocations.Insert(TrustedLocations.Count, args.Path);
            RaiseCanExecuteChanged();
        }

        private void ShowFileDialog(TrustedPathEventArgs e)
        {
            OnRequestShowFileDialog(this, e);

            if (e.Cancel == false && TrustedLocations.Contains(e.Path))
                e.Cancel = true;
        }

        private void UpdatePathAt(int index)
        {
            var args = new TrustedPathEventArgs
            {
                Path = TrustedLocations[index]
            };

            ShowFileDialog(args);

            if (args.Cancel)
                return;

            TrustedLocations[index] = args.Path;
        }

        private void RemovePathAt(int index)
        {
            TrustedLocations.RemoveAt(index);
            RaiseCanExecuteChanged();
        }

        private void CommitChanges(object param)
        {
            settings?.SetTrustedLocations(TrustedLocations);
        }

        internal void InitializeTrustedLocations()
        {
            TrustedLocations = new ObservableCollection<string>(settings?.TrustedLocations ?? new List<string>());
        }
    }
}