using System;

namespace Dynamo.Events
{
    public delegate void WorkspaceAddedEventHandler(WorkspacesModificationEventArgs args);
    public delegate void WorkspaceRemoveStartedEventHandler(WorkspacesModificationEventArgs args);
    public delegate void WorkspaceRemovedEventHandler(WorkspacesModificationEventArgs args);
    public delegate void WorkspaceSettingsChangedEventHandler(WorkspacesSettingsChangedEventArgs args);
    public delegate void WorkspaceClearingEventHandler();
    public delegate void WorkspaceClearedEventHandler();

    public static class WorkspaceEvents
    {
        /// <summary>
        /// An event that is triggered when a workspace is added
        /// to the DynamoModel's Workspaces collection.
        /// </summary>
        public static event WorkspaceAddedEventHandler WorkspaceAdded;
        internal static void OnWorkspaceAdded(Guid id, string name, Type type)
        {
            if (WorkspaceAdded != null)
                WorkspaceAdded(new WorkspacesModificationEventArgs(id, name, type));
        }

        /// <summary>
        /// An event that is triggered prior to the removal of the workspace
        /// from the Workspaces collection.
        /// </summary>
        public static event WorkspaceRemoveStartedEventHandler WorkspaceRemoveStarted;
        internal static void OnWorkspaceRemoveStarted(Guid id, string name, Type type)
        {
            if (WorkspaceRemoveStarted != null)
                WorkspaceRemoveStarted(new WorkspacesModificationEventArgs(id, name, type));
        }

        /// <summary>
        /// An event that is triggered when a workspace is removed
        /// from the DynamoModel's Workspaces collection.
        /// </summary>
        public static event WorkspaceRemovedEventHandler WorkspaceRemoved;
        internal static void OnWorkspaceRemoved(Guid id, string name, Type type)
        {
            if (WorkspaceRemoved != null)
                WorkspaceRemoved(new WorkspacesModificationEventArgs(id, name, type));
        }

        /// <summary>
        /// An event that is triggered before a workspace is cleared.
        /// </summary>
        public static event WorkspaceClearingEventHandler WorkspaceClearing;
        internal static void OnWorkspaceClearing()
        {
            if (WorkspaceClearing != null)
                WorkspaceClearing();
        }

        /// <summary>
        /// An event that is triggered after a workspace is cleared.
        /// </summary>
        public static event WorkspaceClearedEventHandler WorkspaceCleared;
        internal static void OnWorkspaceCleared()
        {
            if (WorkspaceCleared != null)
                WorkspaceCleared();
        }

        /// <summary>
        /// An event raised when workspace ScaleFactor setting is changed.
        /// </summary>
        public static event WorkspaceSettingsChangedEventHandler WorkspaceSettingsChanged;
        internal static void OnWorkspaceSettingsChanged(double scaleFactor)
        {
            var handler = WorkspaceSettingsChanged;
            if (handler != null)
            {
                handler(new WorkspacesSettingsChangedEventArgs(scaleFactor));
            }
        }

        /// <summary>
        /// An event raised when workspace EnableLegacyPolyCurveBehavior setting is changed.
        /// </summary>
        // TODO: Remove in Dynamo 4.0.
        internal static event WorkspaceSettingsChangedEventHandler WorkspaceEnableLegacyPolyCurveSettingChanged;
        internal static void OnWorkspaceSettingsChanged(bool enableLegacyPolyCurveBehavior)
        {
            var handler = WorkspaceEnableLegacyPolyCurveSettingChanged;
            if (handler != null)
            {
                handler(new WorkspacesSettingsChangedEventArgs(enableLegacyPolyCurveBehavior));
            }
        }
    }

    public class WorkspacesModificationEventArgs : EventArgs
    {
        public Guid Id { get; internal set; }
        public string Name { get; internal set; }
        public Type Type { get; internal set; }

        public WorkspacesModificationEventArgs(Guid id, string name, Type type)
        {
            Id = id;
            Name = name;
            Type = type;
        }
    }

    public class WorkspacesSettingsChangedEventArgs : EventArgs
    {
        public double ScaleFactor { get; private set; }

        /// <summary>
        /// PolyCurve normal and direction behavior has been made predictable in Dynamo 3.0 and has therefore changed. 
        /// This reflects whether legacy (pre-3.0) PolyCurve behavior is selected either in preference settings or in the workspace.
        /// A workspace setting if exists, overrides the default preference setting. 
        /// </summary>
        // TODO: Remove in Dynamo 4.0.
        internal bool EnableLegacyPolyCurveBehavior { get; private set; }

        public WorkspacesSettingsChangedEventArgs(double scaleFactor)
        {
            ScaleFactor = scaleFactor;
        }

        // TODO: Remove in Dynamo 4.0.
        internal WorkspacesSettingsChangedEventArgs(bool enableLegacyPolyCurveBehavior)
        {
            EnableLegacyPolyCurveBehavior = enableLegacyPolyCurveBehavior;
        }
    }
}
