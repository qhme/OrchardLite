using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace Orchard.UI.Navigation
{
    public static class NavigationHelper
    {
        /// <summary>
        /// Identifies the currently selected path, starting from the selected node.
        /// </summary>
        /// <param name="menuItems">All the menuitems in the navigation menu.</param>
        /// <param name="currentRequest">The currently executed request if any</param>
        /// <param name="currentRouteData">The current route data.</param>
        /// <returns>A stack with the selection path being the last node the currently selected one.</returns>
        public static Stack<MenuItem> SetSelectedPath(IEnumerable<MenuItem> menuItems, HttpRequestBase currentRequest, RouteData currentRouteData)
        {
            return SetSelectedPath(menuItems, currentRequest, currentRouteData.Values);
        }

        /// <summary>
        /// Identifies the currently selected path, starting from the selected node.
        /// </summary>
        /// <param name="menuItems">All the menuitems in the navigation menu.</param>
        /// <param name="currentRequest">The currently executed request if any</param>
        /// <param name="currentRouteData">The current route data.</param>
        /// <returns>A stack with the selection path being the last node the currently selected one.</returns>
        public static Stack<MenuItem> SetSelectedPath(IEnumerable<MenuItem> menuItems, HttpRequestBase currentRequest, RouteValueDictionary currentRouteData)
        {
            if (menuItems == null)
                return null;

            foreach (MenuItem menuItem in menuItems)
            {
                Stack<MenuItem> selectedPath = SetSelectedPath(menuItem.Items, currentRequest, currentRouteData);
                if (selectedPath != null)
                {
                    menuItem.Selected = true;
                    selectedPath.Push(menuItem);
                    return selectedPath;
                }

                bool match = false;
                // if the menu item doesn't have route values, compare urls
                if (currentRequest != null && menuItem.RouteValues == null)
                {

                    string requestUrl = currentRequest.Path.Replace(currentRequest.ApplicationPath ?? "/", string.Empty);
                    string modelUrl = menuItem.Href.Replace("~/", currentRequest.ApplicationPath);
                    modelUrl = modelUrl.Replace(currentRequest.ApplicationPath ?? "/", string.Empty);
                    if (requestUrl.Equals(modelUrl, StringComparison.OrdinalIgnoreCase) || (!string.IsNullOrEmpty(modelUrl) && requestUrl.StartsWith(modelUrl + "/", StringComparison.OrdinalIgnoreCase)))
                    {
                        match = true;
                    }
                }
                else
                {
                    if (RouteMatches(menuItem.RouteValues, currentRouteData))
                    {
                        match = true;
                    }
                }

                if (match)
                {
                    menuItem.Selected = true;

                    selectedPath = new Stack<MenuItem>();
                    selectedPath.Push(menuItem);
                    return selectedPath;
                }
            }

            return null;
        }

        /// <summary>
        /// Find the first level in the selection path, starting from the bottom, that is not a local task.
        /// </summary>
        /// <param name="selectedPath">The selection path stack. The bottom node is the currently selected one.</param>
        /// <returns>The first node, starting from the bottom, that is not a local task. Otherwise, null.</returns>
        public static MenuItem FindParentLocalTask(Stack<MenuItem> selectedPath)
        {
            if (selectedPath != null)
            {
                MenuItem parentMenuItem = selectedPath.Pop();
                if (parentMenuItem != null)
                {
                    while (selectedPath.Count > 0)
                    {
                        MenuItem currentMenuItem = selectedPath.Pop();
                        if (currentMenuItem.LocalNav)
                        {
                            return parentMenuItem;
                        }

                        parentMenuItem = currentMenuItem;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Determines if a menu item corresponds to a given route.
        /// </summary>
        /// <param name="itemValues">The menu item.</param>
        /// <param name="requestValues">The route data.</param>
        /// <returns>True if the menu item's action corresponds to the route data; false otherwise.</returns>
        public static bool RouteMatches(RouteValueDictionary itemValues, RouteValueDictionary requestValues)
        {
            if (itemValues == null && requestValues == null)
            {
                return true;
            }
            if (itemValues == null || requestValues == null)
            {
                return false;
            }
            if (itemValues.Keys.Any(key => requestValues.ContainsKey(key) == false))
            {
                return false;
            }
            return itemValues.Keys.All(key => string.Equals(Convert.ToString(itemValues[key]), Convert.ToString(requestValues[key]), StringComparison.OrdinalIgnoreCase));
        }


    }

}
