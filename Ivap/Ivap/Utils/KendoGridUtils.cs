using Ivap.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ivap.Utils
{


    public class KendoGridUtils
    {

        public string Data { set; get; }

        public Command Command { set; get; }

        public List<fields> columns { set; get; }


        public KendoGridUtils GetCommandButtonForGrid(string RouteName)
        {
            try
            {
                KendoGridUtils objKGrid = new KendoGridUtils();
                int CommandCount = 0;
                List<CommandButton> LstCommand = new List<CommandButton>();
                if (RouteName == "ViewContract")
                {
                    CommandButton ObjViewButton = new CommandButton();
                    ObjViewButton.name = "ViewRateCard";
                    ObjViewButton.text = "";
                    ObjViewButton.click = "EditHandler";
                    ObjViewButton.iconClass = "k-icon k-i-change-manually";
                    ObjViewButton.title = "View Rate Card";
                    LstCommand.Add(ObjViewButton);
                    CommandCount++;
                }
                if (AuthorizationRepo.IsValidAction(RouteName, "UpdateAction"))
                {
                    CommandButton ObjEditButton = new CommandButton();
                    ObjEditButton.name = "Edit";
                    ObjEditButton.text = "";
                    ObjEditButton.click = "EditHandler";
                    ObjEditButton.iconClass = "kIcon kIconEdit ";
                    ObjEditButton.title = "Edit";
                    LstCommand.Add(ObjEditButton);
                    CommandCount++;
                }

                if (AuthorizationRepo.IsValidAction(RouteName, "ViewAction"))
                {
                    CommandButton ObjViewButton = new CommandButton();
                    ObjViewButton.name = "View";
                    ObjViewButton.text = "";
                    ObjViewButton.click = "EditHandler";
                    ObjViewButton.iconClass = "kIcon kIconView";
                    ObjViewButton.title = "View";
                    LstCommand.Add(ObjViewButton);
                    CommandCount++;
                }

                if (CommandCount > 0)
                {
                    Command ObjCommand = new Command();
                    ObjCommand.title = "Action";
                    ObjCommand.width = 40;
                    ObjCommand.command = LstCommand;
                    objKGrid.Command = ObjCommand;
                    if (CommandCount > 1)
                        ObjCommand.width = 100;
                }
                return objKGrid;
            }
            catch
            {
                throw;
            }

        }
    }
    public class fields
    {
        public string field { get; set; }
        public string title { get; set; }
        public string format { get; set; }
        public int width { get; set; }
        public filterable filterable { set; get; }

    }
    public class Command
    {
        public List<CommandButton> command { set; get; }
        public string title { set; get; }
        public int width { set; get; }

    }

    public class CommandButton
    {
        public string name { get; set; }
        public string text { get; set; }
        public string click { get; set; }
        public string iconClass { get; set; }
        public string title { set; get; }

    }

    public class filterable
    {
        public bool multi { set; get; }
        public bool search { set; get; }
    }
}