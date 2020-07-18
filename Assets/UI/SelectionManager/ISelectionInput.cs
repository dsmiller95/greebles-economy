using UnityEngine;

namespace Assets.UI.SelectionManager
{
    /// <summary>
    /// An interface which represents a prompt to the user to select something off of the screen
    /// </summary>
    public interface ISelectionInput
    {
        /// <summary>
        /// Begin the selection input -- can be used to highlight certain objects on the screen
        /// </summary>
        void BeginSelectionInput();
        /// <summary>
        /// Called when this input is no longer active. Will happen if the input is removed from the input stack;
        ///     or if it something else is put on top if it in the stack
        /// </summary>
        void CloseSelectionInput();

        /// <summary>
        /// This selection input has been supersceded with a different input option.
        /// </summary>
        /// <param name="other">the other selection input which superscedes this one</param>
        /// <returns>True if this selection input should be removed as a result</returns>
        bool Supersceded(ISelectionInput other);

        /// <summary>
        /// Check if the given GameObject is a valid option for this input
        /// </summary>
        /// <param name="o">The GameObject</param>
        /// <returns>if the object is a valid selection</returns>
        bool IsValidClick(GameObject o);
        /// <summary>
        /// Called when an object is selected, and is a valid selection for this operation
        /// </summary>
        /// <param name="o"></param>
        /// <returns>true if this selection input should be removed from the stack, if it is completed</returns>
        bool ObjectClicked(GameObject o, RaycastHit rayHit);

    }
}
