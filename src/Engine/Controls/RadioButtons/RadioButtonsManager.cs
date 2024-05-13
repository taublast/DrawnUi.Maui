using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DrawnUi.Maui.Controls
{
    public class RadioButtonGroupManager
    {
        static RadioButtonGroupManager _instance;

        public static RadioButtonGroupManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new RadioButtonGroupManager();

                return _instance;
            }
        }

        protected Dictionary<string, List<ISkiaRadioButton>> GroupsByName { get; private set; }
        protected Dictionary<SkiaControl, List<ISkiaRadioButton>> GroupsByParent { get; private set; }

        public RadioButtonGroupManager()
        {
            GroupsByName = new Dictionary<string, List<ISkiaRadioButton>>();
            GroupsByParent = new Dictionary<SkiaControl, List<ISkiaRadioButton>>();
        }

        // Adds a control to a named group (Group A).
        public void AddToGroup(ISkiaRadioButton control, string groupName)
        {
            if (!GroupsByName.ContainsKey(groupName))
            {
                GroupsByName[groupName] = new();
            }
            GroupsByName[groupName].Add(control);
            MakeAtLeastOneSelected(GroupsByName[groupName]);
        }

        // Adds a control to a parent-based group (Group B).
        public void AddToGroup(ISkiaRadioButton control, SkiaControl parent)
        {
            if (!GroupsByParent.ContainsKey(parent))
            {
                GroupsByParent[parent] = new();
            }
            GroupsByParent[parent].Add(control);
            MakeAtLeastOneSelected(GroupsByParent[parent]);
        }

        // Removes a control from a named group (Group A).
        public void RemoveFromGroup(string groupName, ISkiaRadioButton control)
        {
            if (GroupsByName.ContainsKey(groupName) && GroupsByName[groupName].Contains(control))
            {
                GroupsByName[groupName].Remove(control);
                // After removal, check and ensure that at least one button is selected.
                MakeAtLeastOneSelected(GroupsByName[groupName]);
            }
        }

        // Removes a control from a parent-based group (Group B).
        public void RemoveFromGroup(SkiaControl parent, ISkiaRadioButton control)
        {
            if (GroupsByParent.ContainsKey(parent) && GroupsByParent[parent].Contains(control))
            {
                GroupsByParent[parent].Remove(control);
                // After removal, check and ensure that at least one button is selected.
                MakeAtLeastOneSelected(GroupsByParent[parent]);
            }
        }

        public void RemoveFromGroups(ISkiaRadioButton control)
        {
            // Attempt to remove from GroupsByName
            bool removed = false;
            foreach (var groupName in GroupsByName.Keys.ToList()) // ToList to avoid modification issues during iteration
            {
                var group = GroupsByName[groupName];
                if (group.Contains(control))
                {
                    group.Remove(control);
                    MakeAtLeastOneSelected(group);
                    removed = true;
                    break; // Assuming a control can only belong to one group, we stop once found
                }
            }

            if (!removed) // If not removed from GroupsByName, try GroupsByParent
            {
                foreach (var parent in GroupsByParent.Keys.ToList()) // ToList for safe iteration
                {
                    var group = GroupsByParent[parent];
                    if (group.Contains(control))
                    {
                        group.Remove(control);
                        MakeAtLeastOneSelected(group);
                        break; // Assuming a control can only belong to one group, we stop once found
                    }
                }
            }
        }


        private void MakeAtLeastOneSelected(List<ISkiaRadioButton> group)
        {
            // Check if any button is still selected in the group.
            if (!group.Any(c => c.GetValueInternal()))
            {
                // If no button is selected, and the group is not empty, select the first one.
                var firstControl = group.FirstOrDefault();
                if (firstControl != null)
                {
                    firstControl.SetValueInternal(true);
                }
            }
        }

        // Method called by a SkiaControl to report its value change.
        public void ReportValueChange(ISkiaRadioButton control, bool newValue)
        {
            foreach (var groupName in GroupsByName.Keys)
            {
                if (GroupsByName[groupName].Contains(control))
                {
                    SetGroupValuesExcept(groupName, control, newValue, true);
                    return;
                }
            }

            foreach (var parent in GroupsByParent.Keys)
            {
                if (GroupsByParent[parent].Contains(control))
                {
                    SetGroupValuesExcept(parent, control, newValue, false);
                    return;
                }
            }
        }

        private void SetGroupValuesExcept(string groupName, ISkiaRadioButton exceptControl, bool newValue, bool isGroupA)
        {
            var group = isGroupA ? GroupsByName[groupName] : null;

            if (newValue)
            {
                group?.ForEach(c => { if (c != exceptControl) c.SetValueInternal(false); });
            }
            else
            {
                EnsureOneTrueInGroup(group, exceptControl);
            }
        }

        private void SetGroupValuesExcept(SkiaControl parent, ISkiaRadioButton exceptControl, bool newValue, bool isGroupB)
        {
            var group = isGroupB ? GroupsByParent[parent] : null;

            if (newValue)
            {
                group?.ForEach(c => { if (c != exceptControl) c.SetValueInternal(false); });
            }
            else
            {
                EnsureOneTrueInGroup(group, exceptControl);
            }
        }

        private void EnsureOneTrueInGroup(List<ISkiaRadioButton> group, ISkiaRadioButton exceptControl)
        {
            var trueControls = group?.Where(c => c.GetValueInternal()).ToList();

            if (trueControls == null || trueControls.Count == 0 || (trueControls.Count == 1 && trueControls.Contains(exceptControl)))
            {
                var controlToSetTrue = group?.FirstOrDefault(c => c != exceptControl);
                controlToSetTrue?.SetValueInternal(true);
            }
        }
    }
}
