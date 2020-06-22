using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.UI.SelectionMananger
{
    /// <summary>
    /// An interface which represents a prompt to the user to select something off of the screen
    /// </summary>
    public interface ISelectionInput
    {
        /// <summary>
        /// Begin the selection input -- can be used to highlight certain objects on the screen
        /// </summary>
        void BeginSelection();
        /// <summary>
        /// Check if the given GameObject is a valid option for this input
        /// </summary>
        /// <param name="o">The GameObject</param>
        /// <returns>if the object is a valid selection</returns>
        bool IsValidSelection(GameObject o);
        /// <summary>
        /// Called when an object is selected, and is a valid selection for this operation
        /// </summary>
        /// <param name="o"></param>
        /// <returns>true if this selection input should be removed from the stack, if it is completed</returns>
        bool SelectedObject(GameObject o);

    }
}
