using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{
    public delegate void EmptyDel();
    public delegate void BoolDel(bool value);
    public delegate void ItemDel(Item item);
    public delegate void ObjDel(GameObject obj);
    public static EmptyDel WhenStartConversation;
    public static EmptyDel WhenEndConversation;
    public static EmptyDel WhenEndPuzzleConversation;
    public static EmptyDel WhenEndPuzzleInvestigation;
    public static EmptyDel WhenDecidedWhichItemToUse;
    /// <summary>
    /// bool means start or finished showing
    /// </summary>
    public static BoolDel WhenShowingAquirePage;
    /// <summary>
    /// This del is made to notice player interactable detection
    /// trigger to renew it's state when an item activate/ deactivate.
    /// </summary>
    public static EmptyDel WhenSwitchInteractableSimple;
    public static ItemDel WhenReceiveItem;
    public static ItemDel WhenUseItem;
    public static ObjDel WhenReceiveNotebookPage;
}
